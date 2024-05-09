/// 작성자: 백인성
/// 작성일: 2022-07-11
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
    public partial class IS_GameObjectSetting
    {
        private int gridWidth = 3;

        private Type[] types = new Type[]
        {
            typeof(BoxCollider),
            typeof(MeshCollider),
        };
    }
}