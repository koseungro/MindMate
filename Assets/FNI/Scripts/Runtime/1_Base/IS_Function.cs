using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Reflection;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using System.Text;
using System.Text.RegularExpressions;

namespace FNI.IS
{
    #region Enum
    public enum AxisType
    {
        X,
        Y,
        Z
    }
    /// <summary>
    /// 메트리얼의 모드목록입니다.
    /// </summary>
    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public enum DigitType
    {
        One = 10,
        Two = 100,
        Three = 1000,
        Four = 10000,
        Five = 100000,
    }

    public enum ColorList
    {
        _Color,
        _EmissionColor,
        _SpectColor
    }
    /// <summary>
    /// 삼각함수 값 종류
    /// </summary>
    public enum TriType
    {
        밑변_A,
        높이_B,
        빗변_C,
        AC_a,
        AB_c,
        BC_b
    }
    #endregion

    #region Struct
    /// <summary>
    /// Mathf에 없는 함수들입니다.
    /// </summary>
    public struct MathIS
    {
        public static Vector3 Axis(AxisType value)
        {
            switch (value)
            {
                case AxisType.X: return Vector3.right;
                case AxisType.Y: return Vector3.up;
                default: return Vector3.forward;
            }
        }
        /// <summary>
        /// Vector3를 소수점 4자리로 변환하서 문자화 합니다.
        /// </summary>
        /// <param name="forwardText">앞에 더해질 문자</param>
        /// <param name="value">값</param>
        /// <param name="behindText">뒤에 더해질 문자</param>
        /// <returns></returns>
        public static string Vector3ToString(string forwardText, Vector3 value, string behindText)
        {
            return $"{forwardText}{value.x:0.0000},{value.y:0.0000},{value.z:0.0000}{behindText}";
        }
        /// <summary>
        /// 두 백터 간의 각도를 구합니다.
        /// </summary>
        /// <param name="vStart">시작</param>
        /// <param name="vEnd">종료, 현재</param>
        /// <returns></returns>
        public static float GetAngle(Vector3 vStart, Vector3 vEnd)
        {
            Vector3 v = vEnd - vStart;

            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }
        /// <summary>
        /// 각도를 -180~180으로 변경해줍니다.
        /// 짐벌락을 방지합니다.
        /// </summary>
        /// <param name="angle">현재 각도</param>
        /// <returns></returns>
        public static float Angle(float angle)
        {
            return 180 < angle ? angle - 360 : angle;
        }
        /// <summary>
        /// 각도를 -180~180으로 변경해줍니다.
        /// 짐벌락을 방지합니다.
        /// </summary>
        /// <param name="angle">현재 각도</param>
        /// <returns></returns>
        public static Vector2 Angle(Vector2 angle)
        {
            return new Vector2(Angle(angle.x), Angle(angle.y));
        }
        /// <summary>
        /// 각도를 -180~180으로 변경해줍니다.
        /// 짐벌락을 방지합니다.
        /// </summary>
        /// <param name="angle">현재 각도</param>
        /// <returns></returns>
        public static Vector3 Angle(Vector3 angle)
        {
            return new Vector3(Angle(angle.x), Angle(angle.y), Angle(angle.z));
        }
        /// <summary>
        /// Vector3를 수직(y)를 제외하여 Vector2로 변경하여 리턴합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2 ConvertV2(Vector3 value)
        {
            return new Vector2(value.x, value.z);
        }
        /// <summary>
        /// Vector3를 수직(y)를 제외하여 Vector2로 변경하여 리턴합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2 ConvertUIV2(Vector3 value)
        {
            return new Vector2(value.x, value.y);
        }

        internal static float Contain_0To360(float angle)
        {
            if (angle < 0)
                angle += 360;
            if (360 <= angle)
                angle %= 360;

            return angle;
        }

        /// <summary>
        /// 두 쿼터니언의 더한 값을 구합니다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Quaternion AddQuaternion(Quaternion a, Quaternion b)
        {
            return a * b;
        }
        /// <summary>
        /// target의 로컬 쿼터니언 값을 구합니다.
        /// </summary>
        /// <param name="parent">로컬의 기준</param>
        /// <param name="target">로컬로 변경하고자 하는 대상 값</param>
        /// <returns></returns>
        public static Quaternion GetLocalQuaternion(Quaternion parent, Quaternion target)
        {
            return Quaternion.Inverse(parent) * target;
        }

        /// <summary>
        /// float을 int로 변경하여 뒷자리의 오차를 줄입니다.
        /// </summary>
        /// <param name="value">원 값</param>
        /// <param name="unit">단위 변경</param>
        /// <returns></returns>
        public static int FtoInt(float value, int unit)
        {
            return (int)(value * unit);
        }

        /// <summary>
        /// float을 지정한 자릿수 만큼 남기고 자릅니다.
        /// </summary>
        /// <param name="value">자를 숫자</param>
        /// <param name="tenSquared">자를 자릿수</param>
        /// <returns></returns>
        public static float Cut(float value, int tenSquared)
        {
            int pow = (int)Mathf.Pow(10, tenSquared);
            return (float)((int)(value * pow)) / pow;
        }

        /// <summary>
        /// Vector3을 지정한 자릿수 만큼 남기고 자릅니다.
        /// </summary>
        /// <param name="value">자를 숫자</param>
        /// <param name="tenSquared">자를 자릿수</param>
        /// <returns></returns>
        public static Vector2 Cut(Vector2 value, int tenSquared)
        {
            return new Vector2(Cut(value.x, tenSquared), Cut(value.y, tenSquared));
        }

        /// <summary>
        /// Vector3을 지정한 자릿수 만큼 남기고 자릅니다.
        /// </summary>
        /// <param name="value">자를 숫자</param>
        /// <param name="tenSquared">자를 자릿수</param>
        /// <returns></returns>
        public static Vector3 Cut(Vector3 value, int tenSquared)
        {
            return new Vector3(Cut(value.x, tenSquared), Cut(value.y, tenSquared), Cut(value.z, tenSquared));
        }

