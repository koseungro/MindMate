using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using cakeslice;


public class ObjectBase : MonoBehaviour
{
    public UnityEvent onObjectEvent;
    public SceneData selectData;

    private Outline myOutline;
    public Outline MyOutline
    {
        get
        {
            if (myOutline == null)
                myOutline = GetComponent<Outline>();

            return myOutline;
        }
    }

    private Main myMain;
    private Main MyMain
    {
        get
        {
            if (myMain == null)
                myMain = GameObject.Find("---------------Scripts/Main").GetComponent<Main>();

            return myMain;
        }
    }

    private Animator animator;
    public Animator Dummy_Animator { get => dummy_Animator; }
    private Animator dummy_Animator;

    private readonly int hashIsMove = Animator.StringToHash("Dummy_Selection_Move");
    private readonly int hashIsReturn = Animator.StringToHash("Dummy_Selection_Move(Reverse)");

    /// <summary>
    /// 현재, Outline 활성화 여부 (Static)
    /// </summary>
    private static bool onOutline = false;

    /// <summary>
    /// 현재, 오브젝트 Select 여부 (Static)
    /// </summary>
    private static bool onSelected = false;
    public static bool IsMoved { get => onSelected; set => onSelected = value; }

    private void Awake()
    {
        animator = transform.parent.GetComponent<Animator>();
        dummy_Animator = transform.parent.parent.GetComponent<Animator>();

        //hashIsMove = Animator.StringToHash("Seed_Dummy_Selection_Move");
        //hashIsReturn = Animator.StringToHash("Seed_Dummy_Selection_Move(Reverse)");
    }

    private void OnDisable()
    {
        onSelected = false;
    }

    public void RayOnEnter()
    {
        if (ObjectBase.onOutline == false && MyOutline.enabled == false && !ObjectBase.onSelected)
        {
            //Debug.Log($"<color=yellow>{this.gameObject.name}/ {ObjectBase.onOutline} / Outline On</color>");
            MyOutline.enabled = true;
            ObjectBase.onOutline = true;
        }
    }

    public void RayOnExit()
    {
        if (ObjectBase.onOutline == true && MyOutline.enabled == true)
        {
            //Debug.Log($"<color=gray>{this.gameObject.name}/ {ObjectBase.onOutline} / Outline Off</color>");
            MyOutline.enabled = false;
            ObjectBase.onOutline = false;
        }
    }

    public void RayOnSelect()
    {
        //Debug.Log($"<color=magenta> {MyObject.name} Select</color>");
        if (!MyMain.IsPlaying)
        {
            Anim_SelectionMove();

        }
    }

    public void Anim_SelectionMove()
    {
        if (ObjectBase.onSelected == false)
        {
            dummy_Animator.SetTrigger(hashIsMove);
            MyMain.ChangeScene(selectData);
            ObjectBase.onSelected = true;
            //Debug.Log($"<color=yellow>{this.gameObject.name}/ {ObjectBase.onSelected} / Selected On</color>"); 

        }

    }

    public void Anim_SelectionReturn()
    {
        if (ObjectBase.onSelected == true)
        {
            dummy_Animator.SetTrigger(hashIsReturn);
            ObjectBase.onSelected = false;
            //Debug.Log($"<color=gray>{this.gameObject.name}/ {ObjectBase.onSelected} / Selected Off</color>");

        }

    }

}
