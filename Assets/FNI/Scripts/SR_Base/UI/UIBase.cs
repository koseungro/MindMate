
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FNI.XR;

public class UIBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private XRManager manager;

    protected virtual void Start()
    {

    }
    private void Reset()
    {
        manager = FindObjectOfType<XRManager>();
    }

    // 사운드, Hover 공통 효과
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        //print("UI Enter");
        manager?.PlayHoverEffect();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //print("UI Exit");
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //print("UI Click");
        manager?.PlayClickEffect();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //print("UI Down");
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        //print("UI Up");
    }
}
