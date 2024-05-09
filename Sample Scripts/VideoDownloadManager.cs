using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using FNI;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public class CustomAttribute : PropertyAttribute
{
    public string[] NewName { get; private set; }
    public CustomAttribute(string[] name)
    {
        NewName = name;
    }
}

[CustomPropertyDrawer(typeof(CustomAttribute))]
public class NamedArrayDDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
        EditorGUI.PropertyField(position, property, new GUIContent(((CustomAttribute)attribute).NewName[pos]));
    }
}
#endif


/// <summary>
/// 콘텐츠 비디오 다운로드 관리 클래스
/// </summary>
public class VideoDownloadManager : MonoBehaviour
{
#if UNITY_EDITOR
    [CustomAttribute(new string[] {
    "Elevator_01","Elevator_02","Elevator_03","Theater_01","Theater_02","Theater_03","Bus_01","Bus_02","Bus_03",
    })]
#endif
    // 영상 주소(ID) 목록
    public int[] ContentsID = new int[9];

#if UNITY_EDITOR
    [CustomAttribute(new string[] {
    "Elevator_01","Elevator_02","Elevator_03","Theater_01","Theater_02","Theater_03","Bus_01","Bus_02","Bus_03",
    })]
#endif
    // 영상 크기(해상도 2160p) 목록
    public int[] ContentsLength = new int[9];

#if UNITY_EDITOR
    [CustomAttribute(new string[] {
    "Elevator_01","Elevator_02","Elevator_03","Theater_01","Theater_02","Theater_03","Bus_01","Bus_02","Bus_03",
    })]
#endif
    // 다운받아서 저장할 영상 이름 목록
    public string[] ContentsName = new string[9];

    private static VideoDownloadManager instance = null;

    public static VideoDownloadManager Instance()
    {
        return instance;
    }

    string downloadURL;
    string downName;
    string url;

    public string VideoFolderPath { get => videoFolderPath; }
    private string videoFolderPath = "";

    int cnt = 0;

    bool isDownloading = false;
    bool isNetworkConnecting = false;
    public bool isDownloadComplete = false;

    public Text debugText;
    private string debugString;

    private void Awake()
    {
        if (instance == null) instance = this;

        CreateVideoFolder();
    }


