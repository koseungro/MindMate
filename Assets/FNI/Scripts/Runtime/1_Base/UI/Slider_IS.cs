/// 작성자: 백인성
/// 작성일: 2021-07-13
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.IS;
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
    public class Slider_IS : MonoBehaviour
    {
        [Serializable]
        public class FloatEvent : UnityEvent<float> { }
        public Slider MySlider
        { 
            get
            {
                if (slider == null)
                {
                    slider = transform.Find("Slider").GetComponent<Slider>();
                    slider.onValueChanged.AddListener(OnValueChanged);
                }
                return slider;
            }
        }
        private Slider slider;

        public float startValue;
        [Space]
        public MinMax range;
        [Space]
        public FloatEvent onValueChanged;

        private void Start()
        {
            MySlider.minValue = range.Min;
            MySlider.maxValue = range.Max;

            MySlider.value = startValue;
            onValueChanged?.Invoke(startValue);
        }

        private void OnValueChanged(float value)
        {
            onValueChanged?.Invoke(value);
        }

    }
}