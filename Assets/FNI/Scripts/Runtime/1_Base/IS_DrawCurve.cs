using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FNI;
using FNI.IS;
using System.Linq;

/// <summary>
/// 라인렌더러를 통해 커브를 그립니다. 활성화 되어 있는 동안 항상 그립니다.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class IS_DrawCurve : MonoBehaviour
{
	/// <summary>
    /// [0] 시작위치, [n] 중간 위치, [end] 종료 위치
    /// </summary>
    [Header("Curve Point (Min 2, Max 4)")]
	public Transform[] point;

	/// <summary>
	/// 라인의 퀄리티 입니다.
	/// </summary>
	[Range(2, 100)]
	public int quality = 10;
	[Range(0.0001f, 0.1f)]
	public float width = 0.005f;

	/// <summary>
	/// 사용할 라인렌더러입니다.
	/// </summary>
	private LineRenderer m_lineRenderer;

	private Bezier bezier;

	private void Start()
    {
		m_lineRenderer = GetComponent<LineRenderer>();
		if (m_lineRenderer == null)
		{
			m_lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

		bezier = new Bezier(point.Length);

		SetCurve();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        SetCurve();
    }

    /// <summary>
    /// 커브의 상태를 설정합니다.
    /// </summary>
    private void SetCurve()
	{
		if (CheckNull()) return;

		m_lineRenderer.widthMultiplier = width;

		//라인의 퀄리티를 지정합니다.
		if (m_lineRenderer.positionCount != quality)
			m_lineRenderer.positionCount = quality;

		bezier.posList = point.Select(x => x.position).ToArray();

		//각 포지션을 지정합니다.
		for (int cnt = 0; cnt < quality; cnt++)
		{
			float t = cnt / (float)(quality - 1);//-1을 하지 않으면 마지막에 비어버린다.
			m_lineRenderer.SetPosition(cnt, bezier.Evaluate(t));
		}
	}
	private bool CheckNull()
    {
        if (point == null) return true;

        bool isNull = false;
		for (int cnt = 0; cnt < point.Length; cnt++)
		{
			if (point[cnt] == null)
			{
				isNull = true;
				break;
			}
		}

		return isNull;
	}
}
