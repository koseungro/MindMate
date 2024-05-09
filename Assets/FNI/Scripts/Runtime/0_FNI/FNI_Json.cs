/// 작성자: 백인성
/// 작성일: 2021-08-05
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.Common.Utils;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FNI.IS;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI.IO
{
    public class FNI_Json
    {
        public class Line
        {
            public int GetInt
            {
                get
                {
                    if (int.TryParse(value, out int _value))
                        return _value;
                    else
                        throw new NotSupportedException("현재 값은 int가 아닙니다.");
                }
            }
            public float GetFloat
            {
                get
                {
                    if (float.TryParse(value, out float _value))
                        return _value;
                    else
                        throw new NotSupportedException("현재 값은 float이 아닙니다.");
                }
            }
            public string GetString
            {
                get
                {
                    return value;
                }
            }

            public string name;
            public string value;
        }

        public string Loaded => loaded;
        private string path;
        private string loaded;

        public FNI_Json(string path)
        {
            this.path = path;
        }

        public T Load<T>()
        {
            StreamReader reader = new StreamReader(path);
            loaded = reader.ReadToEnd();

            reader.Close();
            reader.Dispose();

            return (T)JsonUtility.FromJson(loaded, typeof(T));
        }

        public void Save<T>(T data, bool isBeauty = false)
        {
            StreamWriter writer = new StreamWriter(path);

            string dataToString = JsonUtility.ToJson(data);

            if (isBeauty)
                dataToString = dataToString.ToBeauty();

            writer.Write(dataToString);

            writer.Close();
            writer.Dispose();
        }
    }
}