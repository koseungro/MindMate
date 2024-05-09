/// 작성자: 고승로
/// 작성일: 2020-08-24
/// 수정일: 2020-09-04
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNI
{
    /// <summary>
    /// 컷 종류 타입
    /// </summary>
    public enum VisualType
    {
        Line,
        Narration,
        Animation,
        Wait,
        FadeInOut,
        UI,
        Video,
        Lay,
        Object,
        Quit,
        SceneMgr,
        Timer,
        XRManager,
    }

    /// <summary>
    /// 시퀀스 상태
    /// </summary>
    public enum SequenceState
    {
        Setting,
        Start,
        Update,
        Check,
        End,
        Pause,
    }

    /// <summary>
    /// 카메라 인칭 시점
    /// </summary>
    public enum CameraViewType
    {
        None,
        firstPerson,
        thirdPerson,
    }

    public enum Anim_FuncType
    {
        PlayAnimation,
        ResetAnimList,
    }

    /// <summary>
    /// 캐릭터 타입
    /// </summary>
    public enum CharType
    {
        Player,
        Table,
        Seed_Dummy,
        Seed,
        Butterfly_Dummy,
        Butterfly,
        Turtle_Dummy,
        Turtle,
        DarkTree,
        BG_DarkTrees,
        Turtle_Travel,
        Door,
        Jelly,
    }

    public enum NarrationType
    {
        Manager, // Narration
        Player,        
        BackgroundSound,
        Ambience,
    }

    public enum AudioSubtitleType
    {
        None,
        Intro,
        Prologue_00,
        Prologue_01,
        Home,
    }
    
    public enum VideoSubtitleType
    {
        None,
        Elevator_01,
        Elevator_02,
        Elevator_03,
        Theater_01,
        Theater_02,
        Theater_03,
        Bus_01,
        Bus_02,
        Bus_03,
    }

    public enum Sound_FuncType
    {
        PlaySound,
        SetVolume,
        PlaySubtitle,
    }

    /// <summary>
    /// VisualType.UI -> UI 기능 타입
    /// </summary>
    public enum UI_FuncType
    {
        None,
        AddScene_Button,
        Active,
        AllInactive,
        Animation,
        Transform,
        AddQuitEvent_Button,
        Breathe,
        AddFunc_Button, 
        ActiveBreatheCanvas,
        ActiveVideoCheckButton,
    }

    /// <summary>
    /// VisualType.Object -> Obj 기능 타입
    /// </summary>
    public enum Obj_FuncType
    {
        None,
        Active,
        SkyMatChange, // SceneManager로 기능 이동해야 함
        Transform,
        Transform_Move,
        MoveOn,
        SetBezierPosition,
        AllInactive,
        ChangeLayer,
        EnabledComponent,
        SetParent,
        ChangeCameraView,
    }

    public enum XR_FuncType
    {
        ChangeControllerMat,
        SendHapticInpulse,
        OutlineColorChange,
        UsingStopButton,
    }

    /// <summary>
    /// 씬 관련 기능 타입
    /// </summary>
    public enum SceneMgrType
    {
        Load,
        UnLoad,
        FogSetting,
        CameraView,
        CameraSetting,
        CheckVideoContent_Completed,
        CheckFileDownload,
    }

    public enum TimerType
    {
        Timer_Button,
        Timer_Only,
        RecordTimer_Waiting,
    }

    /// <summary>
    /// UI 애니메이션 타입
    /// </summary>
    public enum UIAnimationType
    {
        Move,
        TextFadeInOut,
        ImageFadeInOut,
        CanvasFadeInOut,
        ChangeImage,
        ChangeText
    }
    /// <summary>
    /// 각 오브젝트들의 타입에 대하여 정의함
    /// </summary>
    public enum ObjType
    {
        Character
    }

    /// <summary>
    /// Audio 설정 타입
    /// </summary>
    public enum AudioSettingType
    {
        Play,
        Stop,
        Volume,
        AllStop
    }

    public enum VideoState
    {
        Load,
        Play,
        Pause,
        Stop,
        Seek,
        Repeat,
        UnRepeat,
        Jump,
        Loop,
        UnLoop,
        Rotation,
        Position,
        SeekAndRepeat,
        Restart,
        SeekFrame,
        End,
        WaitingPopup,
        WaitingEndVideo,
    }

    public enum VideoType
    {
        Main
    }

    public enum SeekState
    {
        Down,
        Drag,
        Up,
        Order,
        Complete
    }

    public enum LoadState
    {
        Fail,
        Succeed,
        Loading
    }

    public enum UICanvasType
    {
        NomalBackground,
        UICanvas,
        FadeInOutCanvas,
        BreatheCanvas,
    }

    public enum GenderType
    {
        Man,
        Woman
    }

}