/// 작성자: 작성자 이름
/// 작성일: 2020-04-14
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FNI
{
    public class IS_Selectables : MonoBehaviour
    {
        protected List<IS_Selectable_Suppoter> selectable_SuppoterList = new List<IS_Selectable_Suppoter>();

        //protected virtual void Awake()
        //{
        //    GetComponentsInChildren(selectable_SuppoterList, transform);
        //}

        public void AddSupport(IS_Selectable_Suppoter suppoter)
        {
            selectable_SuppoterList.Add(suppoter);
        }

        public void RemoveSupport(IS_Selectable_Suppoter suppoter)
        {
            selectable_SuppoterList.Remove(suppoter);
        }

        protected virtual void LateUpdate()
        {
            if (selectable_SuppoterList == null) return;

            for (int cnt = 0; cnt < selectable_SuppoterList.Count; cnt++)
                selectable_SuppoterList[cnt].Animation();
        }

        /// <summary>
        /// T 컴퍼넌트를 가진 모든 자녀를 찾습니다. 자녀가 숨겨져 있어도 찾습니다.
        /// </summary>
        /// <typeparam name="T">찾을 타입</typeparam>
        /// <param name="list">찾은 데이터를 넣을 곳</param>
        /// <param name="parent">찾을 부모</param>
        public static void GetComponentsInChildren<T>(List<T> list, Transform parent)
        {
            T find = parent.GetComponent<T>();

            if (list == null)
                list = new List<T>();

            if (find != null)
            {
                list.Add(find);
            }

            for (int cnt = 0; cnt < parent.childCount; cnt++)
            {
                if (parent.GetChild(cnt).childCount != 0)
                {
                    GetComponentsInChildren(list, parent.GetChild(cnt));
                }
                else
                {
                    find = parent.GetChild(cnt).GetComponent<T>();

                    if (find != null)
                    {
                        list.Add(find);
                    }
                }
            }
        }
    }
}