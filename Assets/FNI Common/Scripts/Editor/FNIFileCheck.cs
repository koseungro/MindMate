/// 작성자: 김윤빈
/// 작성일: 2020-01-23
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using System.IO;
using UnityEditor;
using UnityEngine;


namespace FNI.Common.Editor
{
    [InitializeOnLoad]
    public class FNIFileCheck
    {
        private static DirectoryInfo Top;
        private static DirectoryInfo Middle;
        private static DirectoryInfo Low;


        static FNIFileCheck()
        {
            TopFileCreate();
        }

        #region 최상위 폴더 생성

        /// <summary>
        /// 생성하고싶은 최상위 파일 이름
        /// </summary>
        static string[] TopFiles = new string[] { "StreamingAssets", "FNI", "ScriptTemplates" };

        /// <summary>
        /// 최상위 파일 생성
        /// </summary>
        static void TopFileCreate()
        {
            for (int cnt = 0; cnt < TopFiles.Length; cnt++)
            {
                Top = new DirectoryInfo("Assets/" + TopFiles[cnt]);
                if (Top.Exists == false)
                {
                    Debug.Log(string.Format("<color=yellow>[폴더확인]</color>{0} 폴더를 생성합니다.", Top.Name));
                    Top.Create();
                }
                else if (Top.Exists == true)
                {
                    // Debug.Log(string.Format("{0} 폴더 있음", Top.Name));
                }
            }
            CheckTopFile();
        }

        /// <summary>
        /// 최상위 파일 2개가 모두 생성 되면 체크하여 중간 폴더를 생성하는 함수를 호출합니다.
        /// </summary>
        static void CheckTopFile()
        {
            int fileCheck = 0;
            for (int cnt = 0; cnt < TopFiles.Length; cnt++)
            {
                Top = new DirectoryInfo("Assets/" + TopFiles[cnt]);
                if (Top.Exists == true)
                {
                    fileCheck++;
                    if (fileCheck == TopFiles.Length)
                    {
                        MiddleFile();
                    }
                }
            }
        }

        #endregion


        #region 중간 폴더 생성

        /// <summary>
        /// 중간 폴더 파일 이름
        /// 최상위 파일의 하위 폴더 이름
        /// </summary>
        static string[] StreamingAssetsFiles = new string[] { "FNI" };
        static string[] FNIFiles = new string[] { "Scripts", "Scenes", "Plugins", "Res" };

        /// <summary>
        /// 중간 폴더 체크
        /// </summary>
        static void MiddleFile()
        {
            for (int cnt = 0; cnt < TopFiles.Length; cnt++)
            {
                Top = new DirectoryInfo("Assets/" + TopFiles[cnt]);

                // 최상위 폴더가 없을 경우
                if (Top.Exists == false)
                {
                    Debug.Log(string.Format("<color=yellow>[폴더확인]</color>{0} 폴더가 없습니다.", Top.Name));
                    // 최상위 폴더 생성을 다시 실행해줍니다.
                    TopFileCreate();
                }
                // 최상위 폴더가 있는 경우
                else if (Top.Exists == true)
                {
                    // StreamingAssets 폴더가 있을 경우 중간 폴더 생성
                    if (Top.Name == "StreamingAssets")
                    {
                        // Debug.Log(string.Format("{0} 폴더가 있습니다. 중간 폴더를 생성합니다.", Top.Name));
                        CreateMiddleFile(StreamingAssetsFiles, Top.Name);
                    }
                    // FNI 폴더가 있을 경우 중간 폴더 생성
                    else if (Top.Name == "FNI")
                    {
                        // Debug.Log(string.Format("{0} 폴더가 있습니다. 중간 폴더를 생성합니다.", Top.Name));
                        CreateMiddleFile(FNIFiles, Top.Name);
                    }
                }
            }
            CheckMiddleFile();
        }

        /// <summary>
        /// 중간 폴더를 생성합니다.
        /// </summary>
        /// <param name="FileArray">생성할 파일 이름 배열</param>
        /// <param name="TopName">상위 폴더 이름</param>
        static void CreateMiddleFile(string[] FileArray, string TopName)
        {
            for (int cnt = 0; cnt < FileArray.Length; cnt++)
            {
                Middle = new DirectoryInfo(string.Format("Assets/{0}/{1}", TopName, FileArray[cnt]));
                if (Middle.Exists == false)
                {
                    Debug.Log(string.Format("<color=yellow>[폴더확인]</color>{0} 폴더가 없습니다. 폴더를생성합니다.", FileArray[cnt]));
                    Middle.Create();
                }
                else if (Middle.Exists == true)
                {
                    // Debug.Log(string.Format("{0} 폴더가 있습니다.", FileArray[cnt]));
                }
            }
        }

