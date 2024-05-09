/// 작성자: 고승로
/// 작성일: 2020-08-25
/// 수정일: 2020-09-04
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FNI
{
    /// <summary>
    /// UI 관련 씬 데이터를 처리하는 클래스
    /// </summary>
    public class UIForSequence : MonoBehaviour, IVisualObject
    {
        #region Singleton
        private static UIForSequence _instance;
        public static UIForSequence Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<UIForSequence>();

                return _instance;
            }
        }
        #endregion

        public VisualType Type { get => VisualType.UI; }
        public bool IsFinish { get => isFinish; set => isFinish = value; }
        private bool isFinish = false;

        private GameObject curUI;
        public Breathe breathe;

        /// <summary>
        /// 데이터에 맞게 해당 UI 셋팅
        /// </summary>
        /// <param name="option"></param>
        public void Active(CutData option)
        {
            isFinish = false;

            try
            {
                if (option.uiOption.objPath != "")
                    curUI = gameObject.transform.Find(option.uiOption.objPath).gameObject;
                else // Breathe
                    curUI = breathe.gameObject;
            }
            catch 
            {
                Debug.Log($"<color=red>[{Type}]/ [{option.uiOption.uiType}]</color> 기능 실행 중 <color=red>[{option.uiOption.objPath}]</color>를 찾지 못하였습니다.");

                return;
            }


            switch (option.uiOption.uiType)
            {
                case UI_FuncType.None:
                    break;
                case UI_FuncType.AddScene_Button:
                    ButtonNextSceneEvent(option.uiOption.nextScene);
                    break;
                case UI_FuncType.Active:
                    ActiveEvent(option.uiOption.isActive);
                    break;
                case UI_FuncType.AllInactive:
                    AllInactiveEvent();
                    break;
                case UI_FuncType.Animation:
                    AnimationEvent(option.uiOption.uiAnimationOption);
                    break;
                case UI_FuncType.Transform:
                    TransformEvent(option.uiOption.pos_UI, option.uiOption.rot_UI);
                    break;
                case UI_FuncType.AddQuitEvent_Button:
                    ButtonQuitEvent();
                    break;
                case UI_FuncType.Breathe:
                    StartBreathe(option.uiOption);
                    break;
                case UI_FuncType.AddFunc_Button:
                    ButtonFuncEvent();
                    break;
                case UI_FuncType.ActiveBreatheCanvas:
                    ActiveBreatheCanvas(option.uiOption);
                    break;
                case UI_FuncType.ActiveVideoCheckButton:
                    ActiveMainMenuButton();
                    break;
                default:
                    break;
            }

            if(option.uiOption.uiType != UI_FuncType.Breathe)
                isFinish = true;
        }



        public void Init()
        {

        }

        public void MyUpdate()
        {

        }

        #region UI Event

        VideoContentChecker[] checkerButtons;
        public GameObject checkButton_Start;
        public GameObject checkButton_End;
        private void ActiveMainMenuButton()
        {
            checkerButtons = GetComponentsInChildren<VideoContentChecker>(true);

            for (int i = 0; i < checkerButtons.Length; i++) // Video 콘텐츠 Completed 확인
            {
                if (!checkerButtons[i].isCompleted)
                {
                    checkButton_Start.SetActive(true);
                    return; // 반복문 종료
                }
            }

            checkButton_End.SetActive(true);
        }

        private void ActiveBreatheCanvas(UIOption option)
        {
            curUI.transform.parent.gameObject.SetActive(option.isActive);
        }

        private void StartBreathe(UIOption option)
        {
            Breathe breathe = curUI.GetComponent<Breathe>();
            breathe.RunBreathCoroutine(option.breatheCnt, option.isSame, option.useHoldBreath);
        }

        /// <summary>
        /// 해당 버튼 OnClick에 원하는 함수 할당 기능
        /// </summary>
        private void ButtonFuncEvent()
        {
            ButtonBase btn = curUI.GetComponent<ButtonBase>();
            btn.AddButtonEvent();
        }

        private void ButtonQuitEvent()
        {
            ButtonBase btn = curUI.GetComponent<ButtonBase>();
            btn.MyButton.onClick.AddListener(() => Application.Quit());
        }

        private void TransformEvent(Vector3 position, Vector3 rotation)
        {
            curUI.GetComponent<RectTransform>().anchoredPosition3D = position;
            curUI.transform.eulerAngles = rotation;
        }

        /// <summary>
        /// UI 버튼에 다음 씬 넘어가는 이벤트 달아주는 함수
        /// </summary>
        /// <param name="nextScene"></param>
        private void ButtonNextSceneEvent(SceneData nextScene)
        {
            ButtonBase btn = curUI.GetComponent<ButtonBase>();
            btn.AddNextSceneEvent(nextScene);
        }

        /// <summary>
        /// 해당 UI 키거나 꺼주는 함수
        /// </summary>
        /// <param name="isActive"></param>
        private void ActiveEvent(bool isActive)
        {
            curUI.SetActive(isActive);
        }

        /// <summary>
        /// 해당 UI 바로 아래 자식들만 꺼주는 함수
        /// </summary>
        private void AllInactiveEvent()
        {
            foreach (Transform child in curUI.transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        private void AnimationEvent(UIAnimationOption aniOption)
        {
            switch (aniOption.aniType)
            {
                case UIAnimationType.Move:
                    StartCoroutine(UIAnimations.Instance.UpAnimation(curUI.GetComponent<RectTransform>(), aniOption));
                    break;
                case UIAnimationType.TextFadeInOut:
                    StartCoroutine(UIAnimations.Instance.AlphaAnimation(aniOption.startA, aniOption.endA, curUI.GetComponent<Text>(), aniOption.time));
                    break;
                case UIAnimationType.ImageFadeInOut:
                    StartCoroutine(UIAnimations.Instance.AlphaAnimation(aniOption.startA, aniOption.endA, curUI.GetComponent<Image>(), aniOption.time));
                    break;
                case UIAnimationType.CanvasFadeInOut:
                    StartCoroutine(UIAnimations.Instance.AlphaAnimation(aniOption.startA, aniOption.endA, curUI.GetComponent<CanvasGroup>(), aniOption.time));
                    break;
                case UIAnimationType.ChangeImage:
                    UIAnimations.Instance.ChangeSprite(curUI.GetComponent<Image>(), aniOption.changeSprite);
                    break;
                case UIAnimationType.ChangeText:
                    UIAnimations.Instance.ChangeText(curUI.GetComponent<Text>(), aniOption.changeText);
                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}