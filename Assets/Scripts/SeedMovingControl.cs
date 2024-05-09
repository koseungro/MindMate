using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

namespace FNI
{
    public class SeedMovingControl : MonoBehaviour
    {
        private static SeedMovingControl _instance;
        public static SeedMovingControl Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<SeedMovingControl>();

                return _instance;
            }
        }

        public GameObject Seed_Dummy;
        public GameObject Seed_Body;
        public GameObject BezierSpline;
        public BezierWalkerWithTime BezierWalkerWithTime;

        //public GameObject Ref_Cam_0;
        //public GameObject Ref_Cam_1;
        //public GameObject Ref_Cam_2;
        //public GameObject Ref_Cam_3;
        //public GameObject Ref_Cam_4;

        private float LerpValueForChangeLocation;
        private float LerpValueForMoving;
        private float LocationValue_0;
        private float LocationValue_1;
        private float LocationValue_2;
        private float LocationValue_3;
        private float LocationValue_4;

        List<GameObject> Ref_Cam_List;


        void Start()
        {

#if UNITY_EDITOR

            //Ref_Cam_0 = GameObject.Find("Ref_Cam_0");
            //Ref_Cam_1 = GameObject.Find("Ref_Cam_1");
            //Ref_Cam_2 = GameObject.Find("Ref_Cam_2");
            //Ref_Cam_3 = GameObject.Find("Ref_Cam_3");
            //Ref_Cam_4 = GameObject.Find("Ref_Cam_4");

            //Ref_Cam_List = new List<GameObject>();

            //Ref_Cam_List.Add(Ref_Cam_0);
            //Ref_Cam_List.Add(Ref_Cam_1);
            //Ref_Cam_List.Add(Ref_Cam_2);
            //Ref_Cam_List.Add(Ref_Cam_3);
            //Ref_Cam_List.Add(Ref_Cam_4);

#endif

            //BezierWalkerWithTime.enabled = false;

            BezierWalkerWithTime.NormalizedT = 0.0f;
            LerpValueForChangeLocation = 500.0f;
            LerpValueForMoving = 10.0f;

            #region ���� BezierSpline ��ġ�� ���� (Bezier Ŀ�� ������ 4�� ��ġ ��ġ ���� �ʿ���)

            //�ٶ����� �ʱ� ��ġ
            LocationValue_0 = 0.0f;

            //1��° Ű��ƽ��Ʈ �� ��ġ
            LocationValue_1 = 0.0884f;

            //2��° Ű��ƽ��Ʈ �� ��ġ
            LocationValue_2 = 0.3514f;

            //3��° Ű��ƽ��Ʈ �� ��ġ
            LocationValue_3 = 0.6261f;

            //������ ��ġ
            LocationValue_4 = 0.8769f;

            #endregion

        }
        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Tab))
        //    {
        //        SeedMove();
        //    }

        //    else if (Input.GetKeyDown(KeyCode.Alpha1))
        //    {
        //        SetSeedLocation(LocationValue_1);
        //    }

        //}

        //Bezier � ���� ������ġ�� ��ֶ�����(0 ~ 1)�� ��ġ�� ����
        public void SetSeedLocation(float normaliezdValue)
        {
            StartCoroutine(SeedMoveOnBezier(normaliezdValue));
            //ActSeedBody();
        }

        //Bezier � ���� ��ġ�� ������ �̵��� Ȱ��ȭ
        public void SeedMove()
        {
            BezierWalkerWithTime.enabled = true;
        }

#if UNITY_EDITOR
        //�ӽ� ī�޶� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ�� �Լ�
        void ActRefCam(int num)
        {
            for (int i = 0; i < 5; i++)
            {
                Ref_Cam_List[i].SetActive(false);
            }
            Ref_Cam_List[num].SetActive(true);
        }

#endif
        private IEnumerator SetSeedPosition(float nomalizedValue)
        {
            Debug.Log(nomalizedValue);

            BezierWalkerWithTime.enabled = false;
            yield return new WaitForSeconds(0.5f);
            BezierWalkerWithTime.NormalizedT = nomalizedValue;
            BezierWalkerWithTime.enabled = true;
        }

        //Bezier � ������ ���ϴ� Location���� ����(Seed)�� �̵���Ű�� �ڷ�ƾ
        private IEnumerator SeedMoveOnBezier(float nomaliezdValue)
        {
            Debug.Log($"Seed : { nomaliezdValue}");

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

        //����(Seed)�� Bezier � �� �̵� �Ϸ� �� Seed_Body ������Ʈ�� ��Ȱ��ȭ �Ǳ� ������ �ٽ� Ȱ��ȭ�� �ʿ��� ��� ���
        void ActSeedBody()
        {
            if (!Seed_Body.activeSelf)
                Seed_Body.SetActive(true);
        }
    }
}