        /// <summary>
        /// 중간 파일이 모두 생성되면 체크하여 최하위 폴더를 생성하는 함수를 호출합니다.
        /// </summary>
        static void CheckMiddleFile()
        {
            int StreamingfileCheck = 0;
            int FNIfileCheck = 0;

            for (int cnt3 = 0; cnt3 < StreamingAssetsFiles.Length; cnt3++)
            {
                Middle = new DirectoryInfo(string.Format("Assets/{0}/{1}", TopFiles[0], StreamingAssetsFiles[cnt3]));
                if (Middle.Exists == true)
                {
                    StreamingfileCheck++;
                }
            }
            for (int cnt4 = 0; cnt4 < FNIFiles.Length; cnt4++)
            {
                Middle = new DirectoryInfo(string.Format("Assets/{0}/{1}", TopFiles[1], FNIFiles[cnt4]));
                if (Middle.Exists == true)
                {
                    FNIfileCheck++;
                }
            }
            // 파일 갯수를 카운트해서 맞게 생성되면 하위 파일을 생성해줍니다.
            if ((StreamingfileCheck + FNIfileCheck) == (StreamingAssetsFiles.Length + FNIFiles.Length))
            {
                LowFile();
            }
        }

        #endregion


        #region 최하위 폴더 생성

        /// <summary>
        /// 최하위에 생성할 파일 이름
        /// </summary>
        static string[] ModelsFiles = new string[] { "Materials", "Textures" };
        static string[] UIFiles = new string[] { "UIEffects" };
        static string[] ScriptsFiles = new string[] { "Editor", "Editor Default Resources", "Runtime", "Tests" };
        static string[] PluginsFiles = new string[] { "x86", "x86_64", "Android", "iOS" };
        static string[] ResFiles = new string[] { ".Font", "Font", "Models", "UI", "Prefabs", "Shaders", "Animations" };

        /// <summary>
        /// 최하위 폴더 생성
        /// </summary>
        static void LowFile()
        {
            for (int cnt = 0; cnt < FNIFiles.Length; cnt++)
            {
                Middle = new DirectoryInfo(string.Format("Assets/FNI/{0}", FNIFiles[cnt]));

                if (Middle.Exists == false)
                {
                    Debug.Log(string.Format("<color=yellow>[폴더확인]</color>{0} 폴더가 없습니다.", Middle.Name));
                }
                else if (Middle.Exists == true)
                {
                    switch (Middle.Name)
                    {
                        case "Models":
                            CreateLowFile(ModelsFiles, Middle.Name);
                            break;

                        case "UI":
                            CreateLowFile(UIFiles, Middle.Name);
                            break;

                        case "Scripts":
                            CreateLowFile(ScriptsFiles, Middle.Name);
                            break;

                        case "Plugins":
                            CreateLowFile(PluginsFiles, Middle.Name);
                            break;

                        case "Res":
                            CreateLowFile(ResFiles, Middle.Name);
                            break;
                    }
                }
            }
            Debug.Log("<color=yellow>[폴더확인]</color>폴더 생성이 완료됐습니다.");

            // 폴더를 전부 만든 후, 해당 스크립트를 Editor폴더로 옯겨줍니다. 필요할 때 사용하면 됩니다. 
            // fileMove("FNIFileCheck.cs");
        }

        /// <summary>
        /// 파일의 위치를 옮겨줍니다.
        /// </summary>
        /// <param name="fileName">옮기고싶은 파일의 이름을 확장자 까지 넣어줍니다.</param>
        /// <returns></returns>
        static void FileMove(string fileName)
        {
            FileInfo fi = new FileInfo(Application.dataPath + string.Format("/FNI/Scripts/Editor/{0}", fileName));
            // 파일이 원하는 폴더에 있는지 먼저 검사해줍니다.
            // 원하는 폴더에 파일이 있다면 로그를 띄워줍니다.
            if (fi.Exists)
            {
                Debug.Log(string.Format("Editor폴더 안에 {0}가 있습니다.", fileName));
            }
            // 폴더안에 파일이 없다면 프로젝트폴더에서 파일을 찾아 옮겨줍니다.
            else
            {
                string[] searchFNIFileCheck = Directory.GetFiles(Application.dataPath, fileName, SearchOption.AllDirectories);

                if (searchFNIFileCheck.Length != 0)
                {
                    // 한개의 파일만 찾는경우는 0번 인덱스에 경로가 들어갑니다.
                    // File.Move(구 경로, 신 경로);
                    File.Move(searchFNIFileCheck[0], Application.dataPath + "/FNI/Scripts/Editor/FNIFileCheck.cs");
                    Debug.Log(string.Format("Editor폴더로 {0}을 이동합니다.", fileName));
                }
                // 찾는 파일이 없으면 Length가 0입니다.
                else if (searchFNIFileCheck.Length == 0)
                {
                    Debug.Log("파일이 존재하지 않습니다. 파일명을 확인해주세요.");
                }
            }
        }


        /// <summary>
        /// 최하위 폴더를 체크해서 만들어 줍니다 
        /// </summary>
        /// <param name="fileArray">하위폴더 배열을 넣어줍니다.</param>
        /// <param name="MiddleFileName">체크할 중간 폴더의 이름을 넣어줍니다.</param>
        static void CreateLowFile(string[] fileArray, string MiddleFileName)
        {
            for (int cnt = 0; cnt < fileArray.Length; cnt++)
            {
                Low = new DirectoryInfo(string.Format("Assets/FNI/{0}/{1}", MiddleFileName, fileArray[cnt]));
                if (Low.Exists == false)
                {
                    Low.Create();
                }
            }
        }

        #endregion        
    }
}