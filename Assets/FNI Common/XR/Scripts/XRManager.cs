/// 작성자: 조효련
/// 작성일: 2022-01-28
/// 수정일: 
/// 저작권: Copyright(C) FNIKorea Co., Ltd.. (주)에프앤아이코리아

using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using FNI;
using System.Collections;

namespace FNI.XR
{
    // Packages/com.unity.xr.interaction.toolkit/Runtime/Interaction/XRInteractionManager.cs

    // XR Origin
    //  XR Manager
    //      XR Interaction Manager
    //      XR Device Simulator
    //      XR EventSystem
    //  Camera Offset
    //      Main Camera
    //          UI Camera
    //      LeftHand Controller (ActionBasedController, XRRayInteractor, XRInteractorLineVisual, LineRender, XROculusController, XROculusControllerAnimator)
    //          OculusTouchForQuest2_Left
    //      RightHand Controller
    //          OculusTouchForQuest2_Right

    // @OculusControllerControls


    /// <summary>
    /// 컨트롤러 연결 확인
    /// 컨트롤러 값 확인
    /// 
    /// UI 터치 - 기본값 : 오른쪽
    ///     검지손가락으로 트리거 버튼을 눌러 작동
    /// 
    ///     훈련 종료 기능
    ///         훈련종료 버튼은 UI가 없는 빈 곳에서 트리거 버튼이 눌리면 표시되고 3초간 사용이 없으면 사라짐
    ///         (Opacity 표시0.5초 숨김 2초간 Animation 제외한 표시시간 3초)  
    ///         해당기능은 #main 이후로 항상 존재
    /// 
    /// 태블릿 관리 - 기본값 : 왼손
    ///     의무기록 태블릿은 “산전관리 문진” 과 “결과설명” 훈련 에서만 보여짐
    ///     좌측 컨트롤러 엄지 스틱 
    ///     좌우로 넘기면 항목 좌 우 로 이동해 페이지 전환
    ///     상하로 움직여 페이지 상하 스크롤 
    ///         실제 스크롤이 필요한 페이지는 “수술 전 기본검사” 항목 임 스크롤이 없거나 끝난 영역도 스틱 위아래로 작동 시 조금씩 움직여줌
    ///         가장 상단에서 위로 스틱미는동안 약간 화면이 밀려있다가 스틱놓으면 제자리 하단도 마찬가지로 스틱 내리는동안 살짝 스크롤 올라가있다가 스틱놓으면 제자리 가능동작
    /// 
    /// </summary>
    public class XRManager : MonoBehaviour
    {
        public class checkCoroutine
        {
            /// <summary>
            /// 현재 Material 변경 코루틴을 진행해도 되는지 체크
            /// </summary>
            public bool matChangeRoutine_CanPlaying = false;
            public string targetName = "";
        }

        private static XRManager _instance;
        public static XRManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<XRManager>();

                return _instance;
            }
        }
        struct InteractorController
        {
            /*
             * InputActionProperty = InputAction, InputActionReference
             * XRController : XRBaseController
             * ActionBasedController : XRBaseController
             * 
             * Grab
             *      https://sneakydaggergames.medium.com/vr-in-unity-advanced-grab-interaction-behaviors-98e79956c638
             * 
             * 입력 컨트롤러
             *      https://docs.unity3d.com/kr/2021.2/Manual/xr_input.html
             *      https://docs.microsoft.com/ko-kr/windows/mixed-reality/altspace-vr/getting-started/oculus-controls
             * 
             * GetControllerState
             *      XRControllerState.activateInteractionState
             *      active행동이 참일 때 참이고,activatedThisFrame트리거된 프레임에서만 true가 됩니다.
             * 
             * XRBaseInteractor : XRBaseControllerInteractor
             *      private XRBaseControllerInteractor leftGrabInteractable; -> XRRayInteractor
             *      leftGrabInteractable.selectEntered.AddListener(interactor =>
             *      {
             *          Debug.Log($"selectEntered {interactor}");
             *      });
             *      
             *      leftGrabInteractable.selectExited.AddListener(interactor =>
             *      {
             *          Debug.Log($"selectExited {interactor}");
             *      });
             *      
             *      leftGrabInteractable.hoverEntered.AddListener(interactor =>
             *      {
             *          Debug.Log($"hoverEntered {interactor}");
             *      });
             *      
             *      leftGrabInteractable.hoverExited.AddListener(interactor =>
             *      {
             *          Debug.Log($"hoverExited {interactor}");
             *      });
            */

            public GameObject target;

            // 컨트롤러에 붙어있는 컴포넌트
            public ActionBasedController xrController; // XR Controller (Action-based) = ActionBasedController : XRBaseController
            public XRRayInteractor xrinteractor; // XRRayInteractor : XRBaseInteractor
            public XRInteractorLineVisual xrlineRenderer;
            // public LineRenderer lineRenderer;
            public XROculusController controller;
            // public XROculusControllerAnimator animator;


            public GameObject model;

            private bool useUITouch;

            public void Attach(GameObject gameObject)
            {
                target = gameObject;
                if (target != null)
                {
                    xrController = target.GetComponent<ActionBasedController>();
                    xrinteractor = target.GetComponent<XRRayInteractor>();
                    xrlineRenderer = target.GetComponent<XRInteractorLineVisual>();
                    controller = target.GetComponent<XROculusController>();

                    model = xrController.model?.gameObject;

                    Leave();
                }
            }

            public void SendHapticImpulse(float amplitude, float duration)
            {
                if (xrController != null)
                    xrController.SendHapticImpulse(amplitude, duration);
            }

            public bool IsHoverUI()
            {
                return xrlineRenderer.reticle.activeInHierarchy;
            }

            public void EnableUITouch()
            {
                //Debug.Log($"<color=cyan>EnableUITouch {target.name}</color>");

                useUITouch = true;
                SetUITouch();
            }

            public void DisableUITouch()
            {
                //Debug.Log($"DisableUITouch {target.name}");

                useUITouch = false;
                SetUITouch();
            }

            private void SetUITouch()
            {
                if (xrlineRenderer)
                {
                    xrlineRenderer.enabled = useUITouch;
                }
                if (xrController)
                {
                    xrController.enableInputActions = useUITouch;
                }
                if (xrinteractor)
                {
                    xrinteractor.enabled = useUITouch;
                }
            }

            public void Enter()
            {
                DisableUITouch();

                if (model)
                {
                    model.SetActive(true);
                }
            }

            public void Leave()
            {
                DisableUITouch();

                if (model)
                {
                    model.SetActive(false);
                }
            }
        }
        public HandType CurController { get => UITouchController; }

        // UI 클릭에 사용될 컨트롤러
        [SerializeField] private HandType UITouchController = HandType.None;


        //  XRUIInputModule 또는 XRInteractionManager에서 UI 관리
        [SerializeField] private XRUIInputModule uiInputModule;


        public GameObject LeftControllerObject { get => leftControllerObject; }
        public GameObject RightControllerObject { get => rightControllerObject; }

        // 컨트롤러가 붙어있는 오브젝트
        [SerializeField] private GameObject leftControllerObject;
        [SerializeField] private GameObject rightControllerObject;

        private InteractorController leftController; // Struct
        private InteractorController rightController;


        // 컨트롤러의 레이가 UI 위에 있는지 여부
        private bool isUIInside = false;

        private bool isCheckLeftThumbstick = false;
        private bool isCheckRightThumbstick = false;

        // 사용 콘트롤러가 할당되었는지 여부
        private bool isControllerAssigned = false;


        // 기기 연결 확인에 필요한 변수
        private const string leftcontrollerName = "Controller - Left";
        private const string rightcontrollerName = "Controller - Right";

        // 디바이스 연결 및 연결해제에 따른 로그 표시
        private bool isPrintLogDeviceConnect = false;
        // 로그 예제
        // connect name: 'Oculus Quest', role: 'HeadMounted, TrackedDevice'
        // connect name: 'Oculus Touch Controller - Left', role: 'HeldInHand, TrackedDevice, Controller, Left'
        // connect name: 'Oculus Touch Controller - Right', role: 'HeldInHand, TrackedDevice, Controller, Right'

        public GameObject controllerWidget;

