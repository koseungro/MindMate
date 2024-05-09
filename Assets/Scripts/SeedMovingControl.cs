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

            #region 씨앗 BezierSpline 위치값 정보 (Bezier 커브 변형시 4개 위치 수치 갱신 필요함)

            //바람여행 초기 위치
            LocationValue_0 = 0.0f;

            //1번째 키네틱아트 앞 위치
            LocationValue_1 = 0.0884f;

            //2번째 키네틱아트 앞 위치
            LocationValue_2 = 0.3514f;

            //3번째 키네틱아트 앞 위치
            LocationValue_3 = 0.6261f;

            //마지막 위치
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

        //Bezier 곡선 위에 씨앗위치를 노멀라이즈(0 ~ 1)된 수치로 설정
        public void SetSeedLocation(float normaliezdValue)
        {
            StartCoroutine(SeedMoveOnBezier(normaliezdValue));
            //ActSeedBody();
        }

        //Bezier 곡선 위에 배치된 씨앗의 이동을 활성화
        public void SeedMove()
        {
            BezierWalkerWithTime.enabled = true;
        }

#if UNITY_EDITOR
        //임시 카메라 활성화 또는 비활성화용 함수
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

        //Bezier 곡선 위에서 원하는 Location으로 씨앗(Seed)을 이동시키는 코루틴
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

        //씨앗(Seed)이 Bezier 곡선 위 이동 완료 후 Seed_Body 오브젝트가 비활성화 되기 때문에 다시 활성화가 필요할 경우 사용
        void ActSeedBody()
        {
            if (!Seed_Body.activeSelf)
                Seed_Body.SetActive(true);
        }
    }
}
