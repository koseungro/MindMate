/// 작성자: 백인성
/// 작성일: 2020-11-26
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using System;
using System.Collections;
using System.Collections.Generic;

using FNI;
using FNI.Common.Utils;
using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    public class IS_EventMaker : MonoBehaviour
    {
        public UnityEvent enableAction;
        public UnityEvent disableAction;

        public void OnEnable()
        {
            enableAction?.Invoke();
        }
        public void OnDisable()
        {
            disableAction?.Invoke();
        }
    }
}