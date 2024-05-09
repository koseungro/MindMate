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
    /// �ڸ� ����ϴ� �ó������� ����� ��ϵ� ����Ʈ
    /// </summary>
    public List<AudioClip[]> audioClipsList = new List<AudioClip[]>();
    /// <summary>
    /// �ڸ��� �� String ��ϵ� ����Ʈ
    /// </summary>
    public List<string[]> audioSubtitlesList = new List<string[]>();
    public List<string[]> videoSubtitlesList = new List<string[]>();

    /// <summary>
    /// audioClipList, subtitlesList ������ �ѹ�
    /// </summary>
    private int contentNum = 0;

    /// <summary>
    /// �ڸ� ��ȣ
    /// </summary>
    private int subtitleNum = 0;

    /// <summary>
    /// Subtitle Check Point�� ���������� Event�� ����� �� ���� Subtitle�� �Ѿ�� �Ǵ��� üũ
    /// </summary>
    public bool checkPoint_End = false;

    public bool NowWaitingEvent { get => nowWaitingEvent; }

    /// <summary>
    /// ���� �ڸ� Check Point Event�� ����Ǿ� [checkPoint_End]�� true�� �Ǳ⸦ ��� ������ üũ
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
    /// �ڸ� ���� �ʱ�ȭ
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
    /// ���� �������� ���缭 Audio�� Subtitle ����
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
            contentNum = (int)curAudioSubtitleType - 1; // Enum.None(0)�̱� ������ -1 ���ֱ�

            Debug.Log($"[SetSubtitle/SubtitleManager] - Audio\n<color=yellow> [{curAudioSubtitleType}]</color> Content �ڸ� ����");

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
                        option.checkSubtitleList[i].isChecked = false; // �̹� Inspector�� ������� �ִ� List<>. isChecked = false���� �� �ʱ�ȭ
                    }
                }
                StartAudioSubtitle(contentNum, option.waitSecond, option.checkSubtitleList);
            }
        }
        else
            Debug.Log($"[SetSubtitle/SubtitleManager]\n<color=red> Subtitle Type : {curAudioSubtitleType} �� Ȯ�����ֽʽÿ�. </color>");
    }

    private void StartAudioSubtitle(int contentNum, float waitSecond =1, List<SubtitleCheckPoint> PointList = null)
    {
        if (audioClipsList[contentNum].Length - 1 < subtitleNum)
        {
            Debug.Log($"<color=red> Audio �ڸ� ��� ����</color>/ subtitleNum : <color=yellow>[{subtitleNum}/ {audioClipsList[contentNum].Length - 1}]</color> Content Type : <color=yellow>[{curAudioSubtitleType}]</color>");
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
    /// ����� Ŭ�� ������ŭ �ڸ� �����ִ� ��ƾ
    /// </summary>
    /// <param name="contentNum"></param>
    /// <param name="waitSecond"> �ڸ� �� ��� �ð� </param>
    /// <returns></returns>
    IEnumerator AudioSubtitleRoutine(int contentNum, float waitSecond, List<SubtitleCheckPoint> PointList = null)
    {
        // clip ���� ��ŭ �ڷ�ƾ �ݺ�
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

                        if (PointList[i].checkEventList.Count != 0) // Check Point Event ����
                        {
                            subtitle_Main.StartCutLoop(i);
                        }

                        yield return new WaitUntil(() => checkPoint_End == true);
                        PointList[i].isChecked = checkPoint_End;
                        nowWaitingEvent = false;

                        checkPoint_End = false; // �ʱ�ȭ
                        Debug.Log($"[CheckPoint]subtitleNum : <color=cyan>[{subtitleNum}]</color> => Check Point Done" +
                            $"/ <color=green>Checked:[{PointList[i].isChecked}]</color>");

                    }
                }
            }
            subtitleBox.SetActive(true);
            audioSource.Play();

            yield return new WaitUntil(() => !audioSource.isPlaying); // Audio ���������� ���

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
    /// ���� �������� ���缭 Video�� Subtitle ����
    /// </summary>
    /// <param name="type"> ���� ������ �ڸ� Ÿ�� </param>
    public void SetSubtitle(VideoSubtitleType type, string url = "")
    {
        curVideoSubtitleType = type;
        Debug.Log(curVideoSubtitleType);

        if (curVideoSubtitleType != VideoSubtitleType.None)
        {
            contentNum = (int)curVideoSubtitleType - 1; // Enum.None(0) ������ -1 ���ֱ�

            Debug.Log($"[SetSubtitle/SubtitleManager] - Video\n<color=yellow> [{curVideoSubtitleType}]</color> Content �ڸ� ����");

            StartVideoSubtitle(contentNum, url);
        }
        else
            Debug.Log($"[SetSubtitle/SubtitleManager]\n<color=red> Subtitle Type : {curVideoSubtitleType} �� Ȯ�����ֽʽÿ�. </color>");
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
            Debug.Log($"���� url�� Null �Դϴ�./ [{VR_VideoPlayer.Instance.MyVideoPlayer.url}]");
    }

    /// <summary>
    /// ���� ���� �ڸ� ��ƾ
    /// </summary>
    /// <param name="contentNum"></param>
    /// <returns></returns>
    IEnumerator VideoSubtitleRoutine(int contentNum, string url)
    {
        Debug.Log($"[VideoSubtitleRoutine/SubtitleManager]\n<color=cyan> [{curVideoSubtitleType}/ {url}]</color> Subtitle Routine�� �����մϴ�.");
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

        Debug.Log($"subtitleNum : <color=red>[{subtitleNum}/ {videoSubtitlesList[contentNum].Length - 1}]</color> Content Type : <color=yellow>[{curVideoSubtitleType}]</color> Video Subtitle ����");
        ResetSubtitle();
    }

    /// <summary>
    /// ������ SetVideoSubtitle �ڷ�ƾ�� �������� ����( ������������ ���� �ڷ�ƾ ���� ��� üũ��)
    /// </summary>
    bool preVideoRoutine_End = true;

    /// <summary>
    /// Video�� �ڸ��� �����ϰ� �����ݴϴ�.
    /// </summary>
    /// <param name="contentNum"> ���� ���� ���� ������ �ѹ�</param>
    /// <param name="nextSeconds"> ���� �ڸ��� ���� �ڸ� ���� ��� �ð� </param>
    /// <returns></returns>
    IEnumerator SetVideoSubtitle(int contentNum, float nextSeconds = 0)
    {
        if (preVideoRoutine_End)
        {
            preVideoRoutine_End = false;
            Debug.Log($"[SetVideoSubtitle/SubtitleManager]\nVideoSubtitleNum : <color=yellow>[{subtitleNum}/ {videoSubtitlesList[contentNum].Length-1}]</color> Content Type : <color=yellow>[{curVideoSubtitleType}]</color>");
            if (subtitleNum > videoSubtitlesList[contentNum].Length-1) // ������ �� if���� ������ ����
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
            Debug.Log($"<color=yellow>{subtitleText.text}</color> => {subTextArray.Length} �� * 0.2 = <color=yellow>{waitSeconds} ��</color>");
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
        "�ΰ��� ������ �ҾȰ� ������ �����ϴ�",
        "�츮�� ��ư��� ���� �� �ʿ��� �����Դϴ�",
        "�ҾȰ� ������ ������ ���� ������ �ֽ��ϴ�",
        "�츮�� ������ ����, �ൿ�� ������ �ݴϴ�",
        "������ �� ũ�⸦ �����ϱ� ��ư�",
        "������ �� ���� ����ް� �ֳ���",
        "�̰��� ����� �������� '����' ��ȣ�Դϴ�",
        "���Ϸ��� �ϸ� �Ƿ� Ŀ������",
        "�Ⲩ�� �޾Ƶ��̰�, �����Ѵٸ� ��ó�� �� �־��",
        "����������. ���� ũ��� �ǵ����ּ���",
        "����� ���� �۰ų�, �������� �������� �����",
        "����� �ٷ� �� �ִ� �Ϸ� ��������",
        "�� �� �־��",
        "���ϼ̽��ϴ�",
    };
    /// <summary>
    /// Prologue_01
    /// </summary>
    private string[] audioSubtitles_02 = new string[]
    {
        "����, �� ���� ������ ���Դϴ�",
        "�������� �Ҿ��� �޾Ƶ��̰�, �� ������ �����ϴ� ���Դϴ�",
        "ó���� ����� �� ������, ���� �������� �� ���Դϴ�",
        "������ �Ҿ��� ����ϰų� ȸ���ϴ� ���Դϴ�",
        "��� ������ �� �Ҿ��ϴٰ� ���� �� ������",
        "�� ��� �Ҿ��� �ٷ� ��ȸ�� �پ���, ���� �� ȥ���������� ���� �ֽ��ϴ�",
        "����� ���ư����� �ϴ� ������ �ִٴ� ���� �� �дϴ�",
        "�׷��� �� ���α׷����� �����ϼ������� �Ͻ��ϴ�",
        "������ ����� �Ҿȿ� �е��Ǿ� ȥ�� ���� ����� �ʱ⸦ �ٶ��ϴ�",
        "�츮�� �Բ� ������ ����",
        "�غ� �Ƴ���?",
    };

    /// <summary>
    /// Home
    /// </summary>
    private string[] audioSubtitles_03 = new string[]
    {
        "�Ʒ� ���� ��Ȳ�� �����ų�, ���� �Ҿ��� ������ ���",
        "�ش� ��ư�� ���� �������� �Ʒ� ���α׷��� ���� �� �� �ֽ��ϴ�.",
        "��ư�� ��ġ�� Ȯ���ϼ���.",
        "�� �ϼ̽��ϴ�.",
    };
    #endregion

    #region Video Subtitles
    /// <summary>
    /// Elevator
    /// </summary>
    private string[] videoSubtitles_01 = new string[]
    {
        "�� ������ ������?",
        "�� �� 20���̿�",
        "�����մϴ�",

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
