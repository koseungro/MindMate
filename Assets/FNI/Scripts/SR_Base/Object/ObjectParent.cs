using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ������Ʈ Outline�� ���� Base Ŭ���� (�ֻ��� �θ� ����)
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
        // ������Ʈ�� �±� ����
        //gameObject.tag = characterType.ToString();
    }

    public void Click()
    {
        print($"Click ������Ʈ �̸� : {gameObject.name}");

        // Cut�� ���������� ���� �� Ŭ���ϸ� UI ������
        //if (!MyMain.IsPlaying)
        //{

        //}

        // UI �� ���� �ֵ���
    }

}


