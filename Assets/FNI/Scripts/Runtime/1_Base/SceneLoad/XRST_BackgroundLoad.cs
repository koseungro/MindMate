/// 작성자: 백인성
/// 작성일: 2021-07-28
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.Common;
using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI.XRST
{
    public class XRST_BackgroundLoad : FNI_BackgroundLoad
    {
        public string Name => $"{gameObject.name}({GetType().FullName})";

        public List<SceneName> loadedBackground = new List<SceneName>();

        public enum SceneName
        {
            None,
            /// <summary>
            /// 연습
            /// </summary>
            Training,
            /// <summary>
            /// 응급실
            /// </summary>
            EmergencyRoom,

            /// <summary>
            /// 구급차
            /// </summary>
            Ambulence,
            /// <summary>
            /// 닥터헬기
            /// </summary>
            DoctorHelicopter,

            /// <summary>
            /// 교통 사고 현장
            /// </summary>
            Accident_Car,
            /// <summary>
            /// 추락 사고 현장
            /// </summary>
            Accident_Fall,
            /// <summary>
            /// 폭발 사고 현장
            /// </summary>
            Accident_Explosion,
            /// <summary>
            /// 관통 사고 현장
            /// </summary>
            Accident_Penetrate_knife,
            /// <summary>
            /// 관통 사고 현장
            /// </summary>
            Accident_Penetrate_Gun,
            /// <summary>
            /// 절단 사고 현장
            /// </summary>
            Accident_Amputation,
            /// <summary>
            /// 압궤 사고 현장
            /// </summary>
            Accident_Compressed,
            //...추가
        }
        public void Load(SceneName scene)
        {
            loadedBackground.Add(scene);

            LoadBackground(scene.ToString());
        }
        private void Unload()
        {
            if (loadedBackground.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }
            UnloadBackground(loadedBackground[0].ToString(), onComplete);
            loadedBackground.RemoveAt(0);
        }

        private UnityAction onComplete;
        public void Shutdown(UnityAction onComplete)
        {
            this.onComplete = onComplete;
            Unload();
        }

        public void Reset(UnityAction onComplete)
        {
            Shutdown(onComplete);
        }
    }
}