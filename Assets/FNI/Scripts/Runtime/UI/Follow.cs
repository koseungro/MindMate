/// 작성자: 오상진
/// 작성일: 2021-07-07
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 

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
    public class Follow : MonoBehaviour
    {
        private Transform targetTr;

        void Awake()
        {
            targetTr = GameObject.Find(this.gameObject.name + "Base").transform;
            this.gameObject.transform.SetParent(targetTr.parent.transform);

            if (this.GetComponent<Transform>() != null)
            {
                this.gameObject.transform.localPosition = targetTr.transform.localPosition;
                this.gameObject.transform.localRotation = targetTr.transform.localRotation;
            }
            else
            {
                this.gameObject.GetComponent<RectTransform>().transform.localPosition = targetTr.transform.position;
                this.gameObject.GetComponent<RectTransform>().transform.localRotation = targetTr.transform.rotation;
            }
        }
    }
}