using FNI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using cakeslice;
using FNI.XR;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class JellyController : MonoBehaviour
{
    public UnityEvent onObjectEvent;

    private cakeslice.Outline myOutline;
    public cakeslice.Outline MyOutline
    {
        get
        {
            if (myOutline == null)
                myOutline = GetComponent<cakeslice.Outline>();

            return myOutline;
        }
    }

    private Animator animator;
    private MeshCollider m_Col;
    private CapsuleCollider c_Col;


    private readonly int hashIsShrink = Animator.StringToHash("Jelly_Shrink");
    private readonly int hashIsJellyRest = Animator.StringToHash("Jelly_Reset");
    /// <summary>
    /// 현재 Outline 활성화 여부
    /// </summary>
    private bool onOutline = false;

    /// <summary>
    /// 현재 오브젝트 Select 여부
    /// </summary>
    private bool onSelected = false;

    /// <summary>
    /// 몬스터 크기 변화에 따른 교체 Mesh 목록
    /// </summary>
    public Mesh[] monsterMeshs;

    public Text debugText;
    private string debugLog = "";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        m_Col = GetComponent<MeshCollider>();
        c_Col = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        m_Col.sharedMesh = monsterMeshs[0];
        c_Col.enabled = false;
    }

    private void OnDisable()
    {
        onSelected = false;

        animator.SetTrigger(hashIsJellyRest); // 애니메이션 상태 초기화
        m_Col.enabled = false;
        c_Col.enabled = false;
        Debug.Log($"<color=yellow> Jelly Animator Reset </color>");
    }

    int i = 0;
    public void SwitchCollider()
    {
        if (i == 5)
            i = 0;
        m_Col.sharedMesh = monsterMeshs[i];

        debugLog = debugLog + $"\n{i}";
        debugText.text = debugLog;

        i++;
    }

    public void RayOnEnter()
    {
        if (onOutline == false && MyOutline.enabled == false && !onSelected)
        {
            //Debug.Log($"<color=yellow>{this.gameObject.name}/ {ObjectBase.onOutline} / Outline On</color>");
            //debugLog = debugLog + "\nEnter";
            //debugText.text = debugLog;

            MyOutline.enabled = true;
            onOutline = true;
        }
    }

    public void RayOnExit()
    {
        if (onOutline == true && MyOutline.enabled == true)
        {
            //Debug.Log($"<color=gray>{this.gameObject.name}/ {ObjectBase.onOutline} / Outline Off</color>");
            MyOutline.enabled = false;
            onOutline = false;
        }
    }

    public void RayOnSelect()
    {
        Debug.Log($"<color=magenta> {gameObject.name} Select</color>");
        animator.SetTrigger(hashIsShrink);

        // 콘트롤러 진동
        XRManager.Instance.SendHapticImpulse(0.2f, 0.8f);

        StartCoroutine(CheckLastShrink());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckLastShrink()
    {
        yield return new WaitForSeconds(0.3f); // 다음 애니메이션으로 Transition 대기


        if (animator.GetCurrentAnimatorStateInfo(0).IsName("jelly_monster_shape_ver2"))
        {
            m_Col.sharedMesh = monsterMeshs[1];

        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("jelly_monster_shape_ver3"))
        {
            //m_Col.sharedMesh = monsterMeshs[2];
            m_Col.enabled = false;

            c_Col.enabled = true;
            c_Col.center = new Vector3(0, -1.6f, 0);
            c_Col.radius = 2.5f;
            c_Col.height = 7f;

        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("jelly_monster_shape_ver4"))
        {
            //m_Col.sharedMesh = monsterMeshs[3];

            c_Col.center = new Vector3(0, -0.9f, 0);

            c_Col.radius = 2.1f;
            c_Col.height = 2.1f;

        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("jelly_monster_shape_ver5"))
        {
            //m_Col.sharedMesh = monsterMeshs[4];

            c_Col.center = new Vector3(0, -0.4f, 0);

            c_Col.radius = 1f;
            c_Col.height = 1f;

            //Debug.Log("<color=cyan>Last Jelly Shrink</color>");

            float animTime = 0;

            while (animTime >= 0 && animTime < 1.0f)
            {
                animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                yield return null;
            }

            SubtitleManager.Instance.checkPoint_End = true;
            Debug.Log("<color=yellow> Last Shrink End </color>");
        }
    }

}
