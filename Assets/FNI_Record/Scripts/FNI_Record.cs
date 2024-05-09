/// 작성자: 백인성
/// 작성일: 2019-10-18
/// 수정일: 2019-10-18
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

/// 사용 순서
/// 1. 녹음 패널을 활성화 하기
///     FNI_Record.Instance.Show(원하는 옵션 설정);
///     
/// 2. 녹음 시작 버튼 활성화 하기(필요에 따라...Show에서 isActiveStart를 true로 했다면 호출하지 않아도 된다.)
///     FNI_Record.Instance.RecordReady(); 
///     
/// 3. 진행상태 체크
///     while (!FNI_Record.Instance.IsRecording)
///         yield return null;
///     while (FNI_Record.Instance.IsRecording)
///         yield return null;
///         
///     [[[[[or]]]]]
///     
///     if (FNI_Record.Instance.IsRecording)
///     {
///     ...
///     }
/// 3-1. 녹음 파일 저장
///     FNI_Record.Instance.SaveClip(string path);
///     
/// 4. 녹음 패널 숨기기(녹음 완전 종료)
///     FNI_Record.Instance.Hide();
///     
/// 4-1. 다시 녹음 시작
///     FNI_Record.Instance.RecordReady(); 
///     
/// 4-2. 저장 및 저장 경로 받기
///     string path = FNI_Record.Instance.SaveClip(string path);


using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Serialization;


