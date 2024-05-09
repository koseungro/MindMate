using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FNI;

public class DownloadChecker : MonoBehaviour
{
    /// <summary>
    /// 다운로드 받을 영상의 길이를 담은 배열
    /// </summary>    
#if UNITY_EDITOR
    [CustomAttribute(new string[] {
    "Elevator_01","Elevator_02","Elevator_03","Theater_01","Theater_02","Theater_03","Bus_01","Bus_02","Bus_03"})]
#endif
    public int[] ContentsLength = new int[9];

    /// <summary>
    /// 다운로드 받을 영상의 이름을 담은 배열
    /// </summary>
#if UNITY_EDITOR
    [CustomAttribute(new string[] {
    "Elevator_01","Elevator_02","Elevator_03","Theater_01","Theater_02","Theater_03","Bus_01","Bus_02","Bus_03"})]
#endif
    public string[] ContentsName = new string[9];

    int a = 0;

    /// <summary>
    /// 다운로드 목록의 영상이 다운로드 완료 되었는지 체크
    /// </summary>
    public bool isDownloadComplete = false;

    private static DownloadChecker instance = null;
    public static DownloadChecker Instance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(DownloadCheck());
    }

    IEnumerator DownloadCheck()
    {
        foreach (var name in ContentsName)
        {
            Debug.Log($"<color=cyan>{name} : foreach Start</color>");

            if (File.Exists(VideoDownloadManager.Instance().VideoFolderPath + name.ToString() + ".mp4"))
            {
                FileInfo info = new FileInfo(VideoDownloadManager.Instance().VideoFolderPath + name.ToString() + ".mp4");

                if (info.Length == (long)ContentsLength[a])
                {
                    isDownloadComplete = true;
                }
                else
                {
                    isDownloadComplete = false;
                }
                a++;
                yield return null;
            }
            else
            {
                isDownloadComplete = false;
                Debug.Log($"{name} : break");
                yield break;
            }
        }
        Debug.Log($"DownloadCheck 끝");

    }
}
