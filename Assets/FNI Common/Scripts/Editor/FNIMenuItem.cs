/// �ۼ���: ��ȿ��
/// �ۼ���: 2020-11-03
/// ������: 
/// ���۱�: Copyright(C) FNI Co., LTD. 
/// �����̷�
///

using FNI.Common.Utils;
using System.IO;
using UnityEditor;
using UnityEngine;


/// <summary>
/// FNI �޴����� ��� ��Ƶδ� ��ũ��Ʈ�� ���
/// </summary>
namespace FNI.Common.Editor
{
    public class FNIMenuItem
    {
        [MenuItem("FNI/������Ʈ ���� Ȯ�� �� ����", false, 100)]
        public static void CheckFileList()
        {
            for (int cnt = 0; cnt < ProjectSetting.folders.Length; cnt++)
            {
                DirectoryInfo dir = new DirectoryInfo("Assets/" + ProjectSetting.folders[cnt]);
                CommonUtil.CreateDirectoryRecursively(dir.FullName);
            }

            AssetDatabase.Refresh();
        }


        [MenuItem("FNI/�� �����ϱ�", false, 200)]
        protected static void SetAppSetting()
        {
            FNIBuildTool.SetAppSetting(ProjectSetting.buildTarget);
        }


        [MenuItem("FNI/�ѱ� TMP ��Ʈ ����", false, 201)]
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


        [MenuItem("FNI/�÷��̾� �α� ���� ����", false, 301)]
        protected static void OpenEditorLog()
        {
            // ������ �α� => C:\Users\username\AppData\Local\Unity\Editor\Editor.log
            // �÷��̾� �α� => C:\Users\username\AppData\LocalLow\CompanyName\ProductName\Player.log

            // C:/Users/gyfus/AppData/LocalLow/FNI/App
            CommonUtil.OpenFolder(Application.persistentDataPath);
        }


        [MenuItem("FNI/���� ���� ����", false, 302)]
        public static void OpenBuildFolder()
        {
            var target = BuildTarget.StandaloneWindows64;
            var buildPath = ProjectSetting.BuildFolderPath(target);

            CommonUtil.OpenFolder(Path.GetFullPath(buildPath));
        }


        [MenuItem("FNI/���� �� (����� ����)", false, 401)]
        protected static void BuildDebug()
        {
            FNIBuildTool.Build(ProjectSetting.buildTarget, true);
        }


        [MenuItem("FNI/���� �� (������ ����)", false, 402)]
        protected static void BuildRelease()
        {
            FNIBuildTool.Build(ProjectSetting.buildTarget, false);
        }
    }
}



