using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_TrainingReset : MonoBehaviour
{
    private Button myButton;
    private Button MyButton
    {
        get
        {
            if (myButton == null)
                myButton = GetComponent<Button>();

            return myButton;
        }
    }

    private Main myMain;
    private Main MyMain
    {
        get
        {
            if (myMain == null)
                myMain = GameObject.Find("---------------Scripts/Main").GetComponent<Main>();

            return myMain;
        }
    }

    /// <summary>
    /// 훈련 종료 버튼 선택 시 돌아갈 씬 데이터
    /// </summary>
    public SceneData resetScene;

    public bool IsHide { get => IsHide; }
    private bool isHide = true;
    private float timer;

    private void Start()
    {
        if (resetScene != null)
            AddResetSceneEvent();
        else
            print("Reset Scene이 Null");
    }

    public void AddResetSceneEvent()
    {
        MyButton.onClick.AddListener(() => MyMain.OnButtonSequence(resetScene));
    }

    private void OnEnable()
    {
        isHide = false;
        StartCoroutine(HideButton());
    }

    public void ShowButton()
    {
        gameObject.SetActive(true);
    }

    private IEnumerator HideButton()
    {
        if (isHide == false)
        {
            timer = 0f;

            while (timer < 5.0f)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            isHide = true;
            gameObject.SetActive(false);
        }
    }
}