namespace FNI
{
    /// <summary>
    /// 음성을 녹음해주는 기능을 합니다.
    /// </summary>
    public class FNI_Record : MonoBehaviour
    {
        #region Singleton
        private static FNI_Record _instance;
        public static FNI_Record Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<FNI_Record>();
                    if (_instance == null)
                        Debug.LogError("FNI_Record를 찾을 수 없습니다. ");
                }
                return _instance;
            }
        }
        #endregion

        #region public property
        /// <summary>
        /// 현재 녹음 중인지
        /// </summary>
        public bool IsRecording { get { return m_state == RecordState.Recoding; } }
        public bool IsPlaying { get { return m_isReplay; } }
        /// <summary>
        /// 녹음이 완료 되었는지
        /// </summary>
        public bool IsRecordingDone { get { return m_state == RecordState.Done; } }
        /// <summary>
        /// 말을 하지 않고 있으면 true로 됩니다.
        /// </summary>
        public bool IsNospeak { get { return m_isNospeak; } }
        /// <summary>
        /// 현재 오디오클립의 길이, 플레이 가능 시간
        /// </summary>
        public float Length { get { return m_audioSource.clip.length; } }
        /// <summary>
        /// 녹음된 오디오 클립의 길이
        /// </summary>
        public int ClipLength { get { return m_audioSource.clip.samples * m_audioSource.clip.channels; } }
        /// <summary>
        /// 녹음된 오디오 클립을 반환합니다.
        /// </summary>
        public AudioClip GetLastClip { get { return m_recordAudioClips[m_recordAudioClips.Count - 1]; } }

        public RecordState State { get { return m_state; } }
        public RecordType Type { get { return m_type; } }
        public int GetRecordCount { get { return m_recordAudioClips.Count; } }
        #endregion

        #region public field
        [Header("Record Option")]
        public AudioMixerGroup replayAudioMixer;
        /// <summary>
        /// 마이크 디바이스 들의 이름
        /// </summary>
        public string[] deviceName;//마이크 이름
        /// <summary>
        /// 녹음되는 기본시간, 모바일의 경우 녹음시간이 짧은 경우 소리가 끊김
        /// </summary>
        public int baseRecordTime = 60;
        /// <summary>
        /// true이면 녹음 정지시 바로 숨겨 지게 됩니다.
        /// </summary>
        public bool isStopAndHide = false;

        /// <summary>
        /// 그로우를 진행될 시간입니다.
        /// </summary>
        /// 
        [Header("UI Option - Grow")]
        public float m_growTime = 1;
        /// <summary>
        /// m_growTime중에 그로우가 퍼져나가기 시작할 시점입니다.
        /// </summary>
        [Range(0f, 1f)]
        public float m_moveStart = 0.25f;

        /// <summary>
        /// 그로우가 퍼져나가기 시작할 크기 입니다.
        /// </summary>
        [Tooltip("Start Grow Size")]
        public Vector2 m_sGrowSize = new Vector2(10, 10);
        /// <summary>
        /// 그로우가 다 퍼져나간 크기입니다.
        /// </summary>
        [Tooltip("End Grow Size")]
        public Vector2 m_eGrowSize = new Vector2(40, 40);

        /// <summary>
        /// 아이콘의 기본 크기입니다.
        /// </summary>
        [Header("UI Option - State")]
        [Tooltip("Start Icon Size")]
        public Vector2 m_sIconSize = new Vector2(99, 117);
        /// <summary>
        /// 녹음 시작시 아이콘이 최종 위치에 도착했을 때의 크기입니다.
        /// </summary>
        [Tooltip("End Icon Size")]
        public Vector2 m_eIconSize = new Vector2(43, 52);
        /// <summary>
        /// 아이콘의 시작 위치 입니다.
        /// </summary>
        [Tooltip("Start Icon Position")]
        public Vector2 m_sIconPos = new Vector2(0, 0);
        /// <summary>
        /// 녹음 시작시 아이콘의 최종 위치 입니다.
        /// </summary>
        [Tooltip("End Icon Position")]
        public Vector2 m_eIconPos = new Vector2(0, 45);
        /// <summary>
        /// true 일 때 말을 하고 있는지 체크합니다. 배경음의 최대 값을 체크 한 후에 체크하는 것으로 정확하지는 않습니다.
        /// </summary>

        [Header("Speek Check Option")]
        public bool useVolumeCheck = false;
        /// <summary>
        /// 말을 안하는 시간을 체크 합니다.
        /// </summary>
        public float checkTime = 5;
        /// <summary>
        /// 스펙트럼을 부스팅하여 줍니다.
        /// </summary>
        public float volumeCheckBooster = 100;
        #endregion

        #region Audio protected / private field
        /// <summary>
        /// 다시 듣기 상태 표시
        /// </summary>
        private bool m_isReplay = false;
        /// <summary>
        /// 녹음에서 사용하는 믹서 그룹
        /// </summary>
        private AudioMixerGroup m_recordAudioMixer;
        /// <summary>
        /// 녹음 진행 상태
        /// </summary>
        private RecordState m_state;

        public RecordState M_State { get => m_state; }

        private ReplayType m_replayType;
        private int m_replayNum;

        /// <summary>
        /// 녹음된 오디오를 재생할 오디오 소스, 묵음처리 되어 있음
        /// </summary>
        private AudioSource m_audioSource;
        /// <summary>
        /// 녹음한 오디오 클립, 완료 시
        /// </summary>
        private AudioClip m_recordAudioClip;
        /// <summary>
        /// 녹음이 가능한 동안 매초마다 쓰는 임시 오디오 벡터
        /// </summary>
        private List<float> m_tempRecording = new List<float>();
        /// <summary>
        /// 녹음된 클립 목록
        /// </summary>
        private List<float> m_recordData = new List<float>();
        /// <summary>
        /// 녹음을 완료한 클립 목록
        /// </summary>
        private List<AudioClip> m_recordAudioClips = new List<AudioClip>();
        /// <summary>
        /// 녹음시 사용되는 코루틴
        /// </summary>
        private IEnumerator m_Record_Routine;
        /// <summary>
        /// 다시 듣기에서 사용되는 코루틴
        /// </summary>
        private IEnumerator m_replay_Routine;

        /// <summary>
        /// 오디오 녹음 샘플레이트
        /// </summary>
        private int m_sampleRate
#if UNITY_EDITOR
    = 44100;
#elif UNITY_ANDROID || UNITY_IOS
    = 24000;
#else
    = 44100;
#endif
        #endregion

        #region UI protected / private field
        /// <summary>
        /// 녹음 방식입니다.
        /// </summary>
        private RecordType m_type = RecordType.None;

        /// <summary>
        /// 시작 버튼입니다.
        /// </summary>
        private Button m_start;

        /// <summary>
        /// 녹음을 시작한 시간입니다.
        /// </summary>
        private DateTime m_startRercordTime;

        /// <summary>
        /// 제한 시간 녹음 시 적용되는 시간입니다 .TimeLimit타입에서 사용합니다. (초)
        /// </summary>
        private float m_recordTime;
        /// <summary>
        /// 녹음을 종료한 시간입니다. TimeLimit타입에서 사용합니다.
        /// </summary>
        private DateTime m_endRercordTime;
        /// <summary>
        /// m_recordTime를 이용한 진행시간 파악용 입니다. TimeLimit타입에서 사용합니다.
        /// </summary>
        private TimeSpan m_countDownTime;

        /// <summary>
        /// 아이콘 애니메이션에 사용되는 코루틴 변수 입니다.
        /// </summary>
        private IEnumerator m_animationRecord_Routine;

        /// <summary>
        /// checkTime동안 말을 하지 않으면 해당 변수가 true가 됩니다.
        /// </summary>
        private bool m_isNospeak = false;
        /// <summary>
        /// 녹음 전 최대 소리를 측정합니다.
        /// </summary>
        public float lowVolume = 0f;
        /// <summary>
        /// 평균값을 내기 위한 최소 볼륨 목록
        /// </summary>
        private List<float> lowVolumes = new List<float>();
        /// <summary>
        /// 말하지 않은 시간 체크 입니다.
        /// </summary>
        private float curUnTalkTime;
        /// <summary>
        /// 스펙트럼의 프레임당 평균 값입니다.
        /// </summary>
        private float averageValue;
        /// <summary>
        /// 볼륨 체크 루틴 변수 입니다.
        /// </summary>
        private IEnumerator m_checkVolume_Routine;
        /// <summary>
        /// 스펙트럼을 가져오기 위한 샘플 갯수입니다.
        /// </summary>
        private int numSamples = 256;
        /// <summary>
        /// 스펙트럼을 가져올 체널 입니다.
        /// </summary>
        private int sampleChannel = 0;
        /// <summary>
        /// FFT Window 타입 입니다.
        /// </summary>
        private FFTWindow windowUsed = FFTWindow.BlackmanHarris;
        private TimeSpan progress;
        public TimeSpan Progress { get => progress; }

        #endregion

        #region Unity base method
        void Start()
        {
            m_audioSource = GetComponent<AudioSource>();

            m_recordAudioMixer = m_audioSource.outputAudioMixerGroup;

            try
            {
                m_start = transform.Find("Start Button").GetComponent<Button>();
                m_start.onClick.AddListener(Start_Record);
                Debug.Log($"m_start Button : <color=yellow>{m_start.gameObject.name}</color>");
            }
            catch (Exception)
            {
                m_start = transform.Find("Record Button").GetComponent<Button>();
                m_start.onClick.AddListener(Start_Record);
                Debug.Log($"m_start Button : <color=yellow>{m_start.gameObject.name}</color>");
            }

            deviceName = Microphone.devices;

            if (useVolumeCheck)
                MicrophoneStart(true);

            m_state = RecordState.Hide;

        }

        void OnApplicationQuit()
        {
            Microphone.End(deviceName[0]);
        }
        private void LateUpdate()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.Q))
            //    Show();

            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    Show(RecordType.TimeLimit, 10, true);
            //}
            //if (Input.GetKeyDown(KeyCode.E))
            //    Show(RecordType.SoftTimeLimit, 10, true);

            //if (Input.GetKeyDown(KeyCode.R))
            //    Show(RecordType.Replay, true);

            //if (Input.GetKeyDown(KeyCode.A))
            //    Hide();
