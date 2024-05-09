/// 작성자: 조효련
/// 작성일: 2022-01-27
/// 수정일: 
/// 저작권: Copyright(C) FNIKorea Co., Ltd.. (주)에프앤아이코리아

using FNI.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FNI.Common.Utils
{
    // 사용법 : StartCoroutine(CoroutineUtil.TimerRoutine(duration, true, () => { }));
    public class CoroutineUtil
    {
        public enum FadeDirection
        {
            In, // FadeIn : 0 => 1
            Out // FadeOut : 1 => 0
        }

        public static IEnumerator FadeRoutine(FadeDirection fadeDirection, float duration, Action<float> startCallback, Action<float> updateCallback, Action<float> endCallback)
        {
            float currentTime = fadeDirection == FadeDirection.Out ? duration : 0f;

            if (startCallback != null)
                startCallback.Invoke(fadeDirection == FadeDirection.Out ? 1 : 0f);

            // 반환값 : 1 -> 0 
            if (fadeDirection == FadeDirection.Out)
            {
                while (currentTime >= 0f)
                {
                    currentTime -= Time.deltaTime;

                    float nomalize = currentTime / duration;
                    if (nomalize <= 0f)
                        nomalize = 0f;

                    // Debug.Log($"fadeDirection : {fadeDirection} [currentTime : {currentTime} ][ nomalize : {nomalize} ]");

                    if (updateCallback != null)
                        updateCallback(nomalize);

                    yield return null;
                }
            }
            // 반환값 : 0 -> 1
            else
            {
                while (currentTime <= duration)
                {
                    currentTime += Time.deltaTime;

                    float nomalize = currentTime / duration;
                    if (nomalize >= 1f)
                        nomalize = 1f;

                    // Debug.Log($"fadeDirection : {fadeDirection} [currentTime : {currentTime} ][ nomalize : {nomalize} ]");

                    if (updateCallback != null)
                        updateCallback(nomalize);

                    yield return null;
                }
            }

            if (endCallback != null)
                endCallback.Invoke(fadeDirection == FadeDirection.Out ? 0 : 1);
        }

        public static IEnumerator FadeRoutine(FadeDirection fadeDirection, float fadeTime)
        {
            // Fade 애니메이션 재생 시간 (단위:초)
            float value = fadeDirection == FadeDirection.Out ? 1 : 0;
            float speed = 1.0f / fadeTime;

            if (fadeDirection == FadeDirection.Out)
            {
                while (value >= 0)
                {
                    value += Time.deltaTime * speed * -1;
                    Debug.Log($"{value}");

                    yield return null;
                }
            }
            else
            {
                while (value <= 1)
                {
                    value += Time.deltaTime * speed * 1;
                    Debug.Log($"{value}");

                    yield return null;
                }
            }
        }


        public static IEnumerator FadeInAnimationRoutine(AnimationCurve animationCurve, float start, float end, float time, UnityAction<float> startCallback, UnityAction<float> updateCallback, UnityAction<float> endCallback)
        {
            startCallback.Invoke(start);

            float rate = 1 / time;

            float i = 0;
            while (i < 1f)
            {
                i += rate * Time.deltaTime;

                float value = Mathf.Lerp(start, end, animationCurve.Evaluate(i));
                updateCallback.Invoke(value);

                yield return 0;
            }

            endCallback.Invoke(end);
        }

        public static IEnumerator FadeOutAnimationRoutine(AnimationCurve animationCurve, float start, float end, float time, UnityAction<float> startCallback, UnityAction<float> updateCallback, UnityAction<float> endCallback)
        {
            startCallback.Invoke(start);

            float rate = 1 / time;

            float i = 1;
            while (i > 0f)
            {
                i -= rate * Time.deltaTime;

                float value = Mathf.Lerp(start, end, animationCurve.Evaluate(i));
                updateCallback.Invoke(value);

                yield return 0;
            }

            endCallback.Invoke(end);
        }

        // start 0 , end 1
        public static IEnumerator FadeInOutAnimationRoutine(AnimationCurve animationCurve, float start, float end, float time, UnityAction<float> startCallback, UnityAction<float> updateCallback, UnityAction<float> endCallback)
        {
            if (startCallback != null) 
                startCallback.Invoke(start);

            float fadeTime = time * 0.5f;

            yield return FadeInAnimationRoutine(animationCurve, start, end, fadeTime, 
                null, 
                (value) => {
                    if (updateCallback != null) 
                        updateCallback.Invoke(value);
                }, 
                null);

            yield return null;
            // Debug.Log($"ShowFadeOut ------------");

            yield return FadeOutAnimationRoutine(animationCurve, end, start, fadeTime, 
                null, 
                (value) => {
                    if (updateCallback != null)
                        updateCallback.Invoke(value);
                }, 
                null);

            // Mathf.InverseLerp(0, spawnEffectTime, timer)

            if (endCallback != null)
                endCallback.Invoke(start);
        }



        #region 타이머
        public static IEnumerator TimerRoutine(float duration, Action callback)
        {
            return RepeatTimerRoutine(duration, false, callback);
        }

        public static IEnumerator RepeatTimerRoutine(float duration, bool repeat, Action callback)
        {
            do
            {
                yield return new WaitForSeconds(duration);

                if (callback != null)
                    callback();

            } while (repeat);
        }

        public static IEnumerator StartRealtimeRoutine(float time, System.Action callback)
        {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + time)
            {
                yield return null;
            }

            if (callback != null)
                callback();
        }

        public static IEnumerator NextFrame(Action callback)
        {
            yield return new WaitForEndOfFrame();

            if (callback != null)
                callback();
        }

        public static IEnumerator CountdownRoutine(int count, Action callback)
        {
            int counter = count;
            while (counter > 0)
            {
                yield return new WaitForSeconds(1);
                counter--;
            }

            if (callback != null)
                callback();
        }

        public static IEnumerator CountdownRoutine(float seconds, Action<string> process, Action callback)
        {
            float currentSeconds = seconds;
            while (currentSeconds > 0)
            {
                currentSeconds -= Time.deltaTime;

                string min = Mathf.Floor(currentSeconds / 60).ToString("00");
                string sec = (currentSeconds % 60).ToString("00");
                string time = string.Format("{0}:{1}", min, sec);

                if (process != null)
                    process(time);

                yield return null;
            }

            if (callback != null)
                callback.Invoke();
        }


        // System.Timers.Timer timer;
        // timer = new System.Timers.Timer();
        // timer.Interval = 500;
        // timer.Elapsed += OnTimerEvent;
        // 
        // 이벤트 반복 발생 여부 &gt; true : 반복 , false : 한번</span>
        // timer.AutoReset = true;
        // timer.Enabled = true;
        // timer.Start();
        // timer.Stop();

        #endregion


        #region 페이드

        // FadeIn 0f ~ 1f  투명 -> 불투명
        public static IEnumerator FadeInOutColorAnimationRoutine(AnimationCurve animationCurve, Color startColor, Color endColor, float time, UnityAction<Color> startCallback, UnityAction<Color> updateCallback, UnityAction<Color> endCallback)
        {
            startCallback.Invoke(startColor);

            float rate = 1f / time;

            float i = 0;
            while (i < 1f)
            {
                i += rate * Time.deltaTime;

                Color color = Color.Lerp(startColor, endColor, animationCurve.Evaluate(i));
                updateCallback.Invoke(color);

                Debug.Log($"ShowFade 1 - {color.a}");

                yield return 0;
            }

            Debug.Log($"------------------------");

            i = 1;
            while (i > 0f)
            {
                i -= rate * Time.deltaTime;

                if (i >= 0f)
                {
                    float alpha = animationCurve.Evaluate(1f - i);
                    Color color = Color.Lerp(endColor, startColor, alpha);
                    updateCallback.Invoke(color);

                    Debug.Log($"ShowFade 2 - {i} {alpha}");
                }

                yield return 0;
            }

            // Mathf.InverseLerp(0, spawnEffectTime, timer)

            endCallback.Invoke(startColor);
        }

        public static IEnumerator Fade(AnimationCurve animationCurve, FadeDirection direction, Color startColor, Color endColor, float time, UnityAction<Color> startCallback, UnityAction<Color> updateCallback, UnityAction<Color> endCallback)
        {
            var startTime = Time.time;
            var endTime = Time.time + time;
            var elapsedTime = 0f;

            float value = 0;

            if (direction == FadeDirection.In)
                value = animationCurve.Evaluate(0f);
            else
                value = animationCurve.Evaluate(1f);


            Color color = Color.Lerp(startColor, endColor, value);
            // Debug.Log($"{direction} => {color.a * 255}");
            startCallback.Invoke(color);

            while (Time.time <= endTime)
            {
                elapsedTime = Time.time - startTime;
                var percentage = 1 / (time / elapsedTime);

                if ((direction == FadeDirection.In))
                {
                    value = animationCurve.Evaluate(percentage);
                }
                else
                {
                    value = animationCurve.Evaluate(1f - percentage);
                }

                color = Color.Lerp(startColor, endColor, value);
                // Debug.Log($"{direction} => {color.a * 255}");
                updateCallback.Invoke(color);

                yield return new WaitForEndOfFrame();
            }

            if (direction == FadeDirection.In)
                value = animationCurve.Evaluate(1f);
            else
                value = animationCurve.Evaluate(0f);

            color = Color.Lerp(startColor, endColor, value);
            // Debug.Log($"{direction} => {color.a * 255}");
            endCallback.Invoke(color);
        }

        #endregion




        public static IEnumerator FadeOutAudioSource(AudioSource audioSource, float FadeTime)
        {
            float startVolume = audioSource.volume;
            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
                yield return null;
            }
            audioSource.Stop();
        }

        public static IEnumerator FadeInAudioSource(AudioSource audioSource, float FadeTime)
        {
            audioSource.Play();

            audioSource.volume = 0f;
            while (audioSource.volume < 1)
            {
                audioSource.volume += Time.deltaTime / FadeTime;
                yield return null;
            }
        }

        // 오브젝트 이동 - 커브 이용
        public static IEnumerator MoveObjectRoutine(Transform transform, Vector3 destPosition, AnimationCurve animationCurve, float duration = 0.5f)
        {
            Vector3 startPosition = transform.localPosition;

            float i = 0;
            float rate = 1 / duration;
            while (i < 1)
            {
                i += rate * Time.deltaTime;
                float progressValue = animationCurve.Evaluate(i);

                transform.localPosition = Vector3.Lerp(startPosition, destPosition, progressValue);

                yield return 0;
            }

            transform.localPosition = destPosition;
        }

        // 오브젝트 이동 - 시작지에서 목적지에서
        public static IEnumerator MoveObjectRoutine(Transform transform, Vector3 startPosition, Vector3 destPosition, float duration)
        {
            float time = 0f;
            while (time <= duration)
            {
                time = time + Time.deltaTime;
                float percent = Mathf.Clamp01(time / duration);

                transform.position = Vector3.Lerp(startPosition, destPosition, percent);

                yield return null;
            }
        }

        // 오브젝트 이동 - 현재위치에서 목적지까지
        public static IEnumerator LerpPositionAnimationRoutine(Transform target, Vector3 targetPosition, float duration)
        {
            float time = 0;
            Vector3 startPosition = target.position;

            while (time < duration)
            {
                target.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            target.position = targetPosition;
        }
    }
}