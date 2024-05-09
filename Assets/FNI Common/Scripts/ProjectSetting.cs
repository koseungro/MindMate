/// 작성자: 조효련
/// 작성일: 2020-11-03
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
///

using FNI.Common.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FNI.Common
{
    /// <summary>
    /// 
    /// 경로정보
    /// 빌드경로 : [BuildFolderPath]/[DebugFolderName | ReleaseFolderName]
    /// 파 일 명 : [product].exe
    /// 상대경로 : [BuildFolderPath]/[DebugFolderName|ReleaseFolderName]/[product].exe
    /// 절대경로 : [Project Path(드라이브명 포함)]/[BuildFolderPath]/[DebugFolderName | ReleaseFolderName]
    /// StreamingAssets 에디터 : [Project Path(드라이브명 포함)]/[DataFolderName]/
    /// StreamingAssets 런타임 : [Project Path(드라이브명 포함)]/[BuildFolderPath]/[DebugFolderName | ReleaseFolderName]/[product]_Data/StreamingAssets/
    /// 
    /// 
    /// 1. 회사명은 company을 수정합니다.
    /// 2. 제품명은 product을 수정합니다.
    /// 3. PC버전이 아닐 경우 buildTarget을 수정합니다.
    /// 4. 스플래시 이미지 및 앱 아이콘 이미지를 추가합니다.
    /// 
    /// </summary>
    public static class ProjectSetting
    {
#if UNITY_EDITOR
        // 빌드할 신 리스트
        public static string[] scenes = new string[]
        {
            "Assets/FNI/Scenes/main.unity"
            // 추가하려면 밑에 부분에 추가하기
            //, "Assets/FNI/Scenes/main.unity"
        };

        // 회사 이름
        public static string company = "FNI"; 

        // 빌드될 exe의 타이틀 및 파일명
        public static string product = "App";

        // 빌드될 exe 버전명
        public static string Version = "1.0";


        // 빌드할 환경
        public static BuildTarget buildTarget = BuildTarget.StandaloneWindows; 
        

        // 스플래시 이미지 백그라운드 컬러값
        public static Color appSplashColor = Color.black;

        // 스플래시 이미지 경로 
        public static string appSplashPath = "Assets/FNI/Res/UI/app_splash.png";

        // 아이콘 이미지 경로
        public static string appIconPath = "Assets/FNI/Res/UI/app_icon.png";


        // 빌드 경로 
        // 플랫폼이 다양할 경우 "Build/" + target.ToString();으로 설정해도 됨
        public static string BuildFolderPath(BuildTarget target) => "Build";

        public static string DebugFolderName => "Debug";
        public static string ReleaseFolderName => "Release";

        // 에디터 상에서의 StreamingAssets 폴더로 사용할 경로
        public static string DataFolderPath => PathManager.GetBaseURL();


        // 프로젝트 기본 폴더
        public static string[] folders = new string[] {
            StreamingAssetsFolderName,
            "FNI/Plugins/Android",
            "FNI/Plugins/iOS",
            "FNI/Plugins/x86",
            "FNI/Plugins/x86_64",
            "FNI/Res/Animations",
            "FNI/Res/.Font",
            FontFolderName,
            "FNI/Res/Models/Materials",
            "FNI/Res/Models/Textures",
            "FNI/Res/Prefabs",
            "FNI/Res/Shaders",
            UIFolderName,
            "FNI/Scenes",
            "FNI/Scripts/Editor",
            "FNI/Scripts/Editor Default Resources",
            "FNI/Scripts/Runtime",
            "FNI/Scripts/Tests",
        };

        public static string StreamingAssetsFolderName  => "StreamingAssets/FNI";
        public static string FontFolderName             => "FNI/Res/Font";
        public static string UIFolderName               => "FNI/Res/UI";
#endif

        public static string DataFolderName => "DATA"; // 에디터 상태에서늬 StreamingAssets 폴더 이름

        public static string FontFileName => "Assets/FNI Common/FontSetting/KS1001.txt";
    }
}

