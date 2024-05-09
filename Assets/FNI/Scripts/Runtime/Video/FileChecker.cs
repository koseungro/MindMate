using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FNI;

public class FileChecker : MonoBehaviour
{
    #region singletone
    private static FileChecker instance = null;
    public static FileChecker Instance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    #endregion

    void Start()
    {
        GetDirectoryCount();
    }

    public int fileCount;

    public void GetDirectoryCount()
    {
        // Document
        // https://learn.microsoft.com/ko-kr/dotnet/csharp/programming-guide/concepts/linq/how-to-query-for-files-with-a-specified-attribute-or-name

        string targetFolder = VideoDownloadManager.Instance().VideoFolderPath;

        // Take a snapshot of the file system.
        DirectoryInfo dir = new DirectoryInfo(targetFolder);

        // This method assumes that the application has discovery permissions
        // for all folders under the specified path.
        IEnumerable<FileInfo> fileList = dir.GetFiles("*", SearchOption.AllDirectories);
        // Create the query
        IEnumerable<FileInfo> fileQuery =
            from file in fileList
            where file.Extension == ".mp4"
            orderby file.Name
            select file;

        fileCount = fileQuery.Count<FileInfo>();
    }
}