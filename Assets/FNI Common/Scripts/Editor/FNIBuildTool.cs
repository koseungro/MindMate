/// 작성자: 조효련
/// 작성일: 2020-11-03
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.Common.Utils;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace FNI.Common.Editor
{
    /// <summary>
    /// 자동빌드 스크립트
    /// Build 기본 절차
    /// 1) ProjectSetting에 작성된 정보를 기준으로 앱 설정하기
    /// 2) 빌드하기
    /// 3) Data 폴더를 빌드된 앱의 StreamingAssets 폴더에 복사하기
    /// </summary>
    public class FNIBuildTool
    {
        // 빌드 
        public static void Build(BuildTarget target, bool isDebug)
        {
            SetAppSetting(target);

            BuildOptions opts = isDebug ? (BuildOptions.Development | BuildOptions.AllowDebugging) : BuildOptions.None;

            string buildPath = ProjectSetting.BuildFolderPath(target) + "/" + (isDebug ? ProjectSetting.DebugFolderName : ProjectSetting.ReleaseFolderName);
            string fullBuildPath = Directory.GetCurrentDirectory() + "/" + buildPath;

            string exeFileName = Application.productName + ".exe";
            string exePathName = buildPath + "/" + exeFileName;

            string path_streamingAssets_old = ProjectSetting.DataFolderPath;
            string path_streamingAssets_new = $"{fullBuildPath}/{Application.productName}_Data/StreamingAssets/";


            // 빌드 폴더는 삭제한 후 생성하기
            Debug.Log($"<color=yellow>[빌드]</color>빌드 시작...\n* 경로정보\n빌드경로 : {buildPath}\n파 일 명 : {exeFileName}\n상대경로 : {exePathName}\n절대경로 : {fullBuildPath}\nStreamingAssets 에디터 : {path_streamingAssets_old}\nStreamingAssets 런타임 : {path_streamingAssets_new}");


            if (Directory.Exists(buildPath))
                Directory.Delete(buildPath, true);

            Directory.CreateDirectory(buildPath);


            Debug.Log($"<color=yellow>[빌드]</color>빌드 중...");

            var result = BuildPipeline.BuildPlayer(ProjectSetting.scenes, exePathName, target, opts);
            if (isDebug == false)
            {
                var il2cppDirs = Directory.GetDirectories(fullBuildPath).Where(s => s.Contains("BackUpThisFolder_ButDontShipItWithYourGame"));
                if (il2cppDirs.Count() > 0)
                {
                    foreach (var dir in il2cppDirs)
                        Directory.Delete(dir, true);
                }
            }


            Debug.Log($"<color=yellow>[빌드]</color>DATA -> StreamingAssets 복사 시작...");

            if (!Directory.Exists(path_streamingAssets_new))
                Directory.CreateDirectory(path_streamingAssets_new);

            CommonUtil.CopyDirectory(path_streamingAssets_old, path_streamingAssets_new);

            Debug.Log($"<color=yellow>[빌드]</color>DATA -> StreamingAssets 복사 완료...");
            Debug.Log($"<color=yellow>[빌드]</color>==== 빌드 완료 =====");
        }
        
        // 앱 설정 수정하기
        public static void SetAppSetting(BuildTarget target)
        {
            // 앱 설정
            PlayerSettings.companyName = ProjectSetting.company;
            Debug.Log($"<color=yellow>[빌드]</color>회사 명은 {PlayerSettings.companyName}으로 설정되었습니다.");

            PlayerSettings.productName = ProjectSetting.product;
            Debug.Log($"<color=yellow>[빌드]</color>상품 명은 {PlayerSettings.productName}으로 설정되었습니다.");


            // 앱 스플래쉬 설정
            SetSplashSetting();

            // 앱 아이콘 설정
            SetIconSetting();

            // 엡 스크립트 타입 설정
            switch (target)
            {
                case BuildTarget.StandaloneWindows: SetIL2CPP(BuildTargetGroup.Standalone); break;
                case BuildTarget.Android: SetIL2CPP(BuildTargetGroup.Android); break;
                case BuildTarget.iOS: SetIL2CPP(BuildTargetGroup.iOS); break;
            }
        }

        // 스크립트 백그라운드 설정 수정하기
        public static void SetIL2CPP(BuildTargetGroup target)
        {
            if (PlayerSettings.GetScriptingBackend(target) != ScriptingImplementation.IL2CPP)
            {
                PlayerSettings.SetScriptingBackend(target, ScriptingImplementation.IL2CPP);
                PlayerSettings.SetIl2CppCompilerConfiguration(target, Il2CppCompilerConfiguration.Release);

                Debug.Log($"<color=yellow>[빌드]</color>IL2CPP로 설정되었습니다.");
            }
            else
            {
                Debug.Log($"<color=yellow>[빌드]</color>IL2CPP로 설정되어 있습니다.");
            }
        }

        // 스플래시 이미지 수정하기
        public static void SetSplashSetting()
        {            
            // 스플래쉬 이미지의 백그라운드 컬러 셋팅 
            PlayerSettings.SplashScreen.backgroundColor = ProjectSetting.appSplashColor;

            var logos = new PlayerSettings.SplashScreenLogo[1];

            Sprite splash = (Sprite)AssetDatabase.LoadAssetAtPath(ProjectSetting.appSplashPath, typeof(Sprite));
            if (splash)
            {
                logos[0] = PlayerSettings.SplashScreenLogo.Create(2.5f, splash);
                Debug.Log($"<color=yellow>[빌드]</color>Splash 이미지 설정되었습니다.");
            }
            else
            {
                Debug.Log($"<color=magenta>[빌드]</color>Splash 파일이 존재하지 않습니다. {ProjectSetting.appSplashPath} 파일을 확인바랍니다.");
            }

            PlayerSettings.SplashScreen.logos = logos;
        }

        // 앱 아이콘 수정하기
        public static void SetIconSetting()
        {
            Sprite icon = (Sprite)AssetDatabase.LoadAssetAtPath(ProjectSetting.appIconPath, typeof(Sprite));
            if (icon)
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { icon.texture });
                Debug.Log($"<color=yellow>[빌드]</color>Icon 이미지 설정되었습니다.");
            }
            else
            {
                Debug.Log($"<color=magenta>[빌드]</color>Splash 파일이 존재하지 않습니다. {ProjectSetting.appIconPath} 파일을 확인바랍니다.");
            }
        }
    }
}

