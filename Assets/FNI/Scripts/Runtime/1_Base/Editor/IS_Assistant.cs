using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class IS_Assistant : MonoBehaviour
{
	[MenuItem("IS/Assistant/Get Parent Path Key #n")]//%: 컨트롤키, #: 쉬프트, &: 알트
	private static void GetParentPath()
	{
		Transform[] selects = Selection.transforms;

		for (int cnt = 0; cnt < selects.Length; cnt++)
		{
			Debug.Log(ParentName(selects[cnt]));
		}
	}

	private static string ParentName(Transform me)
	{
		string name = me.name;

		if (me.parent != null)
			name = ParentName(me.parent) + "/" + name;

		return name;
	}

    [MenuItem("IS/Select/Count")]
    private static void SelectCounter()
    {
        Debug.Log(Selection.gameObjects.Length + "개가 선택되었습니다.");
    }
    [MenuItem("IS/Select/Names")]
    private static void SelectNames()
    {
        Transform[] selects = Selection.transforms;
        string names = "";
        for (int cnt = 0; cnt < selects.Length; cnt++)
        {
            names += selects[cnt].name + "," + Environment.NewLine;
        }
        Debug.Log(names);
    }
    [MenuItem("IS/Select/Child #c")]
    private static void SelectChild()
    {
        GameObject[] parents = Selection.gameObjects;
        List<GameObject> newSelect = new List<GameObject>();

        for (int cnt = 0; cnt < parents.Length; cnt++)
        {
            if (parents[cnt].transform.childCount != 0)
            {
                for (int _cnt = 0; _cnt < parents[cnt].transform.childCount; _cnt++)
                {
                    newSelect.Add(parents[cnt].transform.GetChild(_cnt).gameObject);
                }
            }
            else
            {
                newSelect.Add(parents[cnt].transform.gameObject);
            }
        }
        Selection.objects = newSelect.ToArray();
    }
    [MenuItem("IS/Select/Parent #p")]
    private static void SelectParent()
    {
        GameObject[] parents = Selection.gameObjects;
        List<GameObject> newSelect = new List<GameObject>();

        for (int cnt = 0; cnt < parents.Length; cnt++)
        {
            GameObject parent = parents[cnt].transform.parent.gameObject;

            if (!newSelect.Contains(parent))
                newSelect.Add(parent);
        }
        Selection.objects = newSelect.ToArray();
    }
    [MenuItem("IS/Select/NoChild &#c")]
    private static void SelectNoChild()
    {
        GameObject[] parents = Selection.gameObjects;
        List<GameObject> newSelect = new List<GameObject>();

        for (int cnt = 0; cnt < parents.Length; cnt++)
        {
            if (parents[cnt].transform.childCount == 0)
            {
                newSelect.Add(parents[cnt].transform.gameObject);
            }
        }
        Selection.objects = newSelect.ToArray();
    }
}
