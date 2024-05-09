/// 작성자: 고승로
/// 작성일: 2023-03-21
/// 수정일: 2023-03-21
/// 저작권: Copyright(C) FNI Co., LTD.
/// 수정이력
/// 

using FNI;
using FNI.Common.Utils;

using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using BezierSolution;

/// <summary>
/// Bezier 움직임 위치 제어 클래스
/// </summary>
public class ObjectMovingControll : MonoBehaviour
{
    #region Singleton
    private static ObjectMovingControll _instance;
    public static ObjectMovingControll Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ObjectMovingControll>();
                if (_instance == null)
                    Debug.LogError("ObjectMovingControll를 찾을 수 없습니다. ");
            }
            return _instance;
        }
    }
    #endregion

    private BezierWalkerWithTime BezierWalkerWithTime;
    public float BezierPosion { get => BezierWalkerWithTime.NormalizedT; } 

    private float LerpValueForChangeLocation;
    private float LerpValueForMoving;

    #region Unity base method

    private void OnEnable()
    {
        BezierWalkerWithTime = FindObjectOfType<BezierWalkerWithTime>();
    }

    private void Start()
    {
        BezierWalkerWithTime.NormalizedT = 0.0f;
        LerpValueForChangeLocation = 500.0f;
        LerpValueForMoving = 10.0f;
    }

    //Bezier 곡선 위에 오브젝트 위치를 노멀라이즈(0 ~ 1)된 수치로 설정
    public void SetObjectLocation(float normaliezdValue)
    {
        if (BezierWalkerWithTime.Instance.MoveOn)
            StartCoroutine(ObjectMoveOnBezier(normaliezdValue));
        
        if(!BezierWalkerWithTime.Instance.MoveOn && normaliezdValue != 0)
            Debug.Log($"<color=red>MoveOn : [{BezierWalkerWithTime.Instance.MoveOn}] => true이여야 함.</color>");
    }

    private IEnumerator ObjectMoveOnBezier(float nomaliezdValue)
    {
        Debug.Log($"Object Bezeier Value : { nomaliezdValue}");

        BezierWalkerWithTime.movementLerpModifier = LerpValueForChangeLocation;
        BezierWalkerWithTime.rotationLerpModifier = LerpValueForChangeLocation;
        yield return null;
        BezierWalkerWithTime.NormalizedT = nomaliezdValue;
        yield return null;
        BezierWalkerWithTime.enabled = true;
        yield return null;
        BezierWalkerWithTime.enabled = false;
        yield return null;
        BezierWalkerWithTime.movementLerpModifier = LerpValueForMoving;
        BezierWalkerWithTime.rotationLerpModifier = LerpValueForMoving;
        yield return null;
        BezierWalkerWithTime.enabled = true;

    }

    #endregion
}
