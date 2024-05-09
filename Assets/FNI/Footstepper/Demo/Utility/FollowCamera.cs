using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

public class FollowCamera : MonoBehaviour
{
    public Camera camera2Follow;
    public float cameraDistance = 6.0F;
    /// <summary>
    /// ���� �������� ���� ��ġ �̵�
    /// </summary>
    public float smoothPosTime = 0.75f;
    /// <summary>
    /// ���� Ŭ���� ���� ȸ��
    /// </summary>
    public float smoothRotSpeed = 50f;
    private Vector3 velocity = Vector3.zero;
    private Transform target;

    public float xPos = 0;
    public float yPos = 0;

    /// <summary>
    /// ȸ�� Smooth Damping ��� ����
    /// </summary>
    public bool useDampingRotation = false;

    void Awake()
    {
        target = camera2Follow.transform;
    }


    void Update()
    {
        // Define my target position in front of the camera ->
        Vector3 targetPosition = target.TransformPoint(new Vector3(xPos, yPos, cameraDistance));

        // Smoothly move my object towards that position ->
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothPosTime);


        if (useDampingRotation)
        {
            // version 1 : my object's rotation isn't finished synchronously with the position smooth.damp ->
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, smoothRotSpeed * Time.deltaTime);
        }
        else
        {
            // version 2: my object's rotation is always facing to camera with no dampening  ->
            transform.LookAt(transform.position + camera2Follow.transform.rotation * Vector3.forward, camera2Follow.transform.rotation * Vector3.up);
        }

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FollowCamera))]
public class FollowCameraEditor : Editor
{
    private FollowCamera _followCam;

    private FollowCamera FollowCam
    {
        get
        {
            if (_followCam == null)
                _followCam = base.target as FollowCamera;

            return _followCam;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        {
            EditorGUILayout.Space();


            EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            {
                EditorGUILayout.LabelField(" <Follow �Ӽ�>", EditorStyles.boldLabel);
                //EditorGUI.indentLevel++;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("UI - Camera �Ÿ�", GUILayout.MaxWidth(180));
                        FollowCam.cameraDistance = EditorGUILayout.FloatField(FollowCam.cameraDistance, GUILayout.MaxWidth(50));
                    }
                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("UI - X Position", GUILayout.MaxWidth(180));
                        FollowCam.xPos = EditorGUILayout.FloatField(FollowCam.xPos, GUILayout.MaxWidth(50));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("UI - Y Position", GUILayout.MaxWidth(180));
                        FollowCam.yPos = EditorGUILayout.FloatField(FollowCam.yPos, GUILayout.MaxWidth(50));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("SmoothDamp ��ġ �̵� �ð�", GUILayout.MaxWidth(180));
                        FollowCam.smoothPosTime = EditorGUILayout.FloatField(FollowCam.smoothPosTime, GUILayout.MaxWidth(50));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("ȸ�� Damping ��� ����", GUILayout.MaxWidth(180));
                        FollowCam.useDampingRotation = EditorGUILayout.Toggle(FollowCam.useDampingRotation);
                    }
                    EditorGUILayout.EndHorizontal();

                    if (FollowCam.useDampingRotation)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("SmoothDamp ȸ�� �̵� ���ǵ�", GUILayout.MaxWidth(190));
                            FollowCam.smoothRotSpeed = EditorGUILayout.FloatField(FollowCam.smoothRotSpeed, GUILayout.MaxWidth(50));
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Changed Update Mode");
            EditorUtility.SetDirty(FollowCam);
        }
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }
}
#endif

