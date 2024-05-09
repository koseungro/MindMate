using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BezierSolution;
using UnityEditor;

namespace FNI
{
    public class Breathe : MonoBehaviour
    {
        [Serializable]
        public class BreathFactor
        {
            public Image breathImage;
            public float checkTime = 0;
            public bool nextGrowOK = false;
            public bool growFinished = true;
        }

        public float breatheInTime;
        public float breatheOutTime;
        public float breatheHoldTime;

        public AnimationCurve breathInCurve;
        public AnimationCurve breathOutCurve;

        private List<BreathFactor> breathFactors = new List<BreathFactor>();

        private bool isBreathedIn = false;

        private Text breathText;

        public IEnumerator m_Breathe_Routine;

        //public Transform mainCamera;
        //public Transform uiCanvas;
        //public Transform followTarget;


        private void OnEnable()
        {
            //followTarget = FindObjectOfType<BezierWalkerWithTime>().transform;
            //uiCanvas = transform.root.Find("UICanvas").transform;
        }

        void Start()
        {
            Init();

        }

        //private void LateUpdate()
        //{
        //    if (this.gameObject.activeSelf)
        //        transform.localRotation = Quaternion.LookRotation(transform.localPosition - mainCamera.localPosition);
        //}

        //private void FollowTarget()
        //{
        //    uiCanvas.localPosition = mainCamera.localPosition;
        //    Debug.Log($"{uiCanvas.localPosition}");
        //    uiCanvas.localRotation = Quaternion.LookRotation(uiCanvas.localPosition - cameraOffset.localPosition);

        //    float distanceRatio;
        //    if (MathF.Abs((uiCanvas.position.x - cameraOffset.position.x) / 3.3f) < 1)
        //    {
        //        distanceRatio = 1;
        //        //Debug.Log($"<color=yellow> {distanceRatio} </color>");
        //    }
        //    else
        //    {
        //        distanceRatio = MathF.Abs((uiCanvas.position.x - cameraOffset.position.x) / 3.3f);
        //        //Debug.Log($"<color=cyan> {distanceRatio} </color>");
        //    }

        //    //Debug.Log($"<color=magenta> ��� : {distanceRatio} </color>");
        //    transform.localScale = new Vector3(distanceRatio, distanceRatio, distanceRatio);
        //    Vector3.left
        //}

        private void Init()
        {
            breathText = transform.Find("BreathText").GetComponent<Text>();

            Image[] breaths = GetComponentsInChildren<Image>();

            for (int i = 0; i < breaths.Length; i++)
            {
                breathFactors.Add(new BreathFactor { breathImage = breaths[i] });
            }

        }

        /// <summary>
        /// ������ �ڷ�ƾ ����
        /// </summary>
        /// <param name="breatheCnt"> �� ������ Ƚ�� </param>
        /// <param name="isSame"> ���� �ư� ���� �������� ���� </param>
        public void RunBreathCoroutine(int breatheCnt, bool isSame, bool useHoldBreath)
        {
            if (m_Breathe_Routine != null)
                StopCoroutine(m_Breathe_Routine);
            m_Breathe_Routine = RunBreath(breatheCnt, isSame, useHoldBreath);

            StartCoroutine(m_Breathe_Routine);
        }

        /// <summary>
        /// ������ Coroutine
        /// </summary>
        /// <param name="breatheCnt"> ������ ��ƾ ������ Ƚ�� </param>
        /// <returns></returns>
        public IEnumerator RunBreath(int breatheCnt, bool isSame, bool useHoldBreath)
        {
            if (isSame)
                UIForSequence.Instance.IsFinish = true;

            int cnt = breatheCnt;

            yield return new WaitForSeconds(1f);

            //Debug.Log($"<color=cyan> �ʱ� breathe Ƚ�� : {cnt}</color>");

            while (cnt > 0)
            {
                breathText.text = "���̸��ð�";

                for (int i = 0; i < breathFactors.Count; i++)
                {
                    if (i == 0)
                        StartCoroutine(BreathIn(breathFactors[i]));
                    else
                    {
                        yield return new WaitUntil(() => breathFactors[i - 1].nextGrowOK == true);
                        StartCoroutine(BreathIn(breathFactors[i]));
                    }
                }

                yield return new WaitUntil(() => breathFactors[breathFactors.Count - 1].growFinished); // ������ �� Grow ��

                yield return new WaitForSeconds(0.5f);

                if (useHoldBreath)
                {
                    //yield return new WaitForSeconds(breatheHoldTime);

                    float checkTime = 0;
                    while (checkTime <= breatheHoldTime)
                    {
                        checkTime += Time.deltaTime;
                        //Debug.Log(checkTime);
                        yield return null;
                    }
                }

                breathText.text = "������";

                for (int i = breathFactors.Count - 1; i >= 0; i--) // �� for
                {
                    if (i == breathFactors.Count - 1)
                        StartCoroutine(BreathOut(breathFactors[i]));
                    else
                    {
                        yield return new WaitUntil(() => breathFactors[i + 1].nextGrowOK == true);
                        StartCoroutine(BreathOut(breathFactors[i]));
                    }
                }

                yield return new WaitUntil(() => breathFactors[0].growFinished); // ������ �� Grow ��

                yield return new WaitForSeconds(0.5f);

                cnt--;
                //Debug.Log($"<color=cyan> ���� breathe Ƚ�� : {cnt}</color>");
            }

            UIForSequence.Instance.IsFinish = true;
            //Debug.Log($"������ �Ϸ� / {breatheCnt} ȸ");

        }

        /// <summary>
        /// ��� Coroutine
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public IEnumerator BreathIn(BreathFactor factor)
        {
            factor.growFinished = false;
            breathText.gameObject.SetActive(true);

            while (factor.checkTime < 1.0f)
            {
                factor.checkTime += Time.deltaTime / breatheInTime;

                factor.breathImage.rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one*2, breathInCurve.Evaluate(factor.checkTime));

                if (factor.checkTime >= 0.25f)
                    factor.nextGrowOK = true;

                yield return null;
            }

            factor.breathImage.rectTransform.localScale = Vector3.one*2;
            factor.checkTime = 0;
            factor.growFinished = true;
            factor.nextGrowOK = false;
        }

        /// <summary>
        /// ���� Coroutine
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public IEnumerator BreathOut(BreathFactor factor)
        {
            factor.growFinished = false;

            while (factor.checkTime < 1.0f)
            {
                factor.checkTime += Time.deltaTime / breatheOutTime;

                factor.breathImage.rectTransform.localScale = Vector3.Lerp(Vector3.one*2, Vector3.zero, factor.checkTime);

                if (factor.checkTime >= 0.25f)
                    factor.nextGrowOK = true;

                yield return null;
            }

            factor.breathImage.rectTransform.localScale = Vector3.zero;
            factor.checkTime = 0;
            factor.growFinished = true;
            factor.nextGrowOK = false;

            breathText.gameObject.SetActive(false);

        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Breathe))]
    public class BreatheEditor : Editor
    {
        private Breathe _breathe;

        private Breathe Breathe
        {
            get
            {
                if (_breathe == null)
                    _breathe = target as Breathe;

                return _breathe;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();
            if (GUILayout.Button("Test"))
            {

            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Changed Update Mode");
                EditorUtility.SetDirty(Breathe);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
#endif
}