        /// <summary>
        /// Quaternion을 지정한 자릿수 만큼 남기고 자릅니다.
        /// </summary>
        /// <param name="value">자를 숫자</param>
        /// <param name="tenSquared">자를 자릿수</param>
        /// <returns></returns>
        public static Quaternion Cut(Quaternion value, int tenSquared)
        {
            return new Quaternion(Cut(value.x, tenSquared), Cut(value.y, tenSquared), Cut(value.z, tenSquared), Cut(value.w, tenSquared));
        }
    }
    /// <summary>
    /// 삼각함수
    /// </summary>
    public struct TriFunc
    {
        public static float Get(TriType get, (TriType, float) A, (TriType, float) B, bool isRadian = true)
        {
            switch (get)
            {
                case TriType.밑변_A:
                         if (A.Item1 == TriType.빗변_C && B.Item1 == TriType.AC_a)   return GetBase1(A.Item2, B.Item2);
                    else if (A.Item1 == TriType.높이_B && B.Item1 == TriType.AC_a)   return GetBase2(A.Item2, B.Item2);
                    else if (A.Item1 == TriType.빗변_C && B.Item1 == TriType.높이_B) return GetBase3(A.Item2, B.Item2);
                    else if (B.Item1 == TriType.빗변_C && A.Item1 == TriType.AC_a)   return GetBase1(B.Item2, A.Item2);
                    else if (B.Item1 == TriType.높이_B && A.Item1 == TriType.AC_a)   return GetBase2(B.Item2, A.Item2);
                    else if (B.Item1 == TriType.빗변_C && A.Item1 == TriType.높이_B) return GetBase3(B.Item2, A.Item2);
                    else
                        throw new ISFuncException("TriFunc.Get", "지원되는 공식이 없습니다.");
                case TriType.높이_B:
                         if (A.Item1 == TriType.빗변_C && B.Item1 == TriType.AC_a)   return GetHeight1(A.Item2, B.Item2);
                    else if (A.Item1 == TriType.밑변_A && B.Item1 == TriType.AC_a)   return GetHeight1(A.Item2, B.Item2);
                    else if (A.Item1 == TriType.빗변_C && B.Item1 == TriType.밑변_A) return GetHeight1(A.Item2, B.Item2);

                    else if (B.Item1 == TriType.빗변_C && A.Item1 == TriType.AC_a)   return GetHeight1(B.Item2, A.Item2);
                    else if (B.Item1 == TriType.밑변_A && A.Item1 == TriType.AC_a)   return GetHeight1(B.Item2, A.Item2);
                    else if (B.Item1 == TriType.빗변_C && A.Item1 == TriType.밑변_A) return GetHeight1(B.Item2, A.Item2);
                    else
                        throw new ISFuncException("TriFunc.Get", "지원되는 공식이 없습니다.");
                case TriType.빗변_C:
                         if (A.Item1 == TriType.밑변_A && B.Item1 == TriType.AC_a)   return GetHypotenuse1(A.Item2, B.Item2);
                    else if (A.Item1 == TriType.밑변_A && B.Item1 == TriType.높이_B) return GetHypotenuse1(A.Item2, B.Item2);
                    else if (A.Item1 == TriType.높이_B && B.Item1 == TriType.AC_a)   return GetHypotenuse1(A.Item2, B.Item2);

                    else if (B.Item1 == TriType.밑변_A && A.Item1 == TriType.AC_a)   return GetHypotenuse1(B.Item2, A.Item2);
                    else if (B.Item1 == TriType.밑변_A && A.Item1 == TriType.높이_B) return GetHypotenuse1(B.Item2, A.Item2);
                    else if (B.Item1 == TriType.높이_B && A.Item1 == TriType.AC_a)   return GetHypotenuse1(B.Item2, A.Item2);
                    else
                        throw new ISFuncException("TriFunc.Get", "지원되는 공식이 없습니다.");
                case TriType.AC_a:
                         if (A.Item1 == TriType.높이_B && B.Item1 == TriType.밑변_A) return GetAngle_a1(A.Item2, B.Item2, isRadian);
                    else if (A.Item1 == TriType.밑변_A && B.Item1 == TriType.빗변_C) return GetAngle_a2(A.Item2, B.Item2, isRadian);
                    else if (A.Item1 == TriType.높이_B && B.Item1 == TriType.빗변_C) return GetAngle_a3(A.Item2, B.Item2, isRadian);

                    else if (B.Item1 == TriType.높이_B && A.Item1 == TriType.밑변_A) return GetAngle_a1(B.Item2, A.Item2, isRadian);
                    else if (B.Item1 == TriType.밑변_A && A.Item1 == TriType.빗변_C) return GetAngle_a2(B.Item2, A.Item2, isRadian);
                    else if (B.Item1 == TriType.높이_B && A.Item1 == TriType.빗변_C) return GetAngle_a3(B.Item2, A.Item2, isRadian);
                    else
                        throw new ISFuncException("TriFunc.Get", "지원되는 공식이 없습니다.");
                default: throw new ISFuncException("TriFunc.Get", "지원되는 공식이 없습니다.");
            }
        }

        /// <summary>
        /// 밑변과 빗변사이의 각도를 구하는 공식
        /// </summary>
        /// <param name="heightLength">높이</param>
        /// <param name="baseLength">밑변</param>
        /// <returns></returns>
        public static float GetAngle_a1(float heightLength, float baseLength, bool isRadian = true)
        {
            return Mathf.Atan(heightLength / baseLength) * (isRadian ? 1 : Mathf.Rad2Deg);
        }
        /// <summary>
        /// 밑변과 빗변사이의 각도를 구하는 공식
        /// </summary>
        /// <param name="baseLength">밑변</param>
        /// <param name="hypotenuseLenght">빗변</param>
        /// <returns></returns>
        public static float GetAngle_a2(float baseLength, float hypotenuseLenght, bool isRadian = true)
        {
            return Mathf.Acos(baseLength / hypotenuseLenght) * (isRadian ? 1 : Mathf.Rad2Deg);
        }
        /// <summary>
        /// 밑변과 빗변사이의 각도를 구하는 공식
        /// </summary>
        /// <param name="heightLength">높이</param>
        /// <param name="hypotenuseLenght">빗변</param>
        /// <returns></returns>
        public static float GetAngle_a3(float heightLength, float hypotenuseLenght, bool isRadian = true)
        {
            return Mathf.Asin(heightLength / hypotenuseLenght) * (isRadian ? 1 : Mathf.Rad2Deg);
        }

        /// <summary>
        /// 밑변의 길이를 구합니다.
        /// </summary>
        /// <param name="hypotenuseLenght">빗변</param>
        /// <param name="a_Angle">밑변과 빗변사이의 각도(a, Radian)</param>
        /// <returns></returns>
        public static float GetBase1(float hypotenuseLenght, float a_Angle)
        {
            return hypotenuseLenght * Mathf.Cos(a_Angle);
        }
        /// <summary>
        /// 밑변의 길이를 구합니다.
        /// </summary>
        /// <param name="heightLength">높이</param>
        /// <param name="a_Angle">밑변과 빗변사이의 각도(a, Radian)</param>
        /// <returns></returns>
        public static float GetBase2(float heightLength, float a_Angle)
        {
            return heightLength / Mathf.Tan(a_Angle);
        }
        /// <summary>
        /// 밑변의 길이를 구합니다.
        /// </summary>
        /// <param name="hypotenuseLenght">빗변</param>
        /// <param name="heightLength">높이</param>
        /// <returns></returns>
        public static float GetBase3(float hypotenuseLenght, float heightLength)
        {
            return Mathf.Sqrt((hypotenuseLenght * hypotenuseLenght) - (heightLength * heightLength));
        }

