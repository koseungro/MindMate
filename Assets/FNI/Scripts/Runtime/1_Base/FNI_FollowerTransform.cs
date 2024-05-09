/// 작성자: 백인성
/// 작성일: 2021-07-28
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

namespace FNI.Common.Utils
{
    public class FNI_FollowerTransform : MonoBehaviour
    {
        public FNI_Follower follow;

        private void LateUpdate()
        {
            follow.Update();
        }
    }
}