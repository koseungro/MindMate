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
    /// ���� �� 3��Ī Camera View�� ���� ī�޶� ��ġ ����
    /// </summary>
    private Vector3 cameraPos = new Vector3();
    private Vector3 cameraRot = new Vector3();

    /// <summary>
    /// 1��Ī Camera View�� Target�� �� ������Ʈ(ex.Seed)
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
    /// ���� �������� ���� ��ġ �̵�
    /// </summary>
    public float smoothPosTime = 0.75f;
    /// <summary>
    /// ���� Ŭ���� ���� ȸ��
    /// </summary>
    public float smoothRotSpeed = 50f;
    private Vector3 velocity = Vector3.zero;

    public float yDistance = 0f;
    /// <summary>
    /// TargetObject�κ��� ������ �Ÿ�
    /// </summary>
    public float zDistance = -2f;
    public bool CanMoveView { get => canMoveView; set => canMoveView = value; }
    /// <summary>
    /// ī�޶� ���� �̵� ��� On/ Off
    /// </summary>
    private bool canMoveView = false;

    /// <summary>
    /// Camera Offset�� �θ� �� Transform/ TargetObject�� Position, Rotation ����
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
            Debug.Log($"<color=red> ī�޶� View Type�� Ȯ�����ּ��� : {cameraViewType}</color>");
    }

    /// <summary>
    /// ī�޶� View ����Ī
    /// </summary>
    /// <param name="type">ī�޶� ��Ī ����</param>
    public void SwitchingCameraView(CameraViewType type)
    {
        if (GameObject.Find("FirstPersonView"))
        {
            Vector3 cameraPos;

            switch (type)
            {
                case CameraViewType.firstPerson: // 1��Ī
                    cameraParent = GameObject.Find("FirstPersonView").transform;
                    CameraOffset.SetParent(cameraParent, false);

                    cameraPos = cameraParent.localPosition + new Vector3(0, yDistance, zDistance);

                    CameraOffset.localPosition = cameraPos;

                    CameraOffset.localRotation = Quaternion.Euler(Vector3.zero);

                    if (TargetObject.CompareTag("Seed")) // �ɾ��� ��쿡�� ī�޶� �ٶ󺸵���
                    {
                        //Vector3 targetDir = (myCamera.transform.position - TargetObject.position);
                        //TargetObject.rotation = Quaternion.LookRotation(targetDir);

                        TargetObject.localRotation = Quaternion.Euler(new Vector3(-5, -180, 0));
                    }

                    if (useCameraDamping)
                        StartCoroutine(DampingCameraView());
                    break;
                case CameraViewType.thirdPerson: // 3��Ī
                    cameraParent = transform;
                    CameraOffset.SetParent(cameraParent, false);

                    CameraOffset.localPosition = CameraPos;
                    CameraOffset.localRotation = Quaternion.Euler(CameraRot);

                    TargetObject.localRotation = Quaternion.LookRotation(Vector3.zero);

                    break;
            }

            cameraViewType = type;
            Debug.Log($"Camera ��Ī ���� �̵� �Ϸ� => TargetParent : <color=yellow>{cameraParent.name}</color>");
        }
        else
            Debug.Log($"<color=red> ���� �̵� ������ ��� PersonObject�� �����ϴ�.</color>");
    }

    /// <summary>
    /// ī�޶� �̵��� SmoothDamping ȿ�� ���� (1��Ī�� ��쿡�� ���)
    /// </summary>
    /// <returns></returns>
    private IEnumerator DampingCameraView()
    {
        cameraParent.SetParent(cameraParent.root);

        yield return new WaitForSeconds(0.5f); // CameraViewType �Ҵ� ��ٸ��� ���� �ð�

        while (cameraViewType == CameraViewType.firstPerson)
        {
            //Debug.Log($"[DampingCameraView/CameraManager] =><color=cyan> {CameraViewType} </color>");

            Vector3 targetPosition = TargetObject.TransformPoint(new Vector3(0, 0, cameraDistance));
            cameraParent.position = Vector3.SmoothDamp(cameraParent.position, targetPosition, ref velocity, smoothPosTime);

            // �ٶ�����(Seed)�� ī�޶�� Object�� ���� ���ֺ����ϱ� ������ ī�޶� 180�� y������ ���� ������ rotation �Ҵ�
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
        Debug.Log($"Camera ��Ī ���� �ʱ�ȭ �Ϸ� => TargetParent : <color=yellow>{cameraParent.name}</color>");
    }

    /// <summary>
    /// ���� �� 3��Ī Camera View�� ���� ī�޶� ��ġ ���� ����
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
                EditorGUILayout.LabelField(" <Follow �Ӽ�>", EditorStyles.boldLabel);
                //EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Camera Damping ��� ����", GUILayout.MaxWidth(180));
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
                            EditorGUILayout.LabelField("SmoothDamp ȸ�� �̵� ���ǵ�", GUILayout.MaxWidth(190));
                            CameraMgr.smoothRotSpeed = EditorGUILayout.FloatField(CameraMgr.smoothRotSpeed, GUILayout.MaxWidth(50));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Target - Camera �Ÿ�", GUILayout.MaxWidth(180));
                            CameraMgr.cameraDistance = EditorGUILayout.FloatField(CameraMgr.cameraDistance, GUILayout.MaxWidth(50));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("SmoothDamp ��ġ �̵� �ð�", GUILayout.MaxWidth(180));
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