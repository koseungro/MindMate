/// 작성자: 백인성
/// 작성일: 2022-05-26
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
    public interface IISToggle
    {
        void Toggle(bool isOn);
        void State(bool isOn);
    }
    public class IS_Toggle : MonoBehaviour
    {
        public bool isOn;

        private IISToggle[] toggles;
        private bool startToggleValue;

        private void Start()
        {
            toggles = GetComponents<IISToggle>();

            for (int cnt = 0; cnt < toggles.Length; cnt++)
            {
                toggles[cnt].State(isOn);
            }

            startToggleValue = isOn;
        }

        public void Toggle()
        {
            if (isOn)
                isOn = false;
            else
                isOn = true;

            for (int cnt = 0; cnt < toggles.Length; cnt++)
            {
                toggles[cnt].Toggle(isOn);
            }
        }
        public void ResetToggle()
        {
            for (int cnt = 0; cnt < toggles.Length; cnt++)
            {
                toggles[cnt].State(startToggleValue);
            }
        }
    }
}