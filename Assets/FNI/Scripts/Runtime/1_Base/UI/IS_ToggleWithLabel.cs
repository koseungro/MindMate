/// 작성자: 백인성
/// 작성일: 2023-01-11
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
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    public class IS_ToggleWithLabel : MonoBehaviour
    {
        public Toggle target;
        public TextMeshProUGUI label;

        public void SetText(string text)
        {
            label.text = text;
        }

        internal void Init(UnityAction<bool> p)
        {
            target.onValueChanged.RemoveAllListeners();
            target.onValueChanged.AddListener(p);
            target.SetIsOnWithoutNotify(false);
        }
    }
}