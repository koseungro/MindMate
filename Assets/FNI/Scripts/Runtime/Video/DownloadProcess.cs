using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using FNI;
using FNI.XR;

namespace Panic2
{
    public class DownloadProcess : MonoBehaviour
    {
        [SerializeField] Text storageText;
        [SerializeField] Slider downloadProcessSlider;

        [Header("버튼")]
        [SerializeField] Button exitBtn;
        [SerializeField] GameObject downProcessBtn;
        [SerializeField] Button startBtn;

        [Header("OBJ")]
        [SerializeField] GameObject errorObj;
        [SerializeField] GameObject sliderObj;
        [SerializeField] GameObject infoObj;

        [SerializeField] Text errorText;
        [SerializeField] Text mainText;

        [Header("에러 문구")]
        public ErrorScript[] error;

        private Animator waitDotanim;

        /// <summary>
        /// 다운로드 완료되면 소리로 알림;
        /// </summary>
        [SerializeField] AudioSource audioSource;
        /// <summary>
        /// 서버에 접속이 가능한지 여부를 나타내는 값
        /// </summary>
        public bool isServerConnection = true;
        /// <summary>
        /// 비메오 파일 업로드 된 url 주소
        /// </summary>
        string serverUrl;

        float updatetime = 0.0f;
        float elapsedtime = 5.0f;

        public static DownloadProcess instance = null;
        public static DownloadProcess Instace()
        {
            return instance;
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            serverUrl = "https://vimeo.com/manage/videos/861904226/37679f0818";

            waitDotanim = GetComponentInChildren<Animator>();

            InitTake();
        }

        private void OnEnable()
        {
            waitDotanim.SetBool("DownloadComplete", VideoDownloadManager.Instance().isDownloadComplete);

            //downProcessBtn.SetActive(!VideoDownloadManager.Instance().isDownloadComplete);
            downProcessBtn.SetActive(true);

            StartCoroutine(CheckDownloadComplete());
        }

        private void Start()
        {
            exitBtn.gameObject.SetActive(true);
            //downProcessBtn.SetActive(true);
            //startBtn.gameObject.SetActive(false);

            errorObj.SetActive(false);

            StartCoroutine(GetServer(serverUrl));
        }

        private void Update()
        {
            storageText.text = $"총 {VideoDownloadManager.Instance().ContentsName.Length}개 중 {FileChecker.Instance().fileCount}개 다운로드됨\n";
            downloadProcessSlider.value = FileChecker.Instance().fileCount;

            if (updatetime > elapsedtime)
            {
                updatetime = 0.0f;
                StartCoroutine(GetServer(serverUrl));
                //StorageChecker.Instance().GetDirectorySize(Application.persistentDataPath);
                //StorageChecker.Instance().StorageCheck();
                FileChecker.Instance().GetDirectoryCount();
            }
            else
            {
                updatetime += Time.deltaTime;
            }

            if (FileChecker.Instance().fileCount != VideoDownloadManager.Instance().ContentsName.Length)
            {
                downProcessBtn.SetActive(true);

                startBtn.gameObject.SetActive(VideoDownloadManager.Instance().isDownloadComplete);
                startBtn.interactable = false;
            }
            //else // 다운로드 완료
            //{
            //    if (downloadProcessSlider.value == FileChecker.Instance().fileCount)
            //    {
            //        waitDotanim.SetBool("DownloadComplete", VideoDownloadManager.Instance().isDownloadComplete);
            //        downProcessBtn.SetActive(false);

            //        startBtn.gameObject.SetActive(VideoDownloadManager.Instance().isDownloadComplete);
            //        startBtn.interactable = true;
            //    }
            //}

#if !UNITY_EDITOR
            if (VideoDownloadManager.Instance().isDownloadComplete)
            {
                waitDotanim.SetBool("DownloadComplete", VideoDownloadManager.Instance().isDownloadComplete);

                startBtn.gameObject.SetActive(true);
                mainText.text = "다운로드가 완료되었습니다";
                infoObj.SetActive(false);

            }

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //Debug.Log("인터넷 없음");
                //downProcessBtn.SetActive(false);
                //sliderObj.SetActive(false);
                //retryBtn.SetActive(true);
                //errorObj.SetActive(true);
                //errorText.text = error[0].str;
                //SetTake(Take.P2_DownloadInfo);
            }
            else if (!isServerConnection)
            {
                //Debug.Log("서버 연결 없음");
                //downProcessBtn.SetActive(false);
                //sliderObj.SetActive(false);
                //retryBtn.SetActive(true);
                //errorObj.SetActive(true);
                //errorText.text = error[1].str;
                //SetTake(Take.P2_DownloadInfo);
            }
            //else if(StorageChecker.Instance().requiredSpace > StorageChecker.Instance().availableSize)
            //{
            //    //Debug.Log(" 저장소 부족 ");
            //    //downProcessBtn.SetActive(false);
            //    //sliderObj.SetActive(false);
            //    //retryBtn.SetActive(true);
            //    //errorObj.SetActive(true);
            //    //errorText.text = error[2].str;
            //    SetTake(Take.P2_DownloadInfo);
            //}
            else if (!VideoDownloadManager.Instance().isDownloadComplete)
            {
                waitDotanim.SetBool("DownloadComplete", VideoDownloadManager.Instance().isDownloadComplete);

                downProcessBtn.SetActive(true);
                sliderObj.SetActive(true);
                errorObj.SetActive(false);
                infoObj.SetActive(true);
            }

#elif UNITY_EDITOR
            //downProcessBtn.SetActive(false);
#endif
        }

        /// <summary>
        /// 영상 다운로드 완료되면 Start 버튼 활성화 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckDownloadComplete()
        {
            downloadProcessSlider.value = 0;

            startBtn.gameObject.SetActive(false);

            yield return new WaitUntil(() => downloadProcessSlider.value == VideoDownloadManager.Instance().ContentsName.Length 
            && VideoDownloadManager.Instance().isDownloadComplete == true);

            FileChecker.Instance().GetDirectoryCount();

            // 다운로드 완료
            waitDotanim.SetBool("DownloadComplete", VideoDownloadManager.Instance().isDownloadComplete);

            yield return new WaitForSeconds(1.5f);

            downProcessBtn.SetActive(false);
            startBtn.gameObject.SetActive(VideoDownloadManager.Instance().isDownloadComplete);
            startBtn.interactable = true;
            XRManager.Instance.SendHapticImpulse(0.2f, 1f);
        }

        public void InitTake()
        {
            exitBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                Application.Quit();
            });
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
                request.timeout = 3;

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
}

