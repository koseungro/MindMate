using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using FNI;

[System.Serializable]
public class ErrorScript
{
    [TextArea(3, 8)]
    public string str;

    public ErrorScript(string str)
    {
        this.str = str;
    }
}

public class DownloadInfo : MonoBehaviour
{
    private static DownloadInfo _instance;

    public static DownloadInfo Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<DownloadInfo>();

            return _instance;
        }
    }

    [SerializeField] Text storageText;
    [SerializeField] Text storageErrorText;
    [Header("에러 문구")]
    public ErrorScript[] error;

    [SerializeField] GameObject errorObj;

    [Header("버튼")]
    public GameObject retryBtn;
    public GameObject startBtn;
    public GameObject exitBtn;

    int fileCount = 0;

    /// <summary>
    /// 인터넷 연결이 되었는지 체크하는 bool 값
    /// </summary>
    bool isServerConnection = true;
    /// <summary>
    /// 인터넷 연결 체크를 위한 서버 uri 값
    /// </summary>
    string serverUrl;

    private GameObject info;
    [SerializeField] private GameObject process;

    void Awake()
    {
        serverUrl = "https://vimeo.com/manage/videos/861904226/37679f0818";

        info = this.gameObject;        

        InitTake();
    }

    private void Start()
    {
        errorObj.SetActive(false);
        retryBtn.SetActive(false);
        startBtn.SetActive(false);

        StartCoroutine(GetServer(serverUrl));

#if !UNITY_EDITOR
        StorageChecker.Instance().GetDirectorySize(Application.persistentDataPath);
        StorageChecker.Instance().StorageCheck();
#endif
    }

    float updatetime = 0.0f;
    /// <summary>
    /// Update 루틴 반복 주기 시간
    /// </summary>
    float elapsedtime = 3.0f;

    private void Update()
    {
#if !UNITY_EDITOR
        if (updatetime > elapsedtime)
            {
                updatetime = 0.0f;
                //StorageChecker.Instance().GetDirectorySize(Application.persistentDataPath);
                //StorageChecker.Instance().StorageCheck();
                StartCoroutine(GetServer(serverUrl));
            }
            else
            {
                updatetime += Time.deltaTime;
            }

            //if (StorageChecker.Instance().requiredSpace > StorageChecker.Instance().availableSize)
            //{
            //    // 저장소공간 부족할 경우
            //    Debug.Log(" 저장소 부족 ");
            //    errorObj.SetActive(true);
            //    startBtn.SetActive(false);
            //    retryBtn.SetActive(true);
            //    storageErrorText.text = error[0].str;
            //}
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                // 인터넷 연결이 안되었을 때 행동
                Debug.Log(" 인터넷 연결 없음 ");
                errorObj.SetActive(true);
                startBtn.SetActive(false);
                retryBtn.SetActive(true);
                storageErrorText.text = error[1].str;
            }
            else if (!isServerConnection)
            {
                // 다운로드 서버에 접속 안되었을 때 행동
                Debug.Log(" 서버 접속 안됨 ");
                errorObj.SetActive(true);
                startBtn.SetActive(false);
                retryBtn.SetActive(true);
                storageErrorText.text = error[2].str;
            }
            else
            {
                // 저장소공간 Enough 인터넷 연결 true
                Debug.Log(" 저장소 공간 충분 & 인터넷 연결 있음 ");
                
                errorObj.SetActive(false);
                retryBtn.SetActive(false);
                startBtn.SetActive(true);
            }

        storageText.text = $"필요한 공간 <b>{string.Format("{0:N2}", StorageChecker.Instance().requiredSpace)} GB</b>  /  " +
                            $"사용 가능한 공간 <b>{string.Format("{0:N2}", StorageChecker.Instance().availableSize)} GB</b>";

#elif UNITY_EDITOR
        errorObj.SetActive(false);
        retryBtn.SetActive(false);
        startBtn.SetActive(true);
#endif

    }

    void InitTake()
    {
        retryBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            //if (StorageChecker.Instance().requiredSpace < StorageChecker.Instance().availableSize)
            //{
            //    startBtn.SetActive(true);
            //    retryBtn.SetActive(false);
            //}
            if (!isServerConnection)
            {
                StartCoroutine(GetServer(serverUrl));
            }
        });

        startBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            info.SetActive(false);
            process.SetActive(true);

            VideoDownloadManager.Instance().DownVideo();
        });
        exitBtn.GetComponent<Button>().onClick.AddListener(() => Application.Quit());
    }

    Color introColor = new Color(10 / 255f, 10 / 255f, 35 / 255f);

    IEnumerator GetServer(string uri)
    {
        // 인터넷 연결 비메오 업로드 영상 URL로 체크 (임시용이라 추후 수정 필요)
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            request.SetRequestHeader("Authorization", "bearer 9135d1508eef8134c4e97837abd5fea7");
            request.SetRequestHeader("Accept", "application/vnd.vimeo.*+json;version=3.4");
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 2;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // 접속 안됐을 때
                Debug.Log("<color=red>Connection Failed</color>");
                isServerConnection = false;
            }
            else
            {
                // 접속 됐을 때
                Debug.Log("Connection Succes");
                isServerConnection = true;
            }
        }
    }
}
