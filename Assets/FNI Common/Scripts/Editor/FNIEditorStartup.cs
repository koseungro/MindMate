/// 작성자: 조효련, 김윤빈
/// 작성일: 2018-11-01
/// 수정일: 2020-01-23
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// (2020-01-23)
/// 1. 쓰지않는 기능 삭제
/// 2. Scenes폴더의 Scene파일 생성 및 자동 새로고침 기능 추가
/// 3. Obj목록 및 생성 버튼 추가
/// 

using FNI.Common.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using UnityToolbarExtender.Examples;


namespace FNI.Common.Editor
{
    /// <summary>
    /// 에디터가 시작할 때 실행되는 일을 설정한 클래스
    /// 1) IL2CPP 설정하기
    /// 2) 프로젝트 폴더 확인하기 (FNI 폴더 대상, FNIFileCheck 스크립트에서 진행함)
    /// 3) 상단 툴바 설정하기
    ///     현재 프로젝트에 존재하는 신들을 상단툴바의 신 드롭다운 UI에 추가하기 (FNIEditorStartup.CreateSceneList 함수)
    /// </summary>
    [InitializeOnLoad]
    public class FNIEditorStartup
    {
        static FNIEditorStartup()
        {
            FNIBuildTool.SetIL2CPP(BuildTargetGroup.Standalone);

            ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);

            // 신 리스트를 생성
            CreateSceneList();
        }


        #region 씬 목록 생성 및 이동
        private static int selectedScene = 0;
        private static int[] sceneList;
        private static string[] sceneNameList;
        private static string[] scenePathList;

        /// <summary>
        /// FNI/Scenes 폴더안의 Scenes 파일을 읽어와서 리스트를 생성해줍니다.
        /// </summary>
        public static void CreateSceneList()
        {
            // 씬들의 정보를 저장해줄 List
            List<int> FNIsceneIndexList = new List<int>();
            List<string> FNIsceneNameList = new List<string>();
            List<string> FNIscenePathList = new List<string>();

            DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(Application.dataPath, "FNI"));

            if (dirInfo.Exists)
            {
                int count = 0;

                // 해당 경로에 있는 씬 파일들을 전부 읽어와서 List에 각각 인덱스, 이름, 경로를 추가해줍니다.
                foreach (DirectoryInfo directory in dirInfo.GetDirectories())
                {
                    foreach (FileInfo file2 in directory.GetFiles("*.unity"))
                    {
                        string name = file2.Name;
                        string path = string.Format("Assets/FNI/{0}/{1}", directory.Name, name);

                        FNIsceneIndexList.Add(count);
                        FNIsceneNameList.Add(name);
                        FNIscenePathList.Add(path);

                        count += 1;                        
                    }
                }
            }

            sceneList = FNIsceneIndexList.ToArray();
            sceneNameList = FNIsceneNameList.ToArray();
            scenePathList = FNIscenePathList.ToArray();
        }

        static void OnLeftToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("↻", "Scenes 파일들을 새로고침합니다."), ToolbarStyles.commandButtonStyle, GUILayout.Height(ToolbarStyles.height)))
            {
                CreateSceneList();
            }
            selectedScene = EditorGUILayout.IntPopup(selectedScene, sceneNameList, sceneList, ToolbarStyles.popupStyle, GUILayout.Height(ToolbarStyles.height), GUILayout.Width(175));

            if (GUILayout.Button(new GUIContent("이동", "선택된 신으로 이동하기"), ToolbarStyles.commandButtonStyle, GUILayout.Height(ToolbarStyles.height)))
            {
                SceneHelper.StartScene(scenePathList[selectedScene]);
            }


            GUILayout.Space(10);

            if (GUILayout.Button(new GUIContent("Assets 열기", "Assets 폴더를 엽니다."), ToolbarStyles.commandButtonStyle, GUILayout.Height(ToolbarStyles.height)))
            {
                Process.Start(Path.Combine(Application.dataPath, "../"));
            }

            GUILayout.FlexibleSpace();
        }
        #endregion


        #region 오브젝트 생성 및 CreateEmpty
        private static bool isShowDropdown = true; // 오브젝트 생성 방식을 드롭다운으로 할지 버튼으로 할지

        public static ObjectType op;
        private static string[] ObjNameList = Enum.GetNames(typeof(ObjectType)).ToArray(); // 오브젝트 리스트

        // 재생버튼이 존재하는 툴바에 버튼 추가하기
        static void OnRightToolbarGUI()
        {
            GUILayout.Space(100);

            if (GUILayout.Button(new GUIContent("Create Empty"), ToolbarStyles.commandButtonStyle, GUILayout.Height(ToolbarStyles.height), GUILayout.Width(100)))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Empty");
            }

            GUILayout.Space(20);

            if (isShowDropdown)
            {
                op = (ObjectType)EditorGUILayout.EnumPopup("", op, ToolbarStyles.popupStyle, GUILayout.Height(ToolbarStyles.height), GUILayout.Width(100));

                if (GUILayout.Button(new GUIContent(" 생성 ", "선택된 오브젝트 생성"), ToolbarStyles.commandButtonStyle, GUILayout.Height(ToolbarStyles.height), GUILayout.Width(100)))
                {
                    EditorApplication.ExecuteMenuItem(string.Format("GameObject/3D Object/{0}", op.ToString()));
                }
            }
            else
            {
                for (int i = 0; i < ObjNameList.Length; i++)
                {
                    if (GUILayout.Button(new GUIContent(ObjNameList[i]), ToolbarStyles.commandButtonStyle, GUILayout.Height(ToolbarStyles.height), GUILayout.Width(60)))
                    {
                        EditorApplication.ExecuteMenuItem(string.Format("GameObject/3D Object/{0}", ObjNameList[i]));
                    }
                }
            }
        }
        #endregion


        public enum ObjectType
        {
            Cube,
            Sphere,
            Capsule,
            Cylinder,
            Plane,
            Quad
        }
    }

    static class ToolbarStyles
    {
        public static readonly int height = 22;
        public static readonly GUIStyle commandButtonStyle;
        public static readonly GUIStyle popupStyle;

        static ToolbarStyles()
        {
            popupStyle = new GUIStyle(EditorStyles.popup)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Normal,
                imagePosition = ImagePosition.ImageAbove,
                fixedHeight = height
            };

            commandButtonStyle = new GUIStyle("Button")
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Normal,
                imagePosition = ImagePosition.ImageAbove,
            };
        }
    }
}

