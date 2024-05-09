/// 작성자: 백인성
/// 작성일: 2021-11-02
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
using FNI.XRST;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
#if UNITY_EDITOR
    public class ChangeTag : MonoBehaviour
    {
        public string targetTag;
        public string changeTag;

        public void SetTag(Transform target)
        {
            if (target.gameObject.CompareTag(targetTag))
            {
                target.gameObject.tag = changeTag;
            }
            for (int cnt = 0; cnt < target.childCount; cnt++)
            {
                SetTag(target.GetChild(cnt));
            }
        }
    }

    [CustomEditor(typeof(ChangeTag))]
    public class ChangeTagEditor : Editor
    {
        private ChangeTag Target
        {
            get
            {
                if (m_target == null)
                    m_target = base.target as ChangeTag;

                return m_target;
            }
        }
        private ChangeTag m_target;
        private int seletedTargetTag;
        private int seletedChangeTag;
        private List<string> tags = new List<string>(); 

        private void OnEnable()
        {
            tags.AddRange(UnityEditorInternal.InternalEditorUtility.tags);
        }
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            if (Target.targetTag == "")
                Target.targetTag = "Untagged";
            seletedTargetTag = tags.FindIndex(x => x.Equals(Target.targetTag));
            seletedTargetTag = EditorGUILayout.Popup("Target Tag", seletedTargetTag, tags.ToArray());
            if (seletedTargetTag != -1)
                Target.targetTag = tags[seletedTargetTag];

            if (Target.changeTag == "")
                Target.changeTag = "Untagged";
            seletedChangeTag = tags.FindIndex(x => x.Equals(Target.changeTag));
            seletedChangeTag = EditorGUILayout.Popup("Change Tag", seletedChangeTag, tags.ToArray());
            if (seletedChangeTag != -1)
                Target.changeTag = tags[seletedChangeTag];

            if (GUILayout.Button("Change"))
            {
                Target.SetTag(Target.transform);
            }
            //여기까지 검사해서 필드에 변화가 있으면
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Changed Update Mode");
                //변경이 있을 시 적용된다. 이 코드가 없으면 인스펙터 창에서 변화는 있지만 적용은 되지 않는다.
                EditorUtility.SetDirty(Target);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
#endif
}