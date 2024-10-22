
using Medimind.Log;
using Medimind.Scripts;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Resources = Steam_Integration_Manager.Properties.Resources;

namespace Medimind
{
    public partial class SteamForm : Form
    {
        /// <summary>
        /// 종료 타입입니다.
        /// </summary>
        public enum ExitType
        {
            /// <summary>
            /// 정상 종료
            /// </summary>
            Normal,
            /// <summary>
            /// 비정상 종료
            /// </summary>
            Abnormal,
            /// <summary>
            /// 프로그램 다시 하기
            /// </summary>
            Retry,
            /// <summary>
            /// 콘텐츠 다시 선택
            /// </summary>
            Reselect,
            /// <summary>
            /// 장비 사용법
            /// </summary>
            HowToUse
        }

        // 시작 화면 로고 슬라이드
        private const int kHomeLevel = 0;
        // 사전 안내 슬라이드
        private const int kTrainingGuideLevel = 1;        
        private const int kLogLimit = 1000;

        // 안경 착용 마지막 슬라이드
        private const int GuideEndLevel = 12;
        // 콘텐츠 선택 슬라이드
        private const int ContentsSelectLevel = 13;
        // VR 장비 사용 방법 슬라이드
        private const int kHowToUseLevel = 14;
        private const int kHowToUseEndLevel = 15;


        private ButtonStateEvent startButton;
        private ButtonStateEvent prevButton;
        private ButtonStateEvent nextButton;
        private ButtonStateEvent startProgButton;
        private ButtonStateEvent select1Button;
        private ButtonStateEvent select2Button;
        private ButtonStateEvent select3Button;
        private ButtonStateEvent returnButton;
        private ButtonStateEvent confirmButton;

        private Image[] bgImages = new Image[]
            {
                /*00*/Resources.bg_image00_Home_Logo,
                /*01*/Resources.bg_image01_훈련안내1,
                /*02*/Resources.bg_image02_훈련안내2,
                /*03*/Resources.bg_image03_훈련안내3,
                /*04*/Resources.bg_image04_훈련안내4,
                /*05*/Resources.bg_image05_VR장비사용방법1,
                /*06*/Resources.bg_image06_VR장비사용방법2,
                /*07*/Resources.bg_image07_VR장비사용방법3,
                /*08*/Resources.bg_image08_VR장비사용방법4,
                /*09*/Resources.bg_image09_VR장비사용방법5,
                /*10*/Resources.bg_image10_VR장비사용방법6,
                /*11*/Resources.bg_image11_VR장비사용방법7,
                /*12*/Resources.bg_image12_VR장비사용방법8,
                /*13*/Resources.bg_image13_프로그램선택1,
                /*14*/Resources.bg_image05_VR장비사용방법1,
                /*15*/Resources.bg_image12_VR장비사용방법8,

            };

        private int curLevel = 0;
        private int oldLevel = -1;
        private string lastUnityName;
        /// <summary>
        /// 유니티 정상 종료시 true
        /// </summary>
        private bool m_isUnityNormalEnded = false;
        private UnityLuncher unityLuncher = new UnityLuncher();
        private NamedPipeResponser unityPipeServer = new NamedPipeResponser();
        private static CustomLogger logger = new CustomLogger();

