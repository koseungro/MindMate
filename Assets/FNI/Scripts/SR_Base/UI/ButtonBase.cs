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
        /// ���� SceneID�� Ȯ���ؼ� Button�� Event �߰�
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
        /// ���� ���� Ȱ��ȭ�Ǿ��ִ� Button�� ���
        /// </summary>
        private ButtonBase[] curButtons;
        private SliderBase curSlider;

        /// <summary>
        /// �ش� ��ư�� �� �̵� �̺�Ʈ ����� �޷��ִ��� üũ
        /// </summary>
        private bool SceneMoveFunc = false;

        /// <summary>
        /// ��ư�� �̹� Ŭ���Ǿ����� üũ => �� �̵� �ߺ�üũ �ȵǰ� ����
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
        /// ���� Active �Ǿ��ִ� ��� ��ư ��Ȱ��ȭ
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
        /// ���� Fade �Ϸ� �� ���Ŀ� ��ư�� Interactable Ȱ��ȭ
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
        /// ��ư OnClick�� ���� ���� ���� SceneID�� �´� Ư�� Function �߰�
        /// </summary>
        public void AddButtonEvent()
        {
            ButtonEvent onButtonEvent;

            MyButton.onClick.RemoveAllListeners();
            onButtonEvent = buttonEventList.Find(x => x.sceneID == Main.curSceneID);
            MyButton.onClick.AddListener(() => onButtonEvent.buttonEvent?.Invoke());
        }

        /// <summary>
        /// �� �̵� �̺�Ʈ�� ������� �ʴٸ� �̺�Ʈ �߰�
        /// </summary>
        /// <param name="nextScene">�Ѿ ��</param>
        public void AddNextSceneEvent(SceneData nextScene)
        {
            if (!SceneMoveFunc)
            {
                //MyButton.onClick.AddListener(() => MyMain.OnButtonSequence(nextScene));
                MyButton.onClick.AddListener(delegate { MyMain.OnButtonSequence(nextScene); });
                SceneMoveFunc = true;
            }
            else
                Debug.Log($"<color=red>{MyButton.name}</color> ��ư�� Next Scene�� �̹� �ֽ��ϴ�.");
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            // Fade ȿ���� ������ ������ ��ư�� ���ͷ��� �����ϵ���
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

                // ���� �������� ��� Audio ��ž
                for (int i = 0; i < NarrationForSequence.Instance.AudioList.Count; i++)
                {
                    AudioSource curAudio = NarrationForSequence.Instance.AudioList[i];
                    if (curAudio.isPlaying)
                    {
                        if (curAudio.name == "BackgroundSound") // ������� Fade�� ����
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
