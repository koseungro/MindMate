using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class ButterflyMovingControl : MonoBehaviour
{
    private static ButterflyMovingControl _instance;
    public static ButterflyMovingControl Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ButterflyMovingControl>();

            return _instance;
        }
    }

    public GameObject Butterfly_Dummy;
    public GameObject Butterfly_Body;
    public GameObject BezierSpline;
    public BezierWalkerWithTime BezierWalkerWithTime;

    //public GameObject Ref_Cam_0;
    //public GameObject Ref_Cam_1;
    //public GameObject Ref_Cam_2;
    //public GameObject Ref_Cam_3;

    private float LerpValueForChangeLocation;
    private float LerpValueForMoving;
    private float LocationValue_0;
    private float LocationValue_1;
    private float LocationValue_2;
    private float LocationValue_3;

    List<GameObject> Ref_Cam_List;


    void Start()
    {
        //Butterfly_Dummy = GameObject.Find("Butterfly_Dummy");
        //Butterfly_Body = GameObject.Find("Butterfly_Body");
        //BezierSpline = GameObject.Find("BezierSpline");
        //BezierWalkerWithTime = (BezierWalkerWithTime)Butterfly_Dummy.GetComponent("BezierWalkerWithTime");

#if UNITY_EDITOR

        //Ref_Cam_0 = GameObject.Find("Ref_Cam_0");
        //Ref_Cam_1 = GameObject.Find("Ref_Cam_1");
        //Ref_Cam_2 = GameObject.Find("Ref_Cam_2");
        //Ref_Cam_3 = GameObject.Find("Ref_Cam_3");

        //Ref_Cam_List = new List<GameObject>
        //{
        //    Ref_Cam_0,
        //    Ref_Cam_1,
        //    Ref_Cam_2,
        //    Ref_Cam_3
        //};

#endif

        //BezierWalkerWithTime.enabled = false;

        BezierWalkerWithTime.NormalizedT = 0.0f;
        LerpValueForChangeLocation = 500.0f;
        LerpValueForMoving = 10.0f;

        #region 씨앗 BezierSpline 위치값 정보 (Bezier 커브 변형시 4개 위치 수치 갱신 필요함)

        //초기 위치
        LocationValue_0 = 0.0f;

        //나뭇잎 발광 위한 비행전 대기 위치
        LocationValue_1 = 0.1f;

        //나뭇잎 흔들림 없애기 위한 비행전 대기 위치
        LocationValue_2 = 0.365f;

        //숲 앞 위치
        LocationValue_3 = 0.835f;

        #endregion

    }

#if UNITY_EDITOR

    //Bezier 애니매이션 테스트용..최종 빌드 전에는 비활성화 처리 필요함.
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Tab))
    //    {
    //        ButterflyMove();
    //    }

    //    else if (Input.GetKeyDown(KeyCode.BackQuote))
    //    {
    //        ActRefCam(0);
    //        SetSeedLocation(LocationValue_0);
    //    }

    //    else if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        ActRefCam(1);
    //        SetSeedLocation(LocationValue_1);
    //    }

    //    else if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        ActRefCam(2);
    //        SetSeedLocation(LocationValue_2);
    //    }

    //    else if (Input.GetKeyDown(KeyCode.Alpha3))
    //    {
    //        ActRefCam(3);
    //        SetSeedLocation(LocationValue_3);
    //    }
    //}

    //임시 카메라 활성화 또는 비활성화용 함수
    void ActRefCam(int num)
    {
        for (int i = 0; i < 4; i++)
        {
            Ref_Cam_List[i].SetActive(false);
        }
        Ref_Cam_List[num].SetActive(true);
    }

#endif

    //Bezier 곡선 위에 씨앗위치를 노멀라이즈(0 ~ 1)된 수치로 설정
    public void SetSeedLocation(float nomaliezdValue)
    {
        StartCoroutine("ButterflyMoveOnBezier", nomaliezdValue);
        ActSeedBody();
    }

    //Bezier 곡선 위에 배치된 씨앗의 이동을 활성화
    public void ButterflyMove()
    {
        BezierWalkerWithTime.enabled = true;
    }

    //Bezier 곡선 위에서 원하는 Location으로 나비(Butterfly)를 이동시키는 코루틴
    private IEnumerator ButterflyMoveOnBezier(float nomaliezdValue)
    {
        Debug.Log($"Butterfly : { nomaliezdValue}");

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

    //나비(Butterfly)가 Bezier 곡선 위 이동 완료 후 Butterfly_Body 오브젝트가 비활성화 되기 때문에 다시 활성화가 필요할 경우 사용
    void ActSeedBody()
    {
        if (!Butterfly_Body.activeSelf)
            Butterfly_Body.SetActive(true);
    }
}
