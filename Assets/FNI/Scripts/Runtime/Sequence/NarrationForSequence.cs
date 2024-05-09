/// 작성자: 고승로
/// 작성일: 2020-08-31
/// 수정일: 2020-09-04
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNI
{
    /// <summary>
    /// 나레이션 관련 씬 데이터를 처리하는 클래스
    /// </summary>
    public class NarrationForSequence : MonoBehaviour, IVisualObject
    {
        #region Singleton
        private static NarrationForSequence _instance;
        public static NarrationForSequence Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<NarrationForSequence>();

                return _instance;
            }
        }
        #endregion

        public VisualType Type { get => VisualType.Narration; }

        public List<AudioSource> AudioList { get => audioList; }
        /// <summary>
        /// [0] : Manager, [1] : Player, [2] : BackgroundSound
        /// </summary>
        private List<AudioSource> audioList = new List<AudioSource>();
        private List<AudioSource> ambienceAudioList = new List<AudioSource>();
        public AudioSource curAudio;

        [SerializeField] private AudioSource backgroundSound;

        bool IVisualObject.IsFinish => isFinish;
        private bool isFinish = false;

        // 나레이션이 끝날때까지 시간 체크
        private float timer = 0.0f;
        // 나레이션의 길이만큼 isFinish가 false 하도록 하는 변수
        private float waitTime = 0.0f;

        /// <summary>
        /// Background 를 처음 Play 하기로 설정했던 볼륨 크기
        /// </summary>
        private float originalBackgroundVolume = 0f;

        /// <summary>
        /// 나레이션 동안 작아질 배경음 Volume 크기
        /// </summary>
        [Range(0.1f, 0.5f)] public float backgroundDownVolume = 0.15f;
        /// <summary>
        /// 배경음 소리 Lerp 조절 시간
        /// </summary>
        [Range(1f, 5f)] public float backgroundDownTime = 3f;


        /// <summary>
        /// 나레이션 재생 및 해당 컷씬에 필요한 데이터 초기화
        /// </summary>
        /// <param name="option"></param>
        void IVisualObject.Active(CutData option)
        {
            isFinish = false;

            switch (option.narrationOption.soundType)
            {
                case Sound_FuncType.PlaySound:
                    PlayAudio(option.narrationOption);
                    break;
                case Sound_FuncType.SetVolume:
                    SetVolume(option.narrationOption);
                    break;
                case Sound_FuncType.PlaySubtitle:
                    PlaySubtitleSound(option.narrationOption);
                    break;

            }
        }

        void IVisualObject.Init()
        {
            audioList.Add(GetComponent<AudioSource>()); // Manager - Narration
            audioList.Add(Camera.main.GetComponent<AudioSource>()); // Player
            audioList.Add(backgroundSound); // BackgroundSound
        }

        /// <summary>
        /// 나레이션 시간동안 Update , 캐릭터 애니메이션 변경 데이터 있다면 변경
        /// </summary>
        void IVisualObject.MyUpdate()
        {
            timer += Time.deltaTime;

            if (timer > waitTime)
            {
                timer = 0.0f;
                //curAudio.clip = null;
                isFinish = true;
            }
        }

        public void CheckIsFinish(bool finish)
        {
            isFinish = finish;
        }

        /// <summary>
        /// Narration/ Audio 조절 관련 메소드 
        /// </summary>
        #region Play Narration
        private void PlayAudio(NarrationOption option)
        {
            //if (curAudio != null && curAudio.isPlaying)
            //    curAudio.Stop();

            curAudio = audioList[(int)option.narrationType];

            curAudio.loop = option.loop;
            curAudio.volume = option.volume;

            if (option.clip == null)
            {
                isFinish = true;
                curAudio.clip = null;
                return;
            }

            curAudio.clip = option.clip;
            curAudio.Play();

            if (option.narrationType == NarrationType.BackgroundSound)
            {
                originalBackgroundVolume = option.volume;
                curAudio.volume = 0;

                StartCoroutine(ControlVolume(curAudio, option.volume, 4f)); // 다음 컷에 충분한 Wait 시간 있어야 소리 끝까지 커짐
            }
            else if (option.narrationType == NarrationType.Manager)
            {
                if (audioList[2].isPlaying && audioList[2].volume != 0)
                {
                    if (!option.notRestoreBackground) // Manager 나레이션이 끝나고 배경음 소리 Fade 회복
                    {
                        if (backgroundRestoreRotine != null)
                            StopCoroutine(backgroundRestoreRotine);
                        backgroundRestoreRotine = BackgroundRestore(originalBackgroundVolume);

                        StartCoroutine(backgroundRestoreRotine); // 배경음 원래대로 회복
                        //StartCoroutine(BackgroundRestore(audioList[2].volume)); 
                    }

                    if (audioList[2].volume == originalBackgroundVolume)
                        StartCoroutine(ControlVolume(audioList[2], backgroundDownVolume, backgroundDownTime)); // 나레이션 나올때는 배경음 소리 줄이기
                }
            }

            timer = 0.0f;

            // 나레이션과 다른 동작 동시 진행 가능하도록
            if (option.isSameTime)
                waitTime = 0;
            else
                waitTime = curAudio.clip.length;
        }

        IEnumerator backgroundRestoreRotine;

        /// <summary>
        /// Manager 나레이션이 나오는동안 줄어든 Background 사운드를 원래대로 되돌리기
        /// </summary>
        /// <param name="originalVolume"> 원래 볼륨 크기 </param>
        /// <returns></returns>
        private IEnumerator BackgroundRestore(float originalVolume)
        {
            while (audioList[0].isPlaying || audioList[2].volume > backgroundDownVolume)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan> [BackgroundRestore/NarrationForSequence]</color> => [{audioList[0].clip}] 나레이션 종료 => Background Restore to [{originalVolume}] Volume");

            StartCoroutine(ControlVolume(audioList[2], originalVolume, backgroundDownTime)); // 줄어든 배경음 소리 다시 키우기

        }
        #endregion
        #region Volume Controll
        private void SetVolume(NarrationOption option)
        {
            if (option.narrationType != NarrationType.Ambience)
            {
                curAudio = audioList[(int)option.narrationType];

                if (!curAudio.isPlaying)
                    Debug.Log($"<color=red> [{curAudio}]가 현재 재생중이 아니므로 볼륨을 조절할 수 없습니다.</color>");
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(ControlVolume(curAudio, option.volume, option.volumeFadeTime));
                }
            }
            else // 씬 내의 Ambience 사운드 모두 찾아서 Fade
            {
                GameObject sound = GameObject.Find("Sound");
                if (sound != null)
                {
                    AudioSource[] ambienceAudios = sound.GetComponentsInChildren<AudioSource>();

                    ambienceAudioList.Clear();
                    ambienceAudioList.AddRange(ambienceAudios);

                    for (int i = 0; i < ambienceAudioList.Count; i++)
                    {
                        StartCoroutine(ControlAmbienceVolume(ambienceAudioList[i], option.volume, option.volumeFadeTime));
                    }
                }
                else
                    Debug.Log($"<color=red> Sound GameObject를 찾지 못하였습니다.</color>");
            }

            isFinish = true; // 볼륨 Fade는 바로 다음 컷 동시진행 가능하도록
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="time">다음 컷에 충분한 Wait 시간 있어야 소리 끝까지 변경 됨</param>
        /// <returns></returns>
        private IEnumerator ControlVolume(AudioSource audio, float volume, float time)
        {
            //Debug.Log($"<color=yellow> [ControlVolume/NarrationForSequence]</color> => [{audio}]/ [{audio.clip}] 볼륨 Fade => {volume}/ Tiem : {time}");

            float targetVolume;

            if (volume > 1)
                targetVolume = 1;
            else
                targetVolume = volume;

            float checkTime = 0;
            float startVolume = audio.volume;

            while (checkTime < 1.0f)
            {
                checkTime += Time.deltaTime / time;
                audio.volume = Mathf.Lerp(startVolume, targetVolume, checkTime);
                yield return null;
            }

            audio.volume = targetVolume;
            if (audio.volume == 0)
                audio.Stop();
            //isFinish = true;

        }

        private IEnumerator ControlAmbienceVolume(AudioSource audio, float volume, float time)
        {
            //Debug.Log($"Ambience [{audio.clip}] 볼륨 Fade => {volume}/ Tiem : {time}");

            float targetVolume;

            if (volume > 1)
                targetVolume = 1;
            else
                targetVolume = volume;

            float checkTime = 0;
            float startVolume = audio.volume;

            while (checkTime < 1.0f)
            {
                checkTime += Time.deltaTime / time;
                audio.volume = Mathf.Lerp(startVolume, targetVolume, checkTime);
                yield return null;
            }

            audio.volume = targetVolume;
            if (audio.volume == 0)
                audio.Stop();
            //isFinish = true;

        }

        public IEnumerator FadeBackgroundVolume(GameObject target, float fadeTime)
        {
            Debug.Log("FadeBackgroundVolume");

            AudioSource[] backgroundAudioList = target.GetComponents<AudioSource>();

            float checkTime = 0;
            float volume = 0;
            List<float> startVolumes = new List<float>();

            for (int i = 0; i < backgroundAudioList.Length; i++)
            {
                startVolumes.Add(backgroundAudioList[i].volume);
            }

            while (checkTime < 1.0f)
            {
                checkTime += Time.deltaTime / fadeTime;

                for (int i = 0; i < backgroundAudioList.Length; i++)
                {
                    volume = Mathf.Lerp(startVolumes[i], 0, checkTime);
                    backgroundAudioList[i].volume = volume;
                    //Debug.Log($"{checkTime}/ {backgroundAudioList[i].clip}/ {backgroundAudioList[i].volume}");
                }

                yield return null;
            }

            for (int i = 0; i < backgroundAudioList.Length; i++)
            {
                backgroundAudioList[i].volume = 0;
                Debug.Log($"{backgroundAudioList[i].name}/{backgroundAudioList[i].volume}");
            }

        }
        #endregion
        #region Subtitle
        private void PlaySubtitleSound(NarrationOption option)
        {
            //isFinish = true;

            curAudio = audioList[0]; // Manager
            curAudio.volume = option.volume;

            // curaudio clip 설정 및 Play
            SubtitleManager.Instance.SetSubtitle(option);

            waitTime = 9999999999;
            //if (audioList[2].isPlaying && audioList[2].volume != 0)
            //{
            //    if (!option.notRestoreBackground)
            //    {
            //        if (backgroundRestoreRotine != null)
            //            StopCoroutine(backgroundRestoreRotine);
            //        backgroundRestoreRotine = BackgroundRestore(originalBackgroundVolume);

            //        StartCoroutine(backgroundRestoreRotine); // 배경음 원래대로 회복 메소드 걸어놓기
            //                                                 //StartCoroutine(BackgroundRestore(audioList[2].volume)); 
            //    }

            //    if (audioList[2].volume == originalBackgroundVolume)
            //        StartCoroutine(ControlVolume(audioList[2], backgroundDownVolume, backgroundDownTime)); // 나레이션 나올때는 배경음 소리 줄이기
            //}
        }
        #endregion

        #region HMD
        public void HMDPlay()
        {
            if (curAudio != null && curAudio.clip != null)
                curAudio.Play();
        }

        public void HMDPause()
        {
            if (curAudio != null)
                curAudio.Pause();
        }

        public void HMDStop()
        {
            curAudio.Stop();
            curAudio.clip = null;
        }
        #endregion

    }
}