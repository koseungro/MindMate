using FNI;
using FNI.DebugWriter;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using FNI.IS;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI.DebugWriter
{
    public class IS_DebugViewer : IS_Debug
    {
        [System.Serializable]
        public class LogSet
        {
            public string log;
            public string stack;
            public LogType type;
            public DateTime wrightTime;
        }
        /// <summary>
        /// Unity LogType에 None이 없어서 추가함
        /// </summary>
        public enum LogType
        {
            None = 0,
            //
            // 요약:
            //     LogType used for Errors.
            Error,
            //
            // 요약:
            //     LogType used for Asserts. (These could also indicate an error inside Unity itself.)
            Assert,
            //
            // 요약:
            //     LogType used for Warnings.
            Warning,
            //
            // 요약:
            //     LogType used for regular log messages.
            Log,
            //
            // 요약:
            //     LogType used for Exceptions.
            Exception
        }

        private Text Contents
        {
            get
            {
                if (content == null)
                    content = transform.Find("Image/Scroll View/Viewport/Content/Text").GetComponent<Text>();

                return content;
            }
        }

        public bool showStack;
        public bool showLogType;
        public bool showTime;
        public LogType logFilter;
        public int limit = 1000;
        public MinMax showRange;

        private Text content;
        private List<LogSet> logList = new List<LogSet>();

        private FNI_ObjectPool<Text> textList = new FNI_ObjectPool<Text>();

        public ScrollRect viewRect;
        public Slider limitSlider;
        public Scrollbar rangeScrollbar;

        public bool isDontDestroyOnLoad = false;
        protected override void Awake()
        {
            textList = new FNI_ObjectPool<Text>(Contents, Contents.transform.parent);

            Contents.gameObject.SetActive(false);

            if (isDontDestroyOnLoad)
            {
                var obj = FindObjectsOfType<IS_DebugViewer>();
                if (obj.Length == 1)
                {
                    DontDestroyOnLoad(gameObject);
                    base.Awake();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
                base.Awake();
        }

        protected override void ExpendHandleLog(string logString, string stackTrace, UnityEngine.LogType type)
        {
            base.ExpendHandleLog(logString, stackTrace, type);

            LogType log = LogType.None;
            switch (type)
            {
                case UnityEngine.LogType.Error:     log = LogType.Error; break;
                case UnityEngine.LogType.Assert:    log = LogType.Assert; break;
                case UnityEngine.LogType.Warning:   log = LogType.Warning; break;
                case UnityEngine.LogType.Log:       log = LogType.Log; break;
                case UnityEngine.LogType.Exception: log = LogType.Exception; break;
            }

            LogSet logSet = new LogSet()
            {
                log = logString,
                stack = stackTrace,
                type = log,
                wrightTime = DateTime.Now
            };

            //if (limit < logList.Count)
            //{
            //    logList.RemoveAt(0);
            //}

            logList.Add(logSet);
            CalcuRangeScrollbarSize();

            ShowContents();

            rangeScrollbar.SetValueWithoutNotify(1);
            viewRect.verticalNormalizedPosition = 0;
            SetRange(1);
        }

        public void OnShowStack(bool isOn)
        {
            showStack = isOn;
            ShowContents();
        }
        public void OnShowLogType(bool isOn)
        {
            showLogType = isOn;
            ShowContents();
        }
        public void OnShowType(int select)
        {
            logFilter = (LogType)select;
            ShowContents();
        }
        public void OnShowTime(bool isOn)
        {
            showTime = isOn;
            ShowContents();
        }
        public void Clear()
        {
            logList.Clear();
            ShowContents();
        }

        public void ShowContents()
        {
            string content = "";

            textList.AllHide();
            int count = 0;
            for (int cnt = 0; cnt < logList.Count; cnt++)
            {
                if (showRange.Contain(cnt))
                {
                    if (logFilter == LogType.None || logList[cnt].type == logFilter)
                    {
                        string text = "";

                        if (count != 0)
                            content += "\n\n";

                        if (showLogType)
                        {
                            string color = "";
                            switch (logList[cnt].type)
                            {
                                case LogType.Error:     color = "#ff0000ff";/*red*/    break;
                                case LogType.Exception: color = "#ffa500ff";/*orange*/ break;
                                case LogType.Warning:   color = "#ffff00ff";/*yello*/  break;
                                case LogType.Assert:    color = "#ffffffff";/*white*/  break;
                                case LogType.Log:       color = "";/*None*/            break;
                            }
                            text += (showTime ? $"[{logList[cnt].wrightTime.ToString(@"hh\:mm\:ss")}] " : "") + $"<color={color}>[{logList[cnt].type}]</color>";
                            text += "\n";
                        }
                        else
                        {
                            if (showTime)
                                text += $"[{logList[cnt].wrightTime.ToString(@"hh\:mm\:ss")}] \n";
                        }
                        text += logList[cnt].log;

                        if (showStack)
                        {
                            text += "\n";
                            text += logList[cnt].stack;
                        }

                        content += text;
                        count++;

                        textList.Pop().text = text;
                    }
                }
            }

            //Contents.text = content;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Alpha1))
                Debug.Log("Test");
            if (Input.GetKey(KeyCode.Alpha2))
                Debug.LogError("Test");
            if (Input.GetKey(KeyCode.Alpha3))
                Debug.LogAssertion("Test");
        }

        private void Start()
        {
            SetRange(0);
            rangeScrollbar.size = 1;
        }
        public void SetLimit(float value)
        {
            if ((int)value < logList.Count)
                limit = (int)value;
            else
                limitSlider.SetValueWithoutNotify(logList.Count);

            if (limit < logList.Count)
            {
                CalcuRangeScrollbarSize();
            }
            else
            {
                rangeScrollbar.size = 1;
            }

            viewRect.verticalNormalizedPosition = 0;
        }
        public void SetRange(float value)
        {
            int baseValue = (int)(value * (logList.Count - limit));
            if (baseValue < 0)
                baseValue = 0;
            showRange = new MinMax(baseValue, baseValue + limit);

            viewRect.verticalNormalizedPosition = 0;
        }
        private void CalcuRangeScrollbarSize()
        {
            rangeScrollbar.size = (float)limit / logList.Count;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(IS_DebugViewer))]
public class IS_DebugViewerEditor : Editor
{
    private IS_DebugViewer m_target;

    void OnEnable()
    {
        m_target = base.target as IS_DebugViewer;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        float labelWidth = (EditorGUIUtility.currentViewWidth - 70) * 0.5f;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is Don't Destroy On Load", GUILayout.Width(labelWidth));
        m_target.isDontDestroyOnLoad = EditorGUILayout.Toggle(m_target.isDontDestroyOnLoad);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Limit", GUILayout.Width(labelWidth));
        m_target.limit = EditorGUILayout.IntField(m_target.limit);
        EditorGUILayout.EndHorizontal();

        if (m_target.viewRect == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("View ScrollRect", GUILayout.Width(labelWidth));
            m_target.viewRect = (ScrollRect)EditorGUILayout.ObjectField(m_target.viewRect, typeof(ScrollRect), true);
            EditorGUILayout.EndHorizontal();
        }
        if (m_target.limitSlider == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Limit Slider", GUILayout.Width(labelWidth));
            m_target.limitSlider = (Slider)EditorGUILayout.ObjectField(m_target.limitSlider, typeof(Slider), true);
            EditorGUILayout.EndHorizontal();
        }
        if (m_target.rangeScrollbar == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Range Scrollbar", GUILayout.Width(labelWidth));
            m_target.rangeScrollbar = (Scrollbar)EditorGUILayout.ObjectField(m_target.rangeScrollbar, typeof(Scrollbar), true);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show Stack", GUILayout.Width(labelWidth));
        bool showStack = EditorGUILayout.Toggle(m_target.showStack);
        if (m_target.showStack != showStack)
        {
            m_target.showStack = showStack;
            m_target.ShowContents();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show Log Type", GUILayout.Width(labelWidth));
        bool showLogType = EditorGUILayout.Toggle(m_target.showLogType);
        if (m_target.showLogType != showLogType)
        {
            m_target.showLogType = showLogType;
            m_target.ShowContents();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show Time", GUILayout.Width(labelWidth));
        bool showTime = EditorGUILayout.Toggle(m_target.showTime);
        if (m_target.showTime != showTime)
        {
            m_target.showTime = showTime;
            m_target.ShowContents();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Type Filter", GUILayout.Width(labelWidth));
        IS_DebugViewer.LogType log = (IS_DebugViewer.LogType)EditorGUILayout.EnumPopup(m_target.logFilter);
        if (m_target.logFilter != log)
        {
            m_target.logFilter = log;
            m_target.ShowContents();
        }
        EditorGUILayout.EndHorizontal();


        //여기까지 검사해서 필드에 변화가 있으면
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Changed Update Mode");
            //변경이 있을 시 적용된다. 이 코드가 없으면 인스펙터 창에서 변화는 있지만 적용은 되지 않는다.
            EditorUtility.SetDirty(m_target);
        }
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }
}
#endif