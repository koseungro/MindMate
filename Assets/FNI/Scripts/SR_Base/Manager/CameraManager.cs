using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FNI;
using UnityEditor;

public class CameraManager : MonoBehaviour
{
    #region Singleton
    private static CameraManager _instance;
    public static CameraManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<CameraManager>();

            return _instance;
        }
    }
    #endregion

    public CameraViewType CameraViewType { get => cameraViewType; }
    [SerializeField]
    private CameraViewType cameraViewType = CameraViewType.None;

    private Transform cameraOffset;
    public Transform CameraOffset
    {
        get
        {
            if (cameraOffset == null)
                cameraOffset = transform.Find("Camera Offset").transform;

            return cameraOffset;
        }
    }

    public Vector3 CameraPos { get => cameraPos; set => cameraPos = value; }
    public Vector3 CameraRot { get => cameraRot; set => cameraRot = value; }
    /// <summary>
    /// 여행 중 3인칭 Camera View에 사용될 카메라 위치 정보
    /// </summary>
    private Vector3 cameraPos = new Vector3();
    private Vector3 cameraRot = new Vector3();

    /// <summary>
    /// 1인칭 Camera View의 Target이 될 오브젝트(ex.Seed)
    /// </summary>
    private Transform targetObject;
    public Transform TargetObject
    {
        get
        {
            if (targetObject == null)
                targetObject = cameraParent.parent.GetChild(0).transform;

            return targetObject;
        }
    }

    public Camera myCamera;

    public bool useCameraDamping = false;
    public float cameraDistance = 6.0F;
    /// <summary>
    /// 값이 작을수록 빨리 위치 이동
    /// </summary>
    public float smoothPosTime = 0.75f;
    /// <summary>
    /// 값이 클수록 빨리 회전
    /// </summary>
    public float smoothRotSpeed = 50f;
    private Vector3 velocity = Vector3.zero;

    public float yDistance = 0f;
    /// <summary>
    /// TargetObject로부터 떨어질 거리
    /// </summary>
    public float zDistance = -2f;
    public bool CanMoveView { get => canMoveView; set => canMoveView = value; }
    /// <summary>
    /// 카메라 시점 이동 기능 On/ Off
    /// </summary>
    private bool canMoveView = false;

    /// <summary>
    /// Camera Offset의 부모가 될 Transform/ TargetObject와 Position, Rotation 같음
    /// </summary>
    private Transform cameraParent;

    private void Reset()
    {
        myCamera = Camera.main;
        canMoveView = false;
    }

    void Start()
    {
        if (cameraViewType != CameraViewType.None)
            Debug.Log($"<color=red> 카메라 View Type을 확인해주세요 : {cameraViewType}</color>");
    }

    /// <summary>
    /// 카메라 View 스위칭
    /// </summary>
    /// <param name="type">카메라 인칭 시점</param>
    public void SwitchingCameraView(CameraViewType type)
    {
        if (GameObject.Find("FirstPersonView"))
        {
            Vector3 cameraPos;

            switch (type)
            {
                case CameraViewType.firstPerson: // 1인칭
                    cameraParent = GameObject.Find("FirstPersonView").transform;
                    CameraOffset.SetParent(cameraParent, false);

                    cameraPos = cameraParent.localPosition + new Vector3(0, yDistance, zDistance);

                    CameraOffset.localPosition = cameraPos;

                    CameraOffset.localRotation = Quaternion.Euler(Vector3.zero);

                    if (TargetObject.CompareTag("Seed")) // 꽃씨일 경우에만 카메라 바라보도록
                    {
                        //Vector3 targetDir = (myCamera.transform.position - TargetObject.position);
                        //TargetObject.rotation = Quaternion.LookRotation(targetDir);

                        TargetObject.localRotation = Quaternion.Euler(new Vector3(-5, -180, 0));
                    }

                    if (useCameraDamping)
                        StartCoroutine(DampingCameraView());
                    break;
                case CameraViewType.thirdPerson: // 3인칭
                    cameraParent = transform;
                    CameraOffset.SetParent(cameraParent, false);

                    CameraOffset.localPosition = CameraPos;
                    CameraOffset.localRotation = Quaternion.Euler(CameraRot);

                    TargetObject.localRotation = Quaternion.LookRotation(Vector3.zero);

                    break;
            }

            cameraViewType = type;
            Debug.Log($"Camera 인칭 시점 이동 완료 => TargetParent : <color=yellow>{cameraParent.name}</color>");
        }
        else
            Debug.Log($"<color=red> 시점 이동 가능한 대상 PersonObject가 없습니다.</color>");
    }

    /// <summary>
    /// 카메라 이동시 SmoothDamping 효과 적용 (1인칭일 경우에만 사용)
    /// </summary>
    /// <returns></returns>
    private IEnumerator DampingCameraView()
    {
        cameraParent.SetParent(cameraParent.root);

        yield return new WaitForSeconds(0.5f); // CameraViewType 할당 기다리기 위한 시간

        while (cameraViewType == CameraViewType.firstPerson)
        {
            //Debug.Log($"[DampingCameraView/CameraManager] =><color=cyan> {CameraViewType} </color>");

            Vector3 targetPosition = TargetObject.TransformPoint(new Vector3(0, 0, cameraDistance));
            cameraParent.position = Vector3.SmoothDamp(cameraParent.position, targetPosition, ref velocity, smoothPosTime);

            // 바람여행(Seed)는 카메라와 Object가 서로 마주봐야하기 때문에 카메라를 180도 y축으로 돌린 값으로 rotation 할당
            Quaternion targetRotation = TargetObject.tag == "Seed" ? Quaternion.Euler(0, TargetObject.eulerAngles.y + 180, 0) : Quaternion.Euler(0, TargetObject.eulerAngles.y, 0); 
            cameraParent.rotation = Quaternion.RotateTowards(cameraParent.rotation, targetRotation, smoothRotSpeed * Time.deltaTime);

            //Quaternion targetRotation = Quaternion.Euler(0, TargetObject.eulerAngles.y + 180, 0);
            //cameraParent.rotation = Quaternion.RotateTowards(cameraParent.rotation, targetRotation, smoothRotSpeed * Time.deltaTime);
            //Debug.Log($"<color=green> {TargetObject.eulerAngles.y}/ {TargetObject.localRotation.eulerAngles.y - 360f} =>{cameraParent.localRotation.eulerAngles.y}</color>");

            yield return null;
        }
    }
    public void ResetCameraView()
    {
        cameraParent = transform;
        CameraOffset.SetParent(cameraParent, false);

        CameraOffset.localPosition = CameraPos;
        CameraOffset.localRotation = Quaternion.Euler(CameraRot);

        TargetObject.localRotation = Quaternion.LookRotation(Vector3.zero);

        cameraViewType = CameraViewType.thirdPerson;
        Debug.Log($"Camera 인칭 시점 초기화 완료 => TargetParent : <color=yellow>{cameraParent.name}</color>");
    }

    /// <summary>
    /// 여행 중 3인칭 Camera View에 사용될 카메라 위치 정보 저장
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    public void SetCameraVec(Vector3 pos, Vector3 rot)
    {
        CameraPos = pos;
        CameraRot = rot;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraManager))]
