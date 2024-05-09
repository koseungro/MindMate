/// 작성자: 백인성
/// 작성일: 2021-12-23
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
using FNI.IS.EDITOR.Property;
#endif

namespace FNI
{
    public class IS_UnityEvent : MonoBehaviour
    {
        [Serializable]
        public class Eventer
        {
#if UNITY_EDITOR
            [ToggleLeft]
#endif
            public bool useThis;
            public UnityEvent onEvent;

            public void Action()
            {
                if (useThis)
                    onEvent?.Invoke();
            }
        }
        [Serializable]
        public class LateEventer : Eventer
        {
            public float latePlayTime = -1;

            public void Action(MonoBehaviour mono = null)
            {
                if (mono == null || latePlayTime == -1)
                    base.Action();
                else
                    mono.StartCoroutine(LateAction_Routine());
            }

            private IEnumerator LateAction_Routine()
            {
                yield return new WaitForSeconds(latePlayTime);
                base.Action();
            }
        }

        public LateEventer onStarted = new LateEventer();
        public LateEventer onEnable = new LateEventer();
        public Eventer onDisable = new Eventer();
        public Eventer onUpdate = new Eventer();
        public Eventer onLateUpdate = new Eventer();

        private void Start()
        {
            onStarted.Action(this);
        }
        private void OnEnable()
        {
            onEnable.Action(this);
        }
        private void OnDisable()
        {
            onDisable.Action();
        }
        private void Update()
        {
            onUpdate.Action();
        }
        private void LateUpdate()
        {
            onLateUpdate.Action();
        }
    }
}