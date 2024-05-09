using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class GenericTest : MonoBehaviour
{
    public event EventHandler Click;
    private static GenericTest _instance;
    public static GenericTest Instance
    {
        get
        {
            if (_instance = null)
                _instance = FindObjectOfType<GenericTest>();

            return _instance;
        }
    }

    public string GenericString { get => genericString; set => genericString = value; }
    private string genericString = "";

    private bool checkBool;

    private List<int> intList = new List<int> { 1, 2, 3, 4, 5, 6 };

    private delegate void genericDel<T>(T a, T b);
    genericDel<int> intCalculate;
    genericDel<float> floatCalculate;

    public class Character
    {
        public string Name = "";
        public string Class = "";
    }
    public class Skill
    {
        public string Name = "";
        public float power = 0;
    }

    private List<Character> charList = new List<Character>();

    //public GenericTest()
    //{
    //    Debug.Log("<color=cyan> GenericTest 생성 </color>");
    //}

    private void Start()
    {
        Debug.Log("<color=yellow> GenericTest Start </color>");
        GenericT<int>(cnt);

        checkBool = OutMethod(a, b, out int _cnt); // out은 변수 초기화를 안해도 되기 때문에 int _cnt로 사용 가능
        Debug.Log($"{checkBool}, {_cnt}");

        var tuple = TupleMethod(floatList);
        Debug.Log($"{tuple.name}, {tuple.cnt}");

        intCalculate += Cal;
    }

    public void Cal(int a, int b)
    {
        int sum = a + b;
        Debug.Log($"Int Sum : {sum}");
    }

    public static void GenericLog<T>(List<T> list) where T : Component
    {
        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log($"{list[i].ToString()}");
        }
    }

    private void GenericT<T>(T x)
    {
        var temp = x;
        Debug.Log(temp.ToString());
    }

    private void WhereTest()
    {
        //var linqQueryVar = from num in intList
        //                 where num %3 ==0
        //                 select num;

        List<int> linqQueryResult = (List<int>)(from num in intList // Linq 쿼리
                                                where num % 3 == 0
                                                select num);

        var _linqQueryResult = (from num in intList // Linq 쿼리
                                where num % 3 == 0
                                select num);

        var linqMethodVar = intList.Where(num => num % 3 == 0); // Linq 람다식 사용

        List<int> linqMethodResult = intList.Where(num => num % 3 == 0).ToList();
    }

    private void SwitchTest(int score)
    {
        string grade = score switch
        {
            10 => "A",
            _ => "F"
        };

        switch (score)
        {
            case 10: Debug.Log($"{score}"); break;
        }

        var character = new Character { Name = "", Class = "" };

        var message = character switch
        {
            { Class: "dd" } => $"{character.Name}",
            _ => throw new System.Exception("dd")
        };

    }

    private int a = 1;
    private int b = 2;
    private int cnt;

    private bool OutMethod(int a, int b, out int c)
    {
        int sum = a + b;

        c = sum;
        return true;
    }

    private List<float> floatList = new List<float> { 1, 2, 3, 4 };

    /// <summary>
    /// Tuple Test => return 값을 여러 개
    /// </summary>
    /// <param name="fList"></param>
    /// <returns></returns>
    private (int cnt, string name) TupleMethod(List<float> fList)
    {
        return (fList.Count, fList.ToString());
    }
}
