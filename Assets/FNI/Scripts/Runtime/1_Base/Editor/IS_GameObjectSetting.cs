/// 작성자: 백인성
/// 작성일: 2022-07-05
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
using FNI.IS.Editor.Window;
using UnityEditor;
#endif

namespace FNI
{
    public partial class IS_GameObjectSetting : IS_EditorWindow<IS_GameObjectSetting>
    {
        [MenuItem("IS/GameObject Setting")]
        static void Open()
        {
            if (InitWindow("GameObject Setting ver 1.0"))
                SetWindowSize(new Vector2(650, 200), new Vector2(650, 200));
        }

        public enum ComponentEditType
        {
            Add,
            Remove,
            Have,
            Without,
        }

        string[] Tags => UnityEditorInternal.InternalEditorUtility.tags;
        string[] Layers => UnityEditorInternal.InternalEditorUtility.layers;
        GUIStyle RichLabel => new GUIStyle(EditorStyles.label) { richText = true };
        GUIStyle RichLabel_LikeTextField => new GUIStyle(EditorStyles.textField) { richText = true };

        int selectedTag = 0;
        int selectedLayer = 0;
        int gridWidth_Tag = 7;
        int gridWidth_Layer = 7;

        string errorMentTag = "";
        string errorMentLayer = "";

        static bool foldTag;
        static bool foldLayer;
        static bool foldComponent;
        static bool foldTransform;

        protected ComponentEditType componentEditType;

        Vector2 scrollPos;

        private void OnEnable()
        {
            selectedTag = 0;
            selectedLayer = 0;
            Selection.selectionChanged += IsDiffernt;
            FindWindow();
        }
        private void OnDisable()
        {
            Selection.selectionChanged -= IsDiffernt;
        }
        private void IsDiffernt()
        {
            errorMentTag = "";
            errorMentLayer = "";
            Repaint();
        }

        private void OnGUI()
        {
            if (Tags.Length < selectedTag)
                selectedTag = 0;
            if (Layers.Length < selectedLayer)
                selectedLayer = 0;

            //TagSetting();
            //LayerSetting();

            TransformEdit();

            EditorGUILayout.Space();

            (bool, int, int) tagReturn = Setting(foldTag, "Tag", gridWidth_Tag, selectedTag, Tags, errorMentTag, FindTag, ChangeTag, ChangeTagWithChildren);
            foldTag = tagReturn.Item1;
            gridWidth_Tag = tagReturn.Item2;
            selectedTag = tagReturn.Item3;

            (bool, int, int) layerReturn = Setting(foldLayer, "Layer", gridWidth_Layer, selectedLayer, Layers, errorMentLayer, FindLayer, ChangeLayer, ChangeLayerWithChildren);
            foldLayer = layerReturn.Item1;
            gridWidth_Layer = layerReturn.Item2;
            selectedLayer = layerReturn.Item3;

            EditorGUILayout.Space();

            Component();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Seleceted. [<color=yellow>{Selection.gameObjects.Length:000}</color>]ea", RichLabel);
            if (GUILayout.Button("Parent", GUILayout.Width(70)))
            {
                SelectParent();
            }
            if (GUILayout.Button("Child", GUILayout.Width(70)))
            {
                SelectChild();
            }
            if (GUILayout.Button("Enable", GUILayout.Width(70)))
            {
                List<GameObject> selected = new List<GameObject>();
                selected.AddRange(Selection.gameObjects);
                selected.RemoveAll(x => !x.activeSelf);

                Selection.objects = selected.ToArray();
            }
            if (GUILayout.Button("Disable", GUILayout.Width(70)))
            {
                List<GameObject> selected = new List<GameObject>();
                selected.AddRange(Selection.gameObjects);
                selected.RemoveAll(x => x.activeSelf);

                Selection.objects = selected.ToArray();
            }
            if (GUILayout.Button("Flip Enable", GUILayout.Width(100)))
            {
                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    Selection.gameObjects[i].SetActive(!Selection.gameObjects[i].activeSelf);
                }
            }
            EditorGUILayout.EndHorizontal();
            SelectedList();
            EditorGUILayout.EndVertical();
        }
        private void Component()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            foldComponent = EditorGUILayout.Foldout(foldComponent, "Component", true);

