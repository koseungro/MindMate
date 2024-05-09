using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FNI;
using UnityEngine.UI;

public class CheckAnimator : MonoBehaviour
{
    private SceneData curSceneData;
    private Animator curDummyAnim;
    private Button myButton;

    // 0 : Seed, 1 : Butterfly, 2 : Turtle
    public List<ObjectBase> objectList = new List<ObjectBase>();

    private void Awake()
    {
        myButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        curSceneData = Main.Instance.CurSequence;

        switch (curSceneData.sceneID)
        {
            case "SeedSelect": curDummyAnim = objectList[0].Dummy_Animator; break;
            case "ButterflySelect": curDummyAnim = objectList[1].Dummy_Animator; break;
            case "TurtleSelect": curDummyAnim = objectList[2].Dummy_Animator; break;
        }

        StartCoroutine(CheckAnimationRoutine());
    }

    /// <summary>
    /// ���� SceneID �� �ش��ϴ� Object Animation Clip�� ���������� Button�� interactable ��Ȱ��ȭ => Ȱ��ȭ
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckAnimationRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        myButton.interactable = false; // Animation ���� ��� �� [CheckFadeCanvas/ButtonBase] interactable true üũ �ʱ�ȭ
             
        yield return new WaitForSeconds(0.4f); // Animation ���� Count ���

        myButton.interactable = false; // �ʱ�ȭ
        
        if (curDummyAnim.GetCurrentAnimatorStateInfo(0).IsName("Dummy_Selection_Move"))
        {
            float animTime = 0;

            while (animTime >= 0 && animTime < 1.0f)
            {
                animTime = curDummyAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                //Debug.Log($"{animTime}");

                yield return null;
            }

            myButton.interactable = true;
            //Debug.Log($"[CheckAnimationRoutine/CheckAnimator] ({gameObject.name}) Button Interactable : <color=yellow> {myButton.interactable} </color>");

        }

    }
}
