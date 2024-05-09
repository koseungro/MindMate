using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FNI
{

    public class VR_VideoController : MonoBehaviour
    {
        public Slider slider;

        public Sprite[] toggleSprite;
        private Image toggleImage;
        private Button toggleButton;
        private Button stopButton;
        private Button plusJumpButton;
        private Button minusJumpButton;

        public UnityAction toggle_ButtonEvent;
        public UnityAction stop_ButtonEvent;
        public UnityAction plus_ButtonEvent;
        public UnityAction minus_ButtonEvent;

        public UnityAction<float> drag_SlideEvent;
        public UnityAction<float> down_SlideEvent;
        public UnityAction<float> up_SlideEvent;


        /// <summary>
        /// 시작할때 필요한 Setting ( Awake, Start 처럼 사용해야함 )
        /// </summary>
        public void Setting()
        {
            toggleButton = transform.Find("Buttons/BTN_Toggle").GetComponent<Button>();
            stopButton = transform.Find("Buttons/BTN_Stop").GetComponent<Button>();
            plusJumpButton = transform.Find("Buttons/BTN_Jump(+)").GetComponent<Button>();
            minusJumpButton = transform.Find("Buttons/BTN_Jump(-)").GetComponent<Button>();
            toggleImage = toggleButton.transform.Find("Image").GetComponent<Image>();
            slider = transform.Find("Slider").GetComponent<Slider>();

            toggleButton.onClick.AddListener(OnToggleButtonEvent);
            stopButton.onClick.AddListener(OnStopButtonEvent);
            plusJumpButton.onClick.AddListener(OnPlusButtonEvent);
            minusJumpButton.onClick.AddListener(OnMinusButtonEvent);

            EventTrigger trigger = slider.transform.GetComponent<EventTrigger>();

            EventTrigger.Entry DownEntry = new EventTrigger.Entry();
            DownEntry.eventID = EventTriggerType.PointerDown;
            DownEntry.callback.AddListener((data) => { OnPointDownEvent((PointerEventData)data); });
            trigger.triggers.Add(DownEntry);

            EventTrigger.Entry UpEntry = new EventTrigger.Entry();
            UpEntry.eventID = EventTriggerType.PointerUp;
            UpEntry.callback.AddListener((data) => { OnPointUpEvent((PointerEventData)data); });
            trigger.triggers.Add(UpEntry);

            EventTrigger.Entry DragEntry = new EventTrigger.Entry();
            DragEntry.eventID = EventTriggerType.Drag;
            DragEntry.callback.AddListener((data) => { OnDragEvent((PointerEventData)data); });
            trigger.triggers.Add(DragEntry);
        }

        public void Init(VideoState state)
        {
            DrawToggleImage(state);
            slider.value = 0;
        }

        /// <summary>
        /// 영상 시간 받아서 슬라이더에 표시
        /// </summary>
        /// <param name="value"></param>
        public void UpdateSlider(double rTime)
        {
            slider.SetValueWithoutNotify((float)rTime);
        }

        /// <summary>
        /// 상황에 따라 토글 버튼 변경
        /// </summary>
        /// <param name="state">해당 상태를 넣으세요</param>
        public void DrawToggleImage(VideoState state)
        {
            switch (state)
            {
                case VideoState.Play:
                    toggleImage.sprite = toggleSprite[1];
                    break;
                default:
                    toggleImage.sprite = toggleSprite[0];
                    break;
            }
        }

        #region Event

        /// <summary>
        /// 버튼 Play 이벤트
        /// </summary>
        private void OnToggleButtonEvent()
        {
            toggle_ButtonEvent?.Invoke();
        }

        /// <summary>
        /// 버튼 Stop 이벤트
        /// </summary>
        private void OnStopButtonEvent()
        {
            stop_ButtonEvent?.Invoke();
        }

        /// <summary>
        /// 버튼 Plus 이벤트
        /// </summary>
        private void OnPlusButtonEvent()
        {
            plus_ButtonEvent?.Invoke();
        }

        /// <summary>
        /// 버튼 Minus 이벤트
        /// </summary>
        private void OnMinusButtonEvent()
        {
            minus_ButtonEvent?.Invoke();
        }

        /// <summary>
        /// Drag Slider Event
        /// </summary>
        private void OnDragEvent(PointerEventData data)
        {
            drag_SlideEvent?.Invoke(slider.value);
        }

        /// <summary>
        /// Down Slider Event
        /// </summary>
        /// <param name="data"></param>
        private void OnPointDownEvent(PointerEventData data)
        {
            down_SlideEvent?.Invoke(slider.value);
        }

        /// <summary>
        /// Slider Point up Event
        /// </summary>
        private void OnPointUpEvent(PointerEventData data)
        {
            up_SlideEvent?.Invoke(slider.value);
        }

        #endregion
    }
}