/// 작성자: 조효련
/// 작성일: 2022-02-07
/// 수정일: 
/// 저작권: Copyright(C) FNIKorea Co., Ltd.. (주)에프앤아이코리아

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static FNI.XR.OculusControllerControls;



namespace FNI.XR
{
    /// <summary>
    /// XROculusController
    /// 
    /// XRControllerAnimator 컴포넌트와 같이 InputActionReference을 이용하여 입력을 관리함
    /// 
    /// Assets/FNI/Res/Models/OculusTouchForQuest2/Oculus Controller Controls.inputactions
    /// 
    /// Input Action
    /// Action Map - Oculus Left Controller와 Oculus Right Controller
    /// Actions (Action Type, 제어 유형, 바인딩)
    /// Primary Button       : Button    None        XR Controller > XR Controller(Left Hand) > Optional Controls > primaryButton [LeftHand XR Controller]
    /// Secondary Button     : Button    None        XR Controller > XR Controller(Left Hand) > Optional Controls > secondaryButton [LeftHand XR Controller]
    /// Menu Button          : Button    None        XR Controller > XR Controller(Left Hand) > Optional Controls > menuBotton [LeftHand XR Controller]
    /// Trigger              : Value     Axis        XR Controller > XR Controller(Left Hand) > Optional Controls > trigger [LeftHand XR Controller]
    /// Grip                 : Value     Axis        XR Controller > XR Controller(Left Hand) > Optional Controls > grip [LeftHand XR Controller]
    /// Thumbstick           : Value     Vector2     XR Controller > XR Controller(Left Hand) > Optional Controls > thumbstick [LeftHand XR Controller]
    /// 좌 -1 ~  우 1
    /// 상 1 ~ 하 -1
    /// 
    /// </summary>

    // URP를 사용할 경우 Main Camera, UI Camera에 UniversalAdditionalCameraData 추가하기


    public enum HandType
    {
        None,
        LeftHand,
        RightHand
    }

    public class XROculusController : MonoBehaviour, IOculusLeftControllerActions, IOculusRightControllerActions
    {
        public HandType handType;

        public @OculusControllerControls asset = null;


        public float PrimaryButtonValue => primaryButton;
        public float SecondaryButtonValue => secondaryButton;
        public float MenuButtonValue => menuButton;
        public float TriggerValue => triggerValue;
        public float GripValue => gripValue;
        public Vector2 ThumbstickValue => thumbstickValue;

        private Vector2 thumbstickValue = Vector2.zero;
        private float primaryButton = 0;
        private float secondaryButton = 0;
        private float menuButton = 0;
        private float triggerValue = 0;
        private float gripValue = 0;

        public string GetValueString()
        {
            return string.Format("primaryButton : {0:F2}\nsecondaryButton : {1:F2}\nmenuButton : {2:F2}\ntriggerValue : {3:F2}\ngripValue : {4:F2}\nthumbstickValue : {5:F2}, {6:F2}", 
                primaryButton, 
                secondaryButton, 
                menuButton, 
                triggerValue, 
                gripValue, 
                thumbstickValue.x, 
                thumbstickValue.y);
        }

        [HideInInspector] public UnityEvent OnPrimaryButtonDown;
        [HideInInspector] public UnityEvent OnPrimaryButtonUp;

        [HideInInspector] public UnityEvent OnSecondaryButtonDown;
        [HideInInspector] public UnityEvent OnSecondaryButtonUp;

        [HideInInspector] public UnityEvent OnMenuButtonDown;
        [HideInInspector] public UnityEvent OnMenuButtonUp;

        [HideInInspector] public UnityEvent OnThumbstickDown;
        [HideInInspector] public UnityEvent<Vector2> OnUpdateThumbstick;
        [HideInInspector] public UnityEvent OnThumbstickUp;

        [HideInInspector] public UnityEvent OnTriggerDown;
        [HideInInspector] public UnityEvent<float> OnUpdateTrigger;
        [HideInInspector] public UnityEvent OnTriggerUp;

        [HideInInspector] public UnityEvent OnGripDown;
        [HideInInspector] public UnityEvent<float> OnUpdateGrip;
        [HideInInspector] public UnityEvent OnGripUp;


#if UNITY_EDITOR
        private void Reset()
        {
            if (gameObject.name.Contains("LeftHand"))
                handType = HandType.LeftHand;
            else if (gameObject.name.Contains("RightHand"))
                handType = HandType.RightHand;
        }
#endif

