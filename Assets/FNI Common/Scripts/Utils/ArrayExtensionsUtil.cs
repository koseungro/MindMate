/// 작성자: 조효련
/// 작성일: 2022-01-27
/// 수정일: 
/// 저작권: Copyright(C) FNIKorea Co., Ltd.. (주)에프앤아이코리아

using FNI.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace FNI
{
    public static class ArrayExtensions
    {
        private static System.Random random = new System.Random();
        // UnityEngine.Random.Range

        public static T[] ShuffleArray<T>(this T[] array)
        {
            int max = array.Length;
            for (int i = 0; i < max - 1; i++)
            {
                //  int randomIndex = i + random.Next(max - i);
                int randomIndex = random.Next(i, max);

                T tempItem = array[randomIndex];
                array[randomIndex] = array[i];
                array[i] = tempItem;
            }

            return array;
        }

        //  약간의 목록 개수를 랜덤으로 순서를 변경할 때 좋음
        public static List<T> ShuffleList<T>(this List<T> list)
        {
            return list.OrderBy(a => Guid.NewGuid()).ToList();
        }

        public static void Shuffle<T>(List<T> list)
        {
            int random1;
            int random2;

            T tmp;

            int max = list.Count;

            for (int index = 0; index < list.Count; ++index)
            {
                random1 = random.Next(0, max); 
                random2 = random.Next(0, max);

                tmp = list[random1];

                list[random1] = list[random2];
                list[random2] = tmp;
            }
        }



        public static T[] GetRow<T>(this T[,] array, int row)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("Not supported for managed types.");

            if (array == null)
                throw new ArgumentNullException("array");

            int cols = array.GetUpperBound(1) + 1;
            T[] result = new T[cols];

            int size;

            if (typeof(T) == typeof(bool))
                size = 1;
            else if (typeof(T) == typeof(char))
                size = 2;
            else
                size = Marshal.SizeOf<T>();

            Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

            return result;
        }
    }

    public class CustomArray<T>
    {
        public T[] GetColumn(T[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        public T[] GetRow(T[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }
    }

}