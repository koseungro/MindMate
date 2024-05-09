/// 작성자: 고승로
/// 작성일: 2021-03-19
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 

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
    public class SliderTextChangeManager : MonoBehaviour
    {
        
        [SerializeField] private TextMeshProUGUI scores;
        [SerializeField] private Slider slider;

        public void TextChange()
        {
            
        }
    }
}