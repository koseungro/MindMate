/// 작성자: 조효련
/// 작성일: 2022-03-22
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

namespace FNI
{
    public class UIFPS : MonoBehaviour
    {
        public TMP_Text text;

        float deltaTime = 0.0f;
        float fps = 0.0f;

        private void Reset()
        {
            text = GetComponentInChildren<TMP_Text>();
        }

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            fps = 1.0f / deltaTime;
        }

        private void LateUpdate()
        {
            text.text = string.Format("{0:F2}", fps);
        }
    }
}