public class CameraManagerEditor : Editor
{
    private CameraManager _cameraMgr;

    private CameraManager CameraMgr
    {
        get
        {
            if (_cameraMgr == null)
                _cameraMgr = base.target as CameraManager;

            return _cameraMgr;
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
                EditorGUILayout.LabelField(" <Follow 속성>", EditorStyles.boldLabel);
                //EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Camera Damping 사용 여부", GUILayout.MaxWidth(180));
                    CameraMgr.useCameraDamping = EditorGUILayout.Toggle(CameraMgr.useCameraDamping);
                }
                EditorGUILayout.EndHorizontal();

                if (CameraMgr.useCameraDamping)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("SmoothDamp 회전 이동 스피드", GUILayout.MaxWidth(190));
                            CameraMgr.smoothRotSpeed = EditorGUILayout.FloatField(CameraMgr.smoothRotSpeed, GUILayout.MaxWidth(50));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Target - Camera 거리", GUILayout.MaxWidth(180));
                            CameraMgr.cameraDistance = EditorGUILayout.FloatField(CameraMgr.cameraDistance, GUILayout.MaxWidth(50));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("SmoothDamp 위치 이동 시간", GUILayout.MaxWidth(180));
                            CameraMgr.smoothPosTime = EditorGUILayout.FloatField(CameraMgr.smoothPosTime, GUILayout.MaxWidth(50));
                        }
                        EditorGUILayout.EndHorizontal();

                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();

        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Changed Update Mode");
            EditorUtility.SetDirty(CameraMgr);
        }
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }
}
#endif