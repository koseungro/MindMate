using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 오브젝트 Outline을 위한 Base 클래스 (최상위 부모에 부착)
/// </summary>
public class ObjectParent : MonoBehaviour
{

    private bool onOutline = false;

    private void Start()
    {
        Init();
    }


    private void Init()
    {
        // 오브젝트의 태그 설정
        //gameObject.tag = characterType.ToString();
    }

    public void Click()
    {
        print($"Click 오브젝트 이름 : {gameObject.name}");

        // Cut이 진행중이지 않을 때 클릭하면 UI 나오게
        //if (!MyMain.IsPlaying)
        //{

        //}

        // UI 끌 수도 있도록
    }

}


