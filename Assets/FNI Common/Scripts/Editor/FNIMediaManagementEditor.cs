/// 작성자: 조효련
/// 작성일: 2020-12-15
/// 수정일: 2022-01-10
/// 저작권: Copyright(C) FNIKorea Co., Ltd.. (주)에프앤아이코리아
/// 수정이력
/// (2022-01-10) 조효련
/// 1. 저작권 변경

using UnityEditor;


namespace FNI.Common.Editor
{
    // 에디터 상에서는 프로젝트 폴더 밖의 영상을 플레이 시키고
    // 빌드 시에는 영상을 복사한 뒤 패키징하도록 관리하는 클래스

    public class FNIMediaManagementEditor : EditorWindow
    {
        [MenuItem("FNI/미디어 관리")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(FNIMediaManagementEditor));
        }

        private static FNIMediaManagementSetting setting;

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            if (!setting)
            {
                setting = FNIMediaManagementSetting.GetOrCreateSettings();
            }

            // if (!File.Exists(FilePath))
            //     return;
            // JsonUtility.FromJsonOverwrite(File.ReadAllText(FilePath), data);
        }

        void OnEnable()
        {
            titleContent = EditorGUIUtility.TrTextContent("미디어 관리");
        }


        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {

                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();


            // 페이지 리스트

        }
    }

}
