/// �ۼ���: ��·�
/// �ۼ���: 2020-08-24
/// ������: 2020-08-27
/// ���۱�: Copyright(C) FNI Co., LTD. 
/// �����̷�
/// 

/// ���������������������������������������൵������������������������������������������������������������������������������������������������������������
/// ��                                                                            ��
/// ��start (�ѹ�): �ű� ���� ���� �մϴ�.                                          ��
/// ��update(�ݺ�): ���� Finish �ɶ����� �ݺ��˴ϴ�.                                ��
/// ��Check  (�ѹ�): ���� ���� �ִ��� Ȯ���ϰ� ������ Start ������ End�� �Ѿ�ϴ�.  �� 
/// ��End (�ѹ�): ���� �������� �����ϸ� Start �� �̵��մϴ�.                        ��
/// ��                                                                            ��
/// ��        ������������������������������������������                                              ��
/// ��     ����������������  ����������������  ������������������  ����������������                                ��
/// ��     ��Start ������Update������ Check ������  End ��                                ��
/// ��     ����������������  ��������������   ������������������   ����������������                               ��
/// ��        ����������������������������������������������������������������                                   ��
/// ������������������������������������������������������������������������������������������������������������������������������������������������������������

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FNI
{
    public class Subtitle_Main : MonoBehaviour
    {
        #region Singleton
        private static Subtitle_Main _instance;
        public static Subtitle_Main Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<Subtitle_Main>();

                return _instance;
            }
        }
        #endregion

        public static string curSceneID;
        public SceneData CurSequence { get => curSequence; }
        /// <summary>
        /// ���� ������ ������
        /// </summary>
        [SerializeField] private SceneData curSequence;

        /// <summary>
        /// ������ �����Ϳ� �°� ���� ������ ������Ʈ ����Ʈ
        /// </summary>
        private List<IVisualObject> cutObject_List = new List<IVisualObject>();

        public SequenceState SequenceState { get => sequenceState; set => sequenceState = value; }
        /// <summary>
        /// ���� ������ ����
        /// </summary>
        public SequenceState sequenceState;

        /// <summary>
        /// ���� �� Index
        /// </summary>
        private int cutIndex = 0;
        public int CutIndex { get => cutIndex; }

        /// <summary>
        /// ���� �������� �� ������
        /// </summary>
        private CutData cur_CutData;
        /// <summary>
        /// ���� �� �����Ǵ�
        /// </summary>
        public bool IsStarted { get { return isStarted; } }
        private bool isStarted = false;

        /// <summary>
        /// �� ���� Ȯ��
        /// </summary>
        public bool IsPlaying { get { return isPlaying; } }
        private bool isPlaying = false;
        /// <summary>
        /// ���� �� ������Ʈ
        /// </summary>
        private IVisualObject cur_VisualObj;

        public Text debugText;


        private void Awake() // GameObject�� Ȱ��ȭ �������� ȣ��
        {
            sequenceState = SequenceState.Setting;
            Setting();

            //sequenceState = SequenceState.Start;
        }

        private void Start() // GameObject/ Script ��� Ȱ��ȭ �Ǿ��־�� ȣ��
        {

        }

        private void Update()
        {
            switch (sequenceState)
            {
                case SequenceState.Start: CutStart(); break;
                case SequenceState.Update: CutUpdate(); break; // => Fixed Update �� �̵�
                case SequenceState.Check: Check(); break;
                case SequenceState.End: End(); break;
                default: break;
            }

        }

        private void OnDisable()
        {
            curSequence = null;
        }

        //private void FixedUpdate()
        //{
        //    switch (sequenceState)
        //    {
        //        case SequenceState.Update: CutUpdate(); break;
        //    }

        //}
        #region Cut Loop

        /// <summary>
        /// �� ó�� �����Ҷ� 1ȸ �����ϴ� �Լ��Դϴ�. 
        /// </summary>
        private void CutStart()
        {
            isPlaying = true;

            cur_CutData = GetCutData(cutIndex);

            // (Line �� �⺻ ������)
            if (cur_CutData.visualType != VisualType.Line &&
                cur_CutData.visualType != VisualType.Quit)
            {
                // Ư�� ����� �����ϴ� Class�� VisualType�� �����ϰ��� �ϴ� Cut�� Type�� ���Ͽ� List���� ����
                cur_VisualObj = cutObject_List.Find(x => x.Type == cur_CutData.visualType);

                // �� Cut�� ������ �´� Active ����
                cur_VisualObj.Active(cur_CutData);
            }

            sequenceState = SequenceState.Update;
        }

        /// <summary>
        /// Start ���� �ݺ������� Update �ϴ� �Լ��Դϴ�.
        /// </summary>
        private void CutUpdate()
        {
            if (!cur_VisualObj.IsFinish)
            {
                cur_VisualObj.MyUpdate();
            }
            else
            {
                sequenceState = SequenceState.Check;
            }
        }

        /// <summary>
        /// ���� ���� �ִ��� üũ�մϴ�. ������ -> Start / ���ٸ� -> End
        /// </summary>
        private void Check()
        {
            //Debug.Log($"{cutIndex}/ {curSequence.cutDataList[Main.Instance.CutIndex].visualType}/ {checkSubtitleList_index}");
            // ���� ���� ���� ��
            if (cutIndex < curSequence.cutDataList[Main.Instance.CutIndex].narrationOption.checkSubtitleList[checkSubtitleList_index].checkEventList.Count - 1)
            {
                cutIndex++;
                sequenceState = SequenceState.Start;
            }
            // ���� ���� ���� ��( ������ �� ���� ���� ��)
            else
            {
                sequenceState = SequenceState.End;
                return;
            }
        }

        /// <summary>
        /// Subtitle_Main CutLoop ��
        /// </summary>
        private void End()
        {
            isPlaying = false;
            if (curSequence.cutDataList[Main.Instance.CutIndex].narrationOption.checkSubtitleList[checkSubtitleList_index].checkEventList[cutIndex].checkPoint_End)
            {
                SubtitleManager.Instance.checkPoint_End = true;
                Debug.Log($"Check Point End �ڵ� üũ/ CheckPoint_End : <color=cyan>{ SubtitleManager.Instance.checkPoint_End}</color>");
            }

            cutIndex = 0;
            sequenceState = SequenceState.Setting;


        }

        public void EndCut()
        {
            sequenceState = SequenceState.Check;
            End();
        }

        #endregion

        /// <summary>
        /// �ڸ� CheckPoint List�� ��ȣ
        /// </summary>
        private int checkSubtitleList_index;

        /// <summary>
        /// Subtitle_Main�� CutLoop�� ���۽�ŵ�ϴ�.
        /// </summary>
        /// <param name="index"> ����� �ڸ� ��� �� Check Point ����Ʈ ��ȣ </param>
        public void StartCutLoop(int index)
        {
            curSequence = Main.Instance.CurSequence;
            checkSubtitleList_index = index;
            Debug.Log($"[StartCutLoop/Subtitle_Main] CheckPoint List index : <color=magenta>[{index}]</color>/\n[{curSequence}]");

            sequenceState = SequenceState.Start;
        }

        /// <summary>
        /// �ʿ��� �����͸� �Ҵ��ϰ� �ʱ�ȭ�մϴ�.
        /// </summary>
        private void Setting()
        {
            cutObject_List.Add(GameObject.Find("---------------UI").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Scripts/FadeInOutForSequence").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Scripts/NarrationForSequence").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Video").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Scripts/LayForSequence").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Scripts/WaitForSequence").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Scripts/SceneMgrForSequence").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Scripts/AnimationForSequence").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Scripts/TimerForSequence").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Scripts/XRManagerForSequence").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Object").GetComponent<IVisualObject>());

            // Subtitle_Main�� ���ص� ��
            //for (int i = 0; i < cutObject_List.Count; i++)
            //{
            //    cutObject_List[i].Init();
            //}
        }

        /// <summary>
        /// ���� ������ ���� �����մϴ�./// </summary>
        /// <param name="cutIndex">������ �� index</param>
        /// <returns></returns>
        public CutData GetCutData(int cutIndex)
        {
            Debug.Log($"���� Check �ڸ� ��ȣ : <color=cyan>[{curSequence.cutDataList[Main.Instance.CutIndex].narrationOption.checkSubtitleList[checkSubtitleList_index].checkSubtitleNum}]</color>\n " +
                $"Check Point Cut ��ȣ : <color=cyan>[{cutIndex}/ {curSequence.cutDataList[Main.Instance.CutIndex].narrationOption.checkSubtitleList[checkSubtitleList_index].checkEventList.Count - 1}]</color>\n " +
                $"Main ���� Sequence : <color=cyan>[{curSequence.name}]</color>\n " +
                $"Main ���� Type : <color=cyan>[{curSequence.cutDataList[Main.Instance.CutIndex].visualType}]</color>\n " +
                $"Subtitle_Main ���� Type : <color=cyan>[{curSequence.cutDataList[Main.Instance.CutIndex].narrationOption.checkSubtitleList[checkSubtitleList_index].checkEventList[cutIndex].visualType}]</color>");


            return curSequence.cutDataList[Main.Instance.CutIndex].narrationOption.checkSubtitleList[checkSubtitleList_index].checkEventList[cutIndex];
        }


        /// <summary>
        /// ��ư�̺�Ʈ�� ���� Other ������
        /// </summary>
        /// <param name="scene"></param>
        public void OnButtonSequence(SceneData scene)
        {
            ChangeSequence(scene);
            cutIndex = 0;

            sequenceState = SequenceState.Start;
        }


        private void ChangeSequence(SceneData data)
        {
            curSequence = data;
            curSceneID = curSequence.sceneID;
        }
        public void ChangeScene(SceneData scene)
        {
            ChangeSequence(scene);
            cutIndex = 0;

            sequenceState = SequenceState.Start;
        }

        public void ResetSubtitle_Main()
        {
            curSequence = null;
            sequenceState = SequenceState.Setting;
        }
    }
}