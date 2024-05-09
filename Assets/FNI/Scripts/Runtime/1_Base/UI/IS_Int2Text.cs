/// 작성자: 백인성
/// 작성일: 2022-09-05
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
using FNI.IS;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    public class IS_Int2Text : MonoBehaviour
    {
        public int IntValue
        {
            get
            {
                if (norText != null)
                    return int.Parse(norText.text);
                else if (tmpInput != null)
                    return int.Parse(tmpInput.text);
                else if (tmpText != null)
                    return int.Parse(tmpText.text);
                else
                    throw new NullReferenceException("값을 적용할 곳이 없습니다.");
            }
            set
            {
                if (norText != null)
                    norText.text = value.ToString();
                else if (tmpInput != null)
                    tmpInput.text = value.ToString();
                else if (tmpText != null)
                    tmpText.text = value.ToString();
                else
                    throw new NullReferenceException("값을 적용할 곳이 없습니다.");
            }
        }

        private Text norText;
        private TMP_InputField tmpInput;
        private TextMeshProUGUI tmpText;

        public MinMax range;

        private void Start()
        {
            if (TryGetComponent(out norText) == false)
                if(TryGetComponent(out tmpText) == false)
                    if(TryGetComponent(out tmpInput) == false)
                        throw new NullReferenceException("UnityEngine.UI.Text나 TMPPro.TextMeshProUGUI를 가지고 있어야 합니다.");
        }
        public void Up()
        {
            IntValue++;

            if (range.Max < IntValue)
                IntValue = (int)range.Max;
        }
        public void Down()
        {
            IntValue--;

            if (IntValue < range.Min)
                IntValue = (int)range.Min;
        }
    }
}