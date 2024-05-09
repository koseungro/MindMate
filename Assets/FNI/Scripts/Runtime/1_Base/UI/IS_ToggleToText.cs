/// 작성자: 백인성
/// 작성일: 2022-04-20
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    public class IS_ToggleToText : MonoBehaviour
    {
        private TextMeshProUGUI tmp;
        [TextArea]
        public string onText;
        [TextArea]
        public string offText;

        private void Start()
        {
            tmp = GetComponent<TextMeshProUGUI>();
        }

        public void Set(bool value)
        {
            tmp.text = value ? onText : offText;
        }
    }
}