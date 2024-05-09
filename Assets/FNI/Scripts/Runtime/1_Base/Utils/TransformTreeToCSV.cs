/// 작성자: 백인성
/// 작성일: 2022-04-25
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
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    public class TransformTreeToCSV : MonoBehaviour
    {
        public StringBuilder builder = new StringBuilder();
        public void Write()
        {
            GetName(transform, 0);

            Debug.Log(builder.ToString());
        }

        public void GetName(Transform target, int depth)
        {
            string comma = "";
            for (int cnt =  0; cnt < depth; cnt++)
            {
                comma += ",";
            }
            string name = $"{comma}{target.name}\n";
            builder.Append(name);

            for (int cnt = 0; cnt < target.childCount; cnt++)
            {
                GetName(target.GetChild(cnt), depth+1);
            }
        }
    }
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TransformTreeToCSV))]
    public class TransformTreeToCSVEditor : Editor
    {
        private TransformTreeToCSV Target
        {
            get
            {
                if (m_target == null)
                    m_target = base.target as TransformTreeToCSV;

                return m_target;
            }
        }
        private TransformTreeToCSV m_target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Make"))
                Target.Write();

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