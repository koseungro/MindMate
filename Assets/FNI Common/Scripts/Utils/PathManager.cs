/// 작성자: 조효련
/// 작성일: 2020-11-03
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using System.Text;
using UnityEngine;


namespace FNI.Common.Utils
{
    /// <summary>
    /// 데이터 경로를 관리하는 클래스
    /// 에디터에서는 Assets 폴더와 같은 위치에 DATA 폴더를 생성한 후 DATA 폴더를 기준으로 로드를 하고
    /// 런타임에서는 StreamingAssetsPath 폴더를 기준으로 로드를 한다.
    /// 빌드 시에 빌드 스크립트에 StreamingAssetsPath로 복사하는 코드를 추가한다.
    /// </summary>
    public static class PathManager
    {
        // 샘플 코드
        // 사용법 :: Medias/SubFolder/[filename]
        public static string GetMediasFile(string filename)
        {
            StringBuilder sb = new StringBuilder("");

            sb.Append(GetBaseURL());

            sb.Append("Medias");

            sb.Append("/");

            sb.Append("SubFolder");

            sb.Append("/");

            sb.Append(filename.ToLower());

            return sb.ToString();
        }


        public static string GetBaseURL()
        {
            StringBuilder sb = new StringBuilder("");

            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                    // sb.Append(@"file:///");
                    sb.Append(Application.dataPath);
                    sb.Replace("Assets", ProjectSetting.DataFolderName);

                    sb.Append("/");

                    break;
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.WindowsPlayer:
                    // sb.Append(@"file:///");
                    sb.Append(Application.streamingAssetsPath);
                    sb.Append("/");
                    break;
            }

            return sb.ToString();
        }
    }
}

