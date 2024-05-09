/// 작성자: 백인성
/// 작성일: 2022-01-20
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
#endif

#if UNITY_EDITOR
namespace FNI.IS.EDITOR.Property
{
    [CustomEditor(typeof(BoxCollider))]
    public class BoxColliderEditor : UnityEditor.Editor
    {
        private BoxCollider[] Target
        {
            get
            {
                if (m_target == null)
                    m_target = Array.ConvertAll(base.targets, x => x as BoxCollider);

                return m_target;
            }
        }
        private BoxCollider[] m_target;

        private Renderer renderer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Switch", GUILayout.Width(EditorGUIUtility.labelWidth));
            if (GUILayout.Button("X ↔ Y"))
            {
                for (int cnt = 0; cnt < Target.Length; cnt++)
                {
                    Target[cnt].size = new Vector3(Target[cnt].size.y, Target[cnt].size.x, Target[cnt].size.z);
                    Target[cnt].center = new Vector3(Target[cnt].center.y, Target[cnt].center.x, Target[cnt].center.z);
                }
            }
            if (GUILayout.Button("Y ↔ Z"))
            {
                for (int cnt = 0; cnt < Target.Length; cnt++)
                {
                    Target[cnt].size = new Vector3(Target[cnt].size.x, Target[cnt].size.z, Target[cnt].size.y);
                    Target[cnt].center = new Vector3(Target[cnt].center.x, Target[cnt].center.z, Target[cnt].center.y);
                }
            }
            if (GUILayout.Button("Z ↔ X"))
            {
                for (int cnt = 0; cnt < Target.Length; cnt++)
                {
                    Target[cnt].size = new Vector3(Target[cnt].size.z, Target[cnt].size.y, Target[cnt].size.x);
                    Target[cnt].center = new Vector3(Target[cnt].center.z, Target[cnt].center.y, Target[cnt].center.x);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Multiply", GUILayout.Width(EditorGUIUtility.labelWidth));
            if (GUILayout.Button("X x 2"))
            {
                for (int cnt = 0; cnt < Target.Length; cnt++)
                {
                    Target[cnt].size = new Vector3(Target[cnt].size.x * 2, Target[cnt].size.y, Target[cnt].size.z);
                    Target[cnt].center = new Vector3(Target[cnt].center.x * 2, Target[cnt].center.y, Target[cnt].center.z);
                }
            }
            if (GUILayout.Button("Y x 2"))
            {
                for (int cnt = 0; cnt < Target.Length; cnt++)
                {
                    Target[cnt].size = new Vector3(Target[cnt].size.x, Target[cnt].size.y * 2, Target[cnt].size.z);
                    Target[cnt].center = new Vector3(Target[cnt].center.x, Target[cnt].center.y * 2, Target[cnt].center.z);
                }
            }
            if (GUILayout.Button("Z x 2"))
            {
                for (int cnt = 0; cnt < Target.Length; cnt++)
                {
                    Target[cnt].size = new Vector3(Target[cnt].size.x, Target[cnt].size.y, Target[cnt].size.z * 2);
                    Target[cnt].center = new Vector3(Target[cnt].center.x, Target[cnt].center.y, Target[cnt].center.z * 2);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Fit", GUILayout.Width(EditorGUIUtility.labelWidth));
            if (renderer == null)
                renderer = (Renderer)EditorGUILayout.ObjectField(renderer, typeof(Renderer), true);
            else
                renderer = (Renderer)EditorGUILayout.ObjectField(renderer, typeof(Renderer), true, GUILayout.Width(110));
            if (renderer != null && GUILayout.Button("Apply"))
            {
                for (int cnt = 0; cnt < Target.Length; cnt++)
                {
                    Target[cnt].size = renderer.bounds.size;
                }
            }
            EditorGUILayout.EndHorizontal();
            //여기까지 검사해서 필드에 변화가 있으면
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Changed Update Mode");
                //변경이 있을 시 적용된다. 이 코드가 없으면 인스펙터 창에서 변화는 있지만 적용은 되지 않는다.
                for (int cnt = 0; cnt < Target.Length; cnt++)
                    EditorUtility.SetDirty(Target[cnt]);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
    [CustomPropertyDrawer(typeof(MinMax))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty findMin = property.FindPropertyRelative("m_min");
            SerializedProperty findMax = property.FindPropertyRelative("m_max");

            float min = findMin.floatValue,
                  max = findMax.floatValue;

            float secondLabelWidth = 30;

            Rect labelRect = new Rect()
            {
                x = position.x,
                y = position.y,
                width = EditorGUIUtility.labelWidth,
                height = position.height
            };
            float valueFeild = ((EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth) * 0.5f) - secondLabelWidth - 17;

            Rect label1Rect = new Rect()
            {
                x = labelRect.x + labelRect.width,
                y = position.y,
                width = secondLabelWidth,
                height = position.height
            };
            Rect field1Rect = new Rect()
            {
                x = label1Rect.x + label1Rect.width,
                y = position.y,
                width = valueFeild,
                height = position.height
            };
            Rect spaceRect = new Rect()
            {
                x = field1Rect.x + field1Rect.width,
                y = position.y,
                width = 10,
                height = position.height
            };
            Rect label2Rect = new Rect()
            {
                x = spaceRect.x + spaceRect.width,
                y = position.y,
                width = secondLabelWidth,
                height = position.height
            };
            Rect field2Rect = new Rect()
            {
                x = label2Rect.x + label2Rect.width,
                y = position.y,
                width = valueFeild,
                height = position.height
            };

            EditorGUI.BeginChangeCheck();

            EditorGUI.LabelField(labelRect, label.text);
            EditorGUI.LabelField(label1Rect, "Min");
            min = EditorGUI.FloatField(field1Rect, min);

            EditorGUI.LabelField(spaceRect, " ");

            EditorGUI.LabelField(label2Rect, "Max");
            max = EditorGUI.FloatField(field2Rect, max);

            if (EditorGUI.EndChangeCheck())
            {
                findMin.floatValue = min;
                findMax.floatValue = max;
            }
        }
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ToggleLeft : PropertyAttribute { }
    [CustomPropertyDrawer(typeof(ToggleLeft))]
    public class ToggleLftDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            bool check = EditorGUI.ToggleLeft(position, label, property.boolValue);

            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = check;
            }
        }
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class StringFlag : PropertyAttribute
    {
        public string[] names;

        public StringFlag(params string[] names)
        {
            this.names = names;
        }

        public int Index(string name)
        {
            for (int cnt = 0; cnt < names.Length; cnt++)
            {
                if (names[cnt] == name)
                    return cnt;
            }
            return -1;
        }
    }
    [CustomPropertyDrawer(typeof(StringFlag))]
    public class StringFlagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            StringFlag getAttribute = (StringFlag)attribute;
            EditorGUI.BeginChangeCheck();

            int check = EditorGUI.Popup(position, label.text, getAttribute.Index(property.stringValue), getAttribute.names);

            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = getAttribute.names[check];
            }
        }
    }