        public SteamForm()
        {
            InitializeComponent();

            logger.maxFolderCount = Steam_Integration_Manager.Properties.Settings.Default.LOG_COUNT;
            if (365 < logger.maxFolderCount)
                logger.maxFolderCount = 365;

            logger.StartUp("Log", kLogLimit);

            startButton = new ButtonStateEvent(button_Start, Resources.ButtonStart_Default, Resources.ButtonStart_Hover, Resources.ButtonStart_Pressed, Resources.ButtonStart_Disabled);
            prevButton = new ButtonStateEvent(button_Prev, Resources.ButtonPrev_Default, Resources.ButtonPrev_Hover, Resources.ButtonPrev_Pressed, Resources.ButtonPrev_Disabled);
            nextButton = new ButtonStateEvent(button_Next, Resources.ButtonNext_Default, Resources.ButtonNext_Hover, Resources.ButtonNext_Pressed, Resources.ButtonNext_Disabled);
            startProgButton = new ButtonStateEvent(button_startProg, Resources.ButtonStartProgram_Default, Resources.ButtonStartProgram_Hover, Resources.ButtonStartProgram_Pressed, Resources.ButtonStartProgram_Disabled);
            select1Button = new ButtonStateEvent(button_Select1, Resources.ButtonChoose_Default, Resources.ButtonChoose_hover, Resources.ButtonChoose_pressed, Resources.ButtonChoose_disabled);
            select2Button = new ButtonStateEvent(button_Select2, Resources.ButtonChoose_Default, Resources.ButtonChoose_hover, Resources.ButtonChoose_pressed, Resources.ButtonChoose_disabled);
            select3Button = new ButtonStateEvent(button_Select3, Resources.ButtonChoose_Default, Resources.ButtonChoose_hover, Resources.ButtonChoose_pressed, Resources.ButtonChoose_disabled);
            returnButton = new ButtonStateEvent(button_Return, Resources.ButtonBack_Default, Resources.ButtonBack_Hover, Resources.ButtonBack_Pressed, Resources.ButtonBack_Disabled);
            confirmButton = new ButtonStateEvent(button_Confirm, Resources.ButtonOk_Default, Resources.ButtonOk_Hover, Resources.ButtonOk_Pressed, Resources.ButtonOk_Disabled);

            prevButton.onClick += Prev;
            startButton.onClick += Next;
            nextButton.onClick += Next;
            startProgButton.onClick += Next;

            select1Button.onClick += delegate
            {
                StartUnity("MindMate_1");
            };
            select2Button.onClick += delegate
            {
                StartUnity("MindMate_2");
            };
            select3Button.onClick += delegate
            {
                StartUnity("MindMate_3");
            };
                                    
            returnButton.onClick += ()=> StartUnity(lastUnityName);
            confirmButton.onClick += () => StartUnity(lastUnityName);

            startButton.Show();
            prevButton.Hide();
            nextButton.Hide();
            startProgButton.Hide();
            select1Button.Hide();
            select2Button.Hide();
            select3Button.Hide();
            returnButton.Hide();
            confirmButton.Hide();

            timer.Tick += QueuePool.Update;

            QueuePool.InitLog(Log);
            QueuePool.InitData((x) => Receiver(x.data));

            ChangeBackground(bgImages[kHomeLevel]);
        }
        private void Log(QueueDate<string> _log)
        {
            string log = $"[{_log.time:HH:mm.ss}] {_log.data}";
            logger.WriteLog(log);
        }

        /// <summary>
        /// 네임드 파이프라인을 통해 유니티로부터 Flag받는 함수
        /// </summary>
        /// <param name="data"></param>
        private void Receiver(byte[] data)
        {
            if (data.Length != 1) return;

            QueuePool.log.Add($"NamedPipe Receive. {(NamedPipeBase.Type)data[0]}");

            NamedPipeBase.Type received = (NamedPipeBase.Type)data[0];

            switch (received)
            {
                case NamedPipeBase.Type.Exit: UnityExit_NormalEnd(); break;
                case NamedPipeBase.Type.Retry: UnityExit_Retry(); break;
                case NamedPipeBase.Type.Reselect: UnityExit_Reselect(); break;
                case NamedPipeBase.Type.HowToUse: UnityExit_HowToUse(); break;
            }
        }
        private void ChangeBackground(Image image)
        {
            BackgroundImage = image;

            this.Invalidate();
            this.Update();
        }

