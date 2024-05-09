/// 작성자: 백인성
/// 작성일: 2022-12-06
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
    public class IS_AddStringNumber : MonoBehaviour
    {
        public TMP_InputField input;
        public bool canEvent = false;

        public void Add(int _value)
        {
            if (int.TryParse(input.text, out int value))
            {
                value += _value;

                Apply(value.ToString());
            }
        }
        public void Add(float _value)
        {
            if (float.TryParse(input.text, out float value))
            {
                value += _value;

                Apply(value.ToString());
            }
        }

        public void Apply(string value)
        {
            if (canEvent)
                input.text = value;
            else
                input.SetTextWithoutNotify(value);
        }
    }
}