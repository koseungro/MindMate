/// 작성자: 조효련
/// 작성일: 2020-11-03
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
///

using FNI.Common.Utils;
using System.IO;
using UnityEditor;
using UnityEngine;


/// <summary>
/// FNI 메뉴들을 모두 모아두는 스크립트로 사용
/// </summary>
namespace FNI.Common.Editor
{
    public class FNIMenuItem
    {
        [MenuItem("FNI/프로젝트 폴더 확인 및 생성", false, 100)]
        public static void CheckFileList()
        {
            for (int cnt = 0; cnt < ProjectSetting.folders.Length; cnt++)
            {
                DirectoryInfo dir = new DirectoryInfo("Assets/" + ProjectSetting.folders[cnt]);
                CommonUtil.CreateDirectoryRecursively(dir.FullName);
            }

            AssetDatabase.Refresh();
        }


        [MenuItem("FNI/앱 설정하기", false, 200)]
        protected static void SetAppSetting()
        {
            FNIBuildTool.SetAppSetting(ProjectSetting.buildTarget);
        }


        [MenuItem("FNI/한글 TMP 폰트 생성", false, 201)]
        public static void CreateTextMeshProFont()
        {
            string[] sAssetGuids = AssetDatabase.FindAssets("t:Font", new[] { "Assets/" + ProjectSetting.FontFolderName });
            foreach (string guid in sAssetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fontName = Path.GetFileNameWithoutExtension(path);
                Font font = AssetDatabase.LoadAssetAtPath<Font>(path);

                FNIFontCreateTool.CreateTextMeshProFont(font);
            }
        }


        [MenuItem("FNI/플레이어 로그 폴더 열기", false, 301)]
        protected static void OpenEditorLog()
        {
            // 에디터 로그 => C:\Users\username\AppData\Local\Unity\Editor\Editor.log
            // 플레이어 로그 => C:\Users\username\AppData\LocalLow\CompanyName\ProductName\Player.log

            // C:/Users/gyfus/AppData/LocalLow/FNI/App
            CommonUtil.OpenFolder(Application.persistentDataPath);
        }


        [MenuItem("FNI/빌드 폴더 열기", false, 302)]
        public static void OpenBuildFolder()
        {
            var target = BuildTarget.StandaloneWindows64;
            var buildPath = ProjectSetting.BuildFolderPath(target);

            CommonUtil.OpenFolder(Path.GetFullPath(buildPath));
        }


        [MenuItem("FNI/빌드 앱 (디버그 버전)", false, 401)]
        protected static void BuildDebug()
        {
            FNIBuildTool.Build(ProjectSetting.buildTarget, true);
        }


        [MenuItem("FNI/빌드 앱 (릴리즈 버전)", false, 402)]
        protected static void BuildRelease()
        {
            FNIBuildTool.Build(ProjectSetting.buildTarget, false);
        }
    }
}



