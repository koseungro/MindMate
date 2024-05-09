using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SliderBase : UIBase
{

    private Slider mySlider;
    public Slider MySlider
    {
        get
        {
            if (mySlider == null)
                mySlider = GetComponent<Slider>();

            return mySlider;
        }
    }


    [SerializeField] private Text text;

    private List<string> feelingTexts = new List<string>();
    private List<string> tensionTexts = new List<string>();

    protected override void Start()
    {
        base.Start();

        Init();
    }

    private void Init()
    {
        feelingTexts.Add("�ſ� ���ۿ�"); // 0
        feelingTexts.Add("���ۿ�");      // 1
        feelingTexts.Add("���� ���ۿ�"); // 2
        feelingTexts.Add("�����ؿ�");    // 3
        feelingTexts.Add("���� �⻵��"); // 4
        feelingTexts.Add("�⻵��");      // 5
        feelingTexts.Add("�ſ� �⻵��"); // 6

        tensionTexts.Add("�ſ� ����ſ�"); // 0
        tensionTexts.Add("����ſ�");      // 1
        tensionTexts.Add("���� ����ſ�"); // 2
        tensionTexts.Add("�����ؿ�");    // 3
        tensionTexts.Add("���� ����ؿ�"); // 4
        tensionTexts.Add("����ؿ�");      // 5
        tensionTexts.Add("�ſ� ����ؿ�"); // 6
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="textCnt"> 0: ���� ����/ 1: ������ ����</param>
    public void ValueChange(int textCnt)
    {
        switch (textCnt)
        {
            case 0:
                text.text = feelingTexts[(int)MySlider.value];
                break;
            case 1:
                text.text = tensionTexts[(int)MySlider.value];
                break;
        }
        
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //print("Slider Click");
        base.OnPointerClick(eventData);

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        //print("Slider Enter");
        base.OnPointerEnter(eventData);
    }
}
