using FNI;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class VideoPlayerForSequence : MonoBehaviour, IVisualObject
{
    public VisualType Type { get => VisualType.Video;}

    bool IVisualObject.IsFinish => MainVideo.isFinish;    

    [SerializeField] VR_VideoPlayer MainVideo = null;

    bool isMainHmdPlay = false;

    public void Init()
    {
    }

    public void Active(CutData option)
    {
        switch (option.videoOption.videoType)
        {
            case VideoType.Main:
                VideoEvent(MainVideo, option.videoOption);
                break;
            default:
                break;
        }
    }

    public void MyUpdate()
    {
        
    }

    private void VideoEvent(VR_VideoPlayer video, VideoOption option)
    {
        video.SetVideoCtrl(option);

        //if (option.state == VideoState.Rotation)
        //{
        //    if(video.gameObject.name == "MomVideoPlayer")
        //        video.transform.parent.transform.rotation = Quaternion.Euler(option.rotation);
        //    else                
        //        video.transform.rotation = Quaternion.Euler(option.rotation);
        //}

        //if(option.state == VideoState.Position)
        //{
        //    video.transform.parent.GetComponent<RectTransform>().anchoredPosition3D = option.position;
        //}

    }

   

    private void HmdPlayEvent()
    {
        VideoOption playVideoOption = new VideoOption();
        playVideoOption.state = VideoState.Play;

        //Debug.Log(MainVideo.Duration);
        //Debug.Log(MainVideo.Time);


        if ((MainVideo.Duration - 1) > MainVideo.VideoTime && isMainHmdPlay)
            VideoEvent(MainVideo, playVideoOption);
    }

    private void HmdPauseEvent()
    {
        VideoOption playVideoOption = new VideoOption();
        playVideoOption.state = VideoState.Pause;

        CheckHmdVideoPlaying();

        VideoEvent(MainVideo, playVideoOption);
    }

    private void CheckHmdVideoPlaying()
    {
        if (MainVideo.IsPlaying)
            isMainHmdPlay = true;
        else
            isMainHmdPlay = false;
    }

}
