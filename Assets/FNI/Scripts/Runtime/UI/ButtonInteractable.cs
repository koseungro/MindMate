using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FNI;

public class ButtonInteractable : Button
{
    public event Action<bool> InteractableChanged;

    protected SelectionState prevState;

    private IS_Selectable_Suppoter selectable_Supporter;

    private IS_Selectable_Suppoter Selectable_Supporter
    {
        get
        {
            if (selectable_Supporter == null)
                selectable_Supporter = GetComponent<IS_Selectable_Suppoter>();

            return selectable_Supporter;
        }
    }

    protected override void Awake()
    {
        //InteractableChanged += TestEvent;
    }

    private void TestEvent(bool testBool)
    {
        Debug.Log($"{testBool}/ {interactable}");
    }
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        if (state != prevState && this.gameObject.activeInHierarchy)
        {
            if (Selectable_Supporter != null)
            {
                if (state == SelectionState.Disabled)
                {
                    Selectable_Supporter.state = FNI.AnimationState.Disable;
                    Selectable_Supporter.SetSprite();
                }
                else if(state == SelectionState.Normal)
                {
                    Selectable_Supporter.state = FNI.AnimationState.Default;
                    Selectable_Supporter.SetSprite();
                }
            }
            else
                Debug.Log($"<color=red>[{this.transform.name}] Selectable_Supporter 가 Null 입니다.</color>");

            InteractableChanged?.Invoke(interactable);
            prevState = state;
        }
    }
}
