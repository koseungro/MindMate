/// 작성자: 고승로
/// 작성일: 2020-09-28
/// 수정일: 2020-09-28
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNI
{
    public class LayForSequence : MonoBehaviour, IVisualObject
    {
        public VisualType Type => VisualType.Lay;

        public bool IsFinish => true;

        [SerializeField] private GameObject layObject;

        public void Active(CutData option)
        {
            ActiveEvent(option.layOption.isOn);
        }

        public void Init()
        {

        }

        public void MyUpdate()
        {

        }
        private void ActiveEvent(bool isActive)
        {
            layObject.SetActive(isActive);
        }
    }
}