#endif


            if (useVolumeCheck)
            {
                if (m_state == RecordState.Hide)
                {
                    float getValue = CheckSound();
                    lowVolumes.Add(getValue);
                    if (100 < lowVolumes.Count)
                        lowVolumes.RemoveAt(0);
                    float value = 0;
                    for (int cnt = 0; cnt < lowVolumes.Count; cnt++)
                    {
                        value += lowVolumes[cnt];
                    }
                    value = value / lowVolumes.Count;

                    if (lowVolume < value)
                        lowVolume = value;
                }

                if (m_state == RecordState.Recoding)
                {
                    averageValue = CheckSound();
                }
            }
        }
        #endregion

        #region public method
        /// <summary>
        /// 녹음 패널을 보여줍니다.
        /// </summary>
        /// <param name="type">녹음 타입</param>
        /// <param name="second">녹음이 진행될 시간, TimeLimit종류 일 때만 적용 됨</param>
        /// <param name="isActiveStart">보여지면서 바로 시작버튼을 활성화 할 지</param>
        /// <param name="isStopAndHide">녹음 종료 버튼을 누르면 녹음 패널을 숨길지</param>
        public void Show(RecordType type = RecordType.Default, float second = 0, bool isActiveStart = false, bool isStopAndHide = false)
        {
            //타입 설정
            this.m_type = type;
            m_state = RecordState.Wait;
            this.isStopAndHide = isStopAndHide;

            switch (type)//타입에 따른 버튼 및 시간 설정
            {
                case RecordType.Default://시작 버튼 활성화, 종료버튼 숨김, 게이지 숨김
                    RecordReady(type, isActiveStart);

                    //m_time.gameObject.SetActive(false);
                    m_audioSource.outputAudioMixerGroup = m_recordAudioMixer;

                    break;
                case RecordType.TimeLimit://시작 버튼 활성화, 종료 버튼 숨김, 게이지 활성화
                    RecordReady(type, isActiveStart);
                    m_audioSource.outputAudioMixerGroup = m_recordAudioMixer;


                    m_recordTime = second;
                    break;
                case RecordType.SoftTimeLimit://시작 버튼 활성화, 종료 버튼 숨김, 게이지 활성화
                    RecordReady(type, isActiveStart);
                    m_audioSource.outputAudioMixerGroup = m_recordAudioMixer;


                    m_recordTime = second;
                    break;
                case RecordType.AutoStart://시작 버튼 숨김, 게이지 활성화, 녹음 바로 시작
                    m_start.gameObject.SetActive(false);
                    m_start.interactable = false;

                    m_audioSource.outputAudioMixerGroup = m_recordAudioMixer;

                    Start_Record();
                    break;
                case RecordType.Replay:
                    RecordReady(type, isActiveStart);


                    m_audioSource.outputAudioMixerGroup = replayAudioMixer;
                    break;
            }

            //m_time.color = new Color(1, 1, 1, 0);

            if (Microphone.IsRecording(deviceName[0]))
                Microphone.End(deviceName[0]);

        }
        public void Show(RecordType type = RecordType.Default, float second = 0, bool isActiveStart = false, bool isStopAndHide = false, ReplayType replayType = ReplayType.First)
        {
            Show(type, second, isActiveStart, isStopAndHide);
            m_replayType = replayType;
        }
        /// <summary>
        /// 기본 녹음 패널을 보여줍니다.
        /// </summary>
        public void Show()
        {
            Show(RecordType.Default, 0, false, false);
        }
        /// <summary>
        /// 녹음 패널을 보여줍니다.
        /// </summary>
        /// <param name="type">녹음 타입</param>
        public void Show(RecordType type)
        {
            Show(type, 0, false, false);
        }
        /// <summary>
        /// 녹음 패널을 보여줍니다.
        /// </summary>
        /// <param name="type">녹음 타입</param>
        public void Show(ReplayType type, int replayNum = -1)
        {
            m_replayNum = replayNum;
            m_replayType = type;
            Show(RecordType.Replay, 0, false, false);
        }
        /// <summary>
        /// 녹음 패널을 보여줍니다.
        /// </summary>
        /// <param name="type">녹음 타입</param>
        /// <param name="isActiveStart">보여지면서 바로 시작버튼을 활성화 할 지</param>
        public void Show(RecordType type, bool isActiveStart)
        {
            Show(type, 0, isActiveStart, false);
        }

        /// <summary>
        /// 녹음 패널을 숨깁니다. 녹음 중이였다면 녹음을 종료 합니다.
        /// </summary>
        /// <returns>녹음하면서 생성된 오디오 클립을 반환합니다.</returns>
        public void Hide()
        {
            if (IsRecording)
                Stop_Record();

            m_type = RecordType.None;
            m_state = RecordState.Hide;

        }
        /// <summary>
        /// 녹음을 시작 할 준비 합니다.
        /// </summary>
        /// <param name="interactable">버튼 활성화 여부</param>
        public void RecordReady()
        {
            RecordReady(RecordType.Default);
        }
        /// <summary>
        /// 녹음을 시작 할 준비 합니다.
        /// </summary>
        /// <param name="type">녹음 형태</param>
        /// <param name="interactable">버튼 활성화 여부</param>
        public void RecordReady(RecordType type = RecordType.Default, bool interactable = true)
        {
            //m_start.gameObject.SetActive(type != RecordType.Replay);

            //if (type != RecordType.Replay)
            //    m_start.interactable = interactable;
            //else
            //    m_start.interactable = false;

            //m_start.gameObject.SetActive(true);

        }

        public void ReplayShow()
        {

        }
        public string SaveClip(string path)
        {
            string _path = path + "/" + m_audioSource.clip.name;
            SavWav.Save(_path, m_audioSource.clip);

            return _path;
        }
        private void Replay()
        {
            switch (m_replayType)
            {
                case ReplayType.First:
                    FirstReplay();
                    break;
                case ReplayType.Last:
                    LastReplay();
                    break;
                case ReplayType.Custom:
                    Replay(m_replayNum);
                    break;
            }
        }
        /// <summary>
        /// 마지막에 녹음된 음성을 재생합니다.
        /// </summary>
        public void LastReplay()
        {
            Replay(m_recordAudioClips.Count - 1);
        }
        /// <summary>
        /// 첫 녹음 파일을 재생합니다.
        /// </summary>
        public void FirstReplay()
        {
            Replay(0);
        }
        public void StopReplay()
        {
            if (m_replay_Routine != null)
                StopCoroutine(m_replay_Routine);
            m_replay_Routine = null;

            m_audioSource.Stop();
        }
        /// <summary>
        /// 녹음 파일을 재생합니다.
        /// </summary>
        /// <param name="clipNum">재생할 파일 번호</param>
        public void Replay(int clipNum)
        {
            if (m_recordAudioClips.Count < 0 || m_recordAudioClips.Count - 1 < clipNum)
            {
                Debug.Log("<color=red>Don't have record Data</color>");
                return;
            }

            if (Microphone.IsRecording(deviceName[0]))
                Microphone.End(deviceName[0]);

            m_audioSource.clip = m_recordAudioClips[clipNum];
            m_audioSource.Play();

            //m_replay.gameObject.SetActive(false);

            if (m_replay_Routine != null)
                StopCoroutine(m_replay_Routine);
            m_replay_Routine = Replay_Routine();
            StartCoroutine(m_replay_Routine);
        }
        #endregion

        #region protected / private method
        /// <summary>
        /// 녹음을 시작합니다.
        /// </summary>
        public void Start_Record()//녹음 시작
        {
            Debug.Log("녹음 시작");
            //초기화
            m_state = RecordState.Recoding;

            //m_start.gameObject.SetActive(false);

            m_startRercordTime = DateTime.Now;

            //if (m_animationRecord_Routine != null)
            //    StopCoroutine(m_animationRecord_Routine);
            //m_animationRecord_Routine = AnimationRecord_Routine(true);
            //StartCoroutine(m_animationRecord_Routine);

            //if (m_type == RecordType.SoftTimeLimit || m_type == RecordType.TimeLimit)
            //{
            //    m_endRercordTime = DateTime.Now.AddSeconds(m_recordTime);
            //    m_countDownTime = m_endRercordTime - m_startRercordTime;
            //}

            m_recordAudioClip = null;

            if (m_audioSource.isPlaying)
                m_audioSource.Stop();

            m_tempRecording.Clear();

            MicrophoneStart(true);

            //if (useVolumeCheck)
            //{
            //    if (m_checkVolume_Routine != null)
            //        StopCoroutine(m_checkVolume_Routine);
            //    m_checkVolume_Routine = CheckVolume_Routine();
            //    StartCoroutine(m_checkVolume_Routine);
            //}

            //녹음 되는 길이를 변경하는 루틴입니다.
            if (m_Record_Routine != null)
                StopCoroutine(m_Record_Routine);
            m_Record_Routine = Record_Routine();
            StartCoroutine(m_Record_Routine);
        }
        public void Stop_Record(string fileName)
        {
            m_audioSource.clip.name = fileName;
            Stop_Record();
        }
        /// <summary>
        /// 녹음을 중단 합니다.
        /// </summary>
        /// <returns></returns>
        public void Stop_Record()
        {
            m_state = RecordState.Done;

            //코루틴 종료
            if (useVolumeCheck)
            {
                if (m_checkVolume_Routine != null)
                    StopCoroutine(m_checkVolume_Routine);
                m_checkVolume_Routine = null;
            }

            if (m_Record_Routine != null)
                StopCoroutine(m_Record_Routine);
            m_Record_Routine = null;

            //if (m_animationRecord_Routine != null)
            //    StopCoroutine(m_animationRecord_Routine);
            //m_animationRecord_Routine = AnimationRecord_Routine(false);
            //StartCoroutine(m_animationRecord_Routine);

            //마이크로폰 기능 중지
            if (Microphone.IsRecording(deviceName[0]))
                Microphone.End(deviceName[0]);

            //데이터 복사 시작
            float[] samples = new float[ClipLength];
            m_audioSource.clip.GetData(samples, 0);
            //임시 녹음을 저장할 수 있는 충분한 공간을 가진 더 큰 벡터를 만들고 현재 녹음의 마지막 섹션을 만듭니다.
            //"tempRecording"에 기록 된 데이터를 "samples"와 더해서 "fullClip"에 기록
            m_recordData = new List<float>();
            m_recordData.AddRange(m_tempRecording);
            m_recordData.AddRange(RemoveZeroData(samples));
            //비어있는 데이터를 삭제하기 위해서 0값을 찾아 없애줍니다. 안하면 이어붙인 곳이 끊김
            //fullClip.RemoveAll(x => x == 0);

            m_audioSource.clip = AudioClip.Create(m_audioSource.clip.name, m_recordData.Count, 1, m_sampleRate, false);
            m_audioSource.clip.SetData(m_recordData.ToArray(), 0);
            m_audioSource.loop = false;


            m_recordAudioClips.Add(m_audioSource.clip);

            //if (isStopAndHide)
            //    Hide();

        }

        /// <summary>
        /// 녹음상태 갱신 및 녹음 길이 변경
        /// </summary>
        /// <returns></returns>
        private IEnumerator Record_Routine()
        {
            while (IsRecording)
            {
                ResizeRecording();

                yield return new WaitForSeconds(baseRecordTime);
            }
        }
        /// <summary>
        /// 임시 벡터에 녹음 된 오디오의 다음 초 추가
        /// </summary>
        private void ResizeRecording()
        {
            //셈플의 길이와 채널을 곱해서 데이터의 길이를 맞춰워야 함
            float[] clipData = new float[ClipLength];

            //데이터를 clipdata에 넣어 줌
            m_audioSource.clip.GetData(clipData, 0);
            //0값 제외 하기

            //넣은 데이터를 tempRecording에 더해 줌
            m_tempRecording.AddRange(RemoveZeroData(clipData));

            //tempRecording.RemoveAll(x => x == 0);

            //새 오디오 클림 넣어 줌
            MicrophoneStart(false);
        }
        /// <summary>
        /// 플롯 배열에서 0값을 제외한 새로운 배열을 반환합니다.
        /// </summary>
        /// <param name="clipData"></param>
        /// <returns></returns>
        private float[] RemoveZeroData(float[] clipData)
        {
            List<float> nonZero = new List<float>();
            nonZero.AddRange(clipData);
            nonZero.RemoveAll(x => x == 0);

            return nonZero.ToArray();
        }
        /// <summary>
        /// 실제 녹음 시작
        /// </summary>
        /// <param name="isStart"></param>
        private void MicrophoneStart(bool isStart)
        {
            //오디오 클립 초기화
            if (isStart && Microphone.IsRecording(deviceName[0]))
                Microphone.End(deviceName[0]);

            //녹음 시작 (마이크 이름, 루프여부, 저장 길이, 샘플레이트)
            m_audioSource.clip = Microphone.Start(deviceName[0], true, baseRecordTime, m_sampleRate);
            while (!(Microphone.GetPosition(null) > 0)) { }
            m_audioSource.loop = true;
            //m_audioSource.clip.name = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            m_audioSource.Play();
        }

        /// <summary>
        /// 녹음 시작 버튼을 누르면 실행되는 애니메이션
        /// </summary>
        /// <param name="isStart">녹음 시작은 True 종료는 False</param>
        /// <returns></returns>
        private IEnumerator AnimationRecord_Routine(bool isStart)
        {
            //초기값 설정
            float sTime = 0;
            float eTime = 0.5f;

            //중간 애니메이션
            while (sTime < eTime)
            {
                sTime += Time.deltaTime;

                float percent = sTime / eTime;

                yield return null;
            }

            //else
            //{
            //    m_time.color = new Color(1, 1, 1, 0);
            //    m_time.gameObject.SetActive(false);
            //}


            //m_time.color = isStart ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
        }



        /// <summary>
        /// 녹음 시간을 표시 합니다.
        /// </summary>
        private void SetTimeText()
        {
            if (IsRecording)//녹음 중 일 때
            {

                if (m_type == RecordType.TimeLimit || m_type == RecordType.SoftTimeLimit)//시간 제한의 경우, 지정 시간 -> 00:00
                {
                    progress = m_countDownTime - (DateTime.Now - m_startRercordTime);

                    if (progress < new TimeSpan())//제한 시간이 넘으면 종료를 시켜 준다.
                        Stop_Record();
                }
                else//정상 진행 시간
                {
                    progress = DateTime.Now - m_startRercordTime;
                }


            }

        }

        /// <summary>
        /// 말을 하고 있는지 볼륨을 체크 합니다.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckVolume_Routine()
        {
            bool check1 = true;
            bool check2 = true;

            while (check1 && check2)
            {
                check1 = averageValue < lowVolume;
                check2 = curUnTalkTime < checkTime;

                curUnTalkTime += Time.deltaTime;

                yield return null;
            }

            if (check1 == true && check2 == false)
            {
                m_isNospeak = true;
                Stop_Record();
            }
        }

        /// <summary>
        /// 스펙트럼을 분석해 현재 말하고 있는 크기를 분석합니다.
        /// </summary>
        /// <returns></returns>
        private float CheckSound()
        {
            float[] spectrum = new float[numSamples];
            m_audioSource.GetSpectrumData(spectrum, sampleChannel, windowUsed);

            float value = 0;
            for (int cnt = 0; cnt < numSamples; cnt++)
            {
                value += spectrum[cnt];
            }
            value = (value / numSamples) * volumeCheckBooster;

            return value;
        }

        /// <summary>
        /// 녹음 파일 재생 후 자동 종료 합니다.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Replay_Routine()
        {
            float cTime = 0;
            float eTime = m_audioSource.clip.length;

            m_isReplay = true;
            while (cTime < eTime)
            {
                cTime += Time.deltaTime;
                //m_amount.fillAmount = cTime / eTime;
                yield return null;
            }

            yield return new WaitForSeconds(1);

            //Hide();
            m_isReplay = false;
        }
        #endregion
    }

    /// <summary>
    /// 녹음 방식입니다.
    /// </summary>
    public enum RecordType
    {
        None,
        /// <summary>
        /// 녹음 시작 -> 종료 버튼이 있는 기본 타입
        /// </summary>
        Default,
        /// <summary>
        /// 디폴트 타입에 제한시간동안 무조건 녹음 해야 하는 타입(종료 버튼 없음)
        /// </summary>
        TimeLimit,
        /// <summary>
        /// 디폴트 타입에 제한시간동안 녹음 해야 하는 타입(중간 종료 가능)
        /// </summary>
        SoftTimeLimit,
        /// <summary>
        /// 녹음이 자동 시작되고 종료버튼만 있습니다.
        /// </summary>
        AutoStart,
        /// <summary>
        /// 마지막으로 녹음한 파일을 듣습니다.
        /// </summary>
        Replay
    }

    /// <summary>
    /// 녹음 상태
    /// </summary>
    public enum RecordState
    {
        /// <summary>
        /// 비활성화
        /// </summary>
        Hide,
        /// <summary>
        /// 활성화, 녹음 대기
        /// </summary>
        Wait,
        /// <summary>
        /// 활성화, 녹음 중
        /// </summary>
        Recoding,
        /// <summary>
        /// 활성화, 녹음 종료
        /// </summary>
        Done
    }
    /// <summary>
    /// 리플레이 종류 입니다.
    /// </summary>
    public enum ReplayType
    {
        /// <summary>
        /// 첫번째 녹음파일을 재생합니다.
        /// </summary>
        First,
        /// <summary>
        /// 마지막 파일을 재생합니다.
        /// </summary>
        Last,
        /// <summary>
        /// 지정한 파일을 재생합니다.
        /// </summary>
        Custom
    }

    /// <summary>
    /// AudioClip을 Wave파일로 저장합니다.
    /// </summary>
    public static class SavWav
    {
        const int HEADER_SIZE = 44;

        /// <summary>
        /// 파일 저장
        /// </summary>
        /// <param name="filename">저장 경로, 확장자 제외</param>
        /// <param name="clip">변경할 오디오 클림</param>
        /// <returns></returns>
        public static bool Save(string filename, AudioClip clip)
        {
            if (clip != null)
            {
                try
                {
                    if (!filename.ToLower().EndsWith(".mp3"))
                    {
                        filename += ".mp3";
                    }

                    //string filepath = Path.Combine(Application.persistentDataPath, filename);

                    // Make sure directory exists if user is saving to sub dir.
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));

                    using (FileStream fileStream = CreateEmpty(filename))
                    {
                        ConvertAndWrite(fileStream, clip);

                        WriteHeader(fileStream, clip);
                    }
                    return true; // TODO: return false if there's a failure saving the file
                }
                catch (UnityException e)
                {
                    Debug.Log(e.Message);
                    return false;
                }
            }
            else
            {
                Debug.Log("AudioClip is Null");
                return false;
            }
        }
        public static AudioClip TrimSilence(AudioClip clip, float min)
        {
            float[] samples = new float[clip.samples];

            clip.GetData(samples, 0);

            return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
        }

        public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz)
        {
            return TrimSilence(samples, min, channels, hz, false);
        }

        public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool stream)
        {
            int i;

            for (i = 0; i < samples.Count; i++)
            {
                if (Mathf.Abs(samples[i]) > min)
                {
                    break;
                }
            }

            samples.RemoveRange(0, i);

            for (i = samples.Count - 1; i > 0; i--)
            {
                if (Mathf.Abs(samples[i]) > min)
                {
                    break;
                }
            }

            samples.RemoveRange(i, samples.Count - i);

            AudioClip clip = AudioClip.Create("TempClip", samples.Count, channels, hz, stream);

            clip.SetData(samples.ToArray(), 0);

            return clip;
        }

        static FileStream CreateEmpty(string filepath)
        {
            FileStream fileStream = new FileStream(filepath, FileMode.Create);
            byte emptyByte = new byte();

            for (int i = 0; i < HEADER_SIZE; i++) //preparing the header
            {
                fileStream.WriteByte(emptyByte);
            }

            return fileStream;
        }

        static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
        {

            float[] samples = new float[clip.samples];

            clip.GetData(samples, 0);

            Int16[] intData = new Int16[samples.Length];
            //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

            Byte[] bytesData = new Byte[samples.Length * 2];
            //bytesData array is twice the size of
            //dataSource array because a float converted in Int16 is 2 bytes.

            int rescaleFactor = 32767; //to convert float to Int16

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * rescaleFactor);
                Byte[] byteArr = new Byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            fileStream.Write(bytesData, 0, bytesData.Length);
        }

        static void WriteHeader(FileStream fileStream, AudioClip clip)
        {
            int hz = clip.frequency;
            int channels = clip.channels;
            int samples = clip.samples;

            fileStream.Seek(0, SeekOrigin.Begin);

            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            fileStream.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
            fileStream.Write(chunkSize, 0, 4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            fileStream.Write(wave, 0, 4);

            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            fileStream.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);
            fileStream.Write(subChunk1, 0, 4);

            UInt16 two = 2;
            UInt16 one = 1;

            Byte[] audioFormat = BitConverter.GetBytes(one);
            fileStream.Write(audioFormat, 0, 2);

            Byte[] numChannels = BitConverter.GetBytes(channels);
            fileStream.Write(numChannels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(hz);
            fileStream.Write(sampleRate, 0, 4);

            Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
            fileStream.Write(byteRate, 0, 4);

            UInt16 blockAlign = (ushort)(channels * 2);
            fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            UInt16 bps = 16;
            Byte[] bitsPerSample = BitConverter.GetBytes(bps);
            fileStream.Write(bitsPerSample, 0, 2);

            Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
            fileStream.Write(datastring, 0, 4);

            Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
            fileStream.Write(subChunk2, 0, 4);

            //		fileStream.Close();
        }
    }
}
