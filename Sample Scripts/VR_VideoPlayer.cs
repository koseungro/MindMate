using FNI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;
using System.Security.Permissions;
using UnityEngine.UI;

namespace FNI
{
    public class VR_VideoPlayer : MonoBehaviour
    {
        #region Singleton
        private static VR_VideoPlayer _instance;
        public static VR_VideoPlayer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<VR_VideoPlayer>();
                }
                return _instance;
            }
        }
        #endregion

        #region Property
        private VideoPlayer videoPlayer;
        public VideoPlayer MyVideoPlayer
        {
            get
            {
                if (videoPlayer == null)
                    videoPlayer = transform.GetComponent<VideoPlayer>();

                return videoPlayer;
            }
        }

        /// <summary>
        /// 영상이 무엇이든 플레이 중인지 확인합니다.
        /// </summary>
        public bool IsPlaying { get { return MyVideoPlayer.isPlaying; } }

        /// <summary>
        /// 반복할지 안할지 확인합니다.
        /// </summary>
        public bool IsLooping { get { return isLoop; } }

        /// <summary>
        /// 재생중인 영상의 현재 프레임입니다.
        /// </summary>
        public long Frame { get { return MyVideoPlayer.frame; } set { MyVideoPlayer.frame = value; } }

        /// <summary>
        /// 재생중인 영상의 현재 시간입니다.
        /// </summary>
        public double VideoTime
        {
            get => MyVideoPlayer.time;
            set => MyVideoPlayer.time = value;
        }
        /// <summary>
        /// 영상의 총 길이입니다.
        /// </summary>
        public ulong Duration { get { return (ulong)(MyVideoPlayer.frameCount / MyVideoPlayer.frameRate); } }

        /// <summary>
        /// 현재 시간의 노말라이즈 입니다.
        /// </summary>
        public double NTime { get { return VideoTime / Duration; } set { MyVideoPlayer.time = value * Duration; } }

        /// <summary>
        /// 영상이 준비 되었는지 확인합니다.
        /// </summary>
        public bool IsPrepared { get { return MyVideoPlayer.isPrepared; } }

        /// <summary>
        /// Seek가 가능한지 확인합니다.
        /// </summary>
        public bool CanSeek { get { return MyVideoPlayer.canSetTime; } }

        /// <summary>
        /// 현재 영상이 반복 중인지 체크합니다.
        /// </summary>
        public bool IsRepeating { get { return isRepeat; } set { isRepeat = value; } }
        #endregion

        public bool isFinish = false;
        private FastMobileBloom fastBloom;

        private bool pastPlaying;
        private double repeatSTime;
        private double repeatETime;
        private bool isLoop;

        public bool IsRepeat { get => isRepeat; set => isRepeat = value; }
        /// <summary>
        /// 현재 영상 구간 반복 사용 상태
        /// </summary>
        private bool isRepeat;
        private bool isDraged = false;

        private bool endVideoCheck = false;

        public LoadState loadState;
        public SeekState seekState = SeekState.Complete;

        private IEnumerator loadRoutine;

        public long testFrame;
        [SerializeField] private Text debugText;

        private void Awake()
        {
            //이벤트 연결
            MyVideoPlayer.errorReceived += ErrorReceived_Event;
            MyVideoPlayer.frameReady += FrameReady_Event;
            MyVideoPlayer.loopPointReached += LoopPointReached_Event;
            MyVideoPlayer.prepareCompleted += PrepareCompleted_Event;
            MyVideoPlayer.seekCompleted += SeekCompleted_Event;
            MyVideoPlayer.started += Started_Event;

            fastBloom = Camera.main.GetComponent<FastMobileBloom>();
        }


        private void Update()
        {
            if (isRepeat)
            {
                if (seekState == SeekState.Complete &&
                    (VideoTime < repeatSTime || repeatETime < VideoTime))
                {
                    seekState = SeekState.Order;
                    Seek(repeatSTime);
                }
            }
#if UNITY_EDITOR

            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    MyVideoPlayer.Prepare();
            //    Debug.Log("Prepare");
            //}

            //if (Input.GetKeyDown(KeyCode.P))
            //{
            //    MyVideoPlayer.Play();
            //}

            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log($"<color=magenta> {Application.targetFrameRate} </color>");
                SeekFrame(testFrame);
                //Seek(6.5f);
                Pause();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Play();
            }
