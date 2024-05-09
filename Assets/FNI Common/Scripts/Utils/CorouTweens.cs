/// 작성자: 조효련
/// 작성일: 2022-01-27
/// 수정일: 
/// 저작권: Copyright(C) FNIKorea Co., Ltd.. (주)에프앤아이코리아

using FNI.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;


namespace FNI.Common.Utils
{
    public static class CorouTweens
    {
        // 사용법 : StartCoroutine(CorouTweens.Interpolate(transform.position, targetPosition, duration, CorouTweens.bounceOut, pos => transform.position = pos));

        public enum UpdateType
        {
            Update, FixedUpdate
        }

        private static readonly YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();

        #region Tweens

        public static AnimationCurve easeInOut
        {
            get { return AnimationCurve.EaseInOut(0, 0, 1, 1); }
        }

        public static AnimationCurve easeOut
        {
            get
            {
                return new AnimationCurve(new Keyframe(0, 0, 0, 2),
                                          new Keyframe(1, 1, 0, 0));
            }
        }

        public static AnimationCurve backOut
        {
            get
            {
                return new AnimationCurve(new Keyframe(0, 0, 0, 4),
                                          new Keyframe(1, 1, 0, 0));
            }
        }

        public static AnimationCurve bounceOut
        {
            get
            {
                return new AnimationCurve(new Keyframe(0, 0, 0, 0),
                                          new Keyframe(0.4f, 1, 5, -4),
                                          new Keyframe(0.7f, 1, 4, -3),
                                          new Keyframe(0.9f, 1, 3, -2),
                                          new Keyframe(1, 1, 2, 0));
            }
        }
        #endregion

        #region Float
        public static IEnumerator Lerp(float from, float to, float duration, Action<float> callback, UpdateType updateType = UpdateType.Update)
        {
            var waitInstruction = updateType == UpdateType.FixedUpdate ? waitForFixedUpdate : null;

            for (var time = Time.deltaTime; time < duration; time += Time.deltaTime)
            {
                var t = time / duration;
                callback(Mathf.Lerp(from, to, t));
                yield return waitInstruction;
            }
            callback(to);
        }

        public static IEnumerator LerpWithSpeed(float from, float to, float speed, Action<float> callback, UpdateType updateType = UpdateType.Update)
        {
            var waitInstruction = updateType == UpdateType.FixedUpdate ? waitForFixedUpdate : null;

            var value = Mathf.MoveTowards(from, to, speed * Time.deltaTime);
            callback(value);
            while (value != to)
            {
                yield return waitInstruction;
                value = Mathf.MoveTowards(value, to, speed * Time.deltaTime);
                callback(value);
            }
            callback(to);
        }

        public static IEnumerator Interpolate(float from, float to, float duration, AnimationCurve curve, Action<float> callback, UpdateType updateType = UpdateType.Update)
        {
            var waitInstruction = updateType == UpdateType.FixedUpdate ? waitForFixedUpdate : null;

            for (var time = Time.deltaTime; time < duration; time += Time.deltaTime)
            {
                var t = time / duration;
                callback(Mathf.Lerp(from, to, curve.Evaluate(t)));
                yield return waitInstruction;
            }
            callback(Mathf.Lerp(from, to, curve.Evaluate(1)));
        }
        #endregion

        #region Vector2
        public static IEnumerator Lerp(Vector2 from, Vector2 to, float duration, Action<Vector2> callback, UpdateType updateType = UpdateType.Update)
        {
            var waitInstruction = updateType == UpdateType.FixedUpdate ? waitForFixedUpdate : null;

            for (var time = Time.deltaTime; time < duration; time += Time.deltaTime)
            {
                var t = time / duration;
                callback(Vector2.Lerp(from, to, t));
                yield return waitInstruction;
            }
            callback(to);
        }

        public static IEnumerator LerpWithSpeed(Vector2 from, Vector2 to, float speed, Action<Vector2> callback, UpdateType updateType = UpdateType.Update)
        {
            var waitInstruction = updateType == UpdateType.FixedUpdate ? waitForFixedUpdate : null;

            var value = Vector2.MoveTowards(from, to, speed * Time.deltaTime);
            callback(value);
            while (value != to)
            {
                yield return waitInstruction;
                value = Vector2.MoveTowards(value, to, speed * Time.deltaTime);
                callback(value);
            }
            callback(to);
        }

