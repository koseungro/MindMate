/// 작성자: 고승로
/// 작성일: 2021-03-12
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
using UnityEngine.EventSystems;

namespace FNI
{
    public class SliderManager : MonoBehaviour
    {
        private static SliderManager _instance;
        public static SliderManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SliderManager>();
                }
                return _instance;
            }
        }

        [SerializeField] private Slider slider;
        [SerializeField] private Button OKButton;
        [SerializeField] private Text handleText;
        [SerializeField] private AudioClip checkAudio = null;

        private AudioSource audioSource = null;

        private void OnEnable()
        {
            Init();
            slider.interactable = true;
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Init()
        {
            slider.value = 3;
        }

        public void AssessmentComplete()
        {
            Score score = new Score();

            score.inputTime = DateTime.Now;
            score.ID = Main.curSceneID;
            score.score = slider.value;
            score.text = handleText.text;

            score.isWritten = false;

            //Debug.Log("Scene ID : " + score.ID);
            //Debug.Log("Score : " + score.score);
            DBManager.Instance.AddScore(score);
            DBManager.Instance.ScoreCheckSceneID();
        }

        public void ScoreChanged()
        {
            if (slider.interactable)
            {
                audioSource.clip = checkAudio;
                audioSource.Play();

            }

            //if (!OKButton.interactable)
            //{
            //    OKButton.interactable = true;
            //}
        }

    }
}