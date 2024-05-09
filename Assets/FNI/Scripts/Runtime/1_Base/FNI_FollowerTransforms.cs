/// 작성자: 백인성
/// 작성일: 2021-10-08
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

#if UNITY_EDITOR
using UnityEditor;
using FNI.IS.EDITOR;
using FNI.IS.EDITOR.Property;
#endif

namespace FNI.Common.Utils
{
    [Serializable]
    public class LerpOption
    {
        public bool use;
        [Range(0, 15)]
        public float speed;
    }
    [Serializable]
    public class FNI_Follower
    {
        public Transform source;
        public List<Transform> destList = new List<Transform>();

        public bool isLerpMode = false;

        public LerpOption pos = new LerpOption();
        public LerpOption rot = new LerpOption();

        public void Arrangement(string text)
        {
            destList.RemoveAll(x => x.name.Contains(text) == false);
        }

        public void Update()
        {
            for (int cnt = 0; cnt < destList.Count; cnt++)
            {
                if (isLerpMode)
                {
                    if (pos.use)
                        destList[cnt].position = Vector3.Lerp(destList[cnt].position, source.position, Time.deltaTime * pos.speed);
                    if (rot.use)
                        destList[cnt].rotation = Quaternion.Lerp(destList[cnt].rotation, source.rotation, Time.deltaTime * rot.speed);
                }
                else
                {
                    if (pos.use)
                        destList[cnt].position = source.position;
                    if (rot.use)
                        destList[cnt].rotation = source.rotation;
                }
            }
        }
    }
    public class FNI_FollowerTransforms : MonoBehaviour
    {
        public List<FNI_Follower> followerList = new List<FNI_Follower>();

        public bool IsUpdate { get => isUpdate; set => isUpdate = value; }
#if UNITY_EDITOR
        [ToggleLeft]
#endif
        public bool isUpdate;

        private void Awake()
        {
            if (!isUpdate)
            {
                for (int cnt = 0; cnt < followerList.Count; cnt++)
                {
                    followerList[cnt].Update();
                }
            }
        }