    private void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            isNetworkConnecting = false;
            StopAllCoroutines();
        }
        else if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            isNetworkConnecting = true;
        }
    }

    #region Download Video
    public void DownVideo()
    {
        Debug.Log("Download Video");
        url = "https://api.vimeo.com/videos/";
        cnt = 0;
        IEnumerator coroutine = VideoNameCheck();
        StartCoroutine(coroutine);
    }

    IEnumerator VideoNameCheck()
    {
        isDownloadComplete = false;

        foreach (var name in ContentsName)
        {
            if (File.Exists(videoFolderPath + name.ToString() + ".mp4"))
            {
                FileInfo info = new FileInfo(videoFolderPath + name.ToString() + ".mp4");

                long videoError = Math.Abs(info.Length - (long)ContentsLength[cnt]); // 영상 크기 오차 절대값

                Debug.Log($" 사이즈 : {info.Length + " Bytes"} ");
                Debug.Log($" [{name.ToString()}] Size 비교하기 : {info.Length} == {(long)ContentsLength[cnt]}/ [{videoError}] => 0?");

                if (debugText.gameObject.activeInHierarchy)
                {
                    debugString = debugString + $"\n[{name.ToString()}] Size 비교하기 : {info.Length}=={(long)ContentsLength[cnt]}";
                    debugText.text = debugString;
                }

                if (videoError < 300000)
                {
                    Debug.Log($" VideoNameCheck 통과 ");
                    cnt++;
                    yield return null;
                }
                else
                {
                    Debug.Log($" VideoNameCheck 삭제 후 다시받기 ");

                    if (debugText.gameObject.activeInHierarchy)
                    {
                        debugString = debugString + $"\nVideoNameCheck 삭제 후 다시받기";
                        debugText.text = debugString;
                    }

                    Debug.Log($" name : {name} == ContentsName : {ContentsName[cnt]} ");
                    Debug.Log($" info.Length : <color=yellow>{info.Length}</color> != ContentsLength : <color=yellow>{(long)ContentsLength[cnt]}</color> ");

                    // 기존 파일 삭제
                    System.IO.File.Delete(videoFolderPath + name.ToString() + ".mp4");

                    IEnumerator coroutine = GetDownloadURL(url + ContentsID[cnt]);
                    StartCoroutine(coroutine);
                    yield return new WaitUntil(() => isDownloading == false);
                }
            }
            else
            {
                Debug.Log($"[{name}] 없음 => 다운로드");
                IEnumerator coroutine = GetDownloadURL(url + ContentsID[cnt]);
                StartCoroutine(coroutine);
                yield return new WaitUntil(() => isDownloading == false);
            }
        }

        if (debugText.gameObject.activeInHierarchy)
        {
            debugString = debugString + $"\nVideoNameCheck 다운로드 끝";
            debugText.text = debugString;
        }

        Debug.Log($"<color=cyan>VideoNameCheck 다운로드 끝</color>");
        isDownloadComplete = true;
        yield return null;
    }

    IEnumerator GetDownloadURL(string uri)
    {
        isDownloading = true;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Vimeo API : https://developer.vimeo.com/api/authentication
            webRequest.SetRequestHeader("Authorization", "bearer 9135d1508eef8134c4e97837abd5fea7");
            webRequest.SetRequestHeader("Accept", "application/vnd.vimeo.*+json;version=3.4");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            // ?
            string[] pages = uri.Split('/');
            int page = pages.Length - 1; 

            JObject o = JObject.Parse(webRequest.downloadHandler.text);

            //JToken name = o.SelectToken("$.download[?(@.rendition == '4320p')]") ??
            //              o.SelectToken("$.download[?(@.rendition == '3780p')]") ??
            //              o.SelectToken("$.download[?(@.rendition == '3240p')]") ??
            //              o.SelectToken("$.download[?(@.rendition == '2160p')]");

            // 비메오 제공 링크중 가장 고해상도 '2160p'
            // rendition = 2160p 영상의 url link를 가져온다
            JToken name = o.SelectToken("$.download[?(@.rendition == '2160p')]");

            //vp.url = (string)name.SelectToken("link");

            Debug.Log($" JToken.name : {name.SelectToken("link")}\n{downloadURL}");
            downloadURL = name.SelectToken("link").ToString();

            IEnumerator coroutine = GetDownloadVideo();
            StartCoroutine(coroutine);
        }
    }

    IEnumerator GetDownloadVideo()
    {
        Debug.Log($"<color=yellow>[{ContentsName[cnt]}]</color> 다운로드 시작");
        //debugString = debugString + $"\n{ContentsName[cnt]}" + " 다운로드 중";
        //debugText.text = debugString;


        using (UnityWebRequest request = UnityWebRequest.Get(downloadURL))
        {
            request.SendWebRequest();

            while (!request.isDone)
            {
                if (request.result == UnityWebRequest.Result.ConnectionError
                    || request.result == UnityWebRequest.Result.DataProcessingError
                    || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 다운로드 데이터를 파일로 저장
                string path = Path.Combine(videoFolderPath, ContentsName[cnt] + ".mp4");
                Debug.Log(path);
                try
                {
                    File.WriteAllBytes(path, request.downloadHandler.data);
                    Debug.Log($" GetDownloadVideo : 다운로드 정상 ");
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
                string[] splitUrl = path.Split('/');
                string targetUrl = splitUrl[splitUrl.Length - 1];
                Debug.Log($"<color=yellow>[{targetUrl}]</color> 다운로드 완료/ request.isdone : <color=yellow>{request.downloadHandler.isDone} </color>");
            }
            else
            {
                // 에러 발생시
                Debug.Log($"request.error : <color=red>{request.error} </color>");
            }
        }
        cnt++;
        isDownloading = false;
    }
    #endregion

    #region Check Video Folder/File
    private void CreateVideoFolder()
    {
        if (videoFolderPath == "")
        {
#if UNITY_EDITOR
            videoFolderPath = Application.dataPath + "/../Video/";
#else
        videoFolderPath = Application.persistentDataPath + "/Video/";
#endif

            if (!Directory.Exists(videoFolderPath))
            {
                Debug.Log($"[비디오 폴더 생성] Path : <color=yellow>[{videoFolderPath}]</color>");

                Directory.CreateDirectory(videoFolderPath);
            }
            else
                Debug.Log($"Video Folder Path : <color=yellow>[{videoFolderPath}]</color>가 이미 존재합니다.");
        }
    }

    /// <summary>
    /// 영상 파일이 다운로드 완료되어있는지 하니씩 체크하여 확인합니다.
    /// </summary>
    /// <returns></returns>
    public bool SetDownloadCompleted()
    {
        for (int i = 0; i < ContentsName.Length; i++)
        {
            if (File.Exists(videoFolderPath + ContentsName[i].ToString() + ".mp4"))
            {
                FileInfo info = new FileInfo(videoFolderPath + ContentsName[i].ToString() + ".mp4");

                long videoError = Math.Abs(info.Length - (long)ContentsLength[i]); // 영상 길이 오차 절대값

                Debug.Log($"[{ContentsName[i]}] 사이즈 : {info.Length + " Bytes"} ");
                Debug.Log($"[{ContentsName[i]}] Size 비교하기 : {info.Length} == {(long)ContentsLength[i]}/ <color=yellow>[{videoError}]</color> => 0?");


                if (videoError > 500000) // 영상 크기 다름
                {
                    Debug.Log($"<color=yellow>[{ContentsName[i]}]</color> 영상 길이 다름");

                    isDownloadComplete = false;
                    return isDownloadComplete;
                }
            }
            else // 파일 존재 안함
            {
                Debug.Log($"<color=yellow>[{ContentsName[i]}]</color> 파일 존재 안함");

                isDownloadComplete = false;
                return isDownloadComplete;
            }
        }

        // 영상 다운로드 완료 확인 끝
        isDownloadComplete = true;
        return isDownloadComplete;
    }
    #endregion
}
