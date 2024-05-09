using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleBase : UIBase
{
    public Toggle MyToggle
    {
        get
        {
            if (myToggle == null)
                myToggle = GetComponent<Toggle>();

            return myToggle;
        }
    }
    public Text MyText
    {
        get
        {
            if (myText == null)
                myText = MyToggle.transform.Find("Text").GetComponent<Text>();

            return myText;
        }
    }

    private Toggle myToggle;
    private Text myText;

    protected override void Start()
    {
        base.Start();
    }

    public void ValueChange()
    {
        //print($"Slider Value : {MySlider.value}");
        MyText.text = "Toggle : " + MyToggle.isOn;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        print("Toggle Enter");
        base.OnPointerEnter(eventData);
    }
}