        public static IEnumerator Interpolate(Vector2 from, Vector2 to, float duration, AnimationCurve curve, Action<Vector2> callback, UpdateType updateType = UpdateType.Update)
        {
            var waitInstruction = updateType == UpdateType.FixedUpdate ? waitForFixedUpdate : null;

            for (var time = Time.deltaTime; time < duration; time += Time.deltaTime)
            {
                var t = time / duration;
                callback(Vector2.Lerp(from, to, curve.Evaluate(t)));
                yield return waitInstruction;
            }
            callback(Vector2.Lerp(from, to, curve.Evaluate(1)));
        }
        #endregion

        #region Vector3
        public static IEnumerator Lerp(Vector3 from, Vector3 to, float duration, Action<Vector3> callback, UpdateType updateType = UpdateType.Update)
        {
            var waitInstruction = updateType == UpdateType.FixedUpdate ? waitForFixedUpdate : null;

            for (var time = Time.deltaTime; time < duration; time += Time.deltaTime)
            {
                var t = time / duration;
                callback(Vector3.Lerp(from, to, t));
                yield return waitInstruction;
            }
            callback(to);
        }

        public static IEnumerator LerpWithSpeed(Vector3 from, Vector3 to, float speed, Action<Vector3> callback, UpdateType updateType = UpdateType.Update)
        {
            var waitInstruction = updateType == UpdateType.FixedUpdate ? waitForFixedUpdate : null;

            var value = Vector3.MoveTowards(from, to, speed * Time.deltaTime);
            callback(value);
            while (value != to)
            {
                yield return waitInstruction;
                value = Vector3.MoveTowards(value, to, speed * Time.deltaTime);
                callback(value);
            }
            callback(to);
        }

        public static IEnumerator Interpolate(Vector3 from, Vector3 to, float duration, AnimationCurve curve, Action<Vector3> callback, UpdateType updateType = UpdateType.Update)
        {
            var waitInstruction = updateType == UpdateType.FixedUpdate ? waitForFixedUpdate : null;

            for (var time = Time.deltaTime; time < duration; time += Time.deltaTime)
            {
                var t = time / duration;
                callback(Vector3.Lerp(from, to, curve.Evaluate(t)));
                yield return waitInstruction;
            }
            callback(Vector3.Lerp(from, to, curve.Evaluate(1)));
        }
        #endregion

        #region Quaternion
        public static IEnumerator Lerp(Quaternion from, Quaternion to, float duration, Action<Quaternion> callback, UpdateType updateType = UpdateType.Update)
        {
            var waitInstruction = updateType == UpdateType.FixedUpdate ? waitForFixedUpdate : null;

            for (var time = Time.deltaTime; time < duration; time += Time.deltaTime)
            {
                var t = time / duration;
                callback(Quaternion.Slerp(from, to, t));
                yield return waitInstruction;
            }
            callback(to);
        }

        public static IEnumerator LerpWithSpeed(Quaternion from, Quaternion to, float speed, Action<Quaternion> callback, UpdateType updateType = UpdateType.Update)
        {
            var waitInstruction = updateType == UpdateType.FixedUpdate ? waitForFixedUpdate : null;

            var value = Quaternion.RotateTowards(from, to, speed * Time.deltaTime);
            callback(value);
            while (value != to)
            {
                yield return waitInstruction;
                value = Quaternion.RotateTowards(value, to, speed * Time.deltaTime);
                callback(value);
            }
            callback(to);
        }

        public static IEnumerator Interpolate(Quaternion from, Quaternion to, float duration, AnimationCurve curve, Action<Quaternion> callback, UpdateType updateType = UpdateType.Update)
        {
            var waitInstruction = updateType == UpdateType.FixedUpdate ? waitForFixedUpdate : null;

            for (var time = Time.deltaTime; time < duration; time += Time.deltaTime)
            {
                var t = time / duration;
                callback(Quaternion.Slerp(from, to, curve.Evaluate(t)));
                yield return waitInstruction;
            }
            callback(Quaternion.Slerp(from, to, curve.Evaluate(1)));
        }
        #endregion
    }
}