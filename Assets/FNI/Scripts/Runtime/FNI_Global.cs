/// 작성자: 백인성
/// 작성일: 2021-04-19
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.IS;

using System;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;


namespace FNI.GLOBAL
{
    public class FNI_Path
    {
#if UNITY_EDITOR
        public static string Base => Application.streamingAssetsPath;
#else
        public static string Base => Application.persistentDataPath;
#endif

        public static string FNI => $"{Base}/FNI";
        public static string LOG => $"{FNI}/Log";

        public static string ProjectSettings => $"{FNI}/ProjectSettings.json";
    }
}