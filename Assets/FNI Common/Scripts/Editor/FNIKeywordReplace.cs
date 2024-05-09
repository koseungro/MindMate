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
    public class FNIKeywordReplace : UnityEditor.AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            // path에 .이 포함되어 있다면 해당 index를 받아옴
            int index = path.LastIndexOf(".");
            // 그렇지않다면 -1이 index로 들어오므로 return
            if (index < 0)
            {
                return;
            }

            // .이 포함된 index로 path를 나눠서 확장자가 .cs가 아니면 return
            string file = path.Substring(index);
            if (file != ".cs")
            {
                return;
            }


            // 유니티 에셋 데이터베이스 상의 주소를 실제 주소로 변경
            index = Application.dataPath.LastIndexOf("Assets");
            path = Application.dataPath.Substring(0, index) + path;

            if (!File.Exists(path))
            {
                return;
            }

            // 스크립트 내용을 불러오기
            string fileContent = File.ReadAllText(path);

            // #DATE# 키워드 대체
            fileContent = fileContent.Replace("#DATE#", System.DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.CreateSpecificCulture("ko-KR")));

            // #AUTHOR#키워드 대체, 작성자 이름을 지우시고 본인 이름을 적으시면 스크립트가 생성될 때 자동으로 작성자 란에 이름이 들어갑니다.
            fileContent = fileContent.Replace("#AUTHOR#", "작성자 이름");

            // 대체가 끝나면 다시 파일에 쓰기
            System.IO.File.WriteAllText(path, fileContent);

            // 다하고 나면 호출
            AssetDatabase.Refresh();
        }
    }
}
