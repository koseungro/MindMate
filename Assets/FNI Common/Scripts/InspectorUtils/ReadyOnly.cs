/// 작성자: 조효련
/// 작성일: 2022-01-25
/// 수정일: 
/// 저작권: Copyright(C) FNIKorea Co., Ltd.. (주)에프앤아이코리아

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FNI.Common
{
    /// <summary>
    /// 변수를 인스펙터에서 수정이 안되도록 만드는 속성
    /// 
    /// 사용법
    /// [ReadOnly] public int count;
    /// 
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {

    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
}