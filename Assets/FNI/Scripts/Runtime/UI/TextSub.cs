/// 작성자: 고승로
/// 작성일: 2020-12-23
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력


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
    public class TextSub : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text = null;

        private void Reset()
        {
            text = gameObject.GetComponent<TextMeshProUGUI>();
        }

        public void ChangeText(int number)
        {
            text.text = "남아있는 정답 개수 : " + number.ToString();
        }


    }
}