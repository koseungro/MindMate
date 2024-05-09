using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageChecker : MonoBehaviour
{
    private static StorageChecker instance = null;
    public static StorageChecker Instance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    /// <summary>
    /// 다운 받을 영상의 총 크기
    /// </summary>
    public float videoSize;
    /// <summary>
    /// Quest2 디바이스 저장가능한 공간 체크용
    /// </summary>
    public float availableSize;
    /// <summary>
    /// Quest2 디바이스 필요한 공간 체크용
    /// </summary>
    public float requiredSpace;
    /// <summary>
    /// Quest2 디바이스 동영상 다운로드 디렉토리 공간 체크용
    /// </summary>
    public float dirSize;

    public Text debugText;

    //private void Start()
    //{
    //    GetDirectorySize(VideoDownloadManager.Instance().VideoFolderPath);
    //    StorageCheck();
    //}

    public void StorageCheck()
    {
        // Android Storage Check
        var jc = new AndroidJavaClass("android.os.Environment");
        var file = jc.CallStatic<AndroidJavaObject>("getDataDirectory");
        var path = file.Call<string>("getAbsolutePath");

        var stat = new AndroidJavaObject("android.os.StatFs", path);
        long blocks = stat.Call<long>("getAvailableBlocksLong");
        long blockSize = stat.Call<long>("getBlockSizeLong");

        // Available Storage Space (byte)
        availableSize = blocks * blockSize;

        availableSize = availableSize / 1024 / 1024 / 1024;

        Debug.Log($" availableSize : {availableSize}");

        requiredSpace = videoSize - dirSize < 0 ? 0 : videoSize - dirSize;

        Debug.Log($" requireSpace : {requiredSpace}");

    }

    /// <summary>
    /// path 해당하는 폴더의 크기를 반환
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public float GetDirectorySize(string path)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(path);

        foreach (FileInfo fi in dirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            dirSize += fi.Length;
        }

        dirSize = dirSize / 1024 / 1024 / 1024;

        Debug.Log($" dirSize : {dirSize}");

        return dirSize;
    }
}
