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
    /// 현재 SceneID 에 해당하는 Object Animation Clip이 끝날때까지 Button의 interactable 비활성화 => 활성화
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckAnimationRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        myButton.interactable = false; // Animation 시작 대기 전 [CheckFadeCanvas/ButtonBase] interactable true 체크 초기화
             
        yield return new WaitForSeconds(0.4f); // Animation 시작 Count 대기

        myButton.interactable = false; // 초기화
        
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
