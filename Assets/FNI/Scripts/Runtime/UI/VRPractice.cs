/// 작성자: 고승로
/// 작성일: 2021-02-08
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력


using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace FNI
{
    public class VRPractice : MonoBehaviour
    {
        [SerializeField] private Animator hintAnimator = null;
        [SerializeField] private Button[] buttons = new Button[3];
        [SerializeField] private float maxTime = 5.0f;
        [SerializeField] private GameObject FailUI = null;

        private float checkTime = 0.0f;

        private void OnEnable()
        {
            hintAnimator.SetBool("Blink", false);
            EnabledButtons(true);
            FailUI.SetActive(false);

            StartVRPracticeRoutine();
        }

        public void OnClickfailEvent()
        {
            EnabledButtons(false);
            

            StopVRPracticeRoutine();
        }

        public void OnClickEvent(GameObject obj)
        {
            if (obj.name == "succeedButton")
            {
                EffectAudioManager.Instance.PlaySoundEffect(EffectAudioClip.OK);
            }
            else
            {
                EffectAudioManager.Instance.PlaySoundEffect(EffectAudioClip.NO);
            }
            

        }

        private void EnabledButtons(bool enabled)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].enabled = enabled;
            }
        }

        private IEnumerator VRPracticeRoutine()
        {
            while(true)
            {
                if (FailUI.activeSelf)
                    continue;

                checkTime = 0.0f;

                while (checkTime < maxTime)
                {
                    checkTime += Time.deltaTime;
                    yield return null;
                }

                hintAnimator.SetBool("Blink", true);

                yield return new WaitForSeconds(10.0f);

                hintAnimator.SetBool("Blink", false);

            }
            
        }


        public void StartVRPracticeRoutine()
        {
            StartCoroutine(VRPracticeRoutine());
        }

        public void StopVRPracticeRoutine()
        {
            StopAllCoroutines();
            hintAnimator.SetBool("Blink", false);
        }

    }


}