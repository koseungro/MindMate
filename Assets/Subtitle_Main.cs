/// 작성자: 고승로
/// 작성일: 2020-08-24
/// 수정일: 2020-08-27
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

/// ┌────────────────컷진행도─────────────────────────────────────────────────────┐
/// │                                                                            │
/// │start (한번): 신규 컷을 실행 합니다.                                          │
/// │update(반복): 컷이 Finish 될때까지 반복됩니다.                                │
/// │Check  (한번): 다음 컷이 있는지 확인하고 있으면 Start 없으면 End로 넘어갑니다.  │ 
/// │End (한번): 다음 시퀀스가 존재하면 Start 로 이동합니다.                        │
/// │                                                                            │
/// │        ┌──────────◁────────┐                                              │
/// │     ┌──┴───┐  ┌──────┐  ┌───┴───┐  ┌──────┐                                │
/// │     │Start ├▷┤Update├▷┤ Check ├▷┤  End │                                │
/// │     └──┬───┘  └─────┘   └───────┘   └──────┘                               │
/// │        └────────────◁─────────────────┘                                   │
/// └────────────────────────────────────────────────────────────────────────────┘

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
        /// 현재 시퀀스 데이터
        /// </summary>
        [SerializeField] private SceneData curSequence;

        /// <summary>
        /// 시퀀스 데이터에 맞게 실행 시켜줄 오브젝트 리스트
        /// </summary>
        private List<IVisualObject> cutObject_List = new List<IVisualObject>();

        public SequenceState SequenceState { get => sequenceState; set => sequenceState = value; }
        /// <summary>
        /// 현재 시퀀스 상태
        /// </summary>
        public SequenceState sequenceState;

        /// <summary>
        /// 현재 컷 Index
        /// </summary>
        private int cutIndex = 0;
        public int CutIndex { get => cutIndex; }

        /// <summary>
        /// 현재 진행중인 컷 데이터
        /// </summary>
        private CutData cur_CutData;
        /// <summary>
        /// 최초 컷 시작판단
        /// </summary>
        public bool IsStarted { get { return isStarted; } }
        private bool isStarted = false;

        /// <summary>
        /// 컷 진행 확인
        /// </summary>
        public bool IsPlaying { get { return isPlaying; } }
        private bool isPlaying = false;
        /// <summary>
        /// 현재 컷 오브젝트
        /// </summary>
        private IVisualObject cur_VisualObj;

        public Text debugText;


        private void Awake() // GameObject만 활성화 돼있으면 호출
        {
            sequenceState = SequenceState.Setting;
            Setting();

            //sequenceState = SequenceState.Start;
        }

        private void Start() // GameObject/ Script 모두 활성화 되어있어야 호출
        {

        }

        private void Update()
        {
            switch (sequenceState)
            {
                case SequenceState.Start: CutStart(); break;
                case SequenceState.Update: CutUpdate(); break; // => Fixed Update 로 이동
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
        /// 컷 처음 시작할때 1회 실행하는 함수입니다. 
        /// </summary>
        private void CutStart()
        {
            isPlaying = true;

            cur_CutData = GetCutData(cutIndex);

            // (Line 은 기본 설정용)
            if (cur_CutData.visualType != VisualType.Line &&
                cur_CutData.visualType != VisualType.Quit)
            {
                // 특정 기능을 관리하는 Class의 VisualType을 실행하고자 하는 Cut의 Type과 비교하여 List에서 추출
                cur_VisualObj = cutObject_List.Find(x => x.Type == cur_CutData.visualType);

                // 각 Cut의 유형의 맞는 Active 실행
                cur_VisualObj.Active(cur_CutData);
            }

            sequenceState = SequenceState.Update;
        }

        /// <summary>
        /// Start 이후 반복적으로 Update 하는 함수입니다.
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
        /// 다음 컷이 있는지 체크합니다. 있으면 -> Start / 없다면 -> End
        /// </summary>
        private void Check()
        {
            //Debug.Log($"{cutIndex}/ {curSequence.cutDataList[Main.Instance.CutIndex].visualType}/ {checkSubtitleList_index}");
            // 다음 컷이 있을 때
            if (cutIndex < curSequence.cutDataList[Main.Instance.CutIndex].narrationOption.checkSubtitleList[checkSubtitleList_index].checkEventList.Count - 1)
            {
                cutIndex++;
                sequenceState = SequenceState.Start;
            }
            // 다음 컷이 없을 때( 마지막 컷 진행 했을 때)
            else
            {
                sequenceState = SequenceState.End;
                return;
            }
        }

        /// <summary>
        /// Subtitle_Main CutLoop 끝
        /// </summary>
        private void End()
        {
            isPlaying = false;
            if (curSequence.cutDataList[Main.Instance.CutIndex].narrationOption.checkSubtitleList[checkSubtitleList_index].checkEventList[cutIndex].checkPoint_End)
            {
                SubtitleManager.Instance.checkPoint_End = true;
                Debug.Log($"Check Point End 자동 체크/ CheckPoint_End : <color=cyan>{ SubtitleManager.Instance.checkPoint_End}</color>");
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
        /// 자막 CheckPoint List의 번호
        /// </summary>
        private int checkSubtitleList_index;

        /// <summary>
        /// Subtitle_Main의 CutLoop를 시작시킵니다.
        /// </summary>
        /// <param name="index"> 오디오 자막 재생 중 Check Point 리스트 번호 </param>
        public void StartCutLoop(int index)
        {
            curSequence = Main.Instance.CurSequence;
            checkSubtitleList_index = index;
            Debug.Log($"[StartCutLoop/Subtitle_Main] CheckPoint List index : <color=magenta>[{index}]</color>/\n[{curSequence}]");

            sequenceState = SequenceState.Start;
        }

        /// <summary>
        /// 필요한 데이터를 할당하고 초기화합니다.
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

            // Subtitle_Main은 안해도 됨
            //for (int i = 0; i < cutObject_List.Count; i++)
            //{
            //    cutObject_List[i].Init();
            //}
        }

        /// <summary>
        /// 현재 시퀀스 컷을 추출합니다./// </summary>
        /// <param name="cutIndex">추출할 컷 index</param>
        /// <returns></returns>
        public CutData GetCutData(int cutIndex)
        {
            Debug.Log($"현재 Check 자막 번호 : <color=cyan>[{curSequence.cutDataList[Main.Instance.CutIndex].narrationOption.checkSubtitleList[checkSubtitleList_index].checkSubtitleNum}]</color>\n " +
                $"Check Point Cut 번호 : <color=cyan>[{cutIndex}/ {curSequence.cutDataList[Main.Instance.CutIndex].narrationOption.checkSubtitleList[checkSubtitleList_index].checkEventList.Count - 1}]</color>\n " +
                $"Main 진행 Sequence : <color=cyan>[{curSequence.name}]</color>\n " +
                $"Main 진행 Type : <color=cyan>[{curSequence.cutDataList[Main.Instance.CutIndex].visualType}]</color>\n " +
                $"Subtitle_Main 진행 Type : <color=cyan>[{curSequence.cutDataList[Main.Instance.CutIndex].narrationOption.checkSubtitleList[checkSubtitleList_index].checkEventList[cutIndex].visualType}]</color>");


            return curSequence.cutDataList[Main.Instance.CutIndex].narrationOption.checkSubtitleList[checkSubtitleList_index].checkEventList[cutIndex];
        }


        /// <summary>
        /// 버튼이벤트를 통한 Other 시퀀스
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