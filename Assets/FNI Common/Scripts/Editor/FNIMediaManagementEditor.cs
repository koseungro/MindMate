/// �ۼ���: ��ȿ��
/// �ۼ���: 2020-12-15
/// ������: 2022-01-10
/// ���۱�: Copyright(C) FNIKorea Co., Ltd.. (��)�����ؾ����ڸ���
/// �����̷�
/// (2022-01-10) ��ȿ��
/// 1. ���۱� ����

using UnityEditor;


namespace FNI.Common.Editor
{
    // ������ �󿡼��� ������Ʈ ���� ���� ������ �÷��� ��Ű��
    // ���� �ÿ��� ������ ������ �� ��Ű¡�ϵ��� �����ϴ� Ŭ����

    public class FNIMediaManagementEditor : EditorWindow
    {
        [MenuItem("FNI/�̵�� ����")]
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
            titleContent = EditorGUIUtility.TrTextContent("�̵�� ����");
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


            // ������ ����Ʈ

        }
    }

}
