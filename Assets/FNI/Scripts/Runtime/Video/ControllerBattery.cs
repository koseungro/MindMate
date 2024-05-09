/// 작성자: 작성자 이름
/// 작성일: 2023-07-12
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;


namespace FNI
{
    public class ControllerBattery : MonoBehaviour
    {
        public InputDevice rightController;
        public InputDevice hmd;

        public Text text;

        private void Start()
        {
            Debug.Log(SystemInfo.batteryLevel);
        }

        private void Update()
        {
            if(!rightController.isValid || !hmd.isValid)
            {
                InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, ref rightController);
            }

            if(Input.GetKeyDown(KeyCode.V))
            {
                if(rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightvelocity))
                {
                    Debug.Log(rightvelocity.magnitude);
                }
            }

#if !UNITY_EDITOR
            // dasdsad
            if (Input.GetKeyDown(KeyCode.B))
            {
                text.text = "100";
            }
            text.text = SystemInfo.batteryLevel.ToString();
            Debug.Log(rightController.TryGetFeatureValue(CommonUsages.batteryLevel, out float _rightBattery));

#else

            text.text = SystemInfo.batteryLevel.ToString();

            if (rightController.isValid)
            {
                Debug.Log(rightController.TryGetFeatureValue(CommonUsages.batteryLevel, out float _rightBattery));
                //text.text = $"{rightController.TryGetFeatureValue(CommonUsages.batteryLevel, out float _rightBattery)}";

                if (rightController.TryGetFeatureValue(CommonUsages.batteryLevel, out float rightBattery))
                {
                    Debug.Log(rightBattery);
                    text.text = rightBattery.ToString();
                }
            }
#endif
        }
        private void InitializeInputDevice(InputDeviceCharacteristics inputCharacteristics, ref InputDevice inputDevice)
        {
            List<InputDevice> devices = new List<InputDevice>();

            InputDevices.GetDevicesWithCharacteristics(inputCharacteristics, devices);

            if (devices.Count > 0)
                inputDevice = devices[0];

            if(inputDevice.TryGetFeatureValue(CommonUsages.batteryLevel, out float rightBattery))
            {
                Debug.Log(rightBattery);
            }
        }
    }
}