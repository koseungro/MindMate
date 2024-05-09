/// 작성자: 오상진
/// 작성일: 2021-06-07
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FNI
{
    public class UIFocus : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Singleton
        private static UIFocus _instance;
        public static UIFocus Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIFocus>();

                    if (_instance == null)
                        Debug.Log("UIFocus를 찾을 수 없습니다. ");
                }
                return _instance;
            }
        }
        #endregion

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

        private bool hoverBool = false;
        private bool pressBool = false;


        void OnDisable()
        {
            hoverBool = false;
            pressBool = false;

        }

        void Update()
        {
            if(!MyMain.IsPlaying)
            {
                // hover 상태
                if (hoverBool)
                {
                    // press 가능
                    pressBool = true;
                }
                // hover가 아닌 상태
                else
                {
                    // press 불가능
                    pressBool = false;
                }
            }
        }

        public void CloseUI()
        {
            // 컨트롤러 눌렀을때 + 빈공간일때 UI 닫기
            if (!MyMain.IsPlaying)
            {
                if (!pressBool && !hoverBool)
                {
                    this.gameObject.SetActive(false);

                    // 애니메이션 Stop
                    //AnimationForSequence.Instance.CloseAnimationByUI();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverBool = true;
            Debug.Log("Hover " + hoverBool);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            hoverBool = false;
            Debug.Log("Hover " + hoverBool);
        }
    }
}