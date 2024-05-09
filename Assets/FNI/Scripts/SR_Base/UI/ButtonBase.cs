using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using FNI.XR;
using System;

namespace FNI
{
    public class ButtonBase : UIBase
    {
        //[SerializeField] private AudioSource audioSource = null;
        //[SerializeField] private AudioClip hoverClip = null;
        //[SerializeField] private AudioClip clickClip = null;

        /// <summary>
        /// 현재 SceneID를 확인해서 Button에 Event 추가
        /// </summary>
        [Serializable]
        public class ButtonEvent
        {
            public string sceneID;
            public UnityEvent buttonEvent;
        }

        private Button myButton;
        public Button MyButton
        {
            get
            {
                if (myButton == null)
                {
                    myButton = GetComponent<Button>();
                }
                return myButton;
            }
        }

        private Main myMain;
        private Main MyMain
        {
            get
            {
                if (myMain == null)
                    myMain = GameObject.Find("---------------Scripts/Main").GetComponent<Main>();

                return myMain;
            }
        }

        public List<ButtonEvent> buttonEventList = new List<ButtonEvent>();

        /// <summary>
        /// 현재 씬에 활성화되어있는 Button들 목록
        /// </summary>
        private ButtonBase[] curButtons;
        private SliderBase curSlider;

        /// <summary>
        /// 해당 버튼에 씬 이동 이벤트 기능이 달려있는지 체크
        /// </summary>
        private bool SceneMoveFunc = false;

        /// <summary>
        /// 버튼이 이미 클릭되었는지 체크 => 씬 이동 중복체크 안되게 위해
        /// </summary>
        private bool isButtonClicked = false;

        private void OnEnable()
        {
            isButtonClicked = false;

            StartCoroutine(CheckFadeCanvas());
        }
        private void OnDisable()
        {
            if (SceneMoveFunc)
                MyButton.onClick.RemoveAllListeners();

            SceneMoveFunc = false;
        }

        /// <summary>
        /// 현재 Active 되어있는 모든 버튼 비활성화
        /// </summary>
        private void DeactiveUI()
        {
            curButtons = FindObjectsOfType<ButtonBase>();

            for (int i = 0; i < curButtons.Length; i++)
            {
                curButtons[i].GetComponent<Button>().interactable = false;
                //Debug.Log($"{curButtons[i].name} Deactive");
            }

            curSlider = FindObjectOfType<SliderBase>();
            if (curSlider != null)
                curSlider.GetComponent<Slider>().interactable = false;

        }
        /// <summary>
        /// 씬이 Fade 완료 된 이후에 버튼의 Interactable 활성화
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckFadeCanvas()
        {
            MyButton.interactable = false;

            while (FadeInOutForSequence.Instance.canvasGroups[2].alpha != 0
                || FadeInOutForSequence.Instance.canvasGroups[1].alpha != 1)
            {
                //Debug.Log("Fading...");
                yield return null;
            }

            MyButton.interactable = true;
            //Debug.Log($"[CheckFadeCanvas/ButtonBase] ({gameObject.name}) Button Interactable : <color=yellow> {MyButton.interactable} </color>");

        }

        public void ButtonFunc()
        {
            print($"<color=green> Button Click Event !</color>");
        }

        /// <summary>
        /// 버튼 OnClick에 현재 진행 중인 SceneID에 맞는 특정 Function 추가
        /// </summary>
        public void AddButtonEvent()
        {
            ButtonEvent onButtonEvent;

            MyButton.onClick.RemoveAllListeners();
            onButtonEvent = buttonEventList.Find(x => x.sceneID == Main.curSceneID);
            MyButton.onClick.AddListener(() => onButtonEvent.buttonEvent?.Invoke());
        }

        /// <summary>
        /// 씬 이동 이벤트가 들어있지 않다면 이벤트 추가
        /// </summary>
        /// <param name="nextScene">넘어갈 씬</param>
        public void AddNextSceneEvent(SceneData nextScene)
        {
            if (!SceneMoveFunc)
            {
                //MyButton.onClick.AddListener(() => MyMain.OnButtonSequence(nextScene));
                MyButton.onClick.AddListener(delegate { MyMain.OnButtonSequence(nextScene); });
                SceneMoveFunc = true;
            }
            else
                Debug.Log($"<color=red>{MyButton.name}</color> 버튼의 Next Scene이 이미 있습니다.");
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            // Fade 효과가 끝났을 때에만 버튼이 인터렉션 가능하도록
            if (FadeInOutForSequence.Instance.canvasGroups[1].alpha == 1 // UI Canvas
                && FadeInOutForSequence.Instance.canvasGroups[2].alpha == 0) // Fade Canvas
            {
                if (MyButton.interactable)
                    base.OnPointerEnter(eventData);

                //audioSource.clip = hoverClip;
                //audioSource.Play();

            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {

        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (isButtonClicked == false)
            {
                if (FadeInOutForSequence.Instance.canvasGroups[1].alpha == 1
                    && FadeInOutForSequence.Instance.canvasGroups[2].alpha == 0)
                {
                    if (MyButton.interactable)
                        base.OnPointerClick(eventData);

                    //audioSource.clip = hoverClip;
                    //audioSource.Play();

                    if (SceneMoveFunc)
                    {
                        isButtonClicked = true;
                        MyButton.interactable = false;
                        DeactiveUI();
                    }
                    //Debug.Log($"<color=yellow> [{gameObject.name}] Button interactable : {MyButton.interactable} </color>");
                }
            }
        }
        public override void OnPointerDown(PointerEventData eventData)
        {

            //if (FadeInOutForSequence.Instance.canvasGroups[1].alpha == 1
            //    && FadeInOutForSequence.Instance.canvasGroups[2].alpha == 0)
            //{
            //    //audioSource.clip = clickClip;
            //    //audioSource.Play();
            //    if (NarrationForSequence.Instance.CurAudio.isPlaying)
            //        NarrationForSequence.Instance.CurAudio.Stop();
            //}
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (FadeInOutForSequence.Instance.canvasGroups[1].alpha == 1
                && FadeInOutForSequence.Instance.canvasGroups[2].alpha == 0)
            {
                //audioSource.clip = clickClip;
                //audioSource.Play();

                // 현재 진행중인 모든 Audio 스탑
                for (int i = 0; i < NarrationForSequence.Instance.AudioList.Count; i++)
                {
                    AudioSource curAudio = NarrationForSequence.Instance.AudioList[i];
                    if (curAudio.isPlaying)
                    {
                        if (curAudio.name == "BackgroundSound") // 배경음은 Fade로 끄기
                        {
                            //StartCoroutine(NarrationForSequence.Instance.FadeBackgroundVolume(curAudio.gameObject, 3f));
                        }
                        else
                        {
                            curAudio.Stop();
                            curAudio.clip = null;
                        }
                    }
                }
            }
        }
    }
}
