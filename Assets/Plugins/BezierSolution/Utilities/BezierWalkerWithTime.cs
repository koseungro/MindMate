﻿using UnityEngine;
using UnityEngine.Events;

namespace BezierSolution
{
    [AddComponentMenu("Bezier Solution/Bezier Walker With Time")]
    [HelpURL("https://github.com/yasirkula/UnityBezierSolution")]
    public class BezierWalkerWithTime : BezierWalker
    {
        #region Singleton
        private static BezierWalkerWithTime _instance;
        public static BezierWalkerWithTime Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<BezierWalkerWithTime>();

                return _instance;
            }
        }
        #endregion

        [SerializeField]
        private bool moveOn = false;
        public bool MoveOn { get => moveOn; set => moveOn = value; }

        public BezierSpline spline;
        public TravelMode travelMode;

        public float travelTime = 5f;
        [SerializeField]
        [Range(0f, 1f)]
        private float m_normalizedT = 0f;

        public bool highQuality = false;

        public override BezierSpline Spline { get { return spline; } }

        public override float NormalizedT
        {
            get { return m_normalizedT; }
            set { m_normalizedT = value; }
        }

        public float movementLerpModifier = 10f;
        public float rotationLerpModifier = 10f;

        public LookAtMode lookAt = LookAtMode.Forward;

        private bool isGoingForward = true;
        public override bool MovingForward { get { return isGoingForward; } }

        public UnityEvent onPathCompleted = new UnityEvent();
        private bool onPathCompletedCalledAt1 = false;
        private bool onPathCompletedCalledAt0 = false;

        private void Update()
        {
            if (moveOn)
            {
                Execute(Time.deltaTime);

            }
        }

        public override void Execute(float deltaTime)
        {
            float _normalizedT = highQuality ? spline.evenlySpacedPoints.GetNormalizedTAtPercentage(m_normalizedT) : m_normalizedT;

            transform.position = Vector3.Lerp(transform.position, spline.GetPoint(_normalizedT), movementLerpModifier * deltaTime);

            if (lookAt == LookAtMode.Forward)
            {
                BezierSpline.Segment segment = spline.GetSegmentAt(_normalizedT);
                Quaternion targetRotation;
                if (isGoingForward)
                    targetRotation = Quaternion.LookRotation(segment.GetTangent(), segment.GetNormal());
                else
                    targetRotation = Quaternion.LookRotation(-segment.GetTangent(), segment.GetNormal());

                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationLerpModifier * deltaTime);
            }
            else if (lookAt == LookAtMode.SplineExtraData)
                transform.rotation = Quaternion.Lerp(transform.rotation, spline.GetExtraData(_normalizedT, extraDataLerpAsQuaternionFunction), rotationLerpModifier * deltaTime);

            if (isGoingForward)
            {
                m_normalizedT += deltaTime / travelTime;

                if (m_normalizedT > 1f)
                {
                    if (travelMode == TravelMode.Once)
                        m_normalizedT = 1f;
                    else if (travelMode == TravelMode.Loop)
                        m_normalizedT -= 1f;
                    else
                    {
                        m_normalizedT = 2f - m_normalizedT;
                        isGoingForward = false;
                    }

                    if (!onPathCompletedCalledAt1)
                    {
                        onPathCompletedCalledAt1 = true;
#if UNITY_EDITOR
                        if (UnityEditor.EditorApplication.isPlaying)
#endif
                            onPathCompleted.Invoke();
                    }
                }
                else
                {
                    onPathCompletedCalledAt1 = false;
                }
            }
            else
            {
                m_normalizedT -= deltaTime / travelTime;

                if (m_normalizedT < 0f)
                {
                    if (travelMode == TravelMode.Once)
                        m_normalizedT = 0f;
                    else if (travelMode == TravelMode.Loop)
                        m_normalizedT += 1f;
                    else
                    {
                        m_normalizedT = -m_normalizedT;
                        isGoingForward = true;
                    }

                    if (!onPathCompletedCalledAt0)
                    {
                        onPathCompletedCalledAt0 = true;
#if UNITY_EDITOR
                        if (UnityEditor.EditorApplication.isPlaying)
#endif
                            onPathCompleted.Invoke();
                    }
                }
                else
                {
                    onPathCompletedCalledAt0 = false;
                }
            }
        }
    }
}