        /// <summary>
        /// 슬라이드 페이지에 따라 버튼 활성화
        /// </summary>
        private void ShowLevel()
        {
            // 모든 버튼 비활성화
            startButton.Hide();
            prevButton.Hide();
            nextButton.Hide();
            startProgButton.Hide();
            select1Button.Hide();
            select2Button.Hide();
            select3Button.Hide();
            returnButton.Hide();
            confirmButton.Hide();

            switch (curLevel)
            {
                case kHomeLevel:
                    startButton.Show();
                    break;
                case kHowToUseLevel:
                    returnButton.Show();
                    nextButton.Show();
                    break;
                case kHowToUseEndLevel:
                    prevButton.Show();
                    confirmButton.Show();
                    break;
                case ContentsSelectLevel:
                    select1Button.Show();
                    select2Button.Show();
                    select3Button.Show();
                    break;
                case GuideEndLevel:
                    prevButton.Show();
                    startProgButton.Show();
                    break;
                default:
                    prevButton.Show();
                    nextButton.Show();
                    break;
            }

            ChangeBackground(bgImages[curLevel]);
        }

        private void StartUnity(string unityName)
        {
            lastUnityName = unityName;

            unityPipeServer = new NamedPipeResponser();
            unityPipeServer.Running();

            //이미 실행 중인지 판단
            if (unityLuncher.IsAlive)
            {
                ShowMessage("이미 실행중입니다.");
                QueuePool.log.Add("이미 실행중입니다.");
            }

            //폴더 검사
            string contentsFolderPath = Path.Combine(Infomation.PATH_UNITYFOLDER, unityName);
            if (Directory.Exists(contentsFolderPath) == false)
            {
                ShowMessageError($"콘텐츠가 있는 폴더를 찾을 수 없습니다.\n\n경로: {contentsFolderPath}");
            }
            //파일 검사
            string filePath = Path.Combine(contentsFolderPath, $"{unityName}.exe");
            if (!File.Exists(filePath))
            {
                ShowMessageError($"다음 폴더에 콘텐츠 파일({unityName}.exe)이 존재하지 않습니다.\n\n경로: {contentsFolderPath}");
            }

            //실행
            try
            {
                QueuePool.log.Add($"{unityName} 실행");

                m_isUnityNormalEnded = false;

                unityLuncher.Start("", contentsFolderPath, filePath, UnityExit_AbnormalEnd);
            }
            catch (Exception ex)
            {
                ShowMessageError($"다음 파일을 실행할 수 없습니다.\n\n경로: {filePath}\n\n내용: {ex}");
            }
        }

        private void ShowMessage(string message, string title = "")
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (title == "")
                    MessageBox.Show(message);
                else
                    MessageBox.Show(message, title);
            });
        }
        private void ShowMessageError(string message)
        {
            this.Invoke((MethodInvoker)delegate
            {
                MessageBox.Show(message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }


        #region ContentsControll
        private void Prev()
        {
            curLevel--;
            if (curLevel < 0)
                curLevel = 0;

            ShowLevel();
        }
        private void Next()
        {
            curLevel++;
            if (kHowToUseEndLevel < curLevel)
                curLevel = kHowToUseEndLevel;

            ShowLevel();
        }
        /// <summary>
        /// 정상 종료 처리
        /// </summary>
        private void UnityExit_NormalEnd()
        {
            QueuePool.log.Add("정상 종료");

            m_isUnityNormalEnded = true;

            Utility.Delay(100);

            //유니티를 먼저 종료 시킨 후 다음 동작을 해야 함
            unityLuncher.Stop();

            ProcessExited(ExitType.Normal);
        }
        /// <summary>
        /// 정상 종료 처리
        /// </summary>
        private void UnityExit_Retry()
        {
            QueuePool.log.Add("정상 종료");

            m_isUnityNormalEnded = true;

            Utility.Delay(100);

            //유니티를 먼저 종료 시킨 후 다음 동작을 해야 함
            unityLuncher.Stop();

            ProcessExited(ExitType.Retry);
        }
        /// <summary>
        /// 정상 종료 처리
        /// </summary>
        private void UnityExit_Reselect()
        {
            QueuePool.log.Add("정상 종료");

            m_isUnityNormalEnded = true;

            Utility.Delay(100);

            //유니티를 먼저 종료 시킨 후 다음 동작을 해야 함
            unityLuncher.Stop();

            ProcessExited(ExitType.Reselect);
        }
        /// <summary>
        /// 정상 종료 처리
        /// </summary>
        private void UnityExit_HowToUse()
        {
            QueuePool.log.Add("정상 종료");

            m_isUnityNormalEnded = true;

            Utility.Delay(100);

            //유니티를 먼저 종료 시킨 후 다음 동작을 해야 함
            unityLuncher.Stop();

            ProcessExited(ExitType.HowToUse);
        }
        /// <summary>
        /// 비정상 종료 처리, Alt+F4, 프로세스 다운등
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnityExit_AbnormalEnd()
        {
            if (m_isUnityNormalEnded) return;
            if (unityLuncher == null) return;

            QueuePool.log.Add($"Unity Abnormal Exit");

            //초기화
            try
            {
                ShowMessage("콘텐츠가 비정상 적으로 종료 되었습니다.", "비정상 종료 발생!");

                ProcessExited(ExitType.Abnormal);

                unityLuncher.Clear();
            }
            catch (SystemException ex)
            {
                ShowMessageError(ex.ToString());

                Utility.Delay(100);

                if (unityPipeServer.IsAlive)
                    unityPipeServer.Stop();
            }
        }
        /// <summary>
        /// 유니티 끝났을 때 이벤트
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void ProcessExited(ExitType exitCode)
        {
            Utility.Delay(100);

            unityPipeServer.Stop();

            Utility.Delay(100);

            try
            {
                QueuePool.log.Add($"ProcessExited: {exitCode}");
                switch (exitCode)
                {
                    case ExitType.Normal:
                        curLevel = kHomeLevel;
                        ShowLevel();
                        break;
                    case ExitType.Abnormal: goto case ExitType.Reselect;
                    case ExitType.Retry:
                        StartUnity(lastUnityName);
                        break;
                    case ExitType.Reselect:
                        curLevel = ContentsSelectLevel;
                        ShowLevel();
                        break;
                    case ExitType.HowToUse:
                        curLevel = kHowToUseLevel;
                        ShowLevel();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowMessage("종료코드 오류: " + ex.Message);
                QueuePool.log.Add("종료코드 오류: " + ex.Message);
            }
        }
        #endregion

        private bool[] press = new bool[6];
        private void SteamForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.OemOpenBrackets)
            {
                if (kHomeLevel <= curLevel)
                    Prev();
            }
            if (e.KeyCode == Keys.OemCloseBrackets)
            {
                if (curLevel <= ContentsSelectLevel)
                    Next();
            }
            if (e.KeyCode == Keys.D0)
            {
                if (!press[0])
                {
                    press[0] = true;
                    curLevel = kHomeLevel;

                    ShowLevel();
                }
            }
            if (e.KeyCode == Keys.D1)
            {
                if (!press[1])
                {
                    press[1] = true;
                    curLevel = kTrainingGuideLevel;

                    ShowLevel();
                }
            }
            if (e.KeyCode == Keys.D2)
            {
                if (!press[2])
                {
                    press[2] = true;
                    curLevel = kHowToUseLevel;

                    ShowLevel();
                }
            }
            if (e.KeyCode == Keys.D3)
            {
                if (!press[3])
                {
                    press[3] = true;
                    curLevel = GuideEndLevel;

                    ShowLevel();
                }
            }
        }

        private void SteamForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D0 && press[0])
            {
                press[0] = false;
            }
            if (e.KeyCode == Keys.D1 && press[1])
            {
                press[1] = false;
            }
            if (e.KeyCode == Keys.D2 && press[2])
            {
                press[2] = false;
            }
            if (e.KeyCode == Keys.D3 && press[3])
            {
                press[3] = false;
            }
        }
    }
}
