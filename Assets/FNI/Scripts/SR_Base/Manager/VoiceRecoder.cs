/// 작성자:고승로
/// 작성일: 2021-03-16
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 

using FNI.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace FNI
{
    public class VoiceRecoder : MonoBehaviour
    {
        private static VoiceRecoder _instance = null;

        public static VoiceRecoder Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<VoiceRecoder>();
                }
                return _instance;
            }
        }

        private Main myMain;
        private Main MyMain
        {
            get
            {
                if (myMain == null)
                    myMain = GameObject.Find("---------------Scripts/Main").GetComponent<Main>();

                return myMain;
            }
        }

        private FNI_Record record = null;

        // 현재 녹음 중인지를 확인합니다.
        private bool isRecording = false;
        public bool IsRecording { get => isRecording; }

        // 사용자가 녹음을 완료했는지를 확인합니다.
        private bool recordCheck = false;

        private CutData cData;
        private void OnEnable()
        {
            RecordInit();
            CallRecordBoard();
        }

        private void Start()
        {
            record = GetComponent<FNI_Record>();
        }

        private void LateUpdate()
        {
            //// 녹음기가 켜져있을 때만 동작
            //if (record.enabled == true)
            //{
            //    // Space 키보드를 통하여 녹음 시작, 정지가 가능하도록 합니다.
            //    if (Input.GetKeyDown("space"))
            //    {
            //        if (!recordCheck)
            //        {
            //            if (!isRecording)
            //            {
            //                FNI_Record.Instance.Start_Record();
            //            }
            //            else if (isRecording)
            //            {
            //                StopRecording();
            //            }
            //        }
            //    }

            //    // 녹음이 끝난 후 방향키를 누르면 다음 씬(제의)으로 넘어갑니다.
            //    if (Input.GetKeyDown(KeyCode.RightArrow))
            //    {
            //        if (recordCheck)
            //        {
            //            int cnt = myMain.CurSequence.cutDataList.Count - 3;
            //            cData = myMain.CurSequence.cutDataList[cnt];
            //            myMain.ChangeScene(cData.uiOption.nextScene);
            //        }
            //    }
            //}

        }

        private void RecordInit()
        {
            isRecording = false;
            recordCheck = false;
        }

        // 녹음시 사용되는 코루틴
        private IEnumerator m_recordWaiting_Routine;

        // 녹음 기능 켜기
        public void CallRecordBoard()
        {
            if (m_recordWaiting_Routine != null)
                StopCoroutine(m_recordWaiting_Routine);
            m_recordWaiting_Routine = RecordWaiting_Routine();
            StartCoroutine(m_recordWaiting_Routine);
        }

        // 녹음 기능
        public IEnumerator RecordWaiting_Routine()
        {
            yield return new WaitForSeconds(0.2f);

            if (!isRecording)
            {
                //녹음 시작하기를 기다립니다.
                while (!record.IsRecording)
                {
                    //Debug.Log("녹음을 시작해 주세요" + isRecording);
                    yield return null;
                }
            }

            //녹음이 시작되면 녹음 끝나기를 기다립니다.
            while (record.IsRecording)
            {
                if (!isRecording)
                    isRecording = true;
                yield return null;
                //Debug.Log("녹음 중 입니다." + isRecording);
            }
            string recordingPath = DBManager.Instance.DataFolderName + "/Recording";

            if (DBManager.Instance.DataFolderName != "")
            {
                //녹음이 끝난 후 오디오 클립을 저장하고 저장한 경로를 받아옵니다.
                record.SaveClip(recordingPath);
                Debug.Log($"녹음 파일 저장 완료 => [{recordingPath}]");
            }
            else
                Debug.Log($"<color=red>녹음 파일 폴더 저장 경로가 존재하지 않습니다.</color>");

            //Debug.Log(FNI_Record.Instance.RecordingTime.text);
        }

        private IEnumerator m_replay_Routine;
        public void CallReplayBoard()
        {
            if (m_replay_Routine != null)
                StopCoroutine(m_replay_Routine);
            m_replay_Routine = Replay_Routine();
            StartCoroutine(m_replay_Routine);
        }

        // 다시듣기
        private IEnumerator Replay_Routine()
        {
            //다시 듣기 버튼을 활성화 한다.
            FNI_Record.Instance.RecordReady(RecordType.Replay);

            //재생이 시작하기를 기다립니다.
            while (!FNI_Record.Instance.IsPlaying)
            {
                yield return null;
                //Debug.Log("다시듣기 재생 대기 중");
            }
            //재생 시작되면 재생 끝나기를 기다립니다.
            while (FNI_Record.Instance.IsPlaying)
            {
                yield return null;
                //Debug.Log("다시듣기 재생 중");
            }

        }
        private void StartRecording()
        {
            Debug.Log("녹음 시작");
            FNI_Record.Instance.Start_Record();
        }

        public void StopRecording()
        {
            if (isRecording)
            {
                Duration duration = new Duration();

                record.Stop_Record(DBManager.Instance.CheckSceneID());

                duration.inputTime = DateTime.Now;
                duration.ID = Main.curSceneID;
                duration.length = TimerForSequence.Instance.RecordingTime;
                duration.isWritten = false;

                DBManager.Instance.AddDuration(duration);

                //CallReplayBoard();
                recordCheck = true;
                isRecording = false;
                Debug.Log("녹음 완료");
            }
        }
    }
}