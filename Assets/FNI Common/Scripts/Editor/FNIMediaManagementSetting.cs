/// �ۼ���: ��ȿ��
/// �ۼ���: 2020-12-15
/// ������: 2022-01-10
/// ���۱�: Copyright(C) FNIKorea Co., Ltd.. (��)�����ؾ����ڸ���
/// �����̷�
/// (2022-01-10) ��ȿ��
/// 1. ���۱� ����

using UnityEditor;
using UnityEngine;


namespace FNI.Common.Editor
{
    public class FNIMediaManagementSetting : ScriptableObject
    {
        private const string FileDirectoryPath = "FNI Common/Scripts/Editor/";
        private const string File = "ProjectSettings.asset";

        [SerializeField]
        private string assetFolder;
        [SerializeField]
        private string netdriveFolder;


        internal static FNIMediaManagementSetting GetOrCreateSettings()
        {
            string path = "Assets/" + FileDirectoryPath + File;

            var settings = AssetDatabase.LoadAssetAtPath<FNIMediaManagementSetting>(path);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<FNIMediaManagementSetting>();
                settings.assetFolder = string.Empty;
                settings.netdriveFolder = string.Empty;

                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Application.dataPath, FileDirectoryPath));

                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}

