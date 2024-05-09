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

        #region ���� BezierSpline ��ġ�� ���� (Bezier Ŀ�� ������ 4�� ��ġ ��ġ ���� �ʿ���)

        //�ʱ� ��ġ
        LocationValue_0 = 0.0f;

        //������ �߱� ���� ������ ��� ��ġ
        LocationValue_1 = 0.1f;

        //������ ��鸲 ���ֱ� ���� ������ ��� ��ġ
        LocationValue_2 = 0.365f;

        //�� �� ��ġ
        LocationValue_3 = 0.835f;

        #endregion

    }

#if UNITY_EDITOR

    //Bezier �ִϸ��̼� �׽�Ʈ��..���� ���� ������ ��Ȱ��ȭ ó�� �ʿ���.
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

    //�ӽ� ī�޶� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ�� �Լ�
    void ActRefCam(int num)
    {
        for (int i = 0; i < 4; i++)
        {
            Ref_Cam_List[i].SetActive(false);
        }
        Ref_Cam_List[num].SetActive(true);
    }

#endif

    //Bezier � ���� ������ġ�� ��ֶ�����(0 ~ 1)�� ��ġ�� ����
    public void SetSeedLocation(float nomaliezdValue)
    {
        StartCoroutine("ButterflyMoveOnBezier", nomaliezdValue);
        ActSeedBody();
    }

    //Bezier � ���� ��ġ�� ������ �̵��� Ȱ��ȭ
    public void ButterflyMove()
    {
        BezierWalkerWithTime.enabled = true;
    }

    //Bezier � ������ ���ϴ� Location���� ����(Butterfly)�� �̵���Ű�� �ڷ�ƾ
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

    //����(Butterfly)�� Bezier � �� �̵� �Ϸ� �� Butterfly_Body ������Ʈ�� ��Ȱ��ȭ �Ǳ� ������ �ٽ� Ȱ��ȭ�� �ʿ��� ��� ���
    void ActSeedBody()
    {
        if (!Butterfly_Body.activeSelf)
            Butterfly_Body.SetActive(true);
    }
}