        /// <summary>
        /// 높이를 구합니다.
        /// </summary>
        /// <param name="hypotenuseLenght">빗변</param>
        /// <param name="a_Angle">밑변과 빗변사이의 각도(a, Radian)</param>
        /// <returns></returns>
        public static float GetHeight1(float hypotenuseLenght, float a_Angle)
        {
            return hypotenuseLenght * Mathf.Sin(a_Angle);
        }
        /// <summary>
        /// 높이를 구합니다.
        /// </summary>
        /// <param name="baseLength">밑변</param>
        /// <param name="a_Angle">밑변과 빗변사이의 각도(a, Radian)</param>
        /// <returns></returns>
        public static float GetHeight2(float baseLength, float a_Angle)
        {
            return baseLength * Mathf.Tan(a_Angle);
        }
        /// <summary>
        /// 높이를 구합니다.
        /// </summary>
        /// <param name="hypotenuseLenght">빗변</param>
        /// <param name="baseLength">밑변</param>
        /// <returns></returns>
        public static float GetHeight3(float hypotenuseLenght, float baseLength)
        {
            return Mathf.Sqrt((hypotenuseLenght * hypotenuseLenght) - (baseLength * baseLength));
        }

        /// <summary>
        /// 빗변의 길이를 구합니다.
        /// </summary>
        /// <param name="baseLength">밑변</param>
        /// <param name="a_Angle">밑변과 빗변사이의 각도(a, Radian)</param>
        /// <returns></returns>
        public static float GetHypotenuse1(float baseLength, float a_Angle)
        {
            return baseLength / Mathf.Cos(a_Angle);
        }
        /// <summary>
        /// 빗변의 길이를 구합니다.
        /// </summary>
        /// <param name="baseLength">밑변</param>
        /// <param name="heightLength">높이</param>
        /// <returns></returns>
        public static float GetHypotenuse2(float baseLength, float heightLength)
        {
            return Mathf.Sqrt((baseLength * baseLength) + (heightLength * heightLength));
        }
        /// <summary>
        /// 빗변의 길이를 구합니다.
        /// </summary>
        /// <param name="heightLength">높이</param>
        /// <param name="a_Angle">밑변과 빗변사이의 각도(a, Radian)</param>
        /// <returns></returns>
        public static float GetHypotenuse3(float heightLength, float a_Angle)
        {
            return heightLength / Mathf.Cos(a_Angle);
        }
    }
    public struct RECT
    {
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int X
        {
            get
            {
                return Left;
            }
            set
            {
                Right -= (Left - value);
                Left = value;
            }
        }

        public int Y
        {
            get
            {
                return Top;
            }
            set
            {
                Bottom -= (Top - value);
                Top = value;
            }
        }

        public int Height
        {
            get
            {
                return Bottom - Top;
            }
            set
            {
                Bottom = value + Top;
            }
        }

        public int Width
        {
            get
            {
                return Right - Left;
            }
            set
            {
                Right = value + Left;
            }
        }
    }
    /// <summary>
    /// 다중 배열을 인스펙터에 표시하기 위한 용도입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public struct IS_Array<T>
    {
        public T[] array;

        public IS_Array(int arraySize)
        {
            this.array = new T[arraySize];
        }
    }

    /// <summary>
    /// 최소값 최대값
    /// </summary>
    [System.Serializable]
    public struct MinMax
    {
        [SerializeField]
        private float m_min;
        [SerializeField]
        private float m_max;

        /// <summary>
        /// percent가 적용된 최소값을 가져오고 설정은 m_min값만 설정
        /// </summary>
        public float Min { get => m_min; }
        /// <summary>
        /// percent가 적용된 최대값을 가져오고 설정은 m_max값만 설정
        /// </summary>
        public float Max { get => m_max; }
        /// <summary>
        /// 최소 최대값의 가운데 값을 반환 합니다.
        /// </summary>
        public float Center { get { return (m_max + m_min) / 2f; } }
        /// <summary>
        /// min부터 max까지의 길이 입니다.
        /// </summary>
        public float Length { get { return Max - Min; } }

        /// <summary>
        /// 최소 값과 최대값을 설정합니다.
        /// Percent의 값을 기준으로 값을 반환합니다.
        /// </summary>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public MinMax(float min = 0, float max = 0)
        {
            m_min = min;
            m_max = max;
        }

        /// <summary>
        /// percent가 적용된 Min, Max를 기준으로 렌덤한 값을 출력함
        /// </summary>
        /// <returns></returns>
        public float GetRandom()
        {
            return UnityEngine.Random.Range(Min, Max);
        }
        /// <summary>
        /// value가 Min Max사이 어디 쯤 있는지 %로 계산함
        /// </summary>
        /// <param name="value">확인할 값</param>
        /// <returns></returns>
        public float GetPercent(float value)
        {
            float percent = (value - Min) / (Max - Min);

            return (float)Math.Round(percent, 4);
        }
        /// <summary>
        /// value가 Min Max사이 어디 쯤 있는지 %로 계산함
        /// </summary>
        /// <param name="value">확인할 값</param>
        /// <returns></returns>
        public float GetAnglePercent(float value)
        {
            float percent = (MathIS.Angle(value) - Min) / (Max - Min);

            return (float)Math.Round(percent, 4);
        }
        /// <summary>
        /// 0~1의 값을 min~max사이값으로 변경하여 반환합니다.
        /// </summary>
        /// <param name="percent">0~1의 값</param>
        /// <returns></returns>
        public float PercentToValue(float percent)
        {
            return Mathf.Lerp(Min, Max, percent);
        }
        /// <summary>
        /// value가 min max를 제외하고 안에 포함 되는 값인지 확인한다.
        /// </summary>
        /// <param name="value">값</param>
        /// <returns>
        /// [T]min max 사이값이다.
        /// [F]min max 사이값이 아니다.
        /// </returns>
        public bool Contain(float value, bool withMin = false, bool withMax = false)
        {
            return (withMin ? (m_min <= value) : (m_min < value)) && 
                   (withMax ? (value <= m_max) : (value < m_max));
        }
        /// <summary>
        /// value가 min max를 제외하고 안에 포함 되는 값인지 확인한다.
        /// </summary>
        /// <param name="value">값</param>
        /// <param name="less">최소값보다 작은가</param>
        /// <param name="over">최대값보다 큰가</param>
        public void Contain(float value, out bool less, out bool over, bool withMin = false, bool withMax = false)
        {
            less = withMin ? (value <= m_min) : (value < m_min);
            over = withMax ? (m_max <= value) : (m_max < value);
        }
        /// <summary>
        /// value가 min max를 제외하고 안에 포함 되는 값인지 확인한다.
        /// </summary>
        /// <param name="value">값</param>
        /// <returns>
        /// [-1]min보다 작다
        /// [0]min max 사이값
        /// [1]max보다 크다
        /// </returns>
        public int Contain2(float value, bool withMin = false, bool withMax = false)
        {
                 if (withMin ? (value <= m_min) : (value < m_min)) return -1;
            else if (withMax ? (m_max <= value) : (m_max < value)) return  1;
            else                                                   return  0;
        }
        /// <summary>
        /// value를 min max 사이의 값으로 변환
        /// </summary>
        /// <param name="value">값</param>
        /// <returns></returns>
        public float Clamp(float value, bool withMin = false, bool withMax = false)
        {
                 if (withMin ? (value <= m_min) : (value < m_min)) return m_min;
            else if (withMax ? (m_max <= value) : (m_max < value)) return m_max;
            else                                                   return value;
        }
        /// <summary>
        /// value를 min값을 기준으로 양수와 음수 사이값에 들어오도록 clamp한 값입니다.
        /// </summary>
        /// <param name="value">값</param>
        /// <returns></returns>
        public float ClampMin(float value, bool withMin = false, bool withMax = false)
        {
                 if (withMin ? (value <= -m_min) : (value < -m_min)) return -m_min;
            else if (withMax ? (m_min <= value) : (m_min < value))   return m_min;
            else                                                     return value;
        }
        /// <summary>
        /// value를 max값을 기준으로 양수와 음수 사이값에 들어오도록 clamp한 값입니다.
        /// </summary>
        /// <param name="value">값</param>
        /// <returns></returns>
        public float ClampMax(float value, bool withMin = false, bool withMax = false)
        {
                 if (withMin ? (value <= -m_max) : (value < -m_max)) return -m_max;
            else if (withMax ? (m_max <= value) : (m_max < value))   return m_max;
            else                                                     return value;
        }
        /// <summary>
        /// Min=>Max로 percent에 맞춰 값을 출력합니다.
        /// </summary>
        /// <param name="percent">0~1의 값</param>
        /// <returns></returns>
        public float Lerp(float percent)
        {
            return Mathf.Lerp(Min, Max, percent);
        }
    }
    #endregion

    #region Function Utils
    public static class GameObjectUtils
    {
        #region Utils
        #endregion

        #region Extention
        public static void ChangeLayer(this GameObject target, int layerNum)
        {
            target.layer = layerNum;

            int childCount = target.transform.childCount;

            if (0 != childCount)
            {
                for (int cnt = 0; cnt < childCount; cnt++)
                {
                    Transform child = target.transform.GetChild(cnt);

                    if (child.childCount != 0)
                    {
                        ChangeLayer(child.gameObject, layerNum);
                    }
                    else
                        child.gameObject.layer = layerNum;
                }
            }
        }
        public static void ChangeLayer(this GameObject target, string layerName)
        {
            target.ChangeLayer(LayerMask.NameToLayer(layerName));
        }
        #endregion
    }
    public static class TransformUtils
    {
        #region Utils
        /// <summary>
        /// target의 Hierarchy경로를 알아내어 전달합니다.
        /// </summary>
        /// <param name="target">경로를 찾을 대상</param>
        /// <returns>찾은 경로</returns>
        public static string GetRoot(Transform target, string stopName = "", bool containStop = false)
        {
            string name = target.name;

            if (target.parent != null)
            {
                if (containStop ? target.name != stopName : target.parent.name != stopName)
                    name = GetRoot(target.parent, stopName) + "/" + name;
            }
            return name;
        }
        /// <summary>
        /// 다른 형식의 배열을 Transform배열로 변환
        /// </summary>
        /// <typeparam name="T">입력 될 컴포넌트</typeparam>
        /// <param name="target">입력될 배열</param>
        /// <returns></returns>
        public static Transform[] ArrayComponentToTransforms<T>(T[] target) where T : Component
        {
            Transform[] a = new Transform[target.Length];
            for (int cnt = 0; cnt < a.Length; cnt++)
            {
                a[cnt] = target[cnt].transform;
            }

            return a;
        }
        /// <summary>
        /// list에 parent의 모든 자녀를 넣어 준다.
        /// </summary>
        /// <param name="list">자녀목록을 넣을 대상</param>
        /// <param name="parent">자녀를 찾을 부모</param>
        public static void GetAllChild(List<Transform> list, Transform parent)
        {
            if (parent.childCount == 0)
            {
                list.Add(parent);
            }
            else
            {
                for (int cnt = 0; cnt < parent.childCount; cnt++)
                {
                    Transform child = parent.GetChild(cnt);
                    GetAllChild(list, child);
                    list.Add(child);
                }
            }
        }
        /// <summary>
        /// T 컴퍼넌트를 가진 모든 자녀를 찾습니다. 자녀가 숨겨져 있어도 찾습니다.
        /// </summary>
        /// <typeparam name="T">찾을 타입</typeparam>
        /// <param name="list">찾은 데이터를 넣을 곳</param>
        /// <param name="parent">찾을 부모</param>
        public static void GetComponentsInChildren<T>(List<T> list, Transform parent) where T : Component
        {
            if (list == null)
                list = new List<T>();

            T find = parent.GetComponent<T>();

            if (find != null)
            {
                list.Add(find);
            }

            for (int cnt = 0; cnt < parent.childCount; cnt++)
            {
                GetComponentsInChildren(list, parent.GetChild(cnt));
            }
        }
        public static T GetCompnent<T>(Transform parent, string name) where T : Component
        {
            Transform find = parent.Find(name);
            if (find)
                return find.GetComponent<T>();
            else
                return default;
        }
        /// <summary>
        /// parent의 첫번째 자녀들의 컴포넌트 목록을 가져 옵니다.
        /// </summary>
        /// <typeparam name="T">가져올 컴포넌트</typeparam>
        /// <param name="parent">가져올 대상</param>
        /// <param name="compriseDeactiveObj">비활성화 된 오브젝트 포함 여부</param>
        /// <returns></returns>
        public static List<T> Get_1stChild_Components<T>(Transform parent, bool compriseDeactiveObj = false) where T : Component
        {
            List<T> rects = new List<T>();
            for (int cnt = 0; cnt < parent.childCount; cnt++)
            {
                if (compriseDeactiveObj)
                    rects.Add(parent.GetChild(cnt).GetComponent<T>());
                else
                {
                    if (parent.GetChild(cnt).gameObject.activeSelf)
                        rects.Add(parent.GetChild(cnt).GetComponent<T>());
                }
            }

            return rects;
        }
        /// <summary>
        /// target의 하위 자녀 목록중 대상을 찾습니다.
        /// </summary>
        /// <typeparam name="T">반환 형식</typeparam>
        /// <param name="target">찾을 대상</param>
        /// <param name="findChildName">target에서 찾을 이름</param>
        /// <returns></returns>
        public static T Find<T>(Transform target, string findChildName) where T : Component
        {
            //T find = default(T);
            Transform findBip = null;

            for (int cnt = 0; cnt < target.childCount; cnt++)
            {
                if (target.GetChild(cnt).name.Contains(findChildName))
                {
                    findBip = target.GetChild(cnt);
                    break;
                }
            }

            return findBip.GetComponent<T>();
        }
        /// <summary>
        /// parent를 기준으로 findChildName이름으로 되어 있는 첫번째 대상을 반환 한다.
        /// </summary>
        /// <param name="parent">찾을 대상</param>
        /// <param name="findChildName">찾을 이름</param>
        public static Transform FindChild(Transform parent, string findChildName, bool showDebug = false)
        {
            Transform find = null;

            for (int cnt = 0; cnt < parent.childCount; cnt++)
            {
                Transform child = parent.GetChild(cnt);
                if (showDebug)
                    Debug.Log($"[{cnt}] {child.name} => {child.name == findChildName}");

                if (child.name == findChildName)
                {
                    return child;
                }
                else
                    find = FindChild(child, findChildName, showDebug);
            }

            return find;
        }
        /// <summary>
        /// 찾아서 반환하고 숨겨줍니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="find"></param>
        /// <returns></returns>
        public static T Find_Hide<T>(Transform find, bool isHide = true) where T : Component
        {
            T target = default;
            if (find)
            {
                target = find.GetComponent<T>();
                if (isHide) find.gameObject.SetActive(false);
            }
            return target;
        }
        #endregion

