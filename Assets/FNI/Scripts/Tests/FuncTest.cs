using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BezierSolution;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class FuncTest : MonoBehaviour
{
    private static FuncTest _instance;
    public static FuncTest Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<FuncTest>();

            return _instance;
        }
    }

    public UnityEvent onButtonEvent;
    public event Action<bool> InteractableChanged;

    public Button myButton;

    public Material skyMat;
    public Transform moveObject;
    public Vector3 endPos;
    public float moveTime;

    public float seedPos;

    public AnimationCurve z_Curve;
    //public Breathe breath;
    public BezierWalkerWithTime bezier;

    private Animator dummy_Animator;
    public Animator Dummy_Animator
    {
        get
        {
            if (dummy_Animator = null)
                dummy_Animator = transform.parent.parent.GetComponent<Animator>();

            return dummy_Animator;
        }
    }

    private Animation anim;
    private List<string> animList = new List<string>();


    private GameObject findObject;

    public Text logText;
    public Text speedText;
    public Text timeOutText;
    private string debugString = "";

    public GameObject childCube;
    public Material startMat;
    public Material endMat;

    public Transform rotateObj;

    public Scene curScene = new Scene();

    private List<int> intList = new List<int>() {0,1,2,3,4,5,6,7,8,9};
    private List<string> stringList = new List<string>();
    private List<Transform> trList = new List<Transform>();

    private void Awake() // GameObject만 활성화 되면 최초 1번만 호출
    {
        Debug.Log("Awake");

        //anim = GetComponent<Animation>();
        //AnimationArray();

    }
    void Start() // GameObject/ Component 모두 활성화 되면 최초 1번만 호출
    {
        Debug.Log("Start");

        //GenericTest.GenericLog<int>(intList);
        //GenericTest.GenericLog<String>(stringList);
        GenericTest.GenericLog<Transform>(trList);

        //dummy_Animator = transform.parent.parent.GetComponent<Animator>();
        //myButton.onClick.AddListener(() => onButtonEvent?.Invoke());
        //onButtonEvent?.Invoke();

        //logText.text = "<Debug Text>";

        //Rigidbody rb = findObject.GetComponent(debugString) as Rigidbody;
        //Rigidbody rb = (Rigidbody)findObject.GetComponent(debugString);
        //Rigidbody rb = findObject.GetComponent<Rigidbody>();

        //foreachTest();
        StartCoroutine(CoroutineTest());
        Debug.Log("Next Start");
    }
    IEnumerator CoroutineTest()
    {
        Debug.Log("Coroutine Start");
        float checkTime = 0;
        float endTime = 3;

        while (checkTime<1.0f)
        {
            checkTime += Time.deltaTime / endTime;
            Debug.Log($"checkTime : {checkTime}");
            yield return null; // 1프레임을 코루틴 호출자에게 양보하고 대기함
        }

    }
    private void foreachTest()
    {
        foreach (int cnt in intList)
        {
            Debug.Log($"{cnt} : foreach");

            if (cnt == 3)
            {
                Debug.Log($"{cnt} : break");

                break; // 반복문(foreach) 밖으로
                return; // 메소드 밖으로
            }

        }
        Debug.Log($"foreach 끝");
    }

    private void OnEnable() // GameObject/ Component 모두 활성화 될때마다 호출
    {
        Debug.Log("OnEnable");

    }

    private void OnDisable() // GameObject 또는 Component 하나만이라도 비활성화 될때마다 호출
    {
        Debug.Log("OnDisable");

    }

    private void AnimationArray()
    {
        foreach (AnimationState state in anim)
        {
            Debug.Log($" {state.name}");
            animList.Add(state.name);
        }
    }

    public void SetLogUI(string log, float speed = 0, float timeout = 0)
    {
        //Debug.Log($"<color=magenta> [FuncTest/SetLogUI] </color>");

        if (log != "")
        {
            debugString += $"\n{log}";
            logText.text = debugString;

        }


        if (speed != 0)
        {
            string floatText = "";
            floatText = $"Speed : {speed}";
            speedText.text = floatText;

        }

        if (timeout != 0 && timeout <= 0)
        {
            string floatText = "";
            floatText += $"Timeout : {timeout}";
            timeOutText.text = floatText;
        }
    }

    public void ButtonFuncTest()
    {
        Debug.Log($"<color=cyan> Button Event </color>");
    }
    float checkTime = 0;
    void Update()
    {
        //float lerp = Mathf.PingPong(Time.time/2f, 1f);
        //childCube.GetComponent<Renderer>().material.Lerp(startMat, endMat, lerp);
        //Debug.Log($"<color=yellow> {lerp}</color>");
#if UNITY_EDITOR
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    Debug.Log($"<color=yellow> Scene Load </color>");
        //    StartCoroutine(RemoveScene("Prologue_Doors"));
        //}

        if (Input.GetKeyDown(KeyCode.R))
        {
            //rotateObj.Rotate(new Vector3(0, 180, 0));
            rotateObj.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            //StartCoroutine(Timer.Instance.TimerStart());
            StartCoroutine(TransformMove());
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            bezier = FindObjectOfType<BezierWalkerWithTime>();

            if (bezier != null)
                bezier.MoveOn = true;
            ObjectMovingControll.Instance.SetObjectLocation(seedPos);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            myButton.interactable = !myButton.interactable;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            FindGameObject();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            curScene = this.gameObject.scene;
            Debug.Log($" {curScene.name}/ {this.gameObject.scene.name}/ {this.gameObject.scene}");
        }
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    CameraManager.Instance.SwitchingCameraView(CameraViewType.firstPerson);
        //}
#endif

    }

    int cnt = 0;
    private void ReturnTest()
    {
        float timer = 0f;
        Debug.Log($"1");

        if (timer <= 10)
        {
            timer += Time.deltaTime;
            Debug.Log($"2");


            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log($"<color=yellow> Return </color>");
                return; // 함수 밖으로
            }
            Debug.Log($"3");
        }
        cnt++;
        Debug.Log($"4/ [{cnt}]");
    }
    Material mat;
    GameObject findObj;
    private void FindGameObject()
    {
        Debug.Log("Find!");
        //findObject = GameObject.Find("FirstPersonView");

        mat = gameObject.GetComponentInChildren<MeshRenderer>().material;
        findObj = transform.Find("FindObj").gameObject;
    }

    private IEnumerator TransformMove()
    {
        Vector3 startPos = moveObject.transform.position;

        float checkTime = 0;

        while (checkTime < 1.0f)
        {
            checkTime += Time.deltaTime / moveTime;
            float z_Pos = endPos.z * z_Curve.Evaluate(checkTime);

            moveObject.position = Vector3.Lerp(startPos, new Vector3(endPos.x, endPos.y, z_Pos), checkTime);

            yield return null;
        }

        Debug.Log($"{moveObject.position}");
    }

    private void MatChange()
    {
        RenderSettings.skybox = skyMat;
    }

    private IEnumerator Load(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }

    private IEnumerator RemoveScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }
}



