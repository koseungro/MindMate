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
using UnityEngine.SceneManagement;
using FNI.XRST;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    public class FNI_BackgroundLoad : MonoBehaviour
    {
        private UnityAction onLoadComplete;
        private UnityAction onUnloadComplete;
        protected virtual void LoadBackground(string name, UnityAction onLoadComplete = null)
        {
            this.onLoadComplete = onLoadComplete;
            StartCoroutine(Load_Routine(name));
        }
        protected virtual void UnloadBackground(string name, UnityAction onUnloadComplete = null)
        {
            this.onUnloadComplete = onUnloadComplete;
            StartCoroutine(Unload_Routine(name));
        }

        private IEnumerator Load_Routine(string sceneName)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            if (async != null)
            {
                while (async.isDone == false)
                    yield return null;

                yield return new WaitForSeconds(1);

                async.allowSceneActivation = true;
                Debug.Log($"[XRST_SequenceManager/Load Success] Background {sceneName} is Valid.");
            }
            else
            {
                Debug.Log($"[XRST_SequenceManager/Load Failed] Background {sceneName} is Invalid.");
            }

            onLoadComplete?.Invoke();
        }

        private IEnumerator Unload_Routine(string sceneName)
        {
            AsyncOperation async = SceneManager.UnloadSceneAsync(sceneName);

            if (async != null)
            {
                while (async.isDone == false)
                    yield return null;

                yield return new WaitForSeconds(1);

                Debug.Log($"[XRST_SequenceManager/Unload Success] Background {sceneName} is UnLoaded.");
            }
            else
            {
                Debug.Log($"[XRST_SequenceManager/Unload Failed] Background {sceneName} is Fail Unload.");
            }

            onUnloadComplete?.Invoke();
        }
    }
}