        private void Awake()
        {
            asset = new @OculusControllerControls();

            if (handType == HandType.LeftHand)
            {
                asset.@OculusLeftController.SetCallbacks(this);
            }
            else
            {
                asset.@OculusRightController.SetCallbacks(this);
            }
        }

        private void OnEnable()
        {
            asset.Enable();
        }

        private void OnDisable()
        {
            asset.Disable();
        }

        public void RemoveAllListeners()
        {
            OnPrimaryButtonDown.RemoveAllListeners();
            OnPrimaryButtonUp.RemoveAllListeners();

            OnSecondaryButtonDown.RemoveAllListeners();
            OnSecondaryButtonUp.RemoveAllListeners();

            OnMenuButtonDown.RemoveAllListeners();
            OnMenuButtonUp.RemoveAllListeners();

            OnThumbstickDown.RemoveAllListeners();
            OnUpdateThumbstick.RemoveAllListeners();
            OnThumbstickUp.RemoveAllListeners();

            OnTriggerDown.RemoveAllListeners();
            OnUpdateTrigger.RemoveAllListeners();
            OnTriggerUp.RemoveAllListeners();

            OnGripDown.RemoveAllListeners();
            OnUpdateGrip.RemoveAllListeners();
            OnGripUp.RemoveAllListeners();
        }

        protected float ReadValue(InputAction action)
        {
            if (action == null)
                return default;

            if (action.activeControl is AxisControl)
                return action.ReadValue<float>();

            if (action.activeControl is Vector2Control)
                return action.ReadValue<Vector2>().magnitude;

            return IsPressed(action) ? 1f : 0f;
        }

        protected bool IsPressed(InputAction action)
        {
            float m_ButtonPressPoint = 0;

            if (action == null)
                return false;

#if INPUT_SYSTEM_1_1_OR_NEWER || INPUT_SYSTEM_1_1_PREVIEW // 1.1.0-preview.2 or newer, including pre-release
                return action.phase == InputActionPhase.Performed;
#else
            if (action.activeControl is ButtonControl buttonControl)
                return buttonControl.isPressed;

            if (action.activeControl is AxisControl)
                return action.ReadValue<float>() >= m_ButtonPressPoint;

            return action.triggered || action.phase == InputActionPhase.Performed;
#endif
        }

        // 	Press
        // 	Release

        public void OnPrimaryButton(InputAction.CallbackContext context)
        {
            primaryButton = context.ReadValue<float>();

            if (context.started)
                OnPrimaryButtonDown.Invoke();

            if (context.canceled)
                OnPrimaryButtonUp.Invoke();
        }

        public void OnSecondaryButton(InputAction.CallbackContext context)
        {
            secondaryButton = context.ReadValue<float>();

            if (context.started)
                OnSecondaryButtonDown.Invoke();

            if (context.canceled)
                OnSecondaryButtonUp.Invoke();
        }

        public void OnMenuButton(InputAction.CallbackContext context)
        {
            menuButton = context.ReadValue<float>();

            if (context.started)
                OnMenuButtonDown.Invoke();

            if (context.canceled)
                OnMenuButtonUp.Invoke();
        }

        public void OnTrigger(InputAction.CallbackContext context)
        {
            triggerValue = context.ReadValue<float>();

            if (context.started)
                OnTriggerDown.Invoke();

            OnUpdateTrigger.Invoke(triggerValue);

            if (context.canceled)
                OnTriggerUp.Invoke();
        }

        public void OnGrip(InputAction.CallbackContext context)
        {
            gripValue = context.ReadValue<float>();

            if (context.started)
                OnGripDown.Invoke();

            OnUpdateGrip.Invoke(gripValue);

            if (context.canceled)
                OnGripUp.Invoke();
        }

        public void OnThumbstick(InputAction.CallbackContext context)
        {
            thumbstickValue = context.ReadValue<Vector2>();
            
            if (context.started)
                OnThumbstickDown.Invoke();

            OnUpdateThumbstick.Invoke(thumbstickValue);

            if (context.canceled)
                OnThumbstickUp.Invoke();
        }
    }
}