            if (foldComponent)
                componentEditType = (ComponentEditType)EditorGUILayout.EnumPopup(componentEditType, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            float width = (WindowSize.x - (Margin * 5)) / gridWidth;

            if (foldComponent)
            {
                int heightCount = types.Length / gridWidth;
                int andHeightCount = types.Length % gridWidth;

                GUI.enabled = Selection.gameObjects.Length != 0;
                int final;
                for (int cnt = 0; cnt < heightCount + andHeightCount; cnt++)
                {
                    if (cnt == heightCount)
                        final = andHeightCount;
                    else
                        final = gridWidth;

                    EditorGUILayout.BeginHorizontal();
                    for (int _cnt = 0; _cnt < final; _cnt++)
                    {
                        int num = (gridWidth * cnt) + _cnt;
                        ComponentEdit(componentEditType, num, width);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;
            }
            EditorGUILayout.EndVertical();
        }
        private void ComponentEdit(ComponentEditType editType, int num, float width)
        {
            if (ComponentButton(num, types[num], width))
            {
                switch (editType)
                {
                    case ComponentEditType.Add:
                        AddComponent(types[num]);
                        break;
                    case ComponentEditType.Remove:
                        RemoveComponent(types[num]);
                        break;
                    case ComponentEditType.Without:
                        SelectNoComponent(types[num]);
                        break;
                    case ComponentEditType.Have:
                        SelectComponent(types[num]);
                        break;
                }
            }
        }
        private bool ComponentButton(int num, Type classType, float width)
        {
            return GUILayout.Button($"[{num}]{classType.Name}", GUILayout.Width(width));
        }
        private void AddComponent(Type classType)
        {
            for (int cnt = 0; cnt < Selection.gameObjects.Length; cnt++)
            {
                if (!Selection.gameObjects[cnt].TryGetComponent(classType, out _))
                    Selection.gameObjects[cnt].AddComponent(classType);
            }
        }
        private void RemoveComponent(Type classType)
        {
            for (int cnt = 0; cnt < Selection.gameObjects.Length; cnt++)
            {
                if (Selection.gameObjects[cnt].TryGetComponent(classType, out Component component))
                    DestroyImmediate(component);
            }
        }
        private void SelectNoComponent(Type classType)
        {
            List<GameObject> selecetd = new List<GameObject>();
            selecetd.AddRange(Selection.gameObjects);

            selecetd.RemoveAll(x => x.TryGetComponent(classType, out _));

            Selection.objects = selecetd.ToArray();
        }
        private void SelectComponent(Type classType)
        {
            List<GameObject> selecetd = new List<GameObject>();
            selecetd.AddRange(Selection.gameObjects);

            selecetd.RemoveAll(x => !x.TryGetComponent(classType, out _));

            Selection.objects = selecetd.ToArray();
        }

        private void TransformEdit()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                foldTransform = EditorGUILayout.Foldout(foldTransform, "Transform", true);
                if (foldTransform)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Reset", RichLabel, GUILayout.Width(100));
                        GUI.enabled = Selection.gameObjects.Length != 0;
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                if (GUILayout.Button("Posision (0,0,0)"))
                                {
                                    for (int i = 0; i < Selection.gameObjects.Length; i++)
                                    {
                                        Selection.gameObjects[i].transform.localPosition = Vector3.zero;
                                    }
                                }
                                if (GUILayout.Button("Rotation (0,0,0)"))
                                {
                                    for (int i = 0; i < Selection.gameObjects.Length; i++)
                                    {
                                        Selection.gameObjects[i].transform.localEulerAngles = Vector3.zero;
                                    }
                                }
                                if (GUILayout.Button("Local Scale (1,1,1)"))
                                {
                                    for (int i = 0; i < Selection.gameObjects.Length; i++)
                                    {
                                        Selection.gameObjects[i].transform.localScale = Vector3.one;
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            if (GUILayout.Button("All\nP:(0,0,0)\nR:(0,0,0)\nS:(1,1,1)", GUILayout.Height(62)))
                            {
                                for (int i = 0; i < Selection.gameObjects.Length; i++)
                                {
                                    Selection.gameObjects[i].transform.localPosition = Vector3.zero;
                                    Selection.gameObjects[i].transform.localEulerAngles = Vector3.zero;
                                    Selection.gameObjects[i].transform.localScale = Vector3.one;
                                }
                            }
                            GUI.enabled = true;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Align", RichLabel, GUILayout.Width(100));
                        GUI.enabled = Selection.gameObjects.Length != 0;
                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                if (GUILayout.Button("Pos X"))
                                {
                                    AlignPos(SnapAxis.X);
                                }
                                if (GUILayout.Button("Pos Y"))
                                {
                                    AlignPos(SnapAxis.Y);
                                }
                                if (GUILayout.Button("Pos Z"))
                                {
                                    AlignPos(SnapAxis.Z);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            {
                                if (GUILayout.Button("Equally Spaced"))
                                {
                                    EquallySpace(SnapAxis.X);
                                    EquallySpace(SnapAxis.Y);
                                    EquallySpace(SnapAxis.Z);
                                }
                                if (GUILayout.Button("X", GUILayout.Width(20)))
                                {
                                    EquallySpace(SnapAxis.X);
                                }
                                if (GUILayout.Button("Y", GUILayout.Width(20)))
                                {
                                    EquallySpace(SnapAxis.Y);
                                }
                                if (GUILayout.Button("Z", GUILayout.Width(20)))
                                {
                                    EquallySpace(SnapAxis.Z);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                        GUI.enabled = true;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        public void AlignPos(SnapAxis axis)
        {
            List<GameObject> selected = new List<GameObject>();
            selected.AddRange(Selection.gameObjects);

            Vector3[] pos = new Vector3[selected.Count];
            float value = 0;

            for (int i = 0; i < selected.Count; i++)
            {
                pos[i] = selected[i].transform.position;
                value += GetAxisValue(pos[i], axis);
            }

            value = value / pos.Length;

            for (int i = 0; i < selected.Count; i++)
            {
                switch (axis)
                {
                    case SnapAxis.X: selected[i].transform.position = new Vector3(value, pos[i].y, pos[i].z); break;
                    case SnapAxis.Y: selected[i].transform.position = new Vector3(pos[i].x, value, pos[i].z); break;
                    case SnapAxis.Z: selected[i].transform.position = new Vector3(pos[i].x, pos[i].y, value); break;
                }
            }
        }

        public void EquallySpace(SnapAxis axis)
        {
            List<GameObject> selected = new List<GameObject>();
            selected.AddRange(Selection.gameObjects);

            selected.Sort(delegate (GameObject a, GameObject b)
            {
                float valueX = GetAxisValue(a.transform.position, axis);
                float valueY = GetAxisValue(b.transform.position, axis);

                if (valueX < valueY)
                    return -1;
                else if (valueX > valueY)
                    return 1;
                else
                    return 0;
            });

            Vector3[] pos = new Vector3[selected.Count];
            float min = GetAxisValue(selected[0].transform.position, axis);
            float max = min;
            for (int i = 0; i < selected.Count; i++)
            {
                pos[i] = selected[i].transform.position;

                float axisValue = GetAxisValue(pos[i], axis);

                if (axisValue < min)
                    min = axisValue;
                if (max < axisValue)
                    max = axisValue;
            }

            float space = (max - min) / (pos.Length - 1);

            float value = 0;
            for (int i = 0; i < selected.Count; i++)
            {
                if (i == 0)
                    value = min;
                else if (i == selected.Count - 1)
                    value = max;
                else
                    value += space;

                switch (axis)
                {
                    case SnapAxis.X: selected[i].transform.position = new Vector3(value, pos[i].y, pos[i].z); break;
                    case SnapAxis.Y: selected[i].transform.position = new Vector3(pos[i].x, value, pos[i].z); break;
                    case SnapAxis.Z: selected[i].transform.position = new Vector3(pos[i].x, pos[i].y, value); break;
                }
            }
        }

        private float GetAxisValue(Vector3 value, SnapAxis axis)
        {
            switch (axis)
            {
                case SnapAxis.X: return value.x;
                case SnapAxis.Y: return value.y;
                case SnapAxis.Z: return value.z;
                default: return 0;
            }
        }

        private void FindTag()
        {
            GameObject select = Selection.gameObjects[0];

            List<GameObject> findedList = new List<GameObject>();
            FindTag(Selection.transforms[0], Tags[selectedTag], findedList);

            if (findedList.Count == 0)
            {
                Selection.objects = new GameObject[] { select };
                WaitingFindMent("조건에 맞는 개체가 없습니다.");
            }
            else
                Selection.objects = findedList.ToArray();
        }
        private void ChangeTag()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Selection.gameObjects[i].tag = Tags[selectedTag];
            }
        }
        private void ChangeTagWithChildren()
        {
            List<GameObject> finded = new List<GameObject>();
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                FindChildren(Selection.transforms[i], finded);
            }

            Selection.objects = finded.ToArray();

            for (int cnt = 0; cnt < finded.Count; cnt++)
            {
                finded[cnt].tag = Tags[selectedTag];
            }
        }
        private void ChangeLayerWithChildren()
        {
            List<GameObject> finded = new List<GameObject>();
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                FindChildren(Selection.transforms[i], finded);
            }

            Selection.objects = finded.ToArray();

            for (int cnt = 0; cnt < finded.Count; cnt++)
            {
                finded[cnt].layer = LayerMask.NameToLayer(Layers[selectedLayer]);
            }
        }
        private async void WaitingFindMent(string ment)
        {
            errorMentTag = ment;
            await System.Threading.Tasks.Task.Delay(2000);
            errorMentTag = "";
            Repaint();
        }
        private void FindLayer()
        {
            GameObject select = Selection.gameObjects[0];

            List<GameObject> findedList = new List<GameObject>();
            FindLayer(Selection.gameObjects[0], Layers[selectedLayer], findedList);

            if (findedList.Count == 0)
            {
                Selection.objects = new GameObject[] { select };
                WaitingFindLayerMent("조건에 맞는 개체가 없습니다.");
            }
            else
                Selection.objects = findedList.ToArray();
        }
        private void ChangeLayer()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Selection.gameObjects[i].layer = LayerMask.NameToLayer(Layers[selectedLayer]);
            }
        }

        private (bool, int, int) Setting(bool fold, string title, int gridWidth, int select, string[] array, string errorText, Action find, Action change, Action changeWith)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (fold)
            {
                EditorGUILayout.BeginHorizontal();
                fold = EditorGUILayout.Foldout(fold, title, true);
                gridWidth = EditorGUILayout.IntSlider(gridWidth, 3, 10);
                GUI.enabled = gridWidth != 3;
                if (GUILayout.Button("▽", GUILayout.Width(20)))
                {
                    gridWidth--;
                }
                GUI.enabled = gridWidth != 10;
                if (GUILayout.Button("△", GUILayout.Width(20)))
                {
                    gridWidth++;
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                select = GUILayout.SelectionGrid(select, array, gridWidth);

                EditorGUILayout.BeginHorizontal();

                float width = (WindowSize.x - (4 * 4)) / 3;
                GUIStyle style = new GUIStyle(EditorStyles.miniButton) { richText = true, fixedHeight = 40 };

                if (Selection.transforms.Length == 0)
                {
                    GUI.enabled = false;
                    GUILayout.Button("찾을 개체 1개를 선택해주세요.", style, GUILayout.Width(width), GUILayout.Height(30));
                }
                else
                {
                    GUI.enabled = true;
                    string addMent = errorText == "" ? "" : $"\n<color=red>{errorText}</color>";
                    if (GUILayout.Button($"Find in Children [<color=yellow>{array[select]}</color>]{addMent}", style, GUILayout.Width(width), GUILayout.Height(30)))
                    {
                        find?.Invoke();
                    }
                }

                if (GUILayout.Button($"Change to [<color=yellow>{array[select]}</color>]", style, GUILayout.Width(width), GUILayout.Height(30)))
                {
                    change?.Invoke();
                }
                if (GUILayout.Button($"Change to [<color=yellow>{array[select]}</color>] with Children", style, GUILayout.Width(width), GUILayout.Height(30)))
                {
                    changeWith?.Invoke();
                }
                EditorGUILayout.EndHorizontal();
                GUI.enabled = true;

                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            else
                fold = EditorGUILayout.Foldout(fold, title, true);
            EditorGUILayout.EndVertical();

            return (fold, gridWidth, select);
        }

        private async void WaitingFindLayerMent(string ment)
        {
            errorMentLayer = ment;
            await System.Threading.Tasks.Task.Delay(2000);
            errorMentLayer = "";
            Repaint();
        }
        private void FindChildren(Transform parent, List<GameObject> findedList)
        {
            findedList.Add(parent.gameObject);

            for (int cnt = 0; cnt < parent.childCount; cnt++)
            {
                FindChildren(parent.GetChild(cnt), findedList);
            }
        }

        private void FindTag(Transform parent, string findedTag, List<GameObject> findedList)
        {
            if (parent.gameObject.CompareTag(findedTag))
                findedList.Add(parent.gameObject);

            for (int cnt = 0; cnt < parent.childCount; cnt++)
            {
                FindTag(parent.GetChild(cnt), findedTag, findedList);
            }
        }
        private void FindLayer(GameObject parent, string findedLayer, List<GameObject> findedList)
        {
            if (parent.gameObject.layer == LayerMask.NameToLayer(findedLayer))
                findedList.Add(parent.gameObject);

            for (int cnt = 0; cnt < parent.transform.childCount; cnt++)
            {
                FindLayer(parent.transform.GetChild(cnt).gameObject, findedLayer, findedList);
            }
        }
        private void SelectedList()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            float labelwidth = WindowSize.x / 5;
            GUIContent contents = new GUIContent();
            for (int cnt = 0; cnt < Selection.gameObjects.Length; cnt++)
            {
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = false;
                EditorGUILayout.Toggle(Selection.gameObjects[cnt].activeInHierarchy, GUILayout.Width(20));
                EditorGUILayout.Toggle(Selection.gameObjects[cnt].activeSelf, GUILayout.Width(20));
                EditorGUILayout.ObjectField(Selection.gameObjects[cnt], typeof(GameObject), true);

                string state = "";

                if (Selection.gameObjects[cnt].tag == Tags[selectedTag] || !foldTag)
                    state = $"T: <color=white>{Selection.gameObjects[cnt].tag}</color>";
                else
                    state = $"T: <color=white>{Selection.gameObjects[cnt].tag}</color>=><color=yellow>{Tags[selectedTag]}</color>";

                EditorGUILayout.LabelField(state, RichLabel_LikeTextField, GUILayout.Width(labelwidth));

                string layerName = LayerMask.LayerToName(Selection.gameObjects[cnt].layer);
                if (layerName == Layers[selectedLayer] || !foldLayer)
                    state = $"L: <color=white>{layerName}</color>";
                else
                    state = $"L: <color=white>{layerName}</color>=><color=yellow>{Layers[selectedLayer]}</color>";
                EditorGUILayout.LabelField(state, RichLabel_LikeTextField, GUILayout.Width(labelwidth));

                for (int _cnt = 0; _cnt < types.Length; _cnt++)
                {
                    if (Selection.gameObjects[cnt].TryGetComponent(types[_cnt], out _))
                        contents.text = $"<color=white>{ _cnt}</color>";
                    else
                        contents.text = $"<color=grey>{ _cnt}</color>";

                    contents.tooltip = types[_cnt].Name;

                    EditorGUILayout.LabelField(contents, RichLabel_LikeTextField, GUILayout.Width(14));
                }
                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        private void SelectChild()
        {
            List<GameObject> selected = new List<GameObject>();
            for (int cnt_A = 0; cnt_A < Selection.transforms.Length; cnt_A++)
            {
                for (int cnt_B = 0; cnt_B < Selection.transforms[cnt_A].childCount; cnt_B++)
                {
                    selected.Add(Selection.transforms[cnt_A].GetChild(cnt_B).gameObject);
                }
            }

            Selection.objects = selected.ToArray();
        }
        private void SelectParent()
        {
            List<GameObject> selected = new List<GameObject>();
            List<GameObject> curSelected = new List<GameObject>(Selection.gameObjects);

            for (int cnt = 0; cnt < curSelected.Count; cnt++)
            {
                Transform selectedParent = curSelected[cnt].transform.parent;

                if (selected.Find(x => x.transform.Equals(selectedParent)) == null)
                {
                    selected.Add(selectedParent.gameObject);
                }
            }

            Selection.objects = selected.ToArray();
        }
    }
}