using FNI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BezierSolution;
using FNI.XR;

namespace FNI
{

    public class ObjectForSequence : MonoBehaviour, IVisualObject
    {
        public VisualType Type => VisualType.Object;

        private bool isFinish = false;
        public bool IsFinish => isFinish;
        /// <summary>
        /// 숨겨져있는 Target Object도 찾기 위한 최상위 부모 Transform
        /// </summary>
        private Transform rootParent;
        private GameObject targetObj;

        public void Active(CutData option)
        {
            isFinish = false;

            // TargetObject 의 경로가 입력되어 있을 때만 찾기
            if (option.objActiveOption.targetObjPath != "")
            {
                try
                {
                    rootParent = GameObject.Find(option.objActiveOption.rootParent).transform;
                    if (rootParent.name != option.objActiveOption.targetObjPath)
                        targetObj = rootParent.Find(option.objActiveOption.targetObjPath).gameObject;
                }
                catch
                {
                    Debug.Log($"<color=red>[{Type}]/ [{option.objActiveOption.ObjType}]</color> 기능 실행 중 <color=red>[{option.objActiveOption.targetObjPath}]</color>를 찾지 못하였습니다.");

                    if (option.objActiveOption.targetObjPath.Contains("Camera Offset") && CameraManager.Instance.CameraViewType == CameraViewType.firstPerson)
                    {
                        Debug.Log($"현재 카메라 인칭 시점 : <color=yellow>{CameraManager.Instance.CameraViewType}</color> 으로 Camera Transform Cut Event를 실행하지 않습니다.");

                        if (option.objActiveOption.ObjType == Obj_FuncType.Transform)
                            CameraManager.Instance.SetCameraVec(option.objActiveOption.pos_Obj, option.objActiveOption.rot_Obj);

                    }
                    isFinish = true;
                    return;
                }
            }

            switch (option.objActiveOption.ObjType)
            {
                case Obj_FuncType.None:
                    break;
                case Obj_FuncType.Active:
                    ActiveEvent(option.objActiveOption.isActive);
                    break;
                case Obj_FuncType.SkyMatChange:
                    SkyMatChange(option.objActiveOption);
                    break;
                case Obj_FuncType.Transform:
                    TransformEvent(option.objActiveOption);
                    break;
                case Obj_FuncType.Transform_Move:
                    StartCoroutine(TransformMoveEvent(option.objActiveOption));
                    break;
                case Obj_FuncType.MoveOn:
                    MoveBezierOn(option.objActiveOption);
                    break;
                case Obj_FuncType.SetBezierPosition:
                    StartCoroutine(SetBezierPosition(option.objActiveOption));
                    break;
                case Obj_FuncType.AllInactive:
                    AllInactiveEvent(option.objActiveOption.isActive);
                    break;
                case Obj_FuncType.ChangeLayer:
                    ChangeLayer(option.objActiveOption);
                    break;
                case Obj_FuncType.EnabledComponent:
                    ActiveComponent(option.objActiveOption);
                    break;
                case Obj_FuncType.SetParent:
                    SetParent(option.objActiveOption);
                    break;
                case Obj_FuncType.ChangeCameraView:
                    CameraManager.Instance.SwitchingCameraView(option.objActiveOption.cameraViewType);
                    break;
            }

            if (option.objActiveOption.ObjType != Obj_FuncType.Transform_Move
                && option.objActiveOption.ObjType != Obj_FuncType.SetBezierPosition)
                isFinish = true;

        }

        public void Init()
        {

        }

        public void MyUpdate()
        {

        }

        private void SetParent(ObjectOption option)
        {
            Transform targetParentRoot;
            GameObject targetParent;

            if (option.targetParentRoot != "")
            {
                targetParentRoot = GameObject.Find(option.targetParentRoot).transform;
                targetParent = targetParentRoot.Find(option.targetParentPath).gameObject;

                targetObj.transform.SetParent(targetParent.transform);
            }

            ResetTargetObj();
        }

        private void ActiveComponent(ObjectOption option)
        {
            var comp = targetObj.GetComponent(option.componentName);
            Behaviour be = comp as Behaviour; // => as 캐스팅시 가능하면 형변환, 그렇지 않으면 null을 return
            //Behaviour be = (Behaviour)comp; // => Specified cast is not valid Error

            if (be != null)
            {
                be.enabled = option.isActive;
            }
            else
            {
                Collider col = comp as Collider;
                if (col != null)
                    col.enabled = option.isActive;
                else
                    Debug.Log($"[ActibeComponent/ObjectForSequence] <color=red>[{option.componentName}]</color> 을 찾을 수 없습니다. ");
            }

            //Debug.Log($"<color=yellow>{option.componentName}/ {be.enabled}</color>");
        }

        private void ChangeLayer(ObjectOption option)
        {
            if (targetObj != null)
                targetObj.layer = option.layerNum;

            ResetTargetObj();
        }

        private void ActiveEvent(bool isActive)
        {
            if (targetObj != null)
                targetObj.SetActive(isActive);
            else
                rootParent.gameObject.SetActive(isActive);

            //ResetTargetObj();
        }

        private void AllInactiveEvent(bool isActive)
        {
            foreach (Transform child in targetObj.transform)
            {
                child.gameObject.SetActive(isActive);
            }

        }

        private void SkyMatChange(ObjectOption objOption)
        {
            if (objOption.skyMat != null)
                RenderSettings.skybox = objOption.skyMat;
            else
                RenderSettings.skybox = null;
        }

        private void TransformEvent(ObjectOption objOption)
        {
            //Debug.Log($"TransformEvent : <color=yellow>{targetObj}</color>");

            if (objOption.targetObjPath.Contains("Camera Offset"))
                CameraManager.Instance.SetCameraVec(objOption.pos_Obj, objOption.rot_Obj);

            targetObj.transform.position = objOption.pos_Obj;

            targetObj.transform.eulerAngles = objOption.rot_Obj;
            //targetObj.transform.localRotation = Quaternion.Euler(objOption.rot_Obj); // 위랑 같음

            ResetTargetObj();
        }

        private IEnumerator TransformMoveEvent(ObjectOption objOption)
        {
            float timer = 0;
            Vector3 startPos = targetObj.transform.localPosition;

            while (timer < objOption.moveTime)
            {
                timer += Time.deltaTime;
                float timerRatio = timer / objOption.moveTime;

                targetObj.transform.localPosition = Vector3.Lerp(startPos, objOption.pos_Obj, objOption.curveMove.Evaluate(timerRatio));

                yield return null;
            }

            targetObj.transform.localPosition = objOption.pos_Obj;
            ResetTargetObj();

            isFinish = true;
        }

        private void MoveBezierOn(ObjectOption objOption)
        {
            BezierWalkerWithTime bezier;

            if (targetObj != null)
                bezier = targetObj.GetComponent<BezierWalkerWithTime>();
            else
                bezier = rootParent.GetComponent<BezierWalkerWithTime>();


            if (bezier != null)
                bezier.MoveOn = objOption.moveOn;

            ResetTargetObj();
        }

        private IEnumerator SetBezierPosition(ObjectOption objectOption)
        {
            ObjectMovingControll.Instance.SetObjectLocation(objectOption.bezierPos);

            while (ObjectMovingControll.Instance.BezierPosion < objectOption.bezierPos)
            {
                yield return null;
            }

            isFinish = true;
            //Debug.Log($"<color=yellow> Bezier Pos 설정 완료 : {objectOption.bezierPos}/ {isFinish}</color>");
        }

        private void ResetTargetObj()
        {
            rootParent = null;
            targetObj = null;
        }

    }
}
