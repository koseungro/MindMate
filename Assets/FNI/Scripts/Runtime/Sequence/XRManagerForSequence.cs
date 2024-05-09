using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FNI.XR;
using cakeslice;

namespace FNI
{
    public class XRManagerForSequence : MonoBehaviour, IVisualObject
    {
        public VisualType Type => VisualType.XRManager;

        public bool IsFinish => isFinish;
        private bool isFinish = false;

        public XRManager xrManager;
        private OutlineEffect outlineEffect;

        public void Active(CutData option)
        {
            isFinish = true;

            switch (option.xrOption.xrType)
            {
                case XR_FuncType.ChangeControllerMat:
                    xrManager.usingControllerMatChange = option.xrOption.useFunction;
                    break;
                case XR_FuncType.SendHapticInpulse:
                    xrManager.SendHapticImpulse(option.xrOption.hapticAmplitude, option.xrOption.hapticDuration);
                    break;
                case XR_FuncType.OutlineColorChange:
                    //outlineEffect.lineColor2 = option.xrOption.outlineColor;
                    OutlineEffect.Instance.lineColor2 = option.xrOption.outlineColor;
                    break;
                case XR_FuncType.UsingStopButton:
                    xrManager.usingStopContentButtion = option.xrOption.useFunction;
                    break;
            }
        }

        public void Init()
        {
            //outlineEffect = Camera.main.GetComponent<OutlineEffect>();
        }

        public void MyUpdate()
        {
            
        }

    }
}

