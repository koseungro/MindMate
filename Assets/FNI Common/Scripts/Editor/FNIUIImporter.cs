/// 작성자: 김윤빈
/// 작성일: 2020-01-23
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// (2020-03-27)김윤빈
/// 1. 필터모드 지정 부분 주석처리
/// 
/// (2021-10-21)백인성
/// 1. 이름 앞에 "MultiSprite_"가 붙으면 SpriteMode를 Multiple로 변경하도록 수정함

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace FNI.Common.Editor
{
    /// <summary>
    /// Assets/FNI/Res/UI 폴더 안에 이미지를 넣으면 자동으로 Sprite로 변환해줍니다.
    /// 파일명 앞에 MultiSprite_를 붙이면 SpriteMode가 Multiple 모드로 적용됩니다.
    /// 기존 파일 FNISpriteImporter.cs에서 이름 변경됨
    /// </summary>
    public class FNIUIImporter : AssetPostprocessor
    {
        public override uint GetVersion()
        {
            return AssetImporterUtility.VERSION;
        }

        private void OnPreprocessTexture()
        {
            if (assetPath.StartsWith("Assets/" + ProjectSetting.UIFolderName) == false)
            {
                return;
            }

            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            // 변경사항 : [mod] 2021-10-21 Multiple Mode 추가
            SetSpriteMode(importer);

            // 변경사항 : [mod] 2020-03-27 필터모드 지정 주석처리
            // importer.filterMode = FilterMode.Point;
        }

        private void OnPostprocessTexture(Texture2D texture)
        {
            int width, height;
            if (TryGetSpriteSize(out width, out height) == false)
                return;

            var filename = Path.GetFileNameWithoutExtension(assetPath);

            // 스프라이트 생성에 관한 변수 설정
            var offset = Vector2.zero;
            var size = new Vector2(width, height);
            var padding = Vector2.zero;

            var rects = InternalSpriteUtility.GenerateGridSpriteRectangles(texture, offset, size, padding);

            // 생성한 스프라이트의 Rect를 기반으로 SpriteMetaData 를 생성
            var spriteMetadata = new List<SpriteMetaData>();

            for (int i = 0; i < rects.Length; i++)
            {
                var rect = rects[i];
                spriteMetadata.Add(new SpriteMetaData
                {
                    name = filename + " " + i,
                    rect = rect
                });
            }

            TextureImporter importer = (TextureImporter)assetImporter;
            // 변경사항 : [mod] 2021-10-21 Multiple Mode 추가
            SetSpriteMode(importer);

            // 마지막으로 스프라이트 정보를 적용
            importer.spritesheet = spriteMetadata.ToArray();
        }

        // 변경사항 : [mod] 2021-10-21 Multiple Mode 추가
        private void SetSpriteMode(TextureImporter importer)
        {
            if (Path.GetFileName(assetPath).StartsWith("MultiSprite_"))
            {
                importer.spriteImportMode = SpriteImportMode.Multiple;
            }
            else
                importer.spriteImportMode = SpriteImportMode.Single;
        }

        // 파일명으로부터 스프라이트의 사이즈를 얻어옴
        private bool TryGetSpriteSize(out int width, out int height)
        {
            width = 0;
            height = 0;

            var filename = Path.GetFileNameWithoutExtension(assetPath);
            var pattern = @"(?<name>.*?)_(?<width>\d+)x(?<height>\d+)";
            var regex = new Regex(pattern);

            if (regex.IsMatch(filename) == false)
                return false;

            var groups = regex.Match(filename).Groups;
            width = int.Parse(groups["width"].Value);
            height = int.Parse(groups["height"].Value);

            return true;
        }
    }

    public class AssetImporterUtility
    {
        public const int VERSION = 2020;
    }

}

