using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FNI.XR;
using UnityEngine.UI;

public class ControllerBlendWidget : MonoBehaviour
{
    private static ControllerBlendWidget _instance;
    public static ControllerBlendWidget Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ControllerBlendWidget>();

            return _instance;
        }
    }

    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Mesh skinnedMesh;
    public Material widgetMat;
    public Material endMat;

    private int blendShapeCount;
    private float blendShapeValue = 100;

    /// <summary>
    /// ���� ������ ���� �� �ð� üũ
    /// </summary>
    private float morphCheckTime = 0.0f;

    /// <summary>
    /// ���� �������� ���� �� �����ϰ� �־�� �ϴ� �ð�
    /// </summary>
    public float morphCompleteTime = 3f;

    /// <summary>
    /// ��ǥ ���ΰ��� �����ߴ��� ����
    /// </summary>
    private bool checkMorphComplete = false;

    /// <summary>
    /// ���� �Ϸ� ����
    /// </summary>
    private bool morphComlete = false;
    [SerializeField] private Text morphTimeText;

    private IEnumerator morphCheckRoutine;

    /// <summary>
    /// ��Ʈ�ѷ��� x�� �̵� ����
    /// </summary>
    private float controllerX;

    /// <summary>
    /// Widget�� Y ȸ����
    /// </summary>
    private float WidgetY;

    /// <summary>
    /// ȸ���� ������ �� Widget�� y�� ����
    /// </summary>
    private float endWidgetY;

    /// <summary>
    /// Widget ȸ���� ���������� ControllerX �ʱ� ��ġ ���� �� ����
    /// </summary>
    private float startPosValue = 0;

    /// <summary>
    /// ��Ʈ�ѷ� ��ġ �̵� ��
    /// </summary>
    private float posMoveValue = 0;

    /// <summary>
    /// ��Ʈ�ѷ� Position Vector
    /// </summary>
    private Vector3 controllerVec = new Vector3();

    public bool onSelect = false;
    private bool canRotate = true;

    /// <summary>
    /// Widget�� ȸ�� ���ǵ�( �ΰ���)
    /// </summary>
    [Range(1, 5)]
    public float rotSpeed = 2;

    private Transform controllerTr;

    private void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
    }

    void Start()
    {
        blendShapeCount = skinnedMesh.blendShapeCount;
        //widgetMat = skinnedMeshRenderer.material;

        skinnedMeshRenderer.SetBlendShapeWeight(blendShapeCount - 1, blendShapeValue);
    }

    private void Update()
    {
        if (controllerTr != null)
        {
            controllerVec = controllerTr.localPosition;
            controllerX = controllerVec.x;

            if (startPosValue == 0)
                startPosValue = controllerX;

            posMoveValue = (controllerX - startPosValue) * (180f * rotSpeed);

            transform.localRotation = Quaternion.Euler(0, posMoveValue + endWidgetY, 0); // Widget ȸ��
            WidgetY = Mathf.Abs(transform.localRotation.eulerAngles.y < 180f ? transform.localRotation.eulerAngles.y : transform.localRotation.eulerAngles.y - 360f);

            SetBlendShapeValue(WidgetY);

            //if (posMoveValue > 0) // ������ ȸ��
            //{
            //    Debug.Log($"<color=magenta>Right [{startPosValue}] /[{controllerX}]/ [{posMoveValue}]</color>");


            //}
            //else // ����
            //{
            //    Debug.Log($"<color=cyan>Left [{startPosValue}] /[{controllerX}]/ [{posMoveValue}]</color>");

            //}
        }

        CheckMorph();
    }

    /// <summary>
    /// Widget�� BlendShape �� ����
    /// </summary>
    private void SetBlendShapeValue(float widgetY)
    {
        blendShapeValue = Mathf.Abs(widgetY - 180f) * (5f / 9f);
        skinnedMeshRenderer.SetBlendShapeWeight(blendShapeCount - 1, blendShapeValue);

        //// Material ����
        //float lerp = widgetY / 180f;
        //skinnedMeshRenderer.material.Lerp(widgetMat, endMat, lerp);
    }

    private void CheckMorph()
    {
        if (blendShapeValue < 2f)
        {
            if (checkMorphComplete == false)
            {
                if (morphCheckRoutine != null)
                    StopCoroutine(morphCheckRoutine);

                morphCheckRoutine = CheckMorphTime();
                StartCoroutine(morphCheckRoutine);

                checkMorphComplete = true;
            }
        }
        else // Reset
        {
            if (checkMorphComplete)
            {
                StopCoroutine(morphCheckRoutine);

                StartCoroutine(ResetWidgetMat());

                checkMorphComplete = false;
                morphCheckTime = 0f;
                morphTimeText.text = "00.00";
            }
        }
    }

    private IEnumerator CheckMorphTime()
    {
        while (morphCheckTime < morphCompleteTime)
        {
            morphCheckTime += Time.deltaTime;
            morphTimeText.text = morphCheckTime.ToString("00.00");
            yield return null;
        }

        skinnedMeshRenderer.SetBlendShapeWeight(blendShapeCount - 1, 0);
        morphTimeText.text = morphCompleteTime.ToString("00.00");
        morphComlete = true;
        Debug.Log($"<color=magenta> Morph Completed </color>");

        yield return new WaitForSeconds(0.5f);

        // Material ���� Routine
        float materialTimer = 0f;

        while (materialTimer < 1.0f)
        {
            materialTimer += Time.deltaTime / 3f;
            skinnedMeshRenderer.material.Lerp(widgetMat, endMat, materialTimer);
            yield return null;
        }

        skinnedMeshRenderer.material = endMat;
        Debug.Log($"<color=magenta> Material Change Completed </color>");

        gameObject.GetComponent<ControllerBlendWidget>().enabled = false;

    }

    private IEnumerator ResetWidgetMat()
    {
        float checkTime = 0f;
        Material curMat = skinnedMeshRenderer.material;

        while (checkTime < 1.0f)
        {
            checkTime += Time.deltaTime;
            skinnedMeshRenderer.material.Lerp(curMat, widgetMat, checkTime);
            yield return null;
        }

        Debug.Log("Reset Material");
        skinnedMeshRenderer.material = widgetMat;
    }

    public void RotateWidget(bool triggerSelected)
    {
        onSelect = triggerSelected;

        if (controllerTr == null)
        {
            switch (XRManager.Instance.CurController)
            {
                case HandType.None:
                    break;
                case HandType.LeftHand:
                    controllerTr = XRManager.Instance.LeftControllerObject.transform;
                    break;
                case HandType.RightHand:
                    controllerTr = XRManager.Instance.RightControllerObject.transform;
                    break;
            }

        }
    }

    public void SelectUp(bool triggerSelected)
    {
        onSelect = triggerSelected;

        controllerTr = null;
        startPosValue = 0;
        posMoveValue = 0;

        //endWidgetY = transform.localRotation.eulerAngles.y - 360;
        endWidgetY = transform.localRotation.eulerAngles.y < 180f ? transform.localRotation.eulerAngles.y : transform.localRotation.eulerAngles.y - 360f;


        Debug.Log($"Select Up");
    }
}
