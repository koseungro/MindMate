/// 작성자: 고승로
/// 작성일: 2020-09-17
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FNI
{
    public class UIAnimations : MonoBehaviour
    {
        private static UIAnimations instance;

        public static UIAnimations Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new UIAnimations();
                }
                return instance;
            }
        }

        public IEnumerator UpAnimation(RectTransform rectTransform , UIAnimationOption aniOption)
        {
            float curTime = 0.0f;
            rectTransform.position = aniOption.startPos;

            while (curTime<aniOption.time)
            {
                curTime += Time.deltaTime;
                rectTransform.localPosition = Vector3.Lerp(aniOption.startPos, aniOption.endPos, curTime / aniOption.time);
                
                yield return null;
            }
        }

        public IEnumerator AlphaAnimation(float startAlpha, float endAlpha ,CanvasGroup obj, float maxTime)
        {
            float checktime = 0.0f;

            while (checktime < 1.0f)
            {
                checktime += Time.deltaTime / maxTime;
                obj.alpha = Mathf.Lerp(startAlpha, endAlpha, checktime);

                yield return null;
            }
        }

        
        public IEnumerator AlphaAnimation(float startAlpha, float endAlpha, Text obj, float maxTime)
        {
            float checktime = 0.0f;
            Color curColor = obj.color;

            while (checktime < 1.0f)
            {
                checktime += Time.deltaTime / maxTime;
                curColor.a = Mathf.Lerp(startAlpha, endAlpha, checktime);
                obj.color = curColor;

                yield return null;
            }
        }

        public IEnumerator AlphaAnimation(float startAlpha, float endAlpha, Image obj, float maxTime)
        {
            float checktime = 0.0f;
            Color curColor = obj.color;

            while (checktime < 1.0f)
            {
                checktime += Time.deltaTime / maxTime;
                curColor.a = Mathf.Lerp(startAlpha, endAlpha, checktime);
                obj.color = curColor;

                yield return null;
            }
        }

        public void ChangeSprite(Image obj, Sprite changeSpirte)
        {
            obj.sprite = changeSpirte;
        }

        public void ChangeText(Text obj, string changeText)
        {
            obj.text = changeText;
        }


    }
}