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

namespace FNI
{
    public class Main : MonoBehaviour
    {
        #region Singleton
        private static Main _instance;
        public static Main Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<Main>();

                return _instance;
            }
        }
        #endregion

        public static string curSceneID = "";
        public SceneData CurSequence { get => curSequence; }
        /// <summary>
        /// 현재 시퀀스 데이터
        /// </summary>
        [SerializeField] private SceneData curSequence;

        public SceneData returnSceneData;

        [SerializeField] private SequenceState sequenceState;
        [SerializeField] private VisualType curVisualType;

        /// <summary>
        /// 시퀀스 데이터에 맞게 실행 시켜줄 오브젝트 리스트
        /// </summary>
        private List<IVisualObject> cutObject_List = new List<IVisualObject>();

        [Space(20)]

        /// <summary>
        /// HMD가 미착용 되었을 때 몇 초 후에 Reset할 건지
        /// </summary>
        [SerializeField] private float hmdResetTime = 10.0f;
        [SerializeField] private SceneData hmdResetSceneData;
        private float hmdResetCheckTime = 0.0f;
        private float PAUSE_TIMESCAPLE;
        /// <summary>
        /// 현재 HMD가 착용 중인지 체크
        /// </summary>
        private bool isHmdPlay;
        private bool onceCheck = false;

        /// <summary>
        /// 현재 시퀀스 상태
        /// </summary>

        public int CutIndex { get => cutIndex; }
        /// <summary>
        /// 현재 컷 Index
        /// </summary>
        private int cutIndex = 0;

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

        public bool IsVideoPlayingCheck { get => isVideoPlayingCheck; set => isVideoPlayingCheck = value; }
        /// <summary>
        /// 콘텐츠 중지 시점에서 비디오 플레이 상태 체크
        /// </summary>
        private bool isVideoPlayingCheck = false;

        /// <summary>
        /// Pause 모드 진입 전 State = Start 처리를 위한 지연 코루틴
        /// </summary>
        IEnumerator pauseDelayRoutine;
        /// <summary>
        /// Pause 모드 진입 후 해제까지 대기 코루틴
        /// </summary>
        IEnumerator pauseRoutine;

        /// <summary>
        /// 콘텐츠 정지시 Popup UI
        /// [0] : Elevator, [1] : Theater, [2] : Bus
        /// </summary>
        public List<GameObject> stopUI_PopupList;

        private void Awake()
        {
            sequenceState = SequenceState.Setting;

            Setting();

            sequenceState = SequenceState.Start;
        }

        private void Update()
        {
            // 개발 중일 때는 헤드셋 Reset 기능 끔
            //HmdCheckUpdate();

            switch (sequenceState)
            {
                case SequenceState.Start: CutStart(); break; // Active
                //case SequenceState.Update: CutUpdate(); break; // => Fixed Update 로 이동
                case SequenceState.Check: Check(); break;
                case SequenceState.End: End(); break;
                case SequenceState.Pause:
                    if (pauseRoutine == null)
                    {
                        pauseRoutine = Pause_StandBy();
                        StartCoroutine(pauseRoutine);
                    }
                    break;
                default: break;
            }

            curVisualType = cur_CutData.visualType;
        }

        private void FixedUpdate()
        {
            switch (sequenceState)
            {
                case SequenceState.Update: CutUpdate(); break;
            }

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

            for (int i = 0; i < cutObject_List.Count; i++)
            {
                cutObject_List[i].Init();
            }
        }

        /// <summary>
        /// 컷 진행 루프 사이클
        /// </summary>
        #region Cut Loop

        public void CutSkip()
        {
            if (curSequence.canSkip)
            {
                sequenceState = SequenceState.Check;
                Debug.Log($"<color=yellow> Skip </color>");
            }
            else
                Debug.Log($"<color=red> [{curSequence.name}] CanSkip : {curSequence.canSkip} </color>");
        }

        /// <summary>
        /// 컷 처음 시작할때 1회 실행하는 함수입니다. 
        /// </summary>
        private void CutStart()
        {
            isPlaying = true;
            cur_CutData = GetCutData(cutIndex);

            //Debug.Log($"[Start]<color=green>{cutIndex}/ {cur_CutData.visualType}</color>");

            if (curSceneID == "")
                curSceneID = curSequence.sceneID;

            if (cutIndex == 0)
            {
                //Debug.Log($"<color=cyan> {CurSequence.name}</color> 시퀀스 Start ");

                //if (!curSequence.canCameraView && CameraManager.Instance.CameraViewType == CameraViewType.firstPerson)
                //{
                //    StartCoroutine(ResetCameraView());
                //}
            }

            //Debug.Log($"[Start!!] <color=yellow>{cur_CutData.visualType}/ {sequenceState}</color>");

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
            // 콘텐츠를 종료합니다.
            if (cur_CutData.visualType == VisualType.Quit)
            {
                Application.Quit();
            }
            else if (!cur_VisualObj.IsFinish)
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
            // 다음 컷이 있을 때
            if (cutIndex < curSequence.cutDataList.Count - 1)
            {
                cutIndex++;
                sequenceState = SequenceState.Start;
                //Debug.Log($" <color=green>{cutIndex}</color>/ {curSequence.cutDataList[cutIndex].visualType}/ {sequenceState}");

            }
            // 다음 컷이 없을 때( 마지막 컷 진행 했을 때)
            else
            {
                sequenceState = SequenceState.End;
                //Debug.Log("End..");
                return;
            }
        }

        /// <summary>
        /// 다음 시퀀스가 있는지 체크합니다. 있으면 -> 새로운 시퀀스로 start 
        /// </summary>
        private void End()
        {
            isPlaying = false;

            if (curSequence.nextScene != null)
            {
                ChangeSequence(curSequence.nextScene);
                cutIndex = 0;

                sequenceState = SequenceState.Start;
            }

        }

        public void EndCut()
        {
            sequenceState = SequenceState.Check;
            End();
        }

        /// <summary>
        /// Cut Loop 일시정지
        /// </summary>
        public void Pause()
        {
            if (pauseDelayRoutine == null)
            {
                pauseDelayRoutine = Pause_Delay(0.2f);
                StartCoroutine(pauseDelayRoutine);
            }
        }

        /// <summary>
        /// Pause 로 State 변경하기 전 Cut(Start) 처리 시간 대기 루틴
        /// </summary>
        /// <param name="sec"> Pause 로 State 변경 전 딜레이 시간</param>
        /// <returns></returns>
        IEnumerator Pause_Delay(float sec)
        {
            isPlaying = false;

            if (VR_VideoPlayer.Instance.MyVideoPlayer.isPlaying)
            {
                VR_VideoPlayer.Instance.Pause();
                isVideoPlayingCheck = true;
            }

            yield return new WaitForSeconds(sec);

            sequenceState = SequenceState.Pause;

            switch (curSceneID) // Main State가 Pause가 되면 Popup되도록
            {
                case var ID when curSceneID.Contains("Elevator"):
                    if (!stopUI_PopupList[0].activeInHierarchy)
                        stopUI_PopupList[0].SetActive(true);
                    break;
                case var ID when curSceneID.Contains("Theater"):
                    if (!stopUI_PopupList[1].activeInHierarchy)
                        stopUI_PopupList[1].SetActive(true);
                    break;
                case var ID when curSceneID.Contains("Bus"):
                    if (!stopUI_PopupList[2].activeInHierarchy)
                        stopUI_PopupList[2].SetActive(true);
                    break;
            }
            Debug.Log($"<color=cyan>{sequenceState}</color> !!");
        }

        /// <summary>
        /// Pause 이후 CutLoop를 다시 시작합니다.
        /// </summary>
        public void Restart_CutLoop()
        {
            sequenceState = SequenceState.Update;
            Debug.Log($"[Restart_CutLoop] <color=cyan>{sequenceState}</color>");
        }

        /// <summary>
        /// Pause가 해제되기를 기다리는 루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator Pause_StandBy()
        {
            //isPlaying = false;

            while (sequenceState == SequenceState.Pause)
            {
                //Debug.Log($"Pause Waiting...");

                yield return null;
            }

            yield return new WaitForSeconds(0.1f); // Update => Start/End 체크를 위한 후순위 대기
            isPlaying = true;

            if (isVideoPlayingCheck)
            {
                isVideoPlayingCheck = false;

                // No 버튼일때만 Play
                if (curSequence != returnSceneData)
                    VR_VideoPlayer.Instance.Play();
            }

            pauseRoutine = null;
            pauseDelayRoutine = null;

            Debug.Log($"[Cut 재시작] <color=magenta>{curSequence.name}/ " +
                $"{sequenceState}/ " +
                $"{cur_CutData.visualType}/ " +
                $"[{curSequence.cutDataList[cutIndex].visualType}]</color>");
        }


        #endregion


        /// <summary>
        /// 현재 시퀀스 컷을 추출합니다./// </summary>
        /// <param name="cutIndex">추출할 컷 index</param>
        /// <returns></returns>
        public CutData GetCutData(int cutIndex)
        {
            return curSequence.cutDataList[cutIndex];
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

        private IEnumerator ResetCameraView()
        {
            while (FadeInOutForSequence.Instance.canvasGroups[2].alpha != 1)
            {
                yield return null;
            }

            CameraManager.Instance.ResetCameraView();

        }

        /// <summary>
        /// 다른 시퀀스로 이동(외부 참조 불가능)
        /// </summary>
        /// <param name="data"></param>
        private void ChangeSequence(SceneData data)
        {
            curSequence = data;
            curSceneID = curSequence.sceneID;
        }

        /// <summary>
        /// 다른 시퀀스로 이동(외부 참조 가능)
        /// </summary>
        /// <param name="scene"></param>
        public void ChangeScene(SceneData scene)
        {
            ChangeSequence(scene);
            cutIndex = 0;

            sequenceState = SequenceState.Start;
        }

        public void SetNextScene(SceneData data)
        {
            curSequence.nextScene = data;
        }

        #region HMD Event
        /// <summary>
        /// Hmd 일시정지 이벤트
        /// </summary>
        private void HmdPauseEvent()
        {
            isHmdPlay = false;
            hmdResetCheckTime = 0.0f;
        }

        /// <summary>
        /// Hmd 시작 이벤트
        /// </summary>
        private void HmdPlayEvnet()
        {
            isHmdPlay = true;
        }

        /// <summary>
        /// Hmd가 미착용 되었을 때 Update하는 함수
        /// </summary>
        private void HmdCheckUpdate()
        {
            if (!isHmdPlay)
            {
                if (onceCheck)
                    hmdResetCheckTime += Time.deltaTime * PAUSE_TIMESCAPLE;
                else
                    onceCheck = true;

                // Reset 씬으로
                if (hmdResetCheckTime > hmdResetTime)
                {
                    print("HMD Reset");
                    NarrationForSequence narration = (NarrationForSequence)cutObject_List.Find(x => x.Type == VisualType.Narration);
                    narration.HMDStop();

                    FadeInOutForSequence fadeInOut = (FadeInOutForSequence)cutObject_List.Find(x => x.Type == VisualType.FadeInOut);
                    fadeInOut.HMDStop();

                    AnimationForSequence anim = (AnimationForSequence)cutObject_List.Find(x => x.Type == VisualType.Animation);
                    anim.ResetAnimation();

                    ChangeSequence(hmdResetSceneData);
                    cutIndex = 0;
                    sequenceState = SequenceState.Start;
                }
            }
        }
        #endregion
    }
}