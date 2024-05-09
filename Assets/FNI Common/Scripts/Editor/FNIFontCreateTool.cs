/// 작성자: 조효련
/// 작성일: 2020-11-03
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
///

using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;


namespace FNI.Common.Editor
{
    /// <summary>
    /// FNI 폴더의 Font 폴더 내의 .ttf, .otf 파일을 TMP Font로 생성하는 코드
    /// 
    /// TMP 폰트 생성 메뉴 : Window/TextMeshPro/Font Asset Creator
    /// Get Kerning Pairs는 특정 문자 간 거리를 조정하여 최적화해주는 기능
    /// 
    /// 팁!
    /// 폰트 애셋을 생성한 후에는 실제 폰트 파일을.Font 폴더로 이동을 하면 
    /// 프로젝트를 재임포트할때 해당 폰트들은 재임포트 하는 시간을 줄일 수 있음
    /// 
    /// TODO : 프로젝트에 따라서 아래의 설정을 수정하기 바랍니다.
    /// </summary>
    public static class FNIFontCreateTool
    {
        public static void CreateTextMeshProFont(Font font)
        {
            if (font == null)
            {
                Debug.LogWarning("폰트를 선택한 후 메뉴를 실행해 주세요.");
                return;
            }

            // 널리 사용될 폰트라면, 2350자 + 특문 영문 다 입력해두고 static으로 작업하기
            // Label로만 사용되고, 몇몇 글자를 강조하기 위해 사용되는 경우라면 
            //      1) 사용되는 텍스트만 넣어서 static으로 사용하기
            //      2) dynamic으로 설정하기
            AtlasPopulationMode atlasPopulationMode = AtlasPopulationMode.Dynamic;

            // SDF (Signed Distance Field) 샘플링의 정확도
            // 50에서 70 사이의 포인트 크기 권장
            int samplingPointSize   = 60;
            
            // 패딩
            // 샘플링 크기에 대한 비율이 1:10이 좋음.
            int atlasPadding        = 6;

            // 텍스쳐 사이즈 - 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192
            // 모바일의 경우에는 2048*2048로 하기. OS 환경에 따라 4096으로 넣을 수 있음
            int atlasWidth          = 4096;
            int atlasHeight         = 8192;

            // 렌더 모드
            // 픽셀 텍스트가 아니라면 SDFAA_HINTED를 사용하는 것이 좋음
            // 픽셀 텍스트이면 RASTER_HINTED를 사용하는 것이 좋음
            // 
            // SMOOTH_HINTED
            //     힌팅, 안티 앨리어싱이 된 글꼴 렌더링.
            //     문자가 커지거나 작아져도 라인이 부드럽게 연결
            // SMOOTH
            //     힌팅없이 안티 앨리어싱된 글꼴 렌더링.
            //     동적으로 사용할 때 렌더링이 가장 빠른 모드
            // RASTER_HINTED
            //     힌팅만하고 안티앨리어싱이 안된 글꼴 렌더링.
            //     가장 선명한 글꼴 렌더링 옵션이며, 픽셀 표현에 유리
            // RASTER
            //     힌팅도 안티앨리어싱도 안된 글꼴 렌더링.
            //     그냥 원래 폰트 처음 그대로 출력
            // SDF (Signed distance field) | SDF8 | SDF16 | SDF32
            //     위의 비트맵 방식과는 다르게 거리에 따라 선명도를 계산하여 보여주는 방식.
            //     셰이더를 통해 아웃라인등을 구현하여 꾸밀 수 있다는 장점이 있다.
            //     작은 글자는 오히려 잘 안보이고, 비트맵처럼 전체 문자를 전부 바꾸면 용량이 커진다는 단점이 있어서 필요한 문장을 추가해나가는 것을 추천한다.
            //     수시로 보이는 거리가 달라지거나 셰이더를 이용해서 아웃라인이나 질감 표현 등을 하는 타이틀 제목, 3D 게임 내 표지판, 간판 등에 어울림
            //     아웃라인을 사용할 수 있음
            // SDFAA
            //      SDF + 안티앨리어싱
            // SDFAA_HINTED
            //      SDF + 선명도 유지
            GlyphRenderMode renderMode = GlyphRenderMode.SDFAA;


            // Packing Method - Fast = 0, Optimum = 4
            // Fast는 글꼴을 빠르게 미리 보는 데 적합하며 Optimum은 최종 아틀라스에 가장 적합. Fast는 중복되는 문자도 존재함
            int packingMode = 0;

            // 텍스트는 파일로 추가함
            // 유니코드 범위
            //      영어 범위 32-126
            //      한글 범위 44032-55203 (완성형 기본 한글 2350개수) 
            //      한글자모_2 12593-12643
            //      특수문자 8200-9900
            // Character Sequence(Decimal)을 32-126,44032-55203,12593-12643,8200-9900으로 넣어도 됨
            TextAsset characterList = AssetDatabase.LoadAssetAtPath<TextAsset>(ProjectSetting.FontFileName);
            



            // 폰트 생성하기
            string sourceFontFilePath       = AssetDatabase.GetAssetPath(font);
            string guid                     = AssetDatabase.AssetPathToGUID(sourceFontFilePath);
            string folderPath               = Path.GetDirectoryName(sourceFontFilePath);
            string assetName                = Path.GetFileNameWithoutExtension(sourceFontFilePath);
            string newAssetFilePathWithName = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + assetName + ".asset");


            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font, samplingPointSize, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode);


