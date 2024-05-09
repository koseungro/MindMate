/// 작성자: 조효련
/// 작성일: 2022-01-14
/// 수정일: 
/// 저작권: Copyright(C) FNIKorea Co., Ltd.. (주)에프앤아이코리아

using FNI.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FNI
{
    public class UIToolUtil
    {
        public static IEnumerator MoveObjectRoutine(RectTransform target, Vector2 destPosition, float time)
        {
            Vector2 startPosition = target.anchoredPosition;
            float startTime = Time.realtimeSinceStartup;

            float fraction = 0f;
            while (fraction < 1f)
            {
                fraction = Mathf.Clamp01((Time.realtimeSinceStartup - startTime) / time);

                target.anchoredPosition = Vector2.Lerp(startPosition, destPosition, fraction);

                yield return null;
            }
        }

        public static IEnumerator Fade(float beginValue, float endValue, Action<float> callback)
        {
            float duration = 1.0f;
            float elapsedTime = 0.0f;

            while (elapsedTime <= duration)
            {
                float blend = Mathf.Clamp01(elapsedTime / duration);
                elapsedTime += Time.deltaTime;

                float _intensity = Mathf.Lerp(beginValue, endValue, blend);
                // 코드 구현

                callback(_intensity);

                yield return new WaitForEndOfFrame();
            }

            callback(endValue);
        }

        public static IEnumerator FadeVolumeRoutine(AudioSource audioSource, float startVolume, float targetVolume, float duration, AnimationCurve curve)
        {
            float counter = 0;

            while (counter < duration)
            {
                float fadeVolume = curve.Evaluate(counter / duration);
                audioSource.volume = fadeVolume * startVolume;

                counter += Time.deltaTime;
                yield return new WaitForSeconds(0);
            }

            audioSource.volume = targetVolume;
        }

        public static IEnumerator PosAnimationRoutine(RectTransform target, AnimationCurve animationCurve, float destPos, float duration = 0.5f)
        {
            if (target == null)
                yield break;

            Vector2 startValue = target.anchoredPosition;
            Vector2 endValue = target.anchoredPosition;
            endValue.x = destPos;

            float i = 0;
            float rate = 1 / duration;
            while (i < 1)
            {
                i += Time.deltaTime * rate;
                float progressValue = animationCurve.Evaluate(i);

                target.anchoredPosition = Vector2.Lerp(startValue, endValue, progressValue);

                yield return null;
            }

            target.anchoredPosition = endValue;
        }

        public static IEnumerator PosAnimationRoutine(RectTransform target, float destPos, float duration = 0.5f)
        {
            if (target == null)
                yield break;

            Vector2 startValue = target.anchoredPosition;
            Vector2 endValue = target.anchoredPosition;
            endValue.x = destPos;

            float time = 0;
            while (time < duration)
            {
                target.anchoredPosition = Vector2.Lerp(startValue, endValue, time / duration);

                time += Time.deltaTime;
                yield return null;
            }

            target.anchoredPosition = endValue;
        }

        public static IEnumerator SizeAnimationRoutine(RectTransform target, AnimationCurve animationCurve, float destHeight, float duration = 0.5f)
        {
            if (target == null)
                yield break;

            Vector2 startValue = target.sizeDelta;
            Vector2 endValue = target.sizeDelta;
            endValue.y = destHeight;

            float i = 0;
            float rate = 1 / duration;
            while (i < 1)
            {
                i += rate * Time.deltaTime;
                float progressValue = animationCurve.Evaluate(i);

                target.sizeDelta = Vector2.Lerp(startValue, endValue, progressValue);

                yield return null;
            }

            target.sizeDelta = endValue;
        }

        public static IEnumerator SizeAnimationRoutine(RectTransform target, float destHeight, float duration = 0.5f)
        {
            if (target == null)
                yield break;

            Vector2 startValue = target.sizeDelta;
            Vector2 endValue = target.sizeDelta;
            endValue.y = destHeight;

            float time = 0;

            while (time < duration)
            {
                target.sizeDelta = Vector2.Lerp(startValue, endValue, time / duration);

                time += Time.deltaTime;
                yield return null;
            }

            target.sizeDelta = endValue;
        }

        public static void SnapTo(ScrollRect scroller, RectTransform child)
        {
            Canvas.ForceUpdateCanvases();

            var contentPos = (Vector2)scroller.transform.InverseTransformPoint(scroller.content.position);
            var childPos = (Vector2)scroller.transform.InverseTransformPoint(child.position);

            var endPos = contentPos - childPos;

            if (!scroller.horizontal)
                endPos.x = contentPos.x;

            if (!scroller.vertical)
                endPos.y = contentPos.y;

            scroller.content.anchoredPosition = endPos;
        }
    }

    

    

#if UNITY_EDITOR
    public static class EditorUtil
    {
        public static void PrintChildName<T>(List<T> contents) where T : UnityEngine.Object
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < contents.Count; i++)
            {
                sb.Append(contents[i].name);
                sb.Append("\n");
            }
            Debug.Log(sb.ToString());
        }

        // "co l:concrete l:architecture t:texture2D"
        // Find all Texture2Ds that have 'co' in their filename,
        // that are labelled with 'concrete' or 'architecture' and 

        // t:texture2D
        // t:scene
        // t:prefab
        // t:material
        // t:GameObject
        // t:TextAsset
        // t:texture2D
        // t:AnimationClip
        // $"t:{typeof(AudioClip).Name}"
        public static T[] FindAllAssetsOfType<T>(string path) where T : UnityEngine.Object
        {
            return AssetDatabase.FindAssets("t:" + typeof(T).Name, new string[] { path }).
                                 Select(guid => AssetDatabase.GUIDToAssetPath(guid)).
                                 Select(path => AssetDatabase.LoadAssetAtPath<T>(path)).
                                 ToArray();
        }
    }
#endif
}