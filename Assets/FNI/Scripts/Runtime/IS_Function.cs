using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Reflection;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace FNI
{
    /// <summary>
    /// Mathf에 없는 함수들입니다.
    /// </summary>
    public struct MathIS
    {
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
                angle = 360 + angle;
            if (360 <= angle)
                angle = angle % 360;

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
        /// <param name="ten">자를 자릿수</param>
        /// <returns></returns>
        public static float FloatCut(float value, DigitType ten)
        {
            return (float)((int)(value * (int)ten)) / (int)ten;
        }
        /// <summary>
        /// 리니어 타입입니다.
        /// </summary>
        /// <param name="t">진행도</param>
        /// <param name="p0">시작 위치</param>
        /// <param name="p1">종료 위치</param>
        /// <returns></returns>
        public static Vector3 GetCurve(float t, Vector3 p0, Vector3 p1)
        {
            return p0 + (t * (p1 - p0));
        }

        /// <summary>
        /// 베지어 타입입니다.
        /// </summary>
        /// <param name="t">진행도</param>
        /// <param name="p0">시작 위치</param>
        /// <param name="p1">중간 위치</param>
        /// <param name="p2">종료 위치</param>
        /// <returns></returns>
        public static Vector3 GetCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            return (uu * p0) + (2 * u * t * p1) + (tt * p2);
        }

        /// <summary>
        /// 베지어 타입입니다. 중간 포인트가 2개 입니다.
        /// </summary>
        /// <param name="t">진행도</param>
        /// <param name="p0">시작 위치</param>
        /// <param name="p1">중간 위치1</param>
        /// <param name="p2">중간 위치2</param>
        /// <param name="p3">종료 위치</param>
        /// <returns></returns>
        public static Vector3 GetCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float uu = u * u;
            float uuu = uu * u;
            float tt = t * t;
            float ttt = tt * t;

            return (uuu * p0) + ((3 * uu) * t * p1) + ((2 * u) * tt * p2) + (ttt * p3);
        }
    }

    public static class IS_Func
    {
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
        public static void GetComponentsInChildren<T>(List<T> list, Transform parent) where T: Component
        {
            if (list == null)
                list = new List<T>();

            for (int cnt = 0; cnt < parent.childCount; cnt++)
            {
                GetComponentsInChildren(list, parent.GetChild(cnt));
            }

            T find = parent.GetComponent<T>();

            if (find != null)
            {
                list.Add(find);
            }
        }
        public static T GetCompnent<T>(Transform parent, string name) where T : Component
        {
            Transform find = parent.Find(name);
            if (find)
                return find.GetComponent<T>();
            else
                return default(T);
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
            T target = default(T);
            if (find)
            {
                target = find.GetComponent<T>();
                if (isHide) find.gameObject.SetActive(false);
            }
            return target;
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
        /// Color를 Hex코드로 변경합니다.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        /// <summary>
        /// Hex코드를 Color로 변경합니다.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string hex)
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
        /// 반환값에 True가 있다면 문제가 있는 문자열이 포함된 경우임
        /// </summary>
        /// <param name="source">체크할 문자열</param>
        /// <returns></returns>
        public static bool InvalidCheckString(string source, bool isFile = false)
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

        public static void ChangeLayer(GameObject target, string layerName)
        {
            target.layer = LayerMask.NameToLayer(layerName);

            int childCount = target.transform.childCount;

            if (0 != childCount)
            {
                for (int cnt = 0; cnt < childCount; cnt++)
                {
                    Transform child = target.transform.GetChild(cnt);
                    
                    if (child.childCount != 0)
                    {
                        ChangeLayer(child.gameObject, layerName);
                    }
                    else
                        child.gameObject.layer = LayerMask.NameToLayer(layerName);
                }
            }
        }
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

    /// <summary>
    /// 다중 배열을 인스펙터에 표시하기 위한 용도입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public struct IS_Array<T>
    {
        public T[] array;
    }

    /// <summary>
    /// 최소값 최대값
    /// </summary>
    [System.Serializable]
    [SerializeField]
    public struct MinMax
    {
        [SerializeField]
        private float m_min;
        [SerializeField]
        private float m_max;

        /// <summary>
        /// percent가 적용된 최소값을 가져오고 설정은 m_min값만 설정
        /// </summary>
        public float Min { get { return m_min; } }
        /// <summary>
        /// percent가 적용된 최대값을 가져오고 설정은 m_max값만 설정
        /// </summary>
        public float Max { get { return m_max; } }
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
        public float PercentToValue(float percent)
        {
            return Mathf.Lerp(Min, Max, percent);
        }
        public bool Contain(float value)
        {
            return m_min < value && value < m_max;
        }
    }
    //public struct IS_Transform
    //{
    //	public Vector3 eulerAngles
    //	{
    //		set
    //		{
    //			rotation.eulerAngles = value;
    //		}
    //		get
    //		{
    //			return rotation.eulerAngles;
    //		}
    //	}
    //	public Vector3 position;
    //	public Quaternion rotation;
    //	public Vector3 scale;
    //}
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
}