        private void LateUpdate()
        {
            FollwerUpdate();
        }
        public void FollwerUpdate()
        {
            if (isUpdate)
            {
                for (int cnt = 0; cnt < followerList.Count; cnt++)
                {
                    followerList[cnt].Update();
                }
            }
        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FNI_FollowerTransforms))]
    public class FNI_FollowerTransformsEditor : IS_Editor
    {
        private FNI_FollowerTransforms Target
        {
            get
            {
                if (m_target == null)
                    m_target = base.target as FNI_FollowerTransforms;

                return m_target;
            }
        }
        private FNI_FollowerTransforms m_target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (GUILayout.Button("Update Pos"))
            {
                Target.FollwerUpdate();
            }

            GetColliders();
            EditorGUILayout.EndVertical();

            SerializedProperty followerArray = serializedObject.FindProperty("followerList");
            SerializedProperty isUpdate = serializedObject.FindProperty("isUpdate");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(isUpdate);
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.indentLevel++;
                    followerArray.isExpanded = EditorGUILayout.Foldout(followerArray.isExpanded, followerArray.displayName, true);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                    if (SetPropertyArraySize(followerArray))
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();

                        SaveSerializedData();
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (followerArray.isExpanded)
                {
                    for (int cnt = 0; cnt < followerArray.arraySize; cnt++)
                    {
                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                        if (SetPropertyArrayController(followerArray, cnt, false, true))
                        {
                            EditorGUILayout.EndHorizontal();
                            break;
                        }
                        SetFollower(followerArray.GetArrayElementAtIndex(cnt), cnt);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndVertical();

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

        private void SetFollower(SerializedProperty follower, int cnt)
        {
            SerializedProperty source = follower.FindPropertyRelative("source");
            SerializedProperty destArray = follower.FindPropertyRelative("destList");
            SerializedProperty isLerpMode = follower.FindPropertyRelative("isLerpMode");
            SerializedProperty pos = follower.FindPropertyRelative("pos");
            SerializedProperty rot = follower.FindPropertyRelative("rot");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUIUtility.labelWidth = 100;
                    EditorGUILayout.PropertyField(isLerpMode);
                    EditorGUIUtility.labelWidth = originLabelWidth;
                    SetLerp(pos, isLerpMode.boolValue);
                    SetLerp(rot, isLerpMode.boolValue);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.labelWidth = 100;
                    EditorGUILayout.PropertyField(source, new GUIContent() { text = "Source" });
                    EditorGUIUtility.labelWidth = originLabelWidth;
                    if (source.objectReferenceValue != null)
                    {
                        EditorGUILayout.LabelField("정리", GUILayout.Width(30));
                        string[] split = source.objectReferenceValue.name.Split(' ');
                        for (int i = 0; i < split.Length; i++)
                        {
                            int length = split[i].Length == 1 ? 2 : split[i].Length;

                            if (GUILayout.Button(new GUIContent() { text = $"{split[i]}", tooltip = "포함단어 삭제" }, GUILayout.Width(length * 10)))
                            {
                                Target.followerList[cnt].Arrangement(split[i]);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUI.indentLevel++;
                        destArray.isExpanded = EditorGUILayout.Foldout(destArray.isExpanded, $"{destArray.displayName}[{destArray.arraySize}]", true);

                        EditorGUILayout.Space();

                        if (SetPropertyArraySize(destArray))
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
                            return;
                        }
                    }
                    EditorGUILayout.EndHorizontal();


                    EditorGUIUtility.labelWidth = 50;
                    if (destArray.isExpanded)
                    {
                        for (int i = 0; i < destArray.arraySize; i++)
                        {
                            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                            if (SetPropertyArrayController(destArray, i, true))
                            {
                                EditorGUILayout.EndHorizontal();
                                break;
                            }
                            SerializedProperty property = destArray.GetArrayElementAtIndex(i);
                            GUI.color = property.objectReferenceValue == null ? Color.red : Color.white;
                            EditorGUILayout.PropertyField(property, new GUIContent() { text = $"[{i:00}]" });
                            GUI.color = Color.white;
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUIUtility.labelWidth = originLabelWidth;
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }


        private void SetLerp(SerializedProperty lerp, bool isLerpMode)
        {
            SerializedProperty use = lerp.FindPropertyRelative("use");
            SerializedProperty speed = lerp.FindPropertyRelative("speed");

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField(lerp.displayName, EditorStyles.boldLabel, GUILayout.Width(30));

            EditorGUIUtility.labelWidth = 40;
            EditorGUILayout.PropertyField(use, new GUIContent() { text = "Move" });
            EditorGUIUtility.labelWidth = originLabelWidth;
            if (isLerpMode)
            {
                speed.floatValue = EditorGUILayout.Slider(speed.floatValue, 0, 15);
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.Slider(speed.floatValue, 0, 15);
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        private Transform colliderParent;
        private List<Transform> notFoundList = new List<Transform>();
        private bool addMode;
        private void GetColliders()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Auto Link : ", GUILayout.Width(60));
            addMode = EditorGUILayout.ToggleLeft("Add mode", addMode, GUILayout.Width(80));
            colliderParent = (Transform)EditorGUILayout.ObjectField(colliderParent, typeof(Transform), true);

            if (colliderParent != null)
            {
                if (GUILayout.Button("Set Auto Link"))
                {
                    List<Transform> list = new List<Transform>();

                    for (int cnt_A = 0; cnt_A < colliderParent.childCount; cnt_A++)
                    {
                        list.Add(colliderParent.GetChild(cnt_A));
                    }

                    if (addMode == false)
                    {
                        for (int cnt_B = 0; cnt_B < Target.followerList.Count; cnt_B++)
                        {
                            Target.followerList[cnt_B].destList.Clear();
                        }
                    }

                    for (int cnt_A = 0; cnt_A < list.Count; cnt_A++)
                    {
                        string[] names = list[cnt_A].name.Split('_');

                        string[] boneNames = names[1].Split('-');
                        string findName = boneNames.Length == 1 ? boneNames[0] : $"{boneNames[1]} {boneNames[0]}";

                        bool add = false;
                        for (int cnt_B = 0; cnt_B < Target.followerList.Count; cnt_B++)
                        {
                            if (Target.followerList[cnt_B].source.name.Contains(findName))
                            {
                                if(Target.followerList[cnt_B].destList.FindIndex(x => x.Equals(list[cnt_A])) == -1)
                                    Target.followerList[cnt_B].destList.Add(list[cnt_A]);
                                add = true;
                            }
                        }
                        if (add == false)
                            notFoundList.Add(list[cnt_A]);
                    }
                }
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.TextField("Collider 묶음 넣어주세요.", EditorStyles.miniButton);
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();

            if (notFoundList.Count != 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.enabled = false;
                for (int cnt = 0; cnt < notFoundList.Count; cnt++)
                {
                    EditorGUILayout.ObjectField(notFoundList[cnt], typeof(Transform), false);
                }
                GUI.enabled = true;
                EditorGUILayout.EndVertical();
            }
        }
    }
#endif
}