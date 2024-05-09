using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FNI;
using System;

public class SubtitleManager : MonoBehaviour
{
    #region Singleton
    private static SubtitleManager _instance;
    public static SubtitleManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SubtitleManager>();

            return _instance;
        }
    }
    #endregion

    private Subtitle_Main subtitle_Main;

    [SerializeField] private AudioSubtitleType curAudioSubtitleType = AudioSubtitleType.None;
    [SerializeField] private VideoSubtitleType curVideoSubtitleType = VideoSubtitleType.None;

    public AudioSource audioSource;

    public GameObject subtitleBox;
    private Text SubtitleText
    {
        get
        {
            if (subtitleText == null)
                subtitleText = subtitleBox.transform.Find("Text").GetComponent<Text>();

            return subtitleText;
        }
    }
    public Text subtitleText;

    [Header("[Intro----------]")]
    public AudioClip[] audioClips_00;
    [Header("[Prologue_00----------]")]
    public AudioClip[] audioClips_01;
    [Header("[Prologue_01----------]")]
    public AudioClip[] audioClips_02;
    [Header("[Home----------]")]
    public AudioClip[] audioClips_03;
    public AudioClip[] audioClips_04;
    public AudioClip[] audioClips_05;
    public AudioClip[] audioClips_06;

    /// <summary>
    /// 자막 사용하는 시나리오의 오디오 목록들 리스트
    /// </summary>
    public List<AudioClip[]> audioClipsList = new List<AudioClip[]>();
    /// <summary>
    /// 자막에 들어갈 String 목록들 리스트
    /// </summary>
    public List<string[]> audioSubtitlesList = new List<string[]>();
    public List<string[]> videoSubtitlesList = new List<string[]>();

    /// <summary>
    /// audioClipList, subtitlesList 콘텐츠 넘버
    /// </summary>
    private int contentNum = 0;

    /// <summary>
    /// 자막 번호
    /// </summary>
    private int subtitleNum = 0;

    /// <summary>
    /// Subtitle Check Point에 도달했을때 Event가 수행된 후 다음 Subtitle로 넘어가도 되는지 체크
    /// </summary>
    public bool checkPoint_End = false;

    public bool NowWaitingEvent { get => nowWaitingEvent; }

    /// <summary>
    /// 현재 자막 Check Point Event가 실행되어 [checkPoint_End]가 true가 되기를 대기 중인지 체크
    /// </summary>
    private bool nowWaitingEvent = false;

    private void Awake()
    {
        subtitle_Main = GetComponent<Subtitle_Main>();
    }

    void Start()
    {
        AddList();
    }

    private void AddList()
    {
        audioClipsList.Add(audioClips_00); // Intro
        audioClipsList.Add(audioClips_01); // Prologue_00
        audioClipsList.Add(audioClips_02); // Prologue_01
        audioClipsList.Add(audioClips_03); // Home
        audioClipsList.Add(audioClips_04); // 
        audioClipsList.Add(audioClips_05); // 
        audioClipsList.Add(audioClips_06); // 

        audioSubtitlesList.Add(audioSubtitles_00); // Intro
        audioSubtitlesList.Add(audioSubtitles_01); // Prologue_00
        audioSubtitlesList.Add(audioSubtitles_02); // Prologue_01
        audioSubtitlesList.Add(audioSubtitles_03); // Home

        videoSubtitlesList.Add(videoSubtitles_01); // 
        videoSubtitlesList.Add(videoSubtitles_02); // 
        videoSubtitlesList.Add(videoSubtitles_03); // 

    }

    public void PassCheckPoint()
    {
        checkPoint_End = true;
    }

    /// <summary>
    /// 자막 설정 초기화
    /// </summary>
    private void ResetSubtitle()
    {
        Debug.Log("Reset Subtitle");

        curAudioSubtitleType = AudioSubtitleType.None;
        curVideoSubtitleType = VideoSubtitleType.None;

        subtitleNum = 0;
        subtitleBox.SetActive(false);

        subtitleText.text = "";
        subtitleText.SetAllDirty();

        subtitle_Main.enabled = false;

    }

    #region Audio Subtitle Function

    /// <summary>
    /// 현재 콘텐츠에 맞춰서 Audio용 Subtitle 세팅
    /// </summary>
    public void SetSubtitle(NarrationOption option)
    {
        subtitle_Main.ResetSubtitle_Main();
        ResetSubtitle();

        curAudioSubtitleType = option.subtitleType;
        if (!subtitle_Main.enabled)
            subtitle_Main.enabled = true;

        if (curAudioSubtitleType != AudioSubtitleType.None)
        {
            contentNum = (int)curAudioSubtitleType - 1; // Enum.None(0)이기 때문에 -1 해주기

            Debug.Log($"[SetSubtitle/SubtitleManager] - Audio\n<color=yellow> [{curAudioSubtitleType}]</color> Content 자막 시작");

            if (!option.useCheckPoint)
                StartAudioSubtitle(contentNum, option.waitSecond);
            else
            {
                if (option.checkSubtitleList.Count == 0)
                    return;
                else
                {
                    for (int i = 0; i < option.checkSubtitleList.Count; i++)
                    {
                        option.checkSubtitleList[i].isChecked = false; // 이미 Inspector에 만들어져 있는 List<>. isChecked = false으로 값 초기화
                    }
                }
                StartAudioSubtitle(contentNum, option.waitSecond, option.checkSubtitleList);
            }
        }
        else
            Debug.Log($"[SetSubtitle/SubtitleManager]\n<color=red> Subtitle Type : {curAudioSubtitleType} 을 확인해주십시오. </color>");
    }

    private void StartAudioSubtitle(int contentNum, float waitSecond =1, List<SubtitleCheckPoint> PointList = null)
    {
        if (audioClipsList[contentNum].Length - 1 < subtitleNum)
        {
            Debug.Log($"<color=red> Audio 자막 기능 종료</color>/ subtitleNum : <color=yellow>[{subtitleNum}/ {audioClipsList[contentNum].Length - 1}]</color> Content Type : <color=yellow>[{curAudioSubtitleType}]</color>");
            ResetSubtitle();
            NarrationForSequence.Instance.CheckIsFinish(true);
        }
        else
        {
            Debug.Log($"[StartAudioSubtitle/SubtitleManager]\nAudioSubtitleNum : <color=yellow>[{subtitleNum}/ {audioClipsList[contentNum].Length - 1}]</color> Content Type : <color=yellow>[{curAudioSubtitleType}]</color>");

            IEnumerator SubRoutine = AudioSubtitleRoutine(contentNum, waitSecond, PointList);
            StartCoroutine(SubRoutine);
        }
    }

    /// <summary>
    /// 오디오 클립 갯수만큼 자막 보여주는 루틴
    /// </summary>
    /// <param name="contentNum"></param>
    /// <param name="waitSecond"> 자막 간 대기 시간 </param>
    /// <returns></returns>
    IEnumerator AudioSubtitleRoutine(int contentNum, float waitSecond, List<SubtitleCheckPoint> PointList = null)
    {
        // clip 개수 만큼 코루틴 반복
        if (audioClipsList[contentNum].Length - 1 >= subtitleNum)
        {
            audioSource.clip = audioClipsList[contentNum][subtitleNum];

            subtitleText.text = audioSubtitlesList[contentNum][subtitleNum];
            subtitleText.SetAllDirty();

            yield return new WaitForSeconds(waitSecond);

            if (PointList != null)
            {
                for (int i = 0; i < PointList.Count; i++)
                {
                    if (!PointList[i].isChecked && subtitleNum == PointList[i].checkSubtitleNum)
                    {
                        Debug.Log($"[CheckPoint]subtitleNum : <color=cyan>[{subtitleNum}]</color> => Check Point Wait.../ <color=red>[{PointList[i].isChecked}]</color>");
                        nowWaitingEvent = true;

                        if (PointList[i].checkEventList.Count != 0) // Check Point Event 실행
                        {
                            subtitle_Main.StartCutLoop(i);
                        }

                        yield return new WaitUntil(() => checkPoint_End == true);
                        PointList[i].isChecked = checkPoint_End;
                        nowWaitingEvent = false;

                        checkPoint_End = false; // 초기화
                        Debug.Log($"[CheckPoint]subtitleNum : <color=cyan>[{subtitleNum}]</color> => Check Point Done" +
                            $"/ <color=green>Checked:[{PointList[i].isChecked}]</color>");

                    }
                }
            }
            subtitleBox.SetActive(true);
            audioSource.Play();

            yield return new WaitUntil(() => !audioSource.isPlaying); // Audio 끝날때까지 대기

            yield return new WaitForSeconds(0.5f);
            subtitleBox.SetActive(false);
            subtitleNum++;

            StartAudioSubtitle(contentNum, waitSecond, PointList);
        }
        else
        {
            ResetSubtitle();
        }
    }

    public void EndSubtitle()
    {
        StopAllCoroutines();
        audioSource.clip = null;

        ResetSubtitle();
    }

    #endregion

    #region Video Subtitle Function

    /// <summary>
    /// 현재 콘텐츠에 맞춰서 Video용 Subtitle 세팅
    /// </summary>
    /// <param name="type"> 현재 콘텐츠 자막 타입 </param>
    public void SetSubtitle(VideoSubtitleType type, string url = "")
    {
        curVideoSubtitleType = type;
        Debug.Log(curVideoSubtitleType);

        if (curVideoSubtitleType != VideoSubtitleType.None)
        {
            contentNum = (int)curVideoSubtitleType - 1; // Enum.None(0) 때문에 -1 해주기

            Debug.Log($"[SetSubtitle/SubtitleManager] - Video\n<color=yellow> [{curVideoSubtitleType}]</color> Content 자막 시작");

            StartVideoSubtitle(contentNum, url);
        }
        else
            Debug.Log($"[SetSubtitle/SubtitleManager]\n<color=red> Subtitle Type : {curVideoSubtitleType} 을 확인해주십시오. </color>");
    }


    private void StartVideoSubtitle(int contentNum, string url)
    {
        //Debug.Log($"VideoSubtitleNum : <color=yellow>[{subtitleNum}/ {subtitlesList[contentNum].Length - 1}]</color> Content Type : <color=yellow>[{curSubtitleType}]</color>");
        Debug.Log($"[StartVideoSubtitle/SubtitleManager]\n<color=yellow>[{curVideoSubtitleType}/ {url}]</color>");

        if (VR_VideoPlayer.Instance.MyVideoPlayer.url != "")
        {
            IEnumerator SubRoutine;
            SubRoutine = VideoSubtitleRoutine(contentNum, url);
            StartCoroutine(SubRoutine);
        }
        else
            Debug.Log($"영상 url이 Null 입니다./ [{VR_VideoPlayer.Instance.MyVideoPlayer.url}]");
    }

    /// <summary>
    /// 비디오 영상 자막 루틴
    /// </summary>
    /// <param name="contentNum"></param>
    /// <returns></returns>
    IEnumerator VideoSubtitleRoutine(int contentNum, string url)
    {
        Debug.Log($"[VideoSubtitleRoutine/SubtitleManager]\n<color=cyan> [{curVideoSubtitleType}/ {url}]</color> Subtitle Routine을 시작합니다.");
        switch (url)
        {
            case "Elevator_01.mp4":
                if (VR_VideoPlayer.Instance.MyVideoPlayer.url != null)
                {
                    yield return new WaitForSeconds(27.2f);
                    StartCoroutine(SetVideoSubtitle(contentNum));
                    yield return new WaitUntil(() => preVideoRoutine_End == true);

                    StartCoroutine(SetVideoSubtitle(contentNum));
                    yield return new WaitUntil(() => preVideoRoutine_End == true);

                    StartCoroutine(SetVideoSubtitle(contentNum, 1.5f));
                    yield return new WaitUntil(() => preVideoRoutine_End == true);


                }
                break;
        }

        Debug.Log($"subtitleNum : <color=red>[{subtitleNum}/ {videoSubtitlesList[contentNum].Length - 1}]</color> Content Type : <color=yellow>[{curVideoSubtitleType}]</color> Video Subtitle 종료");
        ResetSubtitle();
    }

    /// <summary>
    /// 이전의 SetVideoSubtitle 코루틴이 끝났는지 여부( 끝났을때까지 다음 코루틴 시작 대기 체크용)
    /// </summary>
    bool preVideoRoutine_End = true;

    /// <summary>
    /// Video용 자막을 세팅하고 보여줍니다.
    /// </summary>
    /// <param name="contentNum"> 현재 진행 중인 콘텐츠 넘버</param>
    /// <param name="nextSeconds"> 이전 자막과 다음 자막 간의 대기 시간 </param>
    /// <returns></returns>
    IEnumerator SetVideoSubtitle(int contentNum, float nextSeconds = 0)
    {
        if (preVideoRoutine_End)
        {
            preVideoRoutine_End = false;
            Debug.Log($"[SetVideoSubtitle/SubtitleManager]\nVideoSubtitleNum : <color=yellow>[{subtitleNum}/ {videoSubtitlesList[contentNum].Length-1}]</color> Content Type : <color=yellow>[{curVideoSubtitleType}]</color>");
            if (subtitleNum > videoSubtitlesList[contentNum].Length-1) // 실제로 이 if문을 들어오진 않음
            {
                Debug.Log($"{subtitleNum} > {videoSubtitlesList[contentNum].Length-1}");
                preVideoRoutine_End = true;
                yield break;
            }

            yield return new WaitForSeconds(nextSeconds);

            subtitleText.text = videoSubtitlesList[contentNum][subtitleNum];
            subtitleText.SetAllDirty();

            char[] subTextArray = subtitleText.text.ToCharArray();
            float waitSeconds = subTextArray.Length * 0.2f;

            subtitleBox.SetActive(true);
            Debug.Log($"<color=yellow>{subtitleText.text}</color> => {subTextArray.Length} 개 * 0.2 = <color=yellow>{waitSeconds} 초</color>");
            yield return new WaitForSeconds(waitSeconds);

            subtitleBox.SetActive(false);
            subtitleNum++;
            preVideoRoutine_End = true;
            //Debug.Log($"SetVideoSubtitle/ <color=green>{subtitleNum}/ {contentNum}/ {preVideoRoutine_End} </color>");
        }
        else
            Debug.Log($"<color=red> {preVideoRoutine_End} </color>");
    }

    #endregion

    #region Audio Subtitles
    /// <summary>
    /// Intro
    /// </summary>
    private string[] audioSubtitles_00 = new string[]
    {

    };
    /// <summary>
    /// Prologue_00
    /// </summary>
    private string[] audioSubtitles_01 = new string[]
    {
        "인간은 누구나 불안과 공포를 느낍니다",
        "우리가 살아가기 위해 꼭 필요한 감정입니다",
        "불안과 공포를 느끼는 것의 이점이 있습니다",
        "우리의 생존을 돕고, 행동을 조절해 줍니다",
        "하지만 그 크기를 예측하기 어렵고",
        "통제할 수 없어 고통받고 있나요",
        "이것은 당신을 괴롭히는 '위험' 신호입니다",
        "피하려고 하면 되려 커지지만",
        "기꺼이 받아들이고, 마주한다면 대처할 수 있어요",
        "만져보세요. 원래 크기로 되돌려주세요",
        "사실은 아주 작거나, 존재하지 않을지도 몰라요",
        "충분히 다룰 수 있는 일로 만들어보세요",
        "할 수 있어요",
        "잘하셨습니다",
    };
    /// <summary>
    /// Prologue_01
    /// </summary>
    private string[] audioSubtitles_02 = new string[]
    {
        "이제, 두 개의 갈림길 앞입니다",
        "오른쪽은 불안을 받아들이고, 그 과정을 관찰하는 길입니다",
        "처음엔 어려울 수 있지만, 차츰 나아지게 될 것입니다",
        "왼쪽은 불안을 억압하거나 회피하는 길입니다",
        "잠시 동안은 덜 불안하다고 느낄 수 있지만",
        "이 경우 불안을 다룰 기회가 줄어들어, 점점 더 혼란스러워질 수도 있습니다",
        "당신이 나아가고자 하는 방향이 있다는 것을 잘 압니다",
        "그래서 이 프로그램에도 참여하셨으리라 믿습니다",
        "소중한 당신이 불안에 압도되어 혼돈 속을 헤메지 않기를 바랍니다",
        "우리와 함께 연습해 봐요",
        "준비 됐나요?",
    };

    /// <summary>
    /// Home
    /// </summary>
    private string[] audioSubtitles_03 = new string[]
    {
        "훈련 도중 공황을 느끼거나, 심한 불안을 느끼는 경우",
        "해당 버튼을 눌러 언제든지 훈련 프로그램을 중지 할 수 있습니다.",
        "버튼의 위치를 확인하세요.",
        "잘 하셨습니다.",
    };
    #endregion

    #region Video Subtitles
    /// <summary>
    /// Elevator
    /// </summary>
    private string[] videoSubtitles_01 = new string[]
    {
        "몇 층까지 가세요?",
        "아 저 20층이요",
        "감사합니다",

    };
    /// <summary>
    /// Theater
    /// </summary>
    private string[] videoSubtitles_02 = new string[]
    {

    };
    /// <summary>
    /// Bus
    /// </summary>
    private string[] videoSubtitles_03 = new string[]
    {

    };
    #endregion
}