        #region Extention
        /// <summary>
        /// dest가 Source의 위치와 동기화 됩니다.
        /// </summary>
        /// <param name="dest">따라가게 만들 대상</param>
        /// <param name="source">따라갈 대상</param>
        /// <param name="usePos">위치동기화</param>
        /// <param name="useRot">회전 동기화</param>
        public static void Follower(this Transform dest, Transform source, bool usePos = true, bool useRot = true)
        {
            if (usePos)
                dest.position = source.position;
            if (useRot)
                dest.rotation = source.rotation;
        }
        /// <summary>
        /// 지정한 Transform을 찾고 원하는 타입이 있는지 확인한다.
        /// </summary>
        /// <typeparam name="T">찾을 클래스</typeparam>
        /// <param name="target">찾을 대상</param>
        /// <param name="found">찾은 대상</param>
        /// <param name="findName">찾을 이름</param>
        /// <returns></returns>
        public static bool TryFind<T>(this Transform target, out T found, string findName)
        {
            Transform find = target.Find(findName);
            if (find != null)
            {
                found = find.GetComponent<T>();

                return found != null;
            }
            else
            {
                found = default(T);
                return false;
            }
        }
        /// <summary>
        /// 지정한 Transform을 찾고 원하는 타입이 있는지 확인한다.
        /// </summary>
        /// <typeparam name="T">찾을 클래스</typeparam>
        /// <param name="target">찾을 대상</param>
        /// <param name="found">찾은 대상</param>
        /// <param name="findName">찾을 이름</param>
        /// <returns></returns>
        public static bool TryFindAll<T>(this Transform target, out List<T> found, string findName)
        {
            Transform find = target.Find(findName);
            found = new List<T>();
            if (find != null)
            {
                found.AddRange(find.GetComponents<T>());

                return found.Count != 0;
            }
            else
            {
                return false;
            }
        }
        public static void GetComponentsInChildren<T>(this Transform parent, List<T> list, string findName) where T : Component
        {
            if (list == null)
                list = new List<T>();

            T find = parent.GetComponent<T>();

            if (find != null && find.name == findName)
            {
                list.Add(find);
            }

            for (int cnt = 0; cnt < parent.childCount; cnt++)
            {
                parent.GetChild(cnt).GetComponentsInChildren(list, findName);
            }
        }
        #endregion
    }
    public static class MeshUtils
    {
        #region Utils
        #endregion

        #region Extention
        public static string[] GetBlendShapeNames(this Mesh target)
        {
            string[] names = new string[target.blendShapeCount];
            for (int cnt = 0; cnt < target.blendShapeCount; cnt++)
            {
                names[cnt] = target.GetBlendShapeName(cnt);
            }
            return names;
        }
        public static string[] GetMaterialNames(this Renderer target)
        {
            string[] names = new string[target.sharedMaterials.Length];
            for (int cnt = 0; cnt < target.sharedMaterials.Length; cnt++)
            {
                names[cnt] = target.sharedMaterials[cnt].name;
            }
            return names;
        }
        #endregion
    }
    public static class ListUtils
    {
        #region Utils
        #endregion

        #region Extention
        /// <summary>
        /// 리스트에서 조건에 맞는 오브젝트를 찾는다.
        /// </summary>
        /// <typeparam name="T">리스트의 형식</typeparam>
        /// <param name="target">찾을 대상 리스트</param>
        /// <param name="find">찾은 대상</param>
        /// <param name="predicate">찾을 조건</param>
        /// <returns></returns>
        public static bool TryFind<T>(this List<T> target, out T find, Predicate<T> predicate) where T :class
        {
            find = target.Find(predicate);

            return find != null;
        }
        /// <summary>
        /// 리스트에서 조건에 맞는 오브젝트를 찾는다.
        /// </summary>
        /// <typeparam name="T">리스트의 형식</typeparam>
        /// <param name="target">찾을 대상 리스트</param>
        /// <param name="find">찾은 대상</param>
        /// <param name="predicate">찾을 조건</param>
        /// <returns></returns>
        public static bool TryFindAll<T>(this List<T> target, out List<T> finds, Predicate<T> predicate)
        {
            finds = target.FindAll(predicate);

            return finds != null && finds.Count != 0;
        }
        public static T FindLast<T>(this List<T> list)
        {
            if (list.Count != 0)
                return list[list.Count - 1];
            else
                return default(T);
        }
        public static T FindBeforeTheLast<T>(this List<T> list)
        {
            if (1 < list.Count)
                return list[list.Count - 2];
            else
                return default(T);
        }
        #endregion
    }
    public static class ColoUtils
    {
        #region Utils
        public enum FormatType
        {
            None,
            /// <summary>
            /// 형식 => 0x000000
            /// </summary>
            Ox,
            /// <summary>
            /// 형식 => #000000
            /// </summary>
            Shop,
            /// <summary>
            /// 형식 => 0x000000FF
            /// </summary>
            Ox_8,
            /// <summary>
            /// 형식 => #000000FF
            /// </summary>
            Shop_8
        }
        #endregion

        #region Extention
        /// <summary>
        /// Color를 Hex코드로 변경합니다.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToHex(this Color color, FormatType format)
        {
            string hex = $"{color.r.ToString("X2")}{color.g.ToString("X2")}{color.b.ToString("X2")}";
            switch (format)
            {
                case FormatType.Ox:
                    hex = $"0x{hex}";
                    break;
                case FormatType.Shop:
                    hex = $"#{hex}";
                    break;
                case FormatType.Ox_8:
                    hex = $"0x{hex}{color.a.ToString("X2")}";
                    break;
                case FormatType.Shop_8:
                    hex = $"#{hex}{color.a.ToString("X2")}";
                    break;
                default:
                    break;
            }

            return hex;
        }

        /// <summary>
        /// Hex코드를 Color로 변경합니다.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(this string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF

            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return new Color32(r, g, b, a);
        }
        #endregion
    }
    public static class BytesUtils
    {
        #region Utils
        public readonly static int ByteToCount = 4;
        #endregion

        #region Extention
        /// <summary>
        /// 배열의 원하는 중간영역값을 가져옵니다.
        /// </summary>
        /// <typeparam name="T">데이터 타입</typeparam>
        /// <param name="datas">원본 데이터</param>
        /// <param name="start">시작 위치</param>
        /// <param name="count">가져올 갯수</param>
        /// <returns></returns>
        public static T[] GetArea<T>(this T[] datas, int start, int count)
        {
            if (datas.Length < start + count) throw new ISFuncException("GetArea", "가져올 데이터의 영역이 원본 데이터의 값보다 큽니다.");
            if (start < 0) throw new ISFuncException("GetArea", "시작 위치가 0보다 작을 수 없습니다.");

            T[] range = new T[count];

            for (int cnt = 0; cnt < range.Length; cnt++)
            {
                range[cnt] = datas[start + cnt];
            }

            return range;
        }
        /// <summary>
        /// 바이트 데이터를 문자열로 변환 해줍니다. 
        /// ex) 00, 01, 10, 20 => 00-01-10-20
        /// </summary>
        /// <param name="data"></param>
        public static string ByteToString(this byte[] data)
        {
            return string.Join("-", data);
        }
        /// <summary>
        /// <see cref="ByteToString"/>으로  변환한 문자열을 <see cref="byte"/>[]로 변환해줍니다. 
        /// </summary>
        /// <param name="datas">변환할 데이터</param>
        /// <returns>변환된 데이터</returns>
        public static byte[] StringToByte(this string datas)
        {
            string[] split = datas.Split('-');
            byte[] splits = new byte[split.Length];

            for (int cnt = 0; cnt < split.Length; cnt++)
            {
                splits[cnt] = byte.Parse(split[cnt]);
            }

            return splits;
        }

        /// <summary>
        /// 데이터를 byte배열로 변환합니다.
        /// </summary>
        /// <param name="data">변환할 데이터</param>
        /// <returns>변환 된 byte 배열</returns>
        public static byte[] ToByte(this string data)
        {
            if (data == null || data == "")
            {
                return new byte[] { 0, 0, 0, 0 };
            }
            else
            {
                List<byte> _data = new List<byte>();
                byte[] convert = Encoding.UTF8.GetBytes(data);
                _data.AddRange(convert.Length.ToByte());
                _data.AddRange(convert);

                return _data.ToArray();
            }
        }
        /// <summary>
        /// 데이터를 byte배열로 변환합니다.
        /// </summary>
        /// <param name="data">변환할 데이터</param>
        /// <returns>변환 된 byte 배열</returns>
        public static byte[] ToByte(this int data)
        {
            return BitConverter.GetBytes(data);
        }
        /// <summary>
        /// 데이터를 byte배열로 변환합니다.
        /// </summary>
        /// <param name="data">변환할 데이터</param>
        /// <returns>변환 된 byte 배열</returns>
        public static byte[] ToByte(this float data)
        {
            return BitConverter.GetBytes(data);
        }
        /// <summary>
        /// 데이터를 byte배열로 변환합니다.
        /// </summary>
        /// <param name="data">변환할 데이터</param>
        /// <returns>변환 된 byte 배열</returns>
        public static byte[] ToByte(this double data)
        {
            return BitConverter.GetBytes(data);
        }
        /// <summary>
        /// 데이터를 byte배열로 변환합니다.
        /// </summary>
        /// <param name="data">변환할 데이터</param>
        /// <returns>변환 된 byte 배열</returns>
        public static byte[] ToByte(this TimeSpan data)
        {
            List<byte> datas = new List<byte>();

            datas.AddRange(BitConverter.GetBytes(data.Days));
            datas.AddRange(BitConverter.GetBytes(data.Hours));
            datas.AddRange(BitConverter.GetBytes(data.Minutes));
            datas.AddRange(BitConverter.GetBytes(data.Seconds));
            datas.AddRange(BitConverter.GetBytes(data.Milliseconds));

            return datas.ToArray();
        }
        /// <summary>
        /// 데이터를 byte배열로 변환합니다.
        /// </summary>
        /// <param name="data">변환할 데이터</param>
        /// <returns>변환 된 byte 배열</returns>
        public static byte[] ToByte(this Vector3 data)
        {
            List<byte> datas = new List<byte>();
            datas.AddRange(BitConverter.GetBytes(data.x));
            datas.AddRange(BitConverter.GetBytes(data.y));
            datas.AddRange(BitConverter.GetBytes(data.z));

            return datas.ToArray();
        }
        /// <summary>
        /// 바이트 배열을 지정한 형식에 맞춰 반환합니다.
        /// </summary>
        /// <param name="data">변환할 데이터</param>
        /// <param name="count">데이터의 길이(string => 문자열 전체길이 + 4)</param>
        /// <param name="start">데이터 가져오기 시작 부분</param>
        /// <returns>변환된 데이터</returns>
        public static string ByteToString(this byte[] data, out int count, int start = 0)
        {
            int length = BitConverter.ToInt32(data, start);

            if (length <= 0)
            {
                count = 4;
                return "";
            }
            else
            {
                string convert = Encoding.UTF8.GetString(data, start + 4, length);
                count = length + 4;
                return convert;
            }
        }
        /// <summary>
        /// 바이트 배열을 지정한 형식에 맞춰 반환합니다.
        /// </summary>
        /// <param name="data">변환할 데이터</param>
        /// <param name="count">데이터의 길이(string => 문자열 전체길이 + 4)</param>
        /// <param name="start">데이터 가져오기 시작 부분</param>
        /// <returns>변환된 데이터</returns>
        public static int ByteToInt(this byte[] data, out int count, int start = 0)
        {
            count = 4;
            return BitConverter.ToInt32(data, start);
        }
        /// <summary>
        /// 바이트 배열을 지정한 형식에 맞춰 반환합니다.
        /// </summary>
        /// <param name="data">변환할 데이터</param>
        /// <param name="count">데이터의 길이(string => 문자열 전체길이 + 4)</param>
        /// <param name="start">데이터 가져오기 시작 부분</param>
        /// <returns>변환된 데이터</returns>
        public static float ByteToFloat(this byte[] data, out int count, int start = 0)
        {
            count = 4;
            return BitConverter.ToSingle(data, start);
        }
        /// <summary>
        /// 바이트 배열을 지정한 형식에 맞춰 반환합니다.
        /// </summary>
        /// <param name="data">변환할 데이터</param>
        /// <param name="count">데이터의 길이(string => 문자열 전체길이 + 4)</param>
        /// <param name="start">데이터 가져오기 시작 부분</param>
        /// <returns>변환된 데이터</returns>
        public static double ByteToDouble(this byte[] data, out int count, int start = 0)
        {
            count = 4;
            return BitConverter.ToDouble(data, start);
        }
        /// <summary>
        /// 바이트 배열을 지정한 형식에 맞춰 반환합니다.
        /// </summary>
        /// <param name="data">변환할 데이터</param>
        /// <param name="count">데이터의 길이(string => 문자열 전체길이 + 4)</param>
        /// <param name="start">데이터 가져오기 시작 부분</param>
        /// <returns>변환된 데이터</returns>
        public static TimeSpan ByteToTimeSpan(this byte[] data, out int count, int start = 0)
        {
            int d = data.ByteToInt(out count, start);
            start += count;
            int h = data.ByteToInt(out count, start);
            start += count;
            int m = data.ByteToInt(out count, start);
            start += count;
            int s = data.ByteToInt(out _, start);
            start += count;
            int ms = data.ByteToInt(out _, start);

            count = ByteToCount * 5;

            return new TimeSpan(d, h, m, s, ms);
        }
        /// <summary>
        /// 바이트 배열을 지정한 형식에 맞춰 반환합니다.
        /// </summary>
        /// <param name="data">변환할 데이터</param>
        /// <param name="count">데이터의 길이(string => 문자열 전체길이 + 4)</param>
        /// <param name="start">데이터 가져오기 시작 부분</param>
        /// <returns>변환된 데이터</returns>
        public static Vector3 ByteToVector3(this byte[] data, out int count, int start = 0)
        {
            float x =data.ByteToFloat(out count, start);
            start += count;
            float y =data.ByteToFloat(out count, start);
            start += count;
            float z =data.ByteToFloat(out _, start);

            count = ByteToCount * 3;

            return new Vector3(x, y, z);
        }
        #endregion
    }
    public static class RectUtils
    {
        #region Utils
        #endregion

        #region Extention
        public static Rect ScaleMulti(this Rect rect, float value)
        {
            rect.width *= value;
            rect.height *= value;

            return rect;
        }
        #endregion
    }
    public static class JsonUtils
    {
        #region Utils
        #endregion
        #region Extention
        public static string NoSpace(this string json)
        {
            json = json.Replace("\r", "");
            json = json.Replace("\n", "");

            return json;
        }

        public static string ToBeauty(this string json)
        {
            json = json.NoSpace();

            json = json.Replace("{", "{\n");
            json = json.Replace("}", "\n}");
            json = json.Replace("[", "[\n");
            json = json.Replace("]", "\n]");
            json = json.Replace(",", ",\n");

            string[] jsonSplit = json.Split('\n');

            int depth = 0;
            for (int cnt = 0; cnt < jsonSplit.Length; cnt++)
            {
                bool isDepth = false;
                if (jsonSplit[cnt].Contains("{") || jsonSplit[cnt].Contains("["))
                {
                    isDepth = true;
                    depth++;
                }
                if (jsonSplit[cnt].Contains("}") || jsonSplit[cnt].Contains("]"))
                {
                    depth--;
                }
                if (isDepth)
                    jsonSplit[cnt] = string.Join("  ", new string[depth]) + jsonSplit[cnt];
                else
                    jsonSplit[cnt] = string.Join("  ", new string[depth + 1]) + jsonSplit[cnt];
            }

            return string.Join("\n", jsonSplit);
        }
        #endregion
    }
    public static class VectorUtils
    {
        #region Utils
        #endregion
        #region Extention
        public static Vector2 ToVector2(this Vector3 value, AxisType exceptAxis)
        {
            switch (exceptAxis)
            {
                case AxisType.X: return new Vector2(value.y, value.z);
                case AxisType.Y: return new Vector2(value.x, value.z);
                case AxisType.Z: return new Vector2(value.x, value.y);
                default: goto case AxisType.Z;
            }
        }
        #endregion
    }
    public static class ValueUtils
    {
        #region Utils
        #endregion
        #region Extention
        public static string ToOneWord(this bool value)
        {
            return value ? "T" : "F";
        }
        public static string Indent(this string text, int count, bool forwardLineChange = true)
        {
            string indent = "\n";
            for (int cnt = 0; cnt < count; cnt++)
            {
                indent += "\t";
            }

            text = text.Replace("\n", indent);

            if (forwardLineChange)
                text = $"{indent}{text}";

            return text;
        }
        #endregion
    }
    public static class CourutineUtils
    {
        #region Utils
        #endregion
        #region Extention
        public static IEnumerator StartCoroutine(this IEnumerator enumerator, MonoBehaviour mono, IEnumerator start)
        {
            if (enumerator != null)
            {
                mono.StopCoroutine(enumerator);
            }
            enumerator = start;

            mono.StartCoroutine(enumerator);

            return enumerator;
        }
        public static IEnumerator StopCouroutine(this IEnumerator enumerator, MonoBehaviour mono)
        {
            if (enumerator != null)
            {
                mono.StopCoroutine(enumerator);
            }
            enumerator = null;

            return enumerator;
        }
        #endregion
    }
    public static class EnumUtils
    {
        #region Utils
        #endregion
        #region Extention
        public static bool FlagChecker<T>(this T _my, T _option) where T : Enum
        {
            int my = Convert.ToInt32(_my);
            int option = Convert.ToInt32(_option);

            return (my & option) == option;
        }
        #endregion
    }
    #endregion

    #region Exception
    public class ISFuncException : Exception
    {
        public ISFuncException(string callFunc, string message) : base($"[{callFunc}]{message}") { }
    }
    #endregion

    #region ETC
    /// <summary>
    /// 베지어 커브를 그립니다.
    /// 참고: https://blog.naver.com/PostView.naver?blogId=kyuniitale&logNo=40022945907&redirect=Dlog&widgetTypeCall=true&directAccess=false
    /// </summary>
    [System.Serializable]
    public class Bezier
    {
        public Vector3[] posList = null;

        public Bezier(int createCurveQuality)
        {
            if (createCurveQuality < 2)
                createCurveQuality = 2;

            posList = new Vector3[createCurveQuality];
        }

        /// <summary>
        /// 월드 기준으로 값을 넘겨줍니다.
        /// </summary>
        /// <param name="value">진행률</param>
        /// <returns></returns>
        public Vector3 Evaluate(float value)
        {
            Vector3[] newPos0 = posList;
            Vector3[] newPos1 = new Vector3[newPos0.Length - 1];

            while (newPos0.Length != 2)
            {
                for (int cnt = 0; cnt < newPos1.Length; cnt++)
                {
                    newPos1[cnt] = Vector3.Lerp(newPos0[cnt], newPos0[cnt + 1], value);
                }

                newPos0 = newPos1;
                newPos1 = new Vector3[newPos0.Length - 1];
            }

            return Vector3.Lerp(newPos0[0], newPos0[1], value);
        }
        /// <summary>
        /// 로컬 기준으로 값을 넘겨줍니다.
        /// </summary>
        /// <param name="parent">로컬 값의 기준</param>
        /// <param name="value">진행률</param>
        /// <returns></returns>
        public Vector3 GetLocalCurve(Transform parent, float value)
        {
            return parent.TransformPoint(Evaluate(value));
        }
    }
    public static class IS_Func
    {
        public static string InputNumber(string input)
        {
            if (input == null)
                input = "";
            return Regex.Replace(input, "[^0-9 . ]", "");
        }
        public static string InputIntiger(string input, MinMax range)
        {
            if (input == null)
                input = "";

            input = Regex.Replace(input, "[^0-9 . ]", "");
            
            if (int.TryParse(input, out int result))
            {
                result = (int)range.Clamp(result, true, true);
                input = result.ToString();
                return input;
            }
            else
                return input;
        }
        /// <summary>
        /// 애니메이터가 해당 애니메이션 플레이 상태인지 체크 합니다.
        /// </summary>
        /// <param name="target">체크할 애니메이터</param>
        /// <param name="aniClip">체크할 애니메이션 이름</param>
        /// <returns></returns>
        public static bool ISPlay(Animator target, string aniName)
        {
            return target.GetCurrentAnimatorStateInfo(0).IsName(aniName) &&
                   target.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
        }

        /// <summary>
        /// 반환값에 True가 있다면 문제가 있는 문자열이 포함된 경우임
        /// </summary>
        /// <param name="source">체크할 문자열</param>
        /// <returns></returns>
        public static bool InvalidCheckString(string source)//, bool isFile = false)
        {
            char[] invalidChars = new char[] { '/', ':', '*', '?', '"', '<', '>', '|', '\\' };//isFile ? Path.GetInvalidFileNameChars() : Path.GetInvalidPathChars();

            bool isWrong = false;
            for (int cnt = 0; cnt < source.Length; cnt++)
            {
                for (int _cnt = 0; _cnt < invalidChars.Length; _cnt++)
                {
                    if (source[cnt].Equals(invalidChars[_cnt]))
                    {
                        isWrong = true;
                        break;
                    }
                }
            }

            Debug.Log("File: " + source + " / Insert Wrong Text: " + isWrong);

            return isWrong;
        }

        /// <summary>
        /// 현재 적용되어 있는 메트리얼의 렌더링 모드 값을 가져옵니다.
        /// </summary>
        /// <param name="mat">렌더링 모드를 가져옵니다.</param>
        /// <returns></returns>
        public static BlendMode GetRenderingMode(Material mat)
        {
            return (BlendMode)mat.GetFloat("_Mode");
        }
        /// <summary>
        /// 렌더링 모드 값을 변경합니다.
        /// 실시간 렌더링 중 단독으로 적용이 불가 하며 SetRenderingMode와 같이 사용해야 적용됩니다.
        /// </summary>
        /// <param name="mat">렌더링 모드를 설정할 메트리얼</param>
        /// <param name="mode">변경할 모드</param>
        public static void SetBlendMode(Material mat, BlendMode mode)
        {
            mat.SetFloat("_Mode", (int)mode);
        }
        /// <summary>
        /// Material의 Rendering Mode를 실시간으로 변경시켜 줍니다.
        /// </summary>
        /// <param name="mat">변경할 Material</param>
        /// <param name="blendMode">변경하고 싶은 모드</param>
        public static void SetRenderingMode(Material mat, BlendMode blendMode)
        {
            SetBlendMode(mat, blendMode);
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    mat.SetOverrideTag("RenderType", "");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    mat.SetOverrideTag("RenderType", "TransparentCutout");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.EnableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 2450;
                    break;
                case BlendMode.Fade:
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                    break;
                case BlendMode.Transparent:
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                    break;
            }
        }
    }

    [Serializable]
    public class DIR
    {
        public enum Type
        {
            Single = 1,
            Vector2,
            Vector3
        }
        public enum Axis
        {
            x,
            y,
            z
        }
        public enum Dir
        {
            Right = 1,
            Reverse = -1
        }

        public Axis axis;
        public Dir dir;

        private Type m_type;

        public DIR(Type type)
        {
            m_type = type;
        }
        public void Get(out float[] values)
        {
            values = new float[(int)m_type];
            switch (m_type)
            {
                case Type.Vector3:
                    values[2] = axis == Axis.z ? (int)dir : 0;
                    goto case Type.Vector2;
                case Type.Vector2:
                    values[1] = axis == Axis.y ? (int)dir : 0;
                    goto case Type.Single;
                case Type.Single:
                    values[0] = (axis == Axis.x || m_type == Type.Single) ? (int)dir : 0;
                    break;
                default:
                    break;
            }
        }
        public float GetSingle()
        {
            return (int)dir;
        }
        public Vector2 GetV2()
        {
            switch (axis)
            {
                case Axis.x: return new Vector2((int)dir, 0);
                case Axis.y: return new Vector2(0, (int)dir);
                default:
                    throw new FormatException("z 축은 사용 할 수 없습니다. Vector3로 변경 해주세요.");
            }
        }
        public Vector3 GetV3()
        {
            switch (axis)
            {
                case Axis.x: return new Vector3((int)dir, 0, 0);
                case Axis.y: return new Vector3(0, (int)dir, 0);
                case Axis.z: return new Vector3(0, 0, (int)dir);
                default:
                    throw new FormatException("z 축은 사용 할 수 없습니다. Vector3로 변경 해주세요.");
            }
        }
        //public T Get()
        //{
        //    if (typeof(T) == typeof(float))
        //    {
        //        return (dynamic)((int)dir);
        //    }
        //    else if (typeof(T) == typeof(Vector2))
        //    {
        //        switch (axis)
        //        {
        //            case Axis.x: return (dynamic)(new Vector2((int)dir, 0));
        //            case Axis.y: return (dynamic)(new Vector2(0, (int)dir));
        //            default:
        //                throw new FormatException("z 축은 사용 할 수 없습니다. Vector3로 변경 해주세요.");
        //        }
        //    }
        //    else if (typeof(T) == typeof(Vector3))
        //    {
        //        switch (axis)
        //        {
        //            case Axis.x: return (dynamic)(new Vector3((int)dir, 0, 0));
        //            case Axis.y: return (dynamic)(new Vector3(0, (int)dir), 0);
        //            case Axis.z: return (dynamic)(new Vector3(0, 0, (int)dir));
        //            default: goto case Axis.x;
        //        }
        //    }
        //    else
        //        throw new FormatException("지원하지 않는 형식입니다. float, Vector2, Vector3 세가지 형식이 사용가능합니다.");
        //}
    }

    /// <summary>
    /// 오브젝트 활성화를 하기 위한 보조 클래스
    /// </summary>
    [System.Serializable]
    public class ActiveOption
    {
        public enum State
        {
            Active,
            Poser
        }

        public State state;
        public bool isActive = false;
        public GameObject target;
        public Vector3Option pos;
        public Vector3Option rot;

        public void SetActive(bool isActive)
        {
            if (target == null) return;

            switch (state)
            {
                case State.Active:
                    target.SetActive(isActive);
                    break;
                case State.Poser:
                    if (pos.use)
                    {
                        if (pos.isLocal)
                            target.transform.localPosition = pos.Value;
                        else
                            target.transform.position = pos.Value;
                    }
                    if (rot.use)
                    {
                        if (rot.isLocal)
                            target.transform.localEulerAngles = rot.Value;
                        else
                            target.transform.eulerAngles = rot.Value;
                    }
                    break;
            }
        }
        public void Init()
        {
            switch (state)
            {
                case State.Active:
                    target.SetActive(!isActive);
                    break;
                case State.Poser:

                    if (pos.use)
                    {
                        if (pos.isLocal)
                            target.transform.localPosition = pos.Init;
                        else
                            target.transform.position = pos.Init;
                    }
                    if (rot.use)
                    {
                        if (rot.isLocal)
                            target.transform.localEulerAngles = rot.Init;
                        else
                            target.transform.eulerAngles = rot.Init;
                    }
                    break;
            }
        }
        public void SetActive()
        {
            SetActive(isActive);
        }
        public void SetReverse()
        {
            SetActive(!isActive);
        }
    }
    [System.Serializable]
    public class Vector3Option
    {
        public bool use = false;
        public bool isLocal = false;
        public Vector3 Init;
        public Vector3 Value;
    }
    #endregion
}