#endif
        }

        /// <summary>
        /// 비디오 세부 기능 실행
        /// </summary>
        /// <param name="key">세부 기능 옵션</param>
        public void SetVideoCtrl(VideoOption key)
        {
            isFinish = false;

            switch (key.state)
            {
                case VideoState.Load:
#if (UNITY_EDITOR || UNITY_STANDALONE)
                    // 에디터, Window Platform 비디오 로드 경로
                    StartCoroutine(Load(Application.dataPath + "/../Video/" + key.urlPath));
#elif UNITY_ANDROID
                    // 안드로이드 빌드 비디오 로드 경로
                    StartCoroutine(Load(Application.persistentDataPath + "/Video/" + key.urlPath));            
#endif
                    break;
                case VideoState.Play:
                    Play(key);
                    break;
                case VideoState.Pause:
                    Pause();
                    break;
                case VideoState.Stop:
                    Stop();
                    break;
                case VideoState.Seek:
                    Seek(key.sTime);
                    break;
                case VideoState.Repeat:
                    Repeat(key);
                    break;
                case VideoState.UnRepeat:
                    isRepeat = false;
                    break;
                case VideoState.Jump:
                    JumpTime(key.jumpLenth);
                    break;
                case VideoState.Loop:
                    MyVideoPlayer.isLooping = true;
                    isLoop = true;
                    break;
                case VideoState.UnLoop:
                    MyVideoPlayer.isLooping = false;
                    isLoop = false;
                    break;
                case VideoState.Restart:
                    StartCoroutine(RestartVideo(key));
                    break;
                case VideoState.SeekFrame:
                    SeekFrame(key.seekFrame);
                    break;
                case VideoState.End:
                    EndVideo();
                    break;
                case VideoState.WaitingPopup:
                    StartCoroutine(WaitingVideoPopup(MyVideoPlayer.url));
                    break;
                case VideoState.WaitingEndVideo:
                    StartCoroutine(WaitingEndVideo());
                    break;
                case VideoState.AnnouncementQuestionSeek:
                    Seek(AnnouncementQuestionSeek());
                    break;
            }

            // 해당 기능은 바로 Finish 처리 안 함
            if (key.state != VideoState.Load &&
                key.state != VideoState.Restart &&
                key.state != VideoState.WaitingPopup &&
                key.state != VideoState.WaitingEndVideo &&
                key.state != VideoState.Seek && key.state != VideoState.SeekFrame)
                isFinish = true;
        }

        ///// <summary>
        ///// Play Pause Stop 전용
        ///// </summary>
        ///// <param name="key"></param>
        //public void SetVideoCtrl(VideoState key)
        //{
        //    switch (key)
        //    {
        //        case VideoState.Play:
        //            Play();
        //            break;
        //        case VideoState.Pause:
        //            Pause();
        //            break;
        //        case VideoState.Stop:
        //            Stop();
        //            break;
        //    }
        //}


        /// <summary>
        /// Load 기능
        /// </summary>
        /// <param name="path">영상경로</param>
        /// <returns></returns>
        private IEnumerator Load(string path)
        {
            Debug.Log($"[Load/VR_VideoPlayer]\n비디오 경로 : <color=yellow>[{path}]</color>");

            // 비디오 재생 전 Bloom 효과 끄기
            if (fastBloom.enabled == true)
                fastBloom.enabled = false;

            if (path != "")
            {
                if (CheckFileExists(path))
                {
                    loadState = LoadState.Loading;
                    MyVideoPlayer.url = path; // 기존 영상 상태 초기화

                    MyVideoPlayer.Prepare();

                    while (!IsPrepared)
                    {
                        //Debug.Log("[Load] Preparing....");
                        yield return null;
                    }

                    Application.targetFrameRate = Convert.ToInt32(Math.Round(MyVideoPlayer.frameRate)); // 프레임 고정
                    //Debug.Log($"{Application.targetFrameRate}");

                    loadState = LoadState.Succeed;

                    if (debugText.gameObject.activeInHierarchy)
                    {
                        debugText.text = $"LoadState : [{loadState}]/ TargetFrame : [{Application.targetFrameRate.ToString()}]\n{path}";
                        if (File.Exists(path))
                        {
                            debugText.text = debugText.text + $"\n{path}\n{MyVideoPlayer.url}";
                        }
                    }
                    //Debug.Log($"[Load] Load 끝/ {loadState}");
                }
                else
                {
                    Debug.Log($"[{path}] File Not Exist");
                    loadState = LoadState.Fail;
                }
            }
            else
            {
                loadState = LoadState.Fail;
            }

            isFinish = true;
        }

        private IEnumerator LoadSeek()
        {
            // Load Seek 절차
            MyVideoPlayer.frame = Convert.ToInt64(0);
            yield return new WaitForSeconds(0.05f);

            MyVideoPlayer.frame = Convert.ToInt64(MyVideoPlayer.frameCount / 2);
            yield return new WaitForSeconds(0.05f);

            MyVideoPlayer.frame = Convert.ToInt64(MyVideoPlayer.frameCount);
            yield return new WaitForSeconds(0.1f);

            MyVideoPlayer.frame = Convert.ToInt64(0);
            yield return new WaitForSeconds(0.1f);

            loadState = LoadState.Succeed;
        }

        /// <summary>
        /// 영상 Play
        /// </summary>
        private void Play(VideoOption option)
        {
            if (!IsPrepared) return;
            if (loadState != LoadState.Succeed)
            {
                Debug.LogError("비디오가 로드되지 않았습니다.");
                debugText.text = $"{loadState.ToString()}/ <color=red> 비디오가 로드되지 않았습니다.</color>\nurl : {MyVideoPlayer.url}";

                return;
            }

            MyVideoPlayer.Play();
            endVideoCheck = false;

            // 비디오 자막 사용
            // 비디오는 중간에 멈췄다 재생했다 하기 때문에 처음 콘텐츠 시작에만 UseSubtitle로 콘텐츠 Type 할당
            // 이후에는 체크하지 않고 진행
            if (option.useSubtitle)
            {
                string[] splitUrl = MyVideoPlayer.url.Split('/');
                string targetUrl = splitUrl[splitUrl.Length - 1];

                SubtitleManager.Instance.SetSubtitle(option.subtitleType, targetUrl);
            }

            // 재생 영상 정보 Debug
            Debug.Log($"[Play/VR_VideoPlayer] 영상 재생 정보\nTime : <color=yellow>{MyVideoPlayer.time}</color>\n" +
                $"영상 길이 : <color=yellow>{Duration}초</color>\n" +
                $"Clip Frame : <color=yellow>{MyVideoPlayer.frameRate}</color>\n" +
                $"Frame : <color=yellow>{MyVideoPlayer.frameRate} => {Application.targetFrameRate}</color>\n");
        }

        public void Play()
        {
            if (!IsPrepared) return;
            if (Main.Instance.IsPaused)
            {
                Debug.LogError($"현재 Main이 Paused 상태 입니다 => [{Main.Instance.IsPaused}]");
                return;
            }
            if (loadState != LoadState.Succeed)
            {
                Debug.LogError("비디오가 로드되지 않았습니다.");
                debugText.text = $"{loadState.ToString()}/ <color=red> 비디오가 로드되지 않았습니다.</color>\nurl : {MyVideoPlayer.url}";

                return;
            }

            MyVideoPlayer.Play();

            // 재생 영상 정보 Debug
            Debug.Log($"[Play/VR_VideoPlayer] 영상 재생 정보\nTime : <color=yellow>{MyVideoPlayer.time}/ {Duration}</color>\n" +
                $"영상 길이 : <color=yellow>{Duration}초</color>\n" +
                $"Clip Frame : <color=yellow>{MyVideoPlayer.frameRate}</color>\n" +
                $"Frame : <color=yellow>{MyVideoPlayer.frameRate} => {Application.targetFrameRate}</color>\n");
        }

        /// <summary>
        /// 플레이O -> 일시정지
        /// </summary>
        public void Pause()
        {
            if (!IsPlaying)
            {
                Debug.Log($"Video Playing : <color=red>{MyVideoPlayer.isPlaying}</color> => Pause 실행하지 않음");
                return;
            }

            MyVideoPlayer.Pause();
        }

        /// <summary>
        /// 영상 재시작 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator RestartVideo(VideoOption option)
        {
            if (!IsPrepared) yield break;
            Pause();
            Seek(0);

            while (seekState != SeekState.Complete)
            {
                yield return null;
            }

            Play(option);
            isFinish = true;
        }

        private void Restart()
        {
            if (!IsPrepared) return;
            Pause();
            Seek(0);
        }

        /// <summary>
        /// 플레이O -> 정지
        /// </summary>
        private void Stop()
        {
            MyVideoPlayer.Stop();
        }

        /// <summary>
        /// 시간 점프 기능
        /// </summary>
        /// <param name="time">현재 시간에 추가로 점프하고자 하는 시간</param>
        private void JumpTime(double time)
        {
            double temp = MyVideoPlayer.time;

            temp += time;
            temp = Mathf.Clamp((float)temp, 0, Duration);

            MyVideoPlayer.time = temp;
        }


        /// <summary>
        /// Seek 기능
        /// </summary>
        /// <param name="key">SeekState: 일반적으로는 Move 사용하면된다.</param>
        private void Seek(VideoOption key)
        {
            key.sTime = Mathf.Clamp((float)key.sTime, 0, Duration);

            switch (key.seekState)
            {
                case SeekState.Down:
                    pastPlaying = videoPlayer.isPlaying;
                    isDraged = false;
                    seekState = SeekState.Down;
                    break;
                case SeekState.Drag:
                    isDraged = true;
                    Seek(key.sTime, SeekState.Drag);
                    break;
                case SeekState.Up:
                    if (!isDraged)
                    {
                        Seek(key.sTime, SeekState.Up);
                    }
                    else
                        seekState = SeekState.Up;
                    break;
            }
        }

        public void SeekFrame(long frame, SeekState state = SeekState.Order)
        {
            if (!IsPrepared) return;

            seekState = state;
            long frameCount = Convert.ToInt64(MyVideoPlayer.frameCount);

            long temp = Math.Clamp(frame, 0, frameCount);
            MyVideoPlayer.frame = temp;

            Debug.Log($"[Seek Frame] <color=cyan> {MyVideoPlayer.frame}/ {Frame}/ {temp}/ {Application.targetFrameRate} {frameCount} </color>");

        }

        private void Seek(double time, SeekState state = SeekState.Order)
        {
            if (!MyVideoPlayer.canSetTime) return;
            if (!IsPrepared) return;

            seekState = state;
            MyVideoPlayer.time = time;
            Debug.Log($"[Seek] Cur Time :<color=yellow> {VideoTime} </color>=> <color=yellow>{time}</color>");

        }

        /// <summary>
        /// 질문을 다르게 하기 위해 이전에 진행했던 발표 PPT에 따라 Seek 지점을 다르게 설정합니다.
        /// </summary>
        private double AnnouncementQuestionSeek()
        {
            VideoContentMgr curContentMgr = FindObjectOfType<VideoContentMgr>();
            double SeekPoint = 0;

            if (curContentMgr != null)
            {
                AnnouncementPPTMgr curPPTMgr = curContentMgr.PPTList.Find(x => x.aType == curContentMgr.curAnnouncementType);

                if (curPPTMgr != null)
                {
                    switch (curPPTMgr.aType)
                    {
                        case AnnouncementType.K_Pop: SeekPoint = 169; break; 
                        case AnnouncementType.Sustainable: SeekPoint = 222; break;
                        case AnnouncementType.Animal: SeekPoint = 197; break;
                    }
                }
                else
                {
                    Debug.Log($"이전 진행했던 PPT Type<color=red>[{curContentMgr.curAnnouncementType}]</color> 찾을 수 없습니다.");
                }
            }

            return SeekPoint;
        }

        /// <summary>
        /// 특정 구간 반복 기능
        /// </summary>
        /// <param name="key"></param>
        private void Repeat(VideoOption key)
        {
            isRepeat = true;

            repeatSTime = key.sTime;
            repeatETime = key.eTime;

            if (VideoTime < repeatSTime || repeatETime < VideoTime)
                Seek(repeatSTime);
        }

        /// <summary>
        /// 영상 끝내고 VR 설정으로 복귀
        /// </summary>
        private void EndVideo()
        {
            if (MyVideoPlayer.isPlaying)
                MyVideoPlayer.Stop();

            if (fastBloom.enabled == false)
                fastBloom.enabled = true;

            Application.targetFrameRate = -1; // 프레임 초기화

            MyVideoPlayer.url = "";

            Debug.Log($"url:[{MyVideoPlayer.url}]/ Frame Rate : [{Application.targetFrameRate}]");
        }

        private IEnumerator WaitingVideoPopup(string url)
        {
            // 공황 대처하기 팝업 나오기 전까지 영상 Play 시간
            float waitTime = 0;
            float checkTime = 0;

            // 영상 이름 체크
            switch (url)
            {
                // 2차년도
                case var target when url.Contains("Elevator_01"): waitTime = 49.5f; break; // Elevator 1단계
                case var target when url.Contains("Elevator_02"): waitTime = 56; break; // Elevator 2단계
                case var target when url.Contains("Elevator_03"): waitTime = 69; break; // Elevator 3단계
                case var target when url.Contains("Theater_01"): waitTime = 78; break; // Theater 1단계
                case var target when url.Contains("Theater_02"): waitTime = 29; break; // Theater 2단계
                case var target when url.Contains("Theater_03"): waitTime = 62.5f; break; // Theater 3단계
                case var target when url.Contains("Bus_01"): waitTime = 55; break; // Bus 1단계
                case var target when url.Contains("Bus_02"): waitTime = 75; break; // Bus 2단계
                case var target when url.Contains("Bus_03"): waitTime = 53; break; // Bus 3단계

                // 3차년도
                case var target when url.Contains("Interview_Intro"): waitTime = 5; break; // Interview Intro
                case var target when url.Contains("Interview_01"): waitTime = 5; break;  // Interview 1단계(자기소개 하기)
                case var target when url.Contains("Interview_02"): waitTime = 99; break;  // Interview 2단계(장단점 말하기)
                case var target when url.Contains("Interview_03"): waitTime = 99; break;  // Interview 3단계(사회적 이슈 말하기)
                case var target when url.Contains("Announcement_Intro"): waitTime = 0; break;  // Announcement Intro
                case var target when url.Contains("Announcement_01"): waitTime = 99; break;  // Announcement 1단계(질의응답 하기)
                case var target when url.Contains("Announcement_02"): waitTime = 99; break;  // Announcement 2단계(시간재촉)
                case var target when url.Contains("Announcement_03"): waitTime = 99; break; // Announcement 3단계(돌발상황 발생)

            }

            while (checkTime <= waitTime)
            {
                //Debug.Log(checkTime);

                // Stop 버튼으로 MainMene로 Return 되면 break
                if (Main.curSceneID.Contains("MainMenu"))
                {
                    //Main.Instance.IsVideoPlaying_Paused = false;
                    isFinish = true;
                    Debug.Log("<color=red>WaitingVideoPopup Routine break</color>");
                    yield break;
                }

                if (Main.Instance.IsPlaying)
                {
                    //Debug.Log($"VideoPopup /{checkTime}");

                    checkTime += Time.deltaTime;
                }
                yield return null;

            }

            Debug.Log($"[VideoPopup] 현재 영상 시간 : <color=cyan>{MyVideoPlayer.time}</color>/ Popup 대기 시간 : <color=cyan>[{checkTime}/ {waitTime}]]</color> 초 Wait 끝");

            Pause();

            // [2024.08.02] 주석 처리
            //Main.Instance.IsVideoPlaying_Paused = false;
            isFinish = true;
        }

        /// <summary>
        /// 영상이 끝날 떄까지 대기(코루틴 루프)
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitingEndVideo()
        {
            while (!endVideoCheck)
            {
                yield return null;
            }

            Pause();
            endVideoCheck = false;
            isFinish = true;
        }

        #region VideoPlayerEvent

        /// <summary>
        /// 영상 반복 기능을 정지합니다.
        /// </summary>
        public void StopRepeat()
        {
            if (isRepeat)
                isRepeat = false;
        }

        /// <summary>
        /// 영상 관련 디버그
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        private void ErrorReceived_Event(VideoPlayer source, string message)
        {
            Debug.Log("[" + name + "] play Err : " + message);
        }
        private void FrameReady_Event(VideoPlayer source, long frameIdx)
        {
            Debug.Log("[" + name + "] FrameReady : " + frameIdx);
        }
        private void LoopPointReached_Event(VideoPlayer source)
        {
            endVideoCheck = true;
            Debug.Log($"[LoopPointReached_Event]\n<color=yellow>[{MyVideoPlayer.url}]</color> LoopPointReached");
        }
        private void PrepareCompleted_Event(VideoPlayer source)
        {
            //Debug.Log("[" + name + "] Video PrepareCompleted");
        }
        private void SeekCompleted_Event(VideoPlayer source)
        {
            if (seekState == SeekState.Drag) return;

            if (m_lateSeek_Routine != null)
                StopCoroutine(m_lateSeek_Routine);
            m_lateSeek_Routine = LateSeek_Routine();

            StartCoroutine(m_lateSeek_Routine);

        }
        private void Started_Event(VideoPlayer source)
        {
            //Debug.Log("[" + name + "] Started");
        }

        private IEnumerator m_lateSeek_Routine;
        private IEnumerator LateSeek_Routine()
        {
            yield return new WaitForSeconds(0.1f);
            seekState = SeekState.Complete;
            isFinish = true;

            Debug.Log("<color=cyan> SeekCompleted </color>");

        }
        #endregion

        private bool CheckFileExists(string path)
        {
            FileInfo file = new FileInfo(path);

            if (file.Exists)
                return true;
            else
                return false;
        }
    }
}