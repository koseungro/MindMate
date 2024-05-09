/// 작성자: 조효련
/// 작성일: 2020-11-03
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace FNI.Common.Utils
{
    public static class CommonUtil
    {
        #region Component 관리

        public static T FindComponent<T>(this Transform transform, string path) where T : Component
        {
            T target = null;
            Transform find = transform.Find(path);
            if (find)
                target = find.GetComponent<T>();
            else
                Debug.LogError($"오브젝트를 찾을 수 없습니다. 오브젝트 경로 :: {path}");

            return target;
        }

        public static T GetOrAddCompoment<T>(this GameObject target) where T : Component
        {
            if (target == null)
                return null;

            T comp = target.GetComponent<T>();
            if (comp == null)
                comp = target.AddComponent<T>();

            return comp;
        }

        public static T CopyComponet<T>(T origin, GameObject destination, bool duplication = false) where T : Component
        {
            Type type = origin.GetType();
            Component copy;
            if (duplication || !destination.GetComponent(type))
                copy = destination.AddComponent(type);
            else
                copy = destination.GetComponent(type);
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(origin));
            }

            return copy as T;
        }

        #endregion

        #region 버튼 이벤트 등록

        public static void AddPointerClick(this GameObject target, UnityAction<BaseEventData> callback, bool isClear = false)
        {
            target.AddButtonEvent(EventTriggerType.PointerClick, callback, isClear);
        }

        public static void AddPointerDown(this GameObject target, UnityAction<BaseEventData> callback, bool isClear = false)
        {
            target.AddButtonEvent(EventTriggerType.PointerDown, callback, isClear);
        }

        public static void AddPointerUp(this GameObject target, UnityAction<BaseEventData> callback, bool isClear = false)
        {
            target.AddButtonEvent(EventTriggerType.PointerUp, callback, isClear);
        }

        public static void AddButtonEvent(this GameObject target, EventTriggerType TYPE, UnityAction<BaseEventData> callback, bool isClear = false)
        {
            EventTrigger eventTrigger = target.GetOrAddCompoment<EventTrigger>();

            if (eventTrigger)
            {
                if (isClear)
                    eventTrigger.triggers.Clear();

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = TYPE;
                entry.callback.AddListener(callback);

                eventTrigger.triggers.Add(entry);
            }
        }

        public static void AddDragingEvent(this GameObject target, UnityAction<BaseEventData> Draging)
        {
            EventTrigger eventTrigger = target.GetOrAddCompoment<EventTrigger>();
            if (eventTrigger)
            {
                EventTrigger.Entry entry_Draging = new EventTrigger.Entry();

                entry_Draging.eventID = EventTriggerType.Drag;
                entry_Draging.callback.AddListener(Draging);

                eventTrigger.triggers.Clear();
                eventTrigger.triggers.Add(entry_Draging);
            }
        }

        public static void AddDragEvent(this GameObject target, UnityAction<BaseEventData> BeginDrag, UnityAction<BaseEventData> EndDrag)
        {
            EventTrigger eventTrigger = target.GetOrAddCompoment<EventTrigger>();
            if (eventTrigger)
            {
                EventTrigger.Entry entry_BeginDrag = new EventTrigger.Entry();
                entry_BeginDrag.eventID = EventTriggerType.BeginDrag;
                entry_BeginDrag.callback.AddListener(BeginDrag);

                EventTrigger.Entry entry_EndDrag = new EventTrigger.Entry();
                entry_EndDrag.eventID = EventTriggerType.EndDrag;
                entry_EndDrag.callback.AddListener(EndDrag);

                eventTrigger.triggers.Clear();
                eventTrigger.triggers.Add(entry_BeginDrag);
                eventTrigger.triggers.Add(entry_EndDrag);
            }
        }

        public static void RemoveButtonEvent(this GameObject target)
        {
            EventTrigger eventTrigger = target.GetComponent<EventTrigger>();
            if (eventTrigger)
                eventTrigger.triggers.Clear();
        }

        #endregion

        #region 파일 및 폴더 복사하기
        // 파일 및 폴더 복사하기
        public static void CopyDirectory(string SourcePath, string DestinationPath)
        {
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
            {
                string folderPath = dirPath.Replace(SourcePath, DestinationPath);
                Directory.CreateDirectory(folderPath);
                Debug.Log($"[폴더 생성 {folderPath}");
            }

            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
            {
                string filePath = newPath.Replace(SourcePath, DestinationPath);
                File.Copy(newPath, filePath, true);
                Debug.Log($"[파일 복사 {filePath}");
            }
        }

        // 파일 및 폴더 복사하기
        public static bool CreateDirectoryRecursively(string path)
        {
            try
            {
                List<bool> hasFolder = new List<bool>(); // 로그를 위해서 필요함

                string[] pathParts = path.Split('\\');
                for (var i = 0; i < pathParts.Length; i++)
                {
                    // 드라이브명일 경우
                    if (i == 0 && pathParts[i].Contains(":"))
                    {
                        pathParts[i] = pathParts[i] + "\\";
                    } // 파일명일 경우
                    else if (i == pathParts.Length - 1 && pathParts[i].Contains("."))
                    {
                        return true;
                    }

                    if (i > 0)
                    {
                        pathParts[i] = Path.Combine(pathParts[i - 1], pathParts[i]);
                    }

                    if (!Directory.Exists(pathParts[i]))
                    {
                        hasFolder.Add(false);
                        Directory.CreateDirectory(pathParts[i]);
                    }
                    else
                    {
                        hasFolder.Add(true);
                    }
                }

                if (hasFolder.Contains(false))
                    Debug.Log($"[<color=yellow>[FNI 폴더 체크]</color> {path} 폴더를 생성합니다.");
                else
                    Debug.Log($"[<color=yellow>[FNI 폴더 체크]</color> {path} 폴더를 존재합니다.");

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"[<color=yellow>[FNI 폴더 체크]</color> {path} 폴더 확인에 실패했습니다.\n오류메세지 : {ex.ToString()}");
                return false;
            }
        }
        #endregion

        #region 폴더 열기
        public static void OpenFolder(string path)
        {
            if (Directory.Exists(path))
            {
                var p = new System.Diagnostics.Process();
                p.StartInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", path);
                p.Start();
            }
            else
            {
                Debug.LogWarning("<color=magenta>[폴더열기]</color>폴더를 열 수 없습니다. " + path);
            }
        }
        #endregion
    }
}
