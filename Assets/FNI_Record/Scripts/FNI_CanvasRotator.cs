/// 작성자: 백인성
/// 작성일: 2019-10-18
/// 수정일: 2019-10-18
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNI_CanvasRotator : MonoBehaviour
{
	private float GetMyYAngle
	{
		get { return Angle(transform.localEulerAngles.y); }
	}
	private float GetHeadYAngle
	{
		get { return Angle(head.localEulerAngles.y); }
	}

	public Transform head;
	public float followSpeed = 10;

	public CanvasRotType rotType = CanvasRotType.Head;
	public bool isHold = false;

	[Range(0, 180)]
	public float followStartAngle = 30;
	public float addHeight;

	private float m_targetAngle;
	private Vector3 sRot;

	private void Start()
	{
		sRot = transform.eulerAngles;
		m_targetAngle = head.localEulerAngles.y;
	}

	void LateUpdate()
	{
		switch (rotType)
		{
			case CanvasRotType.Head:
				FollowHead();
				break;
			case CanvasRotType.Hold:
				SetHoldRotaion();
				break;
			case CanvasRotType.Anchor:
				SetRotaion();
				break;
		}
	}

	private void FollowHead()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, head.rotation, Time.deltaTime * followSpeed);
		transform.position = head.position;
	}

	private void Rotater()
	{
		float y = Mathf.LerpAngle(GetMyYAngle, GetHeadYAngle, Time.deltaTime * followSpeed);
		transform.eulerAngles = new Vector3(sRot.x, y, sRot.z);

		transform.position = new Vector3(head.position.x, head.position.y + addHeight, head.position.z);
	}

	private void SetRotaion()
	{
		//머리의 각도를 측정한다.
		float angle = Quaternion.Angle(transform.rotation, head.rotation);

		//머리의 각도가 m_startAngle를 넘어서면 새로운 각도를 입력한다.
		if (followStartAngle < Mathf.Abs(angle))
		{
			m_targetAngle = GetHeadYAngle;
		}

		//자기 자신의 각도를 변경 한다.
		float y = Mathf.LerpAngle(GetMyYAngle, m_targetAngle, Time.deltaTime * followSpeed);
		//값 적용
		transform.localEulerAngles = new Vector3(0, y, 0);
		transform.position = new Vector3(head.position.x, head.position.y + addHeight, head.position.z);
	}

	private void SetHoldRotaion()
	{
		if (isHold) return;

		//자기 자신의 각도를 변경 한다.
		float y = Mathf.LerpAngle(GetMyYAngle, GetHeadYAngle, Time.deltaTime * followSpeed);
		//값 적용
		transform.localEulerAngles = new Vector3(0, y, 0);
		transform.position = new Vector3(head.position.x, head.position.y + addHeight, head.position.z);
	}
    public float Angle(float angle)
    {
        return 180 < angle ? angle - 360 : angle;
    }
}

public enum CanvasRotType
{
	Head,
	Hold,
	Anchor
}