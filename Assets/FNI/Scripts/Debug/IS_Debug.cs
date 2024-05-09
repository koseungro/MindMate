/// 작성자: 백인성 
/// 작성일: 2018-05-01 
/// 수정일: 2018-07-25
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력 
/// (2018-07-25) 백인성 
///    1. 함수 구조 수정 및 네이밍 규칙 적용

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;

using FNI;
using FNI.DebugWriter;

namespace FNI
{
    /// <summary>
    /// 디버그를 로그에 기록하는 역할을 합니다.
    /// </summary>
    public class IS_Debug : MonoBehaviour
    {
        #region Singleton
        private static IS_Debug _instance;
        public static IS_Debug Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<IS_Debug>();
                    if (_instance == null)
                        Debug.LogError("IS_Debug를 찾을 수 없습니다. ");
                }
                return _instance;
            }
        }
        #endregion

        #region Const
        private const string kSystemLog = "SystemLog";
        private const string kBPLowLog = "BPLowLog";
        private const string kErrorLog = "ErrorLog";
        #endregion

        #region Private
        /// <summary>
        /// 유니티의 디버그를 받아올 때 사용할 변수 입니다.
        /// </summary>
        private bool DebugAdd = false;
        /// <summary>
        /// 기본 경로입니다.
        /// </summary>
        private string baseLogPath;
        /// <summary>
        /// 시스템로그경로입니다.
        /// </summary>
        private string systemLogPath;
        /// <summary>
        /// bp데이터 로그 경로 입니다.
        /// </summary>
        private string bpLowLogPath;
        /// <summary>
        /// 에러 로그 경로입니다.
        /// </summary>
        private string errorLogPath;
        /// <summary>
        /// 시스템로그를 기록합니다.
        /// </summary>
        private static CustomLogger systemLog;
        /// <summary>
        /// bp데이터를 기록합니다.
        /// </summary>
        private static CustomLogger bpLowLog;
        /// <summary>
        /// 에러로그를 기록합니다.
        /// </summary>
        private static CustomLogger errorLog;
        #endregion
        #region Unity Base Func
        protected virtual void Awake()
        {
            AddEvent();
        }

        protected void OnApplicationQuit()
        {
            RemoveEvent();

            string quitMent = $"\r\n\r\n[{CustomLogger.TimeNow}] [Application Quit]\r\n\r\n";

            if (systemLog != null)
                systemLog.WriteLog(quitMent, false);
            if (bpLowLog != null)
                bpLowLog.WriteLog(quitMent, false);
            if (errorLog != null)
                errorLog.WriteLog(quitMent, false);
        }
        #endregion


        #region Public Func
        /// <summary>
        /// 로그를 남깁니다.
        /// LogType.Log:일반적인 기록
        /// </summary>
        /// <param name="message">로그의 내용</param>
        public static void Log(string message)
        {
            if (systemLog == null)
            {
                systemLog = new CustomLogger();
                systemLog.StartUp(kSystemLog, 1024);
            }
            systemLog.WriteLog(message);
        }
        /// <summary>
        /// 로그를 남깁니다.
        /// LogType.Assert:BP데이터 및 Low데이터 전용
        /// </summary>
        /// <param name="message">로그의 내용</param>
        public static void LogData(string message)
        {
            if (bpLowLog == null)
            {
                bpLowLog = new CustomLogger();
                bpLowLog.StartUp(kBPLowLog, 1024);
            }
            bpLowLog.WriteLog(message);
        }
        /// <summary>
        /// 로그를 남깁니다.
        /// LogType.Error:에러메시지
        /// </summary>
        /// <param name="message">로그의 내용</param>
        public static void LogError(string message)
        {
            if (errorLog == null)
            {
                errorLog = new CustomLogger();
                errorLog.StartUp(kErrorLog, 1024);
            }
            errorLog.WriteLog(message);
        }

        /// <summary>
        /// 유니티에 디버그이벤트를 등록합니다.
        /// </summary>
        private void AddEvent()
        {
            if (DebugAdd == false)
            {
                Application.logMessageReceived += HandleLog;
                DebugAdd = true;
            }
        }
        /// <summary>
        /// 유니티에서 디버그 이벤트를 제외시킵니다. IS_SceneLoader에서 호출 합니다.
        /// </summary>
        public void RemoveEvent()
        {
            if (DebugAdd == true)
            {
                Application.logMessageReceived -= HandleLog;
                DebugAdd = false;
            }
        }
        #endregion

        #region Private Func

        /// <summary>
        /// 유니티의 로그 이벤트 입니다.
        /// </summary>
        /// <param name="logString">로그내용</param>
        /// <param name="stackTrace">경로?</param>
        /// <param name="type">로그의 종류</param>
        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Log)
                Log(logString);
            else if (type == LogType.Assert)
                LogData(logString);
            else
                LogError($"[{type}]\n\n{logString}\n{stackTrace}\n\n");

            ExpendHandleLog(logString, stackTrace, type);
        }

        protected virtual void ExpendHandleLog(string logString, string stackTrace, LogType type)
        {

        }
        #endregion
    }

}
