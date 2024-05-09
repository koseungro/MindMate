/// 작성자: 백인성
/// 작성일: 2020-11-16
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using FNI.IS.EDITOR.Property;
#endif

#if UNITY_EDITOR
namespace FNI.IS.EDITOR
{
    public class IS_Editor : UnityEditor.Editor
    {
        protected float DefSpacingBetweenField => 3;
        protected float DefForwardMargin => 23;
        protected float DefaultLabelWidth => 110;
        protected bool IsWild { get => toggleViewLimit <= EditorGUIUtility.currentViewWidth; }
        protected bool IsSmall { get => EditorGUIUtility.currentViewWidth < toggleViewLimit; }
        protected GUILayoutOption DefWidthLayout => GUILayout.Width(DefaultLabelWidth);
        protected Rect WindowPosition => EditorGUILayout.GetControlRect(true, 0);
        /// <summary>
        /// 현재 속해 있는 레이아웃의 스크롤이 보여지는지 판단함
        /// </summary>
        protected bool IsVerticalScrollOn => EditorGUILayout.GetControlRect(true, 0).width != (EditorGUIUtility.currentViewWidth - DefForwardMargin);
        protected GUIStyle TextArea_WarpMode
        {
            get => new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true
            };
        }
        protected GUIStyle TextField_CenterMode
        {
            get => new GUIStyle(EditorStyles.textField)
            {
                alignment = TextAnchor.MiddleCenter
            };
        }
        protected GUIStyle Label_Align_C_Bold
        {
            get => new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
            };
        }
        protected GUIStyle Label_Align_R_Bold
        {
            get => new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleRight,
                fontStyle = FontStyle.Bold,
            };
        }
        protected GUIStyle Box_Align_C
        {
            get => new GUIStyle(EditorStyles.textArea)
            {
                alignment = TextAnchor.MiddleCenter,
                stretchHeight = true
            };
        }
        protected GUIStyle TextField_Align_C_RT_On
        {
            get => new GUIStyle(EditorStyles.textField)
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
        }
        protected GUIStyle Label_RichTMode
        {
            get => new GUIStyle(EditorStyles.label)
            {
                richText = true
            };
        }
        protected GUIStyle Label_BoldMode
        {
            get => new GUIStyle(EditorStyles.boldLabel);
        }
        public delegate T2 ReturnAction<T1, T2>(T1 a, T2 b);

        protected float toggleViewLimit = 400;

        protected Color originColor;
        protected float originLabelWidth;

        protected bool showBaseOnInspectorGUI;

        public override void OnInspectorGUI()
        {
            originColor = GUI.color;
            originLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUI.indentLevel++;
            showBaseOnInspectorGUI = EditorGUILayout.Foldout(showBaseOnInspectorGUI, "Show Base Inspector GUI", true);
            if (showBaseOnInspectorGUI)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                base.OnInspectorGUI();
                EditorGUILayout.EndVertical();
            }
            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Begin으로 열려진 레이아웃 그룹에서 End가 호출 된 후에 사용해야 함.
        /// </summary>
        /// <returns></returns>

        public static Rect DrawLastRect(bool isShow = true)
        {
            Rect last = GUILayoutUtility.GetLastRect();
            if (isShow)
            {
                EditorGUI.DrawRect(last, new Color(1, 0, 0, 0.5f));
                EditorGUILayout.HelpBox(last.ToString(), MessageType.Info);
            }
            return last;
        }
        private float CurrentViewWidth()
        {
            EditorGUILayout.Space(0f);
            return GUILayoutUtility.GetLastRect().width + DefForwardMargin;
        }
        public static void Grid<T>(List<T> list, int start, int gridWidth, ReturnAction<int, T> action)
        {
            if (list == null) return;
            if (list.Count == 0) return;

            int heightCount = (list.Count - start) / gridWidth;
            int andHeightCount = (list.Count - start) % gridWidth;

            EditorGUILayout.BeginVertical(); int final;
            for (int cnt = 0; cnt < heightCount + 1; cnt++)
            {
                if (cnt == heightCount)
                    final = andHeightCount;
                else
                    final = gridWidth;

                EditorGUILayout.BeginHorizontal();
                for (int _cnt = 0; _cnt < final; _cnt++)
                {
                    int num = (gridWidth * cnt) + _cnt + start;
                    if (action != null)
                        list[num] = action.Invoke(num - start, list[num]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        public static List<T> ListController<T>(List<T> list, int min, int max, bool isRight = true)
        {
            if (list == null)
                list = new List<T>();

            if (min < 0) min = 0;

            if (isRight)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(60));
            {
                EditorGUILayout.LabelField($"Total [{list.Count - min}]", GUILayout.MaxWidth(60));

                if (max != -1)
                {
                    if (GUILayout.Button("R", GUILayout.Width(20)))
                    {
                        if (list.Count != 0)
                        {
                            if (EditorUtility.DisplayDialog("경고", "초기화 할거임?\n복구 못해", "응", "아니"))
                            {
                                list.Clear();
                                list = new List<T>();
                            }
                        }
                        else
                            EditorUtility.DisplayDialog("이런", "초기화 할게 없져", "응");
                    }
                }
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    if (max == -1)
                        list.Add(default);
                    else
                    {
                        if (list.Count < max)
                            list.Add(default);
                    }
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if (min < list.Count)

                    {
                        if (EditorUtility.DisplayDialog("경고", "하나 빼면 지워짐?\n복구 못해도 할 거임?", "응", "아니"))
                        {
                            list.RemoveAt(list.Count - 1);
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return list;
        }
        public static List<T> ListController<T>(List<T> list, bool isRight = true, bool showCount = true)
        {
            if (list == null)
                list = new List<T>();

            if (isRight)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(60));
            {
                if (showCount)
                    EditorGUILayout.LabelField($"Total [{list.Count}]", GUILayout.MaxWidth(60));

                if (GUILayout.Button("R", GUILayout.Width(20)))
                {
                    if (list.Count != 0)
                    {
                        if (EditorUtility.DisplayDialog("경고", "초기화 할거임?\n복구 못해", "응", "아니"))
                        {
                            list.Clear();
                            list = new List<T>();
                        }
                    }
                    else
                        EditorUtility.DisplayDialog("이런", "초기화 할게 없져", "응");
                }
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    list.Add(default);
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {

                    if (EditorUtility.DisplayDialog("경고", "하나 빼면 지워짐?\n복구 못해도 할 거임?", "응", "아니"))
                    {
                        if (list.Count != 0)
                            list.RemoveAt(list.Count - 1);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return list;
        }
        public static List<T> ListController<T>(List<T> list, int num, bool isRight = true)
        {
            if (list == null)
                list = new List<T>();
            if (list.Count == 0)
                list.Add(default);

            if (isRight)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.BeginHorizontal(GUILayout.Width(33));
            {
                if (GUILayout.Button("D", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("경고", "삭제 할거임?\n복구 못해", "응", "아니"))
                        list.RemoveAt(num);
                }
                EditorGUILayout.BeginVertical();
                EditorGUI.BeginDisabledGroup(!(0 < num));
                {
                    if (GUILayout.Button("", GUILayout.Width(10), GUILayout.Height(8)))
                    {
                        list.Insert(num - 1, list[num]);
                        list.RemoveAt(num + 1);
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!(num < list.Count - 1));
                {
                    if (GUILayout.Button("", GUILayout.Width(10), GUILayout.Height(8)))
                    {
                        list.Insert(num + 2, list[num]);
                        list.RemoveAt(num);
                    }
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            return list;
        }
        public static bool ListController2<T>(List<T> list, bool isRight = true, bool showCount = true)
        {
            if (list == null)
                list = new List<T>();

            bool isClicked = false;

            if (isRight)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(60));
            {
                if (showCount)
                    EditorGUILayout.LabelField($"Total [{list.Count}]", GUILayout.MaxWidth(60));

                if (GUILayout.Button("R", GUILayout.Width(20)))
                {
                    if (list.Count != 0)
                    {
                        if (EditorUtility.DisplayDialog("경고", "초기화 할거임?\n복구 못해", "응", "아니"))
                        {
                            list.Clear();
                            list = new List<T>();
                            isClicked = true;
                        }
                    }
                    else
                        EditorUtility.DisplayDialog("이런", "초기화 할게 없져", "응");
                }
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    list.Add(default);
                    isClicked = true;
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("경고", "하나 빼면 지워짐?\n복구 못해도 할 거임?", "응", "아니"))
                    {
                        if (list.Count != 0)
                        {
                            list.RemoveAt(list.Count - 1);
                            isClicked = true;
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return isClicked;
        }
        public static bool ListController2<T>(List<T> list, int num, bool isRight = true)
        {
            if (list == null)
                list = new List<T>();
            if (list.Count == 0)
                list.Add(default);

            bool clicked = false;

            if (isRight)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.BeginHorizontal(GUILayout.Width(33));
            {
                if (GUILayout.Button("D", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("경고", "삭제 할거임?\n복구 못해", "응", "아니"))
                    {
                        list.RemoveAt(num);
                        clicked = true;
                    }
                }
                EditorGUILayout.BeginVertical();
                EditorGUI.BeginDisabledGroup(!(0 < num));
                {
                    if (GUILayout.Button("", GUILayout.Width(10), GUILayout.Height(8)))
                    {
                        list.Insert(num - 1, list[num]);
                        list.RemoveAt(num + 1);
                        clicked = true;
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!(num < list.Count - 1));
                {
                    if (GUILayout.Button("", GUILayout.Width(10), GUILayout.Height(8)))
                    {
                        list.Insert(num + 2, list[num]);
                        list.RemoveAt(num);
                        clicked = true;
                    }
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            return clicked;
        }

        public static void Line(Color color, float height = 5)
        {
            EditorGUILayout.Space(height);

            Rect last = GUILayoutUtility.GetLastRect();
            last.y += height;
            last.height = height;
            EditorGUI.DrawRect(last, color);

            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        }
        public static void DottedLine(Color color, float height, float dot, float space = 0.25f)
        {
            EditorGUILayout.Space(height);

            Rect last = GUILayoutUtility.GetLastRect();

            float pointWidth = dot;
            float spaceWidth = dot * space;
            float length = pointWidth + spaceWidth;
            float width = last.x + last.width;
            int widthCount   = (int)(last.width / length);

            Rect point = new Rect
            {
                x = last.x,
                y = last.y + height,
                width = pointWidth,
                height = height
            };

            for (int cnt = 0; cnt < widthCount; cnt++)
            {
                EditorGUI.DrawRect(point, color);
                //다음 위치
                point.x += length;
            }

            //잔여 길이 표시
            float lastWidth = last.x + last.width - point.x;
            if (pointWidth < lastWidth)
                lastWidth = pointWidth;

            if (0 < lastWidth)
            {
                Rect lastPoint = new Rect
                {
                    x = point.x,
                    y = point.y,
                    width = lastWidth,
                    height = point.height
                };

                EditorGUI.DrawRect(lastPoint, color);
            }

            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        }

        public static MinMax SetRange(string title, MinMax range, MinMax limitRange)
        {
            float min = range.Min, max = range.Max;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(title, GUILayout.Width(100));
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        EditorGUILayout.LabelField("Min", GUILayout.Width(30));
                        min = EditorGUILayout.FloatField(min, GUILayout.Width(30));

                    }
                    EditorGUILayout.EndHorizontal();

                    float limitMin = limitRange.Min, limitMax = limitRange.Max;
                    EditorGUILayout.LabelField("", GUILayout.Width(10));
                    EditorGUILayout.MinMaxSlider(ref min, ref max, limitMin, limitMax);
                    EditorGUILayout.LabelField("", GUILayout.Width(10));

                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        EditorGUILayout.LabelField("Max", GUILayout.Width(30));
                        max = EditorGUILayout.FloatField(max, GUILayout.Width(30));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();

            return new MinMax(min, max);
        }
        public static int GetBlendShape(Mesh target, int sayNum, float width = 0)
        {
            int say = sayNum;
            EditorGUILayout.BeginHorizontal();
            {
                string[] names = new string[target.blendShapeCount];
                for (int cnt = 0; cnt < names.Length; cnt++)
                {
                    names[cnt] = target.GetBlendShapeName(cnt);
                }
                if (width == 0)
                    say = EditorGUILayout.Popup(say, names);
                else
                    say = EditorGUILayout.Popup(say, names, GUILayout.Width(width));
            }
            EditorGUILayout.EndHorizontal();

            return say;
        }
        protected string CheckTrim(string input)
        {
            if (input != "" && input.Contains("\r"))
            {
                GUI.color = Color.red;
                if (GUILayout.Button("Trim", GUILayout.Width(40)))
                {
                    input = input.Replace("\r", "");
                }
                GUI.color = Color.white;
            }

            return input;
        }
        protected bool IsCalcuWild(float value)
        {
            return value <= EditorGUIUtility.currentViewWidth;
        }
        protected int SliderController(int data, int min, int max)
        {
            int _data = 0;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("◀", EditorStyles.miniButton, GUILayout.Width(20)))
                        data--;
                    _data = (int)GUILayout.HorizontalSlider(data, min, max);
                    if (GUILayout.Button("▶", EditorStyles.miniButton, GUILayout.Width(20)))
                        _data++;

                    if (IsWild)
                    {
                        if (_data < min)
                            _data = min;

                        _data = EditorGUILayout.IntField(_data, GUILayout.Width(40));
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (IsSmall)
                {
                    if (_data < min)
                        _data = min;

                    _data = EditorGUILayout.IntField(_data);
                }
            }
            EditorGUILayout.EndVertical();
            return _data;
        }
        protected void SaveSerializedData()
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        protected bool SetFold(string title, bool isFold)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(10));
            isFold = EditorGUILayout.Foldout(isFold, title, true);
            EditorGUILayout.EndHorizontal();

            return isFold;
        }
        protected bool SetFoldout(string title, bool isFold)
        {
            EditorGUI.indentLevel++;
            isFold = EditorGUILayout.Foldout(isFold, title, true);
            EditorGUI.indentLevel--;

            return isFold;
        }
        protected void SetSpaceRepeat(int repeatCount)
        {
            for (int cnt = 0; cnt < repeatCount; cnt++)
            {
                EditorGUILayout.Space();
            }
        }
        protected T SetEnumFeild<T>(string title, T value, float labelWidth = -1) where T : Enum
        {
            T newValue;

            if (labelWidth == -1) labelWidth = DefaultLabelWidth;

            if (title != "")
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(title, GUILayout.Width(labelWidth));
                newValue = (T)EditorGUILayout.EnumPopup(value);
                EditorGUILayout.EndHorizontal();
            }
            else
                newValue = (T)EditorGUILayout.EnumPopup(value);

            return newValue;
        }
        protected T SetFlagFeild<T>(string title, T value, float labelWidth = -1) where T : Enum
        {
            T newValue;

            if (labelWidth == -1) labelWidth = DefaultLabelWidth;

            if (title != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(title, GUILayout.Width(labelWidth));
                newValue = (T)EditorGUILayout.EnumFlagsField(value, GUILayout.Width(20));

                if (value.ToString() != "-1")
                    EditorGUILayout.TextField(value.ToString());
                else
                    EditorGUILayout.TextField("모두 선택됨");
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                newValue = (T)EditorGUILayout.EnumFlagsField(value, GUILayout.Width(20));

                if (value.ToString() != "-1")
                    EditorGUILayout.TextField(value.ToString());
                else
                    EditorGUILayout.TextField("모두 선택됨");
            }

            return newValue;
        }
        protected UnityEngine.Object SetObjectFeild(string title, UnityEngine.Object input, Type type, float labelWidth = -1)
        {
            UnityEngine.Object newValue = null;

            if (labelWidth == -1) labelWidth = DefaultLabelWidth;

            if (title != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(title, GUILayout.Width(labelWidth));
                newValue = EditorGUILayout.ObjectField(input, type, true);
                EditorGUILayout.EndHorizontal();
            }
            else
                newValue = EditorGUILayout.ObjectField(input, type, true);

            return newValue;
        }
        protected List<T> SetList<T>(string title, List<T> list, float labelWidth = -1, bool showCount = true)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title, GUILayout.Width(labelWidth));
            if (IsWild)
                list = IS_Editor.ListController(list, true, showCount);
            EditorGUILayout.EndHorizontal();
            if (IsSmall)
                list = IS_Editor.ListController(list, true, showCount);

            return list;
        }
        protected int SetIntFeild(string title, int input, float labelWidth = -1)
        {
            int newValue = 0;

            if (labelWidth == -1) labelWidth = DefaultLabelWidth;

            if (title != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(title, GUILayout.Width(labelWidth));
                newValue = EditorGUILayout.IntField(input);
                EditorGUILayout.EndHorizontal();
            }
            else
                newValue = EditorGUILayout.IntField(input);

            return newValue;
        }
        protected float SetFloatFeild(string title, float input, float labelWidth = -1)
        {
            float newValue = 0;

            if (labelWidth == -1) labelWidth = DefaultLabelWidth;

            if (title != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(title, GUILayout.Width(labelWidth));
                newValue = EditorGUILayout.FloatField(input);
                EditorGUILayout.EndHorizontal();
            }
            else
                newValue = EditorGUILayout.FloatField(input);

            return newValue;
        }
        protected string SetTextFeild(string title, string input, float labelWidth = -1)
        {
            string newText = "";

            if (labelWidth == -1) labelWidth = DefaultLabelWidth;

            if (title != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(title, GUILayout.Width(labelWidth));
                newText = CheckTrim(EditorGUILayout.TextArea(input));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                newText = CheckTrim(EditorGUILayout.TextArea(input));
                EditorGUILayout.EndHorizontal();
            }
            return newText;
        }
        protected string SetTextArea(string title, string input, float labelWidth = -1)
        {
            string newText = "";

            if (labelWidth == -1) labelWidth = DefaultLabelWidth;

            if (title != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(title, GUILayout.Width(labelWidth));
                if (IsWild)
                    newText = CheckTrim(EditorGUILayout.TextArea(input, TextArea_WarpMode));
                EditorGUILayout.EndHorizontal();
                if (IsSmall)
                {
                    EditorGUILayout.BeginHorizontal();
                    newText = CheckTrim(EditorGUILayout.TextArea(input, TextArea_WarpMode));
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                newText = CheckTrim(EditorGUILayout.TextArea(input, TextArea_WarpMode));
                EditorGUILayout.EndHorizontal();
            }

            return newText;
        }
        protected string SetTextArea_UnWarp(string title, string input, float fieldWidth, float labelWidth = -1)
        {
            string newText = "";

            if (labelWidth == -1) labelWidth = DefaultLabelWidth;

            if (title != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(title, GUILayout.Width(labelWidth));
                if (IsWild)
                    newText = CheckTrim(EditorGUILayout.TextArea(input, GUILayout.Width(fieldWidth)));
                EditorGUILayout.EndHorizontal();
                if (IsSmall)
                {
                    EditorGUILayout.BeginHorizontal();
                    newText = CheckTrim(EditorGUILayout.TextArea(input, GUILayout.Width(fieldWidth)));
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                newText = CheckTrim(EditorGUILayout.TextArea(input, GUILayout.Width(fieldWidth)));
                EditorGUILayout.EndHorizontal();
            }

            return newText;
        }
        protected void SetPropertyFeild(string _label, SerializedProperty property, float oldWidth = -1)
        {
            GUIContent label = new GUIContent() { text = _label };

            float oldLabelWidth = oldWidth == -1 ? EditorGUIUtility.labelWidth : oldWidth;
            if (_label == "")
                EditorGUIUtility.labelWidth = 1;
            EditorGUILayout.PropertyField(property, label);
            if (_label == "")
                EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        protected bool SetPropertyArrayController(SerializedProperty propertyArray, int curNum, bool onlyDelete = false, bool isUpdown = false)
        {
            int count = propertyArray.arraySize;

            bool change = false;
            if (isUpdown)
                EditorGUILayout.BeginVertical(GUILayout.Width(20));
            else
                EditorGUILayout.BeginHorizontal(GUILayout.Width(onlyDelete ? 20 : 33));

            if (GUILayout.Button(new GUIContent() { text = "X", tooltip = "제거" }, GUILayout.Width(20)))
            {
                if (EditorUtility.DisplayDialog("경고", "지울거임?\n복구 못해", "응", "아니"))
                {
                    propertyArray.DeleteArrayElementAtIndex(curNum);
                    change = true;
                }
            }
            if (onlyDelete == false)
            {
                if (GUILayout.Button(new GUIContent() { text = "D", tooltip = "복제" }, GUILayout.Width(20)))
                {
                    propertyArray.GetArrayElementAtIndex(curNum).DuplicateCommand();
                    change = true;
                }
                if (isUpdown)
                {
                    EditorGUILayout.BeginVertical(GUILayout.Width(20));

                    GUI.enabled = 0 <= curNum - 1;
                    if (GUILayout.Button(new GUIContent() { text = "△", tooltip = "Up" }, GUILayout.Width(20), GUILayout.Height(10)))
                    {
                        propertyArray.MoveArrayElement(curNum, curNum - 1);
                        change = true;
                    }
                    GUI.enabled = true;
                    if (GUILayout.Button(new GUIContent() { text = "↕", tooltip = "Move" }, GUILayout.Width(20)))
                    {
                        ShowPopup.Open("Move", curNum, 0, propertyArray.arraySize, (m) => Move(m, curNum, propertyArray));
                        change = true;
                    }
                    GUI.enabled = curNum + 1 < count;
                    if (GUILayout.Button(new GUIContent() { text = "▽", tooltip = "Down" }, GUILayout.Width(20), GUILayout.Height(10)))
                    {
                        propertyArray.MoveArrayElement(curNum, curNum + 1);
                        change = true;
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.BeginVertical(GUILayout.Width(10));
                    EditorGUILayout.LabelField("", GUILayout.Width(10), GUILayout.Height(1));
                    GUI.enabled = 0 <= curNum - 1;
                    if (GUILayout.Button(new GUIContent() { tooltip = "Up" }, GUILayout.Width(10), GUILayout.Height(8)))
                    {
                        propertyArray.MoveArrayElement(curNum, curNum - 1);
                        change = true;
                    }
                    GUI.enabled = curNum + 1 < count;
                    if (GUILayout.Button(new GUIContent() { tooltip = "Down" }, GUILayout.Width(10), GUILayout.Height(8)))
                    {
                        propertyArray.MoveArrayElement(curNum, curNum + 1);
                        change = true;
                    }
                    EditorGUILayout.EndVertical();
                    GUI.enabled = true;
                    if (GUILayout.Button(new GUIContent() { tooltip = "Move" }, GUILayout.Width(10)))
                    {
                        ShowPopup.Open("Move", curNum, 0, propertyArray.arraySize, (m) => Move(m, curNum, propertyArray));
                        change = true;
                    }
                }

            }
            if (isUpdown)
                EditorGUILayout.EndVertical();
            else
                EditorGUILayout.EndHorizontal();

            return change;
        }
        private void Move(int selected, int curPos, SerializedProperty propertyArray)
        {
            propertyArray.MoveArrayElement(curPos, selected);
            Debug.Log($"[Move {curPos} => {selected}");
            SaveSerializedData();
        }
        protected bool SetPropertyArraySize(SerializedProperty propertyArray, bool showCount = false)
        {
            if (propertyArray == null) return false;

            if (showCount)
                EditorGUILayout.LabelField($"[{propertyArray.arraySize:00}]", Label_Align_R_Bold, GUILayout.Width(40));

            if (propertyArray.arraySize == 0)
            {
                EditorGUILayout.LabelField(new GUIContent() { tooltip = "신규" }, EditorStyles.miniButton, GUILayout.Width(20));
                if (GUILayout.Button("N", GUILayout.Width(20)))
                {
                    propertyArray.arraySize = 1;
                    return true;
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent() { text = "+", tooltip = "추가" }, GUILayout.Width(20)))
                {
                    propertyArray.arraySize++;
                    return true;
                }
                if (GUILayout.Button(new GUIContent() { text = "-", tooltip = "제거" }, GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("경고", "지울거임?\n복구 못해", "응", "아니"))
                    {
                        if (propertyArray.arraySize != 0)
                        {
                            propertyArray.arraySize--;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
#endif