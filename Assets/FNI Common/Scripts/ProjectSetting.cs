/// �ۼ���: ��ȿ��
/// �ۼ���: 2020-11-03
/// ������: 
/// ���۱�: Copyright(C) FNI Co., LTD. 
/// �����̷�
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
    /// �������
    /// ������ : [BuildFolderPath]/[DebugFolderName | ReleaseFolderName]
    /// �� �� �� : [product].exe
    /// ����� : [BuildFolderPath]/[DebugFolderName|ReleaseFolderName]/[product].exe
    /// ������ : [Project Path(����̺�� ����)]/[BuildFolderPath]/[DebugFolderName | ReleaseFolderName]
    /// StreamingAssets ������ : [Project Path(����̺�� ����)]/[DataFolderName]/
    /// StreamingAssets ��Ÿ�� : [Project Path(����̺�� ����)]/[BuildFolderPath]/[DebugFolderName | ReleaseFolderName]/[product]_Data/StreamingAssets/
    /// 
    /// 
    /// 1. ȸ����� company�� �����մϴ�.
    /// 2. ��ǰ���� product�� �����մϴ�.
    /// 3. PC������ �ƴ� ��� buildTarget�� �����մϴ�.
    /// 4. ���÷��� �̹��� �� �� ������ �̹����� �߰��մϴ�.
    /// 
    /// </summary>
    public static class ProjectSetting
    {
#if UNITY_EDITOR
        // ������ �� ����Ʈ
        public static string[] scenes = new string[]
        {
            "Assets/FNI/Scenes/main.unity"
            // �߰��Ϸ��� �ؿ� �κп� �߰��ϱ�
            //, "Assets/FNI/Scenes/main.unity"
        };

        // ȸ�� �̸�
        public static string company = "FNI"; 

        // ����� exe�� Ÿ��Ʋ �� ���ϸ�
        public static string product = "App";

        // ����� exe ������
        public static string Version = "1.0";


        // ������ ȯ��
        public static BuildTarget buildTarget = BuildTarget.StandaloneWindows; 
        

        // ���÷��� �̹��� ��׶��� �÷���
        public static Color appSplashColor = Color.black;

        // ���÷��� �̹��� ��� 
        public static string appSplashPath = "Assets/FNI/Res/UI/app_splash.png";

        // ������ �̹��� ���
        public static string appIconPath = "Assets/FNI/Res/UI/app_icon.png";


        // ���� ��� 
        // �÷����� �پ��� ��� "Build/" + target.ToString();���� �����ص� ��
        public static string BuildFolderPath(BuildTarget target) => "Build";

        public static string DebugFolderName => "Debug";
        public static string ReleaseFolderName => "Release";

        // ������ �󿡼��� StreamingAssets ������ ����� ���
        public static string DataFolderPath => PathManager.GetBaseURL();


        // ������Ʈ �⺻ ����
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

        public static string DataFolderName => "DATA"; // ������ ���¿����� StreamingAssets ���� �̸�

        public static string FontFileName => "Assets/FNI Common/FontSetting/KS1001.txt";
    }
}

