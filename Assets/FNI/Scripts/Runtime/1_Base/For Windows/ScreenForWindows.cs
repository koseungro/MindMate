/// 작성자: 백인성
/// 작성일: 2022-05-02
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    public class ScreenForWindows : MonoBehaviour
    {
        /// <summary>
        /// <see cref="FullScreenMode.ExclusiveFullScreen"/> => 전용 모드
        /// <see cref="FullScreenMode.FullScreenWindow"/> => 꽉 채워진 전체 화면
        /// <see cref="FullScreenMode.MaximizedWindow"/> => 화면 최대화
        /// <see cref="FullScreenMode.Windowed"/> => 창모드
        /// </summary>
        public FullScreenMode screenMode = FullScreenMode.ExclusiveFullScreen;
        public Vector2 screenSize = new Vector2(1920, 1080);

        private void Start()
        {
            Screen.SetResolution((int)screenSize.x, (int)screenSize.y, screenMode);
        }
    }
}