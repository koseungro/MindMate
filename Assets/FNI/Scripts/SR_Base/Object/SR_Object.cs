using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SR_Object : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private IEnumerator matChange_Routine;

    private Material mat;

    private void Start()
    {
        //mat = GetComponent<MeshRenderer>().material;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("Object Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("Object Exit");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("Object Click");

        // 코루틴
        //if (matChange_Routine != null)
        //    StopCoroutine(matChange_Routine);
        //matChange_Routine = MatChange();
        //StartCoroutine(matChange_Routine);
    }



    private IEnumerator MatChange()
    {
        Color cur_Color = mat.color;
        Color new_Color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        float progress = 0;

        Debug.Log("<color=yellow> 색상 변경 </color>");


        while (progress < 1)
        {
            progress += Time.deltaTime / 2f;
            mat.color = Color.Lerp(cur_Color, new_Color, progress);

            yield return null;
        }
        mat.color = new_Color;

    }
}