            FontAssetCreationSettings settings = new FontAssetCreationSettings();
            settings.sourceFontFileGUID = guid;
            settings.pointSize          = fontAsset.faceInfo.pointSize;
            settings.padding            = fontAsset.atlasPadding;
            settings.atlasWidth         = fontAsset.atlasWidth;
            settings.atlasHeight        = fontAsset.atlasHeight;
            settings.renderMode         = (int)fontAsset.atlasRenderMode;
            settings.packingMode        = packingMode;

            fontAsset.creationSettings = settings;


            // 파일 방식으로 설정 (2445개 글자)
            if (characterList)
            {
                settings.characterSetSelectionMode = 8;
                settings.characterSequence = characterList.text;
                string missingCharacters = string.Empty;
                fontAsset.TryAddCharacters(settings.characterSequence, out missingCharacters);
                Debug.Log($"폰트 빠진 문자 : {missingCharacters}");
            }
            // 유니코드로 설정
            else
            {
                Debug.Log($"{ProjectSetting.FontFileName} 파일이 존재하지 않아서 유니코드(정수)형으로 추가함.");
                uint[] unicodes = ParseNumberSequence("32-126,44032-55203,12593-12643,8200-9900");
                uint[] missingUnicodes = new uint[] { };
                fontAsset.TryAddCharacters(unicodes, out missingUnicodes);
            }


            AssetDatabase.CreateAsset(fontAsset, newAssetFilePathWithName);

            fontAsset.atlasTextures[0].name = assetName + " Atlas";
            AssetDatabase.AddObjectToAsset(fontAsset.atlasTextures[0], fontAsset);

            string shaderName = string.Empty;
            if (renderMode == GlyphRenderMode.SMOOTH_HINTED 
                || renderMode == GlyphRenderMode.SMOOTH
                || renderMode == GlyphRenderMode.RASTER_HINTED
                || renderMode == GlyphRenderMode.RASTER)
            {
                shaderName = "TextMeshPro/Bitmap";
            }
            else
            {
                shaderName = "TextMeshPro/Distance Field"; 
                // 모바일인 경우에는 TextMeshPro/Mobile/Distance Field
            }

            Shader defaultShader = Shader.Find(shaderName); 
            fontAsset.material.shader = defaultShader;
            fontAsset.material.name = assetName + " Material";
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);


            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
        }


        static uint[] ParseNumberSequence(string sequence)
        {
            List<uint> unicodeList = new List<uint>();
            string[] sequences = sequence.Split(',');

            foreach (string seq in sequences)
            {
                string[] s1 = seq.Split('-');

                if (s1.Length == 1)
                {
                    try
                    {
                        unicodeList.Add(uint.Parse(s1[0]));
                    }
                    catch
                    {
                        Debug.Log("No characters selected or invalid format.");
                    }
                }
                else
                {
                    for (uint j = uint.Parse(s1[0]); j < uint.Parse(s1[1]) + 1; j++)
                    {
                        unicodeList.Add(j);
                    }
                }
            }

            return unicodeList.ToArray();
        }
    }
}