    public class ShowPopup : EditorWindow
    {
        private static ShowPopup popup;

        private static int min;
        private static int max;
        private static int cur;
        private static int selected;
        private static UnityAction<int> onSelected;

        public static void Open(string _title, int _cur, int _min, int _max, UnityAction<int> _selected)
        {
            if (popup == null)
                popup = CreateInstance<ShowPopup>();
            popup.maxSize = popup.minSize = new Vector2(300, 110);
            popup.titleContent = new GUIContent(_title);

            min = _min;
            max = _max - 1;
            cur = _cur;
            onSelected = _selected;
            selected = _cur;

            popup.ShowAuxWindow();
        }
        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Selected {cur}/{max}!", 
                                       new GUIStyle(EditorStyles.label)
                                       {
                                           alignment = TextAnchor.MiddleCenter,
                                           fontStyle = FontStyle.Bold,
                                           wordWrap = true
                                       } );
            EditorGUILayout.Space();

            selected = EditorGUILayout.IntField("Move to", selected);
            selected = Mathf.Clamp(selected, min, max);

            EditorGUILayout.Space();

            float singleWidth = 145;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Move to First", GUILayout.Width(singleWidth)))
            {
                onSelected?.Invoke(min);
                popup.Close();
            }
            if (GUILayout.Button("Move to Ended", GUILayout.Width(singleWidth)))
            {
                onSelected?.Invoke(max);
                popup.Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Conform", GUILayout.Width(singleWidth)))
            {
                onSelected?.Invoke(selected);
                popup.Close();
            }
            if (GUILayout.Button("Cancel", GUILayout.Width(singleWidth)))
            {
                popup.Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }
}
#endif