#if UNITY_EDITOR
        private void Reset()
        {
            uiInputModule = GetComponentInChildren<XRUIInputModule>();

            leftControllerObject = transform.parent.Find("Camera Offset/LeftHand Controller").gameObject;
            rightControllerObject = transform.parent.Find("Camera Offset/RightHand Controller").gameObject;
        }
#endif

        private void Awake()
        {
            leftController.Attach(leftControllerObject);
            rightController.Attach(rightControllerObject);

            if (UITouchController == HandType.LeftHand)
            {
                leftController.EnableUITouch();
            }
            else if (UITouchController == HandType.RightHand)
            {
                rightController.EnableUITouch();
            }
        }

        // 각종 이벤트 연결하기
        private void OnEnable()
        {
            // InputDevice 연결 및 연결
            InputDevices.deviceConnected += OnDeviceConnected;
            InputDevices.deviceDisconnected += OnDeviceDisconnected;

            uiInputModule.pointerEnter += OnPointerEnter;
            uiInputModule.pointerExit += OnPointerExit;
            uiInputModule.pointerClick += OnPointerClick;

            // 각 컨트롤러의 이벤트 연결
            leftController.controller.OnThumbstickDown.AddListener(OnLeftThumbstickDown);
            leftController.controller.OnUpdateThumbstick.AddListener(OnUpdateLeftThumbstick);
            leftController.controller.OnThumbstickUp.AddListener(OnLeftThumbstickUp);

            rightController.controller.OnThumbstickDown.AddListener(OnRightThumbstickDown);
            rightController.controller.OnUpdateThumbstick.AddListener(OnUpdateRightThumbstick);
            rightController.controller.OnThumbstickUp.AddListener(OnRightThumbstickUp);

            leftController.controller.OnTriggerUp.AddListener(OnLeftTriggerUp);
            rightController.controller.OnTriggerUp.AddListener(OnRightTriggerUp);

            leftController.controller.OnPrimaryButtonDown.AddListener(OnLeftPrimaryButtonDown);
            leftController.controller.OnSecondaryButtonDown.AddListener(OnLeftSecondaryButtonDown);

            rightController.controller.OnPrimaryButtonDown.AddListener(OnRightPrimaryButtonDown);
            rightController.controller.OnSecondaryButtonDown.AddListener(OnRightSecondaryButtonDown);
        }

        // 각종 이벤트 연결해제하기
        private void OnDisable()
        {
            // InputDevice 연결 해제 알림
            InputDevices.deviceConnected -= OnDeviceConnected;
            InputDevices.deviceDisconnected -= OnDeviceDisconnected;

            uiInputModule.pointerEnter -= OnPointerEnter;
            uiInputModule.pointerExit -= OnPointerExit;
            uiInputModule.pointerClick -= OnPointerClick;

            // 각 컨트롤러의 이벤트 연결해제
            leftController.controller.RemoveAllListeners();
            rightController.controller.RemoveAllListeners();
        }

        private InputDevice targetDevice;

        private void Update()
        {
            if (UITouchController != HandType.None)
                SleepControllerLine();

            if (usingStopContentButtion)
                DelayPauseButtion();

            GetLeftInput();
            GetRightInput();
            //GetInput();

            //if(rightController.xrController.activateAction.action.IsPressed())
            //    Debug.Log("Check");

        }

        #region 기기 연결 확인

        // 기기 연결이 되었을 때
        private void OnDeviceConnected(InputDevice device)
        {
            if (device.name.Contains(leftcontrollerName))
            {
                if (isPrintLogDeviceConnect)
                {
                    Debug.Log($"connect name: '{device.name}', role: '{ device.characteristics.ToString()}'");
                }

                leftController.Enter();
                if (UITouchController == HandType.LeftHand)
                {
                    leftController.EnableUITouch();
                }
            }
            else if (device.name.Contains(rightcontrollerName))
            {
                if (isPrintLogDeviceConnect)
                {
                    Debug.Log($"connect name: '{device.name}', role: '{ device.characteristics.ToString()}'");
                }

                rightController.Enter();
                if (UITouchController == HandType.RightHand)
                {
                    rightController.EnableUITouch();
                }
            }
        }

        // 기기 연결해제가 되었을 때
        private void OnDeviceDisconnected(InputDevice device)
        {
            if (device.name.Contains(leftcontrollerName))
            {
                if (isPrintLogDeviceConnect)
                {
                    Debug.Log($"disconnect name: '{device.name}', role: '{ device.characteristics.ToString()}'");
                }

                leftController.Leave();
            }
            else if (device.name.Contains(rightcontrollerName))
            {
                if (isPrintLogDeviceConnect)
                {
                    Debug.Log($"disconnect name: '{device.name}', role: '{ device.characteristics.ToString()}'");
                }

                rightController.Leave();
            }
        }

        #endregion


        #region UI 모듈 확인
        public void PlayHoverEffect()
        {
            //soundManager.PlayHoverEffect();
            SendHapticImpulse(0.1f, 0.1f);
        }

        public void PlayClickEffect()
        {
            //soundManager.PlayClickEffect();
            SendHapticImpulse(0.1f, 0.1f);
        }

        private void OnPointerEnter(GameObject arg1, UnityEngine.EventSystems.PointerEventData arg2)
        {
            isUIInside = true;

            // 아래와 같이 한곳에서 사운드 및 진동을 구현할 수는 있음.
            // if (arg2.eligibleForClick)
            // {
            //     PlayHoverEffect();
            //     SendHapticImpulse();
            // }
        }

        private void OnPointerExit(GameObject arg1, UnityEngine.EventSystems.PointerEventData arg2)
        {
            isUIInside = false;
        }

        private void OnPointerClick(GameObject arg1, UnityEngine.EventSystems.PointerEventData arg2)
        {
            // 아래와 같이 한곳에서 사운드 및 진동을 구현할 수는 있음.
            // PlayClickEffect();
            // SendHapticImpulse();
        }


        #endregion

        /// <summary>
        /// 콘트롤러 진동 발생
        /// </summary>
        /// <param name="amplitude"> 진동 세기 </param>
        /// <param name="duration"> 진동 시간 </param>
        public void SendHapticImpulse(float amplitude, float duration)
        {
            if (UITouchController == HandType.LeftHand)
                leftController.SendHapticImpulse(amplitude, duration);
            else if (UITouchController == HandType.RightHand)
                rightController.SendHapticImpulse(amplitude, duration);
            else
                Debug.Log("<color=red>현재 컨트롤러가 할당되지 않아 진동 기능을 사용하지 않습니다.</color>");
        }


        #region 컨트롤러 버튼 및 스틱 이벤트

        // 왼쪽 트리거를 놓았을 때
        private void OnLeftTriggerUp()
        {
            // 지정된 UI 클릭 컨트롤러가 왼쪽일 경우
            if (UITouchController == HandType.LeftHand)
            {
                // UI를 벗어났을 때
                if (isUIInside == false)
                {
                    //TODO :: 필요한 작업 구현하기

                }
            }
        }

        // 오른쪽 트리거를 놓았을 때
        private void OnRightTriggerUp()
        {
            // 지정된 UI 클릭 컨트롤러가 오른쪽일 경우
            if (UITouchController == HandType.RightHand)
            {
                // UI를 벗어났을 때
                if (isUIInside == false)
                {
                    //TODO :: 필요한 작업 구현하기
                }
            }
        }

        // 왼쪽 PrimaryButton을 눌렀을 때
        private void OnLeftPrimaryButtonDown()
        {
            //Main.Instance.CutSkip();

            //if (CameraManager.Instance.CameraViewType == CameraViewType.thirdPerson)
            //    CameraManager.Instance.SwitchingCameraView(CameraViewType.firstPerson);
        }

        // 왼쪽 SecondaryButton을 눌렀을 때
        private void OnLeftSecondaryButtonDown()
        {
            //Main.Instance.CutSkip();

            //if (CameraManager.Instance.CameraViewType == CameraViewType.firstPerson)
            //    CameraManager.Instance.SwitchingCameraView(CameraViewType.thirdPerson);
        }

        // 오른쪽 PrimaryButton을 눌렀을 때
        private void OnRightPrimaryButtonDown()
        {
            //if (CameraManager.Instance.CameraViewType == CameraViewType.thirdPerson)
            //    CameraManager.Instance.SwitchingCameraView(CameraViewType.firstPerson);
        }

        // 오른쪽 SecondaryButton을 눌렀을 때
        private void OnRightSecondaryButtonDown()
        {
            //if (CameraManager.Instance.CameraViewType == CameraViewType.firstPerson)
            //    CameraManager.Instance.SwitchingCameraView(CameraViewType.thirdPerson);
        }


        // 왼쪽 스틱을 조작했을 때
        private void OnLeftThumbstickDown()
        {
            isCheckLeftThumbstick = true;
        }

        private void OnLeftThumbstickUp()
        {
            isCheckLeftThumbstick = false;
        }

        private void OnUpdateLeftThumbstick(Vector2 value)
        {
            // 좌~우 수치 : -1 ~  1
            // 상~하 수치 :  1 ~ -1
        }


        // 오른쪽 스틱을 조작했을 때
        private void OnRightThumbstickDown()
        {
            isCheckRightThumbstick = true;
        }

        private void OnRightThumbstickUp()
        {
            isCheckRightThumbstick = false;
        }

        private void OnUpdateRightThumbstick(Vector2 value)
        {
            // 좌~우 수치 : -1 ~  1
            // 상~하 수치 :  1 ~ -1
        }

        private float checkSleepTime = 0f;
        /// <summary>
        /// 컨트롤러 Sleep 시간
        /// </summary>
        public float sleepTime = 5f;

        /// <summary>
        /// 컨트롤러 Line 숨김 여부
        /// </summary>
        private bool hideLine = false;

        private void SleepControllerLine()
        {
            if (hideLine == false)
            {
                checkSleepTime += Time.deltaTime;

                if (checkSleepTime > sleepTime)
                {
                    if (UITouchController == HandType.LeftHand)
                    {
                        // 컨트롤러 Line 비활성화
                        leftController.xrinteractor.enabled = hideLine;
                        leftController.xrlineRenderer.enabled = hideLine;
                    }
                    else
                    {
                        rightController.xrinteractor.enabled = hideLine;
                        rightController.xrlineRenderer.enabled = hideLine;
                    }

                    checkSleepTime = 0f;
                    hideLine = true;
                }
            }

        }

        private bool canPause = false;
        private float pauseCheckTime = 0;

        private void DelayPauseButtion()
        {
            if (canPause == false)
            {
                pauseCheckTime += Time.deltaTime;

                if (pauseCheckTime > 1.5f)
                {
                    canPause = true;
                    pauseCheckTime = 0;
                    Debug.Log($"<color=yellow> Pause 가능 </color>");
                }
            }
        }

        /// <summary>
        /// 버튼을 누르면 Pause Delay 시간이 초기화되어 카운트 시작
        /// </summary>
        public void StartPauseCheckTime()
        {
            canPause = false;
        }

        #endregion

        /// <summary>
        /// 콘트롤러 머테리얼 변경
        /// </summary>
        /// <param name="changeMat"> 변경되는 Target 머테리얼 </param>
        /// <param name="targetController"> 변경되는 Target 콘트롤러</param>
        /// <param name="useCoroutine"> Coroutine을 사용해서 머테리얼 변경할지 여부(true : 터치/ false : 클릭) </param>
        private void ChangeControllerMat(Material changeMat, GameObject targetController, bool useCoroutine)
        {
            if (useCoroutine == true) // Touch Button
            {
                if (targetControllersList.Count <= 1) // 콘트롤러 2개 다 터치 되면 더 이상 Check 안하도록
                {
                    if (targetControllersList.Count == 0) // 먼저 터치한 콘트롤러
                    {
                        targetControllersList.Add(targetController);
                        preController = targetController;

                        SkinnedMeshRenderer targetRender = targetControllersList[targetControllersList.Count - 1].GetComponentInChildren<SkinnedMeshRenderer>();

                        if (targetRender.material != changeMat)
                        {
                            StartCoroutine(MatChangeRoutine(targetRender));
                        }
                    }
                    else if (targetControllersList.Count == 1 && targetController != preController) // 두번째로 터치한 콘트롤러
                    {
                        targetControllersList.Add(targetController);

                        SkinnedMeshRenderer targetRender = targetControllersList[targetControllersList.Count - 1].GetComponentInChildren<SkinnedMeshRenderer>();

                        if (targetRender.material != changeMat)
                        {
                            StartCoroutine(MatChangeRoutine(targetRender));
                        }
                    }
                }
            }
            else if (useCoroutine == false && SubtitleManager.Instance.NowWaitingEvent) // Press Button
            {
                if (thumbButtonPressed == false)
                {
                    Debug.Log($"Button Pressed : <color=magenta>[{targetController.name}/ {thumbButtonPressed}]</color>");

                    thumbButtonPressed = true;

                    StopAllCoroutines();

                    SkinnedMeshRenderer targetRender = targetController.GetComponentInChildren<SkinnedMeshRenderer>();
                    targetRender.material = changeMat;

                    checkCoroutineList.Remove(checkCoroutineList.Find(x => x.targetName == targetRender.name));

                    // 버튼 Detach 체크 (Reset)
                    StartCoroutine(CheckButton_Detach(targetController));
                }
            }
        }

        /// <summary>
        /// 콘트롤러 버튼 최종 Detach 감지
        /// </summary>
        /// <param name="targetController"></param>
        /// <returns></returns>
        private IEnumerator CheckButton_Detach(GameObject targetController)
        {
            if (targetController.name.Contains("Left"))
                yield return new WaitUntil(() => leftButtonTouched == false);
            else
                yield return new WaitUntil(() => rightButtonTouched == false);

            yield return new WaitForSeconds(0.5f);

            SubtitleManager.Instance.PassCheckPoint(); // [콘트롤러 사용법 - "잘하셨습니다"]

            usingControllerMatChange = false;

            thumbButtonPressed = false;
            rightButtonTouched = false;
            leftButtonTouched = false;

            preController = null;

            //머테리얼 초기화 TargetController_List
            for (int i = targetControllersList.Count - 1; i >= 0; i--)
            {
                SkinnedMeshRenderer targetRender = targetControllersList[i].GetComponentInChildren<SkinnedMeshRenderer>();
                if (targetRender.material != originalMat)
                    targetRender.material = originalMat;

                targetControllersList.Remove(targetControllersList[i]);
            }

            Debug.Log($"<color=cyan> Detach </color>");
        }

        /// <summary>
        /// Highte Mat으로 머테리얼 체인지 되는 동안 코루틴 체크용
        /// </summary>
        [SerializeField] private List<checkCoroutine> checkCoroutineList = new List<checkCoroutine>();

        /// <summary>
        /// 머테리얼 Change Lerp 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator MatChangeRoutine(SkinnedMeshRenderer renderer)
        {
            float checkTime = 0;

            checkCoroutineList.Add(new checkCoroutine { matChangeRoutine_CanPlaying = true, targetName = renderer.name });

            while (checkTime < 1.0f)
            {
                //Debug.Log($"{checkTime}");
                if (!usingControllerMatChange)
                {
                    rightButtonTouched = false;
                    leftButtonTouched = false;

                    renderer.material = originalMat;
                    checkCoroutineList.Clear();
                    targetControllersList.Clear();
                    preController = null;

                    yield break; // 코루틴 중지
                }

                for (int i = checkCoroutineList.Count - 1; i >= 0; i--)
                {
                    if (checkCoroutineList[i].targetName == renderer.name && !checkCoroutineList[i].matChangeRoutine_CanPlaying) // 머테리얼 변경 중 중간에 손을 떼면(Reset)되고 코루틴 중지
                    {
                        Debug.Log($"<color=yellow>{i}/ {renderer.name}/ MatChange Routine 진행해도 되는지 => [{checkCoroutineList[i].matChangeRoutine_CanPlaying}] </color> (Change 도중 Reset)");
                        checkCoroutineList.Remove(checkCoroutineList.Find(x => x.targetName == renderer.name));

                        yield break;
                    }
                }
                checkTime += Time.deltaTime / changeTime;
                renderer.material.Lerp(originalMat, highlightMat, checkTime);
                //Debug.Log($"{checkTime}");

                yield return null;
            }

            renderer.material = highlightMat;

            checkCoroutineList.Remove(checkCoroutineList.Find(x => x.targetName == renderer.name));
            //Debug.Log($"<color=yellow> {renderer.name} </color> Remove");

        }

        /// <summary>
        /// 콘트롤러 머테리얼 초기화
        /// </summary>
        /// <param name="targetController"> </param>
        private void ResetControllerMat(GameObject targetController)
        {
            if (targetControllersList.Count != 0)
            {
                for (int i = targetControllersList.Count - 1; i >= 0; i--)
                {
                    if (targetControllersList[i] == targetController)
                    {
                        if (preController == targetController && targetControllersList.Count == 2)
                            preController = targetControllersList[i + 1]; // if(먼저 버튼에 손을 댄 콘트롤러를 먼저 손 뗐을때) => PreController 교체

                        SkinnedMeshRenderer targetRender = targetControllersList[i].GetComponentInChildren<SkinnedMeshRenderer>();

                        if (targetRender.material != originalMat)
                        {
                            if (checkCoroutineList.Count != 0)
                            {
                                checkCoroutine target = checkCoroutineList.Find(x => x.targetName == targetRender.name);
                                if (target != null)
                                {
                                    target.matChangeRoutine_CanPlaying = false;
                                }
                                else
                                {
                                    Debug.Log($"<color=red>{targetRender.name} Null</color>");
                                }
                            }

                            targetRender.material = originalMat; // 머테리얼 초기화

                        }

                        targetControllersList.Remove(targetControllersList[i]); // 머테리얼 Reset된 콘트롤러는 리스트에서 삭제
                    }
                }
            }
            else
            {
                if (preController != null)
                    preController = null;
            }
        }


        #region 테스트 코드 - 리셋
        private void ResetPos()
        {
            // if (OVRManager.display != null)
            // {
            //     float currentRotY = CameraManager.Main.transform.eulerAngles.y; //This refence a CenterEyeAnchor
            //     float difference = 0 - currentRotY;
            //     InteractionSystem.AnchorPlayer.Rotate(0, difference, 0); // InteractionSystem.Anchor This refence a Player
            // 
            //     Vector3 newPos = new Vector3(0 - CameraManager.Main.transform.position.x, 0, 0 - CameraManager.Main.transform.position.z);
            //     InteractionSystem.AnchorPlayer.transform.position += newPos;
            // }
        }
        private void RecenterHeadset()
        {

        }

        #endregion




        #region 테스트 코드 - 컨트롤러 버튼 관리
        private void SetEnable()
        {
            // Grip 그립 버튼 - selectAction
            // leftController.xrController.selectAction.action.performed   += GripAction_performed;
            // leftController.xrController.selectAction.action.started     += GripAction_performed;
            // leftController.xrController.selectAction.action.canceled    += GripAction_performed;

            // Trigger 트리거 - uiPressAction, activateAction
            // rightController.xrController.activateAction.action.performed += TriggerAction_performed;
            // rightController.xrController.activateAction.action.started   += TriggerAction_performed;
            // rightController.xrController.activateAction.action.canceled  += TriggerAction_performed;
        }


        private void SetDisable()
        {
            // Grip 그립 버튼 - selectAction
            // leftController.xrController.selectAction.action.performed   -= GripAction_performed; // 누르고 있을 때
            // leftController.xrController.selectAction.action.started     -= GripAction_performed; // 눌렀을 때
            // leftController.xrController.selectAction.action.canceled    -= GripAction_performed; // 뗏을때

            // Trigger 트리거 - uiPressAction, activateAction
            // rightController.xrController.activateAction.action.performed -= TriggerAction_performed;
            // rightController.xrController.activateAction.action.started   -= TriggerAction_performed;
            // rightController.xrController.activateAction.action.canceled  -= TriggerAction_performed;
        }

        private void GripAction_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            // if (context.started)
            // {
            //     leftController.debugText.text = "started";
            // }
            // 
            // if (context.canceled)
            // {
            //     leftController.debugText.text = "canceled";
            // }
            // 
            // if (context.performed)
            // {
            //     leftController.debugText.text = $"performed\n값 : {context.ReadValue<float>()}";
            // }
        }

        // Trigger
        private void TriggerAction_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            // if (context.started)
            // {
            //     rightController.debugText.text = "started";
            // }
            // 
            // if (context.canceled)
            // {
            //     rightController.debugText.text = "canceled";
            // }
            // 
            // if (context.performed)
            // {
            //     rightController.debugText.text = $"performed\n값 : {context.ReadValue<float>()}";
            // }
        }

        #endregion

        #region 테스트 코드 - 햅틱


        public void HapticEvent()
        {
            // 1.
            InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            // 2.
            HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    uint channel = 0;
                    float amplitude = 0.5f;
                    float duration = 0.1f;
                    // 3.
                    device.SendHapticImpulse(channel, amplitude, duration);
                }
            }
        }

        // private static readonly float DEFAULT_DURATION = 0.3f;
        // private static readonly float DEFAULT_INTENSITY = 0.2f;

        // public void SendHaptic_XRController(Hand hand, float amplitude, float duration)
        // {
        //     switch (hand)
        //     {
        //         case Hand.Both:
        //             SendHaptic(XRController.leftHand, amplitude, duration);
        //             SendHaptic(XRController.rightHand, amplitude, duration);
        //             break;
        // 
        //         case Hand.Left:
        //             SendHaptic(XRController.leftHand, amplitude, duration);
        //             break;
        // 
        //         case Hand.Right:
        //             SendHaptic(XRController.rightHand, amplitude, duration);
        //             break;
        // 
        //         default:
        //             break;
        //     }
        // }
        // 
        // public void SendHaptic(UnityEngine.InputSystem.InputDevice device, float amplitude, float duration, int channel = 0)
        // {
        //     if (device != null)
        //     {
        //         var command = UnityEngine.InputSystem.XR.Haptics.SendHapticImpulseCommand.Create(channel, amplitude, duration);
        //         device.ExecuteCommand(ref command);
        //     }
        // }
        // 
        // [SerializeField] InputActionReference leftHapticAction;
        // [SerializeField] InputActionReference rightHapticAction;
        // 
        // public void SendHaptic()
        // {
        //     // OpenXRInput.SendHapticImpulse(rightHapticAction, amplitude, duration, UnityEngine.InputSystem.XR.XRController.rightHand); //Right Hand Haptic Impulse
        //     // OpenXRInput.SendHapticImpulse(leftHapticAction, amplitude, duration, UnityEngine.InputSystem.XR.XRController.leftHand); //Left Hand Haptic Impulse
        // }

        #endregion

        #region 테스트 코드 - HMD 마운트 확인

        public static bool isPresent()
        {
            // XRDevice.isPresent 사용안됨

            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
            foreach (var xrDisplay in xrDisplaySubsystems)
            {
                if (xrDisplay.running)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isHMDConnected()
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);
            bool isPresent = devices.Count > 0;

            return isPresent;
        }

        public bool isHMDMounted()
        {
            InputDevice headDevice = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (headDevice.isValid == false)
                return false;

            bool userPresent = false;
            bool presenceFeatureSupported = headDevice.TryGetFeatureValue(CommonUsages.userPresence, out userPresent);

            Debug.Log(headDevice.isValid + " ** " + presenceFeatureSupported + " ** " + userPresent);

            return userPresent;
        }

        #endregion

        #region 테스트 코드 - 이동
        // XR Controller의 원점은 OculusTouch의 A / X 버튼


        // Locomotion System
        // TeleportationProvider    xrRig.MoveCameraToWorldLocation(cameraDestination);
        // SnapTurnProviderBased    xrRig.RotateAroundCameraUsingRigUp(currentTurnAmount);
        // ContinuousTurnProvider   xrRig.RotateAroundCameraUsingRigUp(turnAmount);
        // ContinuousMoveProvider   m_CharacterController.Move(motion) / xrRig.rig.transform.position += motion; EndLocomotion();

        // Teleportation Area 
        // Teleportation Anchor

        private void OnAnimatorMove()
        {

        }

        #endregion

        #region 테스트 코드 - 컨트롤러 연결 확인

        /// <summary>
        /// device.name
        /// ・Oculus Quest
        /// ・Oculus Touch Controller - Left
        /// ・Oculus Touch Controller - Right
        /// 
        /// device.role == device.characteristics
        /// ・Generic :  헤드 마운트 디스플레이나 모바일 디바이스 등의 코어가 되는 XR 디바이스.
        /// ・LeftHanded : 사용자의 왼손과 관련된 장치.
        /// ・RightHanded : 사용자의 오른손과 관련된 장치.
        /// ・GameController : 콘솔 스타일의 게임 컨트롤러.
        /// ・HardwareTracker : 추적 장치.
        /// ・TrackingReference : 정적 추적 참조 객체.
        /// 
        /// device.Characteristics
        /// ・HeadMounted : 머리에 부착되어 있다.
        /// ・Camera : 카메라 추적 기능을 가진다.
        /// ・HeldInHand : 손에 들고 있다.
        /// ・HandTracking : 핸드 트래킹 기능을 가진다.
        /// ・EyeTracking : 시선 추적 기능을 가진다.
        /// ・TrackedDevice : 3D 공간의 디바이스 추적 기능을 가진다.
        /// ・Controller : 버튼과 아날로그 스틱을 가진다.
        /// ・TrackingReference : 정적 추적 참조 객체.
        /// ・Left : 왼손과 관련되어 있다.
        /// ・Right : 오른손에 관련지을 수 있다.
        /// ・Simulated6DOF : 6DOF의 기능을 가진다.
        /// 
        /// InputDevice.XRNode
        /// ・LeftEye : 왼쪽 눈.
        /// ・RightEye : 오른쪽 눈.
        /// ・CenterEye : 눈동자의 중간점.
        /// ・Head : 머리의 중심점.
        /// ・LeftHand : 왼손.
        /// ・RightHand : 오른손.
        /// ・GameController : 콘솔 스타일의 게임 컨트롤러.
        /// ・HardwareTracker : 사용자 또는 물리적 항목에 연결된 하드웨어 추적 장치.
        /// ・TrackingReference : Oculus 카메라 등의 추적 기준점.
        /// </summary>
        /// 
        private void GetDevices()
        {
            // 머리, 양손의 컨트롤러 모두 가져오는 방법
            var inputDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevices(inputDevices);

            foreach (var device in inputDevices)
            {
                Debug.Log(string.Format("name: '{0}', role: '{1}'", device.name, device.characteristics.ToString()));
            }
            Debug.Log("====================");

            // 특정 컨트롤러 하나만 가져오는 방법
            var leftHandDevices = new List<UnityEngine.XR.InputDevice>();

            InputDeviceCharacteristics leftHandCharacteristics =
                InputDeviceCharacteristics.Left |
                InputDeviceCharacteristics.HeldInHand |
                InputDeviceCharacteristics.Controller |
                InputDeviceCharacteristics.TrackedDevice;
            InputDeviceCharacteristics rightHandCharacteristics =
                InputDeviceCharacteristics.Right |
                InputDeviceCharacteristics.HeldInHand |
                InputDeviceCharacteristics.Controller |
                InputDeviceCharacteristics.TrackedDevice;

            var desiredCharacteristics =
                InputDeviceCharacteristics.Left |
                InputDeviceCharacteristics.HeldInHand |
                InputDeviceCharacteristics.Controller;

            var rightControllerCharacteristics =
                InputDeviceCharacteristics.Right |
                InputDeviceCharacteristics.Controller;

            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, leftHandDevices);

            foreach (var device in leftHandDevices)
            {
                Debug.Log(string.Format("name: '{0}', role: '{1}'", device.name, device.characteristics.ToString()));
            }
            Debug.Log("====================");


            // 특정 컨트롤러 하나만 가져오는 방법
            var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);

            foreach (var device in rightHandDevices)
            {
                Debug.Log(string.Format("name: '{0}', role: '{1}'", device.name, device.characteristics.ToString()));
            }
            Debug.Log("====================");

            InputDevice leftHand;
            if (leftHandDevices.Count > 0)
            {
                Debug.Log("Left hand found");
                leftHand = leftHandDevices[0];
                leftHand.SendHapticImpulse(0, 0.5f, 1.0f);
            }

        }

        #endregion

        private bool leftTriggerPressed = false;
        private bool leftGripPressed = false;
        private bool leftJoystickPressed = false;
        private bool leftButtonTouched = false;

        private bool rightTriggerPressed = false;
        private bool rightGripPressed = false;
        private bool rightJoystickPressed = false;
        private bool rightButtonTouched = false;

        /// <summary>
        /// 콘트롤러 버튼를 눌렀는지 체크
        /// </summary>
        private bool thumbButtonPressed = false;

        /// <summary>
        /// 컷 Skip 되었는지 1번 체크용
        /// </summary>
        private bool cutSkipped = false;
        /// <summary>
        /// MainMenu 시퀀스로 이동 되었는지 1번 체크용
        /// </summary>
        private bool sequenceChanged = false;
        /// <summary>
        /// Grip 버튼 누르고 있는 시간 체크
        /// </summary>
        private float checkGripTime = 0f;
        /// <summary>
        /// Joystick 버튼 누르고 있는 시간 체크
        /// </summary>
        private float checkJoystickTime = 0f;
        /// <summary>
        /// 컷 Skip을 위해 누르고 있어야하는 시간
        /// </summary>
        public float skipTime;

        [Header("<Controller Material Change>")]
        /// <summary>
        /// 콘트롤러 머테리얼 변경 기능 사용할지 여부
        /// </summary>
        public bool usingControllerMatChange = false;
        /// <summary>
        /// 머테리얼 변화 시간
        /// </summary>
        public float changeTime;
        /// <summary>
        /// 원래 머테리얼
        /// </summary>
        public Material originalMat;
        /// <summary>
        /// Change 되는 머테리얼
        /// </summary>
        public Material highlightMat;
        /// <summary>
        /// 머테리얼 변경된 콘트롤러 리스트 (최대 2개)
        /// </summary>
        private List<GameObject> targetControllersList = new List<GameObject>();
        /// <summary>
        /// 버튼 터치를 먼저 한 컨트롤러 할당
        /// </summary>
        private GameObject preController;
        [Space]
        [Header("<Stop Content>")]
        /// <summary>
        /// 영상 콘텐츠 진행 중 콘텐츠 중지 기능 사용 여부
        /// </summary>
        public bool usingStopContentButtion = false;

        #region 테스트 코드 - 컨트롤러 키 입력 확인

        /// <summary>
        /// InputDevice 입력 상태 얻기 https://docs.unity3d.com/Manual/xr_input.html
        /// </summary>
        private void GetLeftInput()
        {
            // Float: Axis Range 0 ~ 1
            // Vector2: Touchpad 움직임 -1 ~ 1
            var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
            foreach (var device in leftHandDevices)
            {
                if (usingControllerMatChange)
                {
                    // primaryTouch(Button)   : [X/A] - 터치
                    if (device.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouch) && primaryTouch)
                    {
                        //Debug.Log("Left Touching Primary Button");

                        leftButtonTouched = true;
                        // Material 변경
                        ChangeControllerMat(highlightMat, leftControllerObject, true);
                    }
                    else
                    {
                        leftButtonTouched = false;

                        ResetControllerMat(leftControllerObject);
                    }

                    // primaryButton(Button)  : [X/A] - 푸시
                    if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton) && primaryButton)
                    {
                        Debug.Log("Left Pressing Primary Button/ usingControllerMatChange");
                        ChangeControllerMat(highlightMat, leftControllerObject, false);
                    }
                }

                if (usingStopContentButtion)
                {
                    // primaryButton(Button)  : [X/A] - 푸시
                    if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton) && primaryButton)
                    {
                        if (canPause)
                            Main.Instance.Pause(); // Main CutLoop 중지

                    }
                }

                //// secondaryButton(Button): [Y/B] - 누름
                //if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButton) && secondaryButton)
                //{
                //    Debug.Log("Left Pressing Primary Button");
                //}

                //// secondaryTouch(Button) : [Y/B] - 터치
                //if (device.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool secondaryTouch) && secondaryTouch)
                //{
                //    Debug.Log("Left Touching Primary Button");
                //}

                // primary2DAxisClick(Button) : Thumbstick - Press
                if (device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool down))
                {
                    if (down)
                    {
                        if (leftJoystickPressed == false)
                            leftJoystickPressed = true;

                        checkJoystickTime += Time.deltaTime;

                        if (sequenceChanged == false)
                        {
                            if (checkJoystickTime > skipTime)
                            {
                                if (Main.Instance.CurSequence != Main.Instance.returnSceneData)
                                {
                                    Subtitle_Main.Instance.ResetSubtitle_Main();
                                    SubtitleManager.Instance.EndSubtitle();

                                    Main.Instance.ChangeScene(Main.Instance.returnSceneData); // Return 시퀀스로 이동
                                    sequenceChanged = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (leftJoystickPressed) // 초기화( 기본상태)
                        {
                            leftJoystickPressed = false;

                            sequenceChanged = false;
                            checkJoystickTime = 0f;
                        }
                    }
                }

                // grip(Axis): 그립(Grip)
                if (device.TryGetFeatureValue(CommonUsages.grip, out float gripAmount) && gripAmount < 0.1f)
                {
                    if (leftGripPressed)
                    {
                        leftGripPressed = false;

                        cutSkipped = false;
                        checkGripTime = 0f;
                    }

                }

                // gripButton(Button) : 그립(Grip) - 누름(Press)
                if (device.TryGetFeatureValue(CommonUsages.gripButton, out bool grip) && grip)
                {
                    if (leftGripPressed != true)
                    {
                        leftGripPressed = true;
                    }

                    checkGripTime += Time.deltaTime;

                    if (cutSkipped == false)
                    {
                        if (checkGripTime > skipTime)
                        {
                            Main.Instance.CutSkip();
                            cutSkipped = true;
                        }

                    }
                }

                // triggerButton(Button) : 트리거(Trigger) - 누름(Press)
                if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton) && triggerButton)
                {
                    if (leftTriggerPressed != true)
                    {
                        leftTriggerPressed = true;

                        if (controllerWidget.gameObject.activeSelf)
                            ControllerBlendWidget.Instance.RotateWidget(leftTriggerPressed);
                    }
                }

                // trigger(Axis) : 트리거(Trigger) (primaryIndex, primaryHand)
                if (device.TryGetFeatureValue(CommonUsages.trigger, out float triggerAmount) && triggerAmount < 0.1f) // trigger 버튼 Up 체크
                {
                    if (leftTriggerPressed)
                    {
                        leftTriggerPressed = false;
                        checkSleepTime = 0f;

                        if (controllerWidget.gameObject.activeSelf)
                            ControllerBlendWidget.Instance.SelectUp(leftTriggerPressed);

                        if (isControllerAssigned == false) // Controller 처음 할당 (Left)
                        {
                            UITouchController = HandType.LeftHand;
                            leftController.EnableUITouch();
                            isControllerAssigned = true;

                            rightControllerObject.SetActive(UITouchController == HandType.RightHand);
                            Debug.Log($"<color=yellow> Left Controller Assigned/ {triggerAmount}</color>");
                        }
                        else // Switching (Right => Left)
                        {
                            if (UITouchController == HandType.RightHand)
                            {
                                UITouchController = HandType.LeftHand;

                                leftControllerObject.SetActive(UITouchController == HandType.LeftHand);

                                rightController.DisableUITouch();
                                leftController.EnableUITouch();

                                rightControllerObject.SetActive(UITouchController == HandType.RightHand);
                                //Debug.Log($"<color=yellow> Left Controller to Right Change/ {triggerAmount}</color>");
                            }
                        }

                        if (hideLine)
                        {
                            leftController.xrinteractor.enabled = hideLine;
                            leftController.xrlineRenderer.enabled = hideLine;

                            hideLine = false;
                        }
                    }
                }
                // userPresence(Button) : User presence
            }
        }

        private void GetRightInput()
        {
            // Float: Axis Range 0 ~ 1
            // Vector2: Touchpad 움직임 -1 ~ 1
            var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);
            foreach (var device in rightHandDevices)
            {
                if (usingControllerMatChange)
                {
                    // primaryTouch(Button)   : [X/A] - 터치
                    if (device.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouch) && primaryTouch)
                    {
                        //Debug.Log("Right Touching Primary Button");
                        rightButtonTouched = true;

                        // Material 변경
                        ChangeControllerMat(highlightMat, rightControllerObject, true);
                    }
                    else
                    {
                        rightButtonTouched = false;

                        ResetControllerMat(rightControllerObject);
                    }

                    // primaryButton(Button)  : [X/A] - 푸시
                    if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton) && primaryButton)
                    {
                        //Debug.Log("Right Pressing Primary Button/ usingControllerMatChange");

                        ChangeControllerMat(highlightMat, rightControllerObject, false);
                    }
                }

                if (usingStopContentButtion)
                {
                    // primaryButton(Button)  : [X/A] - 푸시
                    if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton) && primaryButton)
                    {
                        //Debug.Log("콘텐츠 중지");
                        if (canPause)
                            Main.Instance.Pause(); // Main CutLoop 중지

                    }
                }

                //// secondaryButton(Button): [Y/B] - 누름
                //if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButton) && secondaryButton)
                //{
                //    Debug.Log("Right Pressing Primary Button");
                //}

                //// secondaryTouch(Button) : [Y/B] - 터치
                //if (device.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool secondaryTouch) && secondaryTouch)
                //{
                //    Debug.Log("Right Touching Primary Button");
                //}


                // primary2DAxisClick(Button) : Thumbstick - Press
                if (device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool down))
                {
                    if (down)
                    {
                        if (rightJoystickPressed == false)
                            rightJoystickPressed = true;

                        checkJoystickTime += Time.deltaTime;

                        if (sequenceChanged == false)
                        {
                            if (checkJoystickTime > skipTime)
                            {
                                if (Main.Instance.CurSequence != Main.Instance.returnSceneData)
                                {
                                    Subtitle_Main.Instance.ResetSubtitle_Main();
                                    SubtitleManager.Instance.EndSubtitle();

                                    Main.Instance.ChangeScene(Main.Instance.returnSceneData); // Return 시퀀스로 이동
                                    sequenceChanged = true;
                                }
                            }
                        }

                    }
                    else
                    {
                        if (rightJoystickPressed) // 초기화( 기본상태)
                        {
                            rightJoystickPressed = false;

                            sequenceChanged = false;
                            checkJoystickTime = 0f;
                        }
                    }
                }


                // grip(Axis): 그립(Grip) - Detach
                if (device.TryGetFeatureValue(CommonUsages.grip, out float gripAmount) && gripAmount < 0.1f)
                {
                    if (rightGripPressed) // 초기화
                    {
                        rightGripPressed = false;

                        cutSkipped = false;
                        checkGripTime = 0f;
                    }

                }

                // gripButton(Button) : 그립(Grip) - 누름(Press)
                if (device.TryGetFeatureValue(CommonUsages.gripButton, out bool grip) && grip)
                {
                    if (rightGripPressed != true)
                    {
                        rightGripPressed = true;
                        //rightController.xrController.SendHapticImpulse(0.5f, 2f);
                    }

                    checkGripTime += Time.deltaTime;

                    if (cutSkipped == false)
                    {
                        if (checkGripTime > skipTime)
                        {
                            Main.Instance.CutSkip();
                            cutSkipped = true;
                        }

                    }
                }

                // triggerButton(Button) : 트리거(Trigger) - 누름(Press)
                if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton) && triggerButton)
                {
                    if (rightTriggerPressed != true)
                    {
                        rightTriggerPressed = true;
                        if (controllerWidget.gameObject.activeSelf)
                            ControllerBlendWidget.Instance.RotateWidget(rightTriggerPressed);
                    }
                    //Debug.Log($"Trigger button is pressed/ {triggerPressed}");
                }

                // trigger(Axis) : 트리거(Trigger) (primaryIndex, primaryHand)
                if (device.TryGetFeatureValue(CommonUsages.trigger, out float triggerAmount) && triggerAmount < 0.1f)
                {
                    if (rightTriggerPressed)
                    {
                        rightTriggerPressed = false;
                        checkSleepTime = 0f;

                        if (controllerWidget.gameObject.activeSelf)
                            ControllerBlendWidget.Instance.SelectUp(rightTriggerPressed);

                        if (isControllerAssigned == false) // Controller 처음 할당 (Right)
                        {
                            UITouchController = HandType.RightHand;
                            rightController.EnableUITouch();
                            isControllerAssigned = true;

                            leftControllerObject.SetActive(UITouchController == HandType.LeftHand);
                            Debug.Log($"<color=yellow> Right Controller Assigned/ {triggerAmount}</color>");
                        }
                        else // Switching (Left => Right)
                        {
                            if (UITouchController == HandType.LeftHand)
                            {
                                UITouchController = HandType.RightHand;

                                rightControllerObject.SetActive(UITouchController == HandType.RightHand);

                                leftController.DisableUITouch();
                                rightController.EnableUITouch();

                                leftControllerObject.SetActive(UITouchController == HandType.LeftHand);
                                //Debug.Log($"<color=yellow> Right Controller To Left Change/ {triggerAmount}</color>");
                            }
                        }

                        if (hideLine)
                        {
                            // 컨트롤러 Line 활성화
                            rightController.xrinteractor.enabled = hideLine;
                            rightController.xrlineRenderer.enabled = hideLine;

                            hideLine = false;
                        }

                    }

                }
                // userPresence(Button) : User presence
            }
        }

        /// <summary>
        /// InputDevice 입력 상태 얻기 https://docs.unity3d.com/Manual/xr_input.html
        /// </summary>
        private void GetInput()
        {
            // Float: Axis Range 0 ~ 1
            // Vector2: Touchpad 움직임 -1 ~ 1
            var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
            foreach (var device in leftHandDevices)
            {
                // primary2DAxis (2D Axis): 조이스틱(Joystick)
                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 position))
                {
                    Debug.Log("Joystick : " + position.x + "," + position.y);

                    if (position != Vector2.zero)
                        Debug.Log("Primary TouchPad" + position);

                    // 우
                    if (position.x > 0)
                    {
                    }
                    // 좌
                    else if (position.x < 0)
                    {
                    }
                }

                // primary2DAxisTouch(Button) : Thumbstick - Touch
                if (device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool touch))
                {

                }

                // primary2DAxisClick(Button) : Thumbstick - Press
                if (device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool down))
                {

                }


                // primaryButton(Button)  : [X/A] - 푸시
                if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton) && primaryButton)
                {
                    Debug.Log("Pressing Primary Button");
                }

                // primaryTouch(Button)   : [X/A] - 터치
                if (device.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouch) && primaryTouch)
                {
                    Debug.Log("Touching Primary Button");
                }

                // secondaryButton(Button): [Y/B] - 누름
                if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButton) && secondaryButton)
                {
                    Debug.Log("Pressing Primary Button");
                }

                // secondaryTouch(Button) : [Y/B] - 터치
                if (device.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool secondaryTouch) && secondaryTouch)
                {
                    Debug.Log("Touching Primary Button");
                }

                // triggerButton(Button) : 트리거(Trigger) - 누름(Press) => 대략 triggerAmount가 0.5f 이상일 때 true
                if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton) && triggerButton)
                {
                    Debug.Log($"Trigger button is pressed");
                }


                // trigger(Axis) : 트리거(Trigger) (primaryIndex, primaryHand)
                if (device.TryGetFeatureValue(CommonUsages.trigger, out float triggerAmount) && triggerAmount > 0.1f)
                {
                    Debug.Log($"Trigger button is pressed/ {triggerAmount}");
                }

                // grip(Axis): 그립(Grip)
                if (device.TryGetFeatureValue(CommonUsages.grip, out float gripAmount) && gripAmount > 0.1f)
                {
                }

                // gripButton(Button) : 그립(Grip) - 누름(Press)
                if (device.TryGetFeatureValue(CommonUsages.gripButton, out bool grip) && grip)
                {
                }

                // menuButton(Button) : Start (왼쪽 컨트롤러 전용)
                if (device.TryGetFeatureValue(CommonUsages.menuButton, out bool menu) && menu)
                {
                }

                // userPresence(Button) : User presence
            }
        }

        #endregion

        #region 테스트 코드 - 컨트롤러 위치

        private void SetTrackingOriginMode()
        {
            List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetInstances<XRInputSubsystem>(subsystems);
            for (int i = 0; i < subsystems.Count; i++)
            {
                subsystems[i].TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);
            }
        }

        private void GetHandPos()
        {
            List<XRNodeState> mNodeStates = new List<XRNodeState>();

            // GameObject Head = null;
            // Rigidbody LeftHand, RightHand;

            Vector3 mHeadPos, mLeftHandPos, mRightHandPos = Vector3.zero;
            Quaternion mHeadRot, mLeftHandRot, mRightHandRot = Quaternion.identity;


            // Update 문에서 실행
            InputTracking.GetNodeStates(mNodeStates);

            foreach (XRNodeState nodeState in mNodeStates)
            {
                switch (nodeState.nodeType)
                {
                    case XRNode.Head:
                        nodeState.TryGetPosition(out mHeadPos);
                        nodeState.TryGetRotation(out mHeadRot);
                        break;
                }
            }
            // Head.transform.position = mHeadPos;
            // Head.transform.rotation = mHeadRot.normalized;


            // FixedUpdate 문에서 실행
            InputTracking.GetNodeStates(mNodeStates);

            foreach (XRNodeState nodeState in mNodeStates)
            {
                switch (nodeState.nodeType)
                {
                    case XRNode.LeftHand:
                        nodeState.TryGetPosition(out mLeftHandPos);
                        nodeState.TryGetRotation(out mLeftHandRot);
                        break;
                    case XRNode.RightHand:
                        nodeState.TryGetPosition(out mRightHandPos);
                        nodeState.TryGetRotation(out mRightHandRot);
                        break;
                }
            }

            // LeftHand.MovePosition(mLeftHandPos);
            // LeftHand.MoveRotation(mLeftHandRot.normalized);
            // RightHand.MovePosition(mRightHandPos);
            // RightHand.MoveRotation(mRightHandRot.normalized);
        }
        #endregion
    }

}