/// 작성자: 조효련
/// 작성일: 2020-12-15
/// 수정일: 2022-01-10
/// 저작권: Copyright(C) FNIKorea Co., Ltd.. (주)에프앤아이코리아
/// 수정이력
/// (2022-01-10) 조효련
/// 1. 저작권 변경

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

