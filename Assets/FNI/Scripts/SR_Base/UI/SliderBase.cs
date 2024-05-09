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
        feelingTexts.Add("매우 슬퍼요"); // 0
        feelingTexts.Add("슬퍼요");      // 1
        feelingTexts.Add("조금 슬퍼요"); // 2
        feelingTexts.Add("무난해요");    // 3
        feelingTexts.Add("조금 기뻐요"); // 4
        feelingTexts.Add("기뻐요");      // 5
        feelingTexts.Add("매우 기뻐요"); // 6

        tensionTexts.Add("매우 긴장돼요"); // 0
        tensionTexts.Add("긴장돼요");      // 1
        tensionTexts.Add("조금 긴장돼요"); // 2
        tensionTexts.Add("무난해요");    // 3
        tensionTexts.Add("조금 편안해요"); // 4
        tensionTexts.Add("편안해요");      // 5
        tensionTexts.Add("매우 편안해요"); // 6
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="textCnt"> 0: 감정 문구/ 1: 안정감 문구</param>
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
