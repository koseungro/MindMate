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
    public enum EffectAudioClip
    {
        ButtonClick,
        ButtonHover,
        NO,
        OK
    }

    public class EffectAudioManager : MonoBehaviour
    {
        private static EffectAudioManager instance = null;

        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private List<AudioClip> effectAudioClipList = new List<AudioClip>();

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }

        public static EffectAudioManager Instance
        {
            get
            {
                if(instance == null)
                {
                    return null;
                }

                return instance;
            }

        }

        private void Reset()
        {
#if UNITY_EDITOR
            audioSource = GetComponent<AudioSource>();
            effectAudioClipList.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/FNI/Res/Sound/Effect/ButtonClick.mp3", typeof(AudioClip)));
            effectAudioClipList.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/FNI/Res/Sound/Effect/EffectSound0.wav", typeof(AudioClip)));
            effectAudioClipList.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/FNI/Res/Sound/Effect/NO.mp3", typeof(AudioClip)));
            effectAudioClipList.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/FNI/Res/Sound/Effect/OK.mp3", typeof(AudioClip)));
#endif
        }


        public void PlaySoundEffect(EffectAudioClip effectAudioClip)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = effectAudioClipList[(int)effectAudioClip];
            audioSource.Play();
        }

        //public void PlaySoundEffect(int index)
        //{
        //    if (audioSource.isPlaying)
        //        audioSource.Stop();

        //    audioSource.clip = effectAudioClipList[index];
        //    audioSource.Play();
        //}

        //public void HoverSoundEffect()
        //{
        //    PlaySoundEffect(EffectAudioClip.ButtonHover);
        //}
    }
}