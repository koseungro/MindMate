using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FNI;
using UnityEngine.UI;

public class VideoContentChecker : MonoBehaviour
{
    public VideoSubtitleType videoContentType;
    public bool isCompleted = false;

    public Sprite checkImage;

    public void ResetChecker()
    {
        isCompleted = false;
    }
}
