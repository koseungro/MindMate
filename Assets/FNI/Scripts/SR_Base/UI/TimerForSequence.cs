using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FNI
{
    public class TimerForSequence : MonoBehaviour, IVisualObject
    {
        #region Singleton
        private static TimerForSequence _instance;
        public static TimerForSequence Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TimerForSequence>();
                }
                return _instance;
            }
        }
        #endregion

        public VisualType Type => VisualType.Timer;
        public GameObject UIObject;

        // 스톱워치가 작동중인지 체크
        private bool isFinish = false;
        public bool IsFinish { get => isFinish; }

        private GameObject targetText;
        private Slider timerSlider;

        private Text text;

        private DateTime startTime;

        private String recordingTime;
        public String RecordingTime { get => recordingTime; }

        private float fillAmount = 0;

        private bool resetDone = true;

        public CanvasGroup fadeCanvas;
        private AudioSource timerAudio;

        // 녹음용
        [SerializeField]
        private Transform recordTimer;
        private Text recordText;

        private Slider recordSlider;
        private GameObject startButton;
        private GameObject endButton;
        private bool checkRecordStart = false;

        // 버튼 <-> 슬라이더 Switch용
        [SerializeField]
        private Transform recordSwitchTimer;
        private Slider recordSwitchSlider;
        private Text recordSwitchText;
        private Button recordButton;


        public void Active(CutData option)
        {
            isFinish = false;

            if (option.timerOption.textPath != "")
            {
                fillAmount = 0;
                targetText = UIObject.transform.Find(option.timerOption.textPath).gameObject;
                timerSlider = targetText.transform.parent.GetComponentInChildren<Slider>();

                if (targetText != null)
                    text = targetText.GetComponent<Text>();
                else
                    Debug.Log($"<color=yellow> TargetText를 찾지 못했습니다.</color>");
            }

            switch (option.timerOption.timerType)
            {
                case TimerType.Timer_Button:
                    StartCoroutine(TimerButtonStart(option.timerOption)); // 특정 시간이 지나면 다음으로 버튼 나오게 (녹음기능 X)
                    break;
                case TimerType.Timer_Only:
                    StartCoroutine(TimerStart(option.timerOption));
                    break;
                case TimerType.RecordTimer_Waiting:
                    break;
            }

        }

        public void Init()
        {
            resetDone = true;

            if (recordTimer != null)
            {
                recordText = recordTimer.GetComponentInChildren<Text>();
                recordSlider = recordTimer.GetComponentInChildren<Slider>();
                startButton = recordTimer.parent.Find("Start Button").gameObject;
                endButton = recordTimer.parent.Find("End Button").gameObject;
                timerAudio = GetComponent<AudioSource>();
            }

            if (recordSwitchTimer != null)
            {
                recordSwitchSlider = recordSwitchTimer.GetComponentInChildren<Slider>();
                recordSwitchText = recordSwitchTimer.GetComponentInChildren<Text>();
                recordButton = recordSwitchTimer.parent.Find("Record Button").GetComponent<Button>();
            }
        }

        public void MyUpdate()
        {

        }

        public void RecordTimer(float endTime)
        {
            isFinish = false;
            fillAmount = 0;

            StartCoroutine(RecordTimerStart_Increase(endTime));
        }

        /// <summary>
        /// Record 버튼을 누르면 카운트 이미지로 Switch 되는 타이머
        /// </summary>
        /// <param name="recordTime"></param>
        public void RecordSwitchTimer(float recordTime)
        {
            isFinish = false;
            fillAmount = 1;

            double rTime = (double)recordTime;

            recordSwitchTimer.gameObject.SetActive(true);
            StartCoroutine(RecordTimerStart_Decrease(rTime));
        }


        /// <summary>
        /// 녹음 타이머용 Slider(버퍼링 문제 적용/ 시간 증가)
        /// </summary>
        /// <param name="textInt">녹음 기능 버퍼링용 시간 표시 Text(int)</param>
        /// <param name="endTime"></param>
        private void TimerSlider(int textInt, float endTime)
        {
            if (FNI_Record.Instance.M_State == RecordState.Recoding)
            {
                if (fillAmount <= 1)
                {
                    if (!checkRecordStart) // Record Timer While 첫 시기 1번만 들어옴 => 녹음 기능 시작 버퍼링 때문에 Text : 실제 시간 맞지 않는 이유 때문
                    {
                        fillAmount = textInt / endTime;
                        recordSlider.value = fillAmount;
                        checkRecordStart = true;
                        //Debug.Log($"1/ {fillAmount}");
                        return;
                    }

                    fillAmount = fillAmount + (Time.deltaTime / endTime);
                    recordSlider.value = fillAmount;
                    //Debug.Log($"2/ {fillAmount}");

                }
            }
        }

        /// <summary>
        /// 녹음 타이머용 Slider(시간 감소)
        /// </summary>
        /// <param name="endTime"></param>
        private void TimerSlider(int textInt, double recordTime)
        {
            if (FNI_Record.Instance.M_State == RecordState.Recoding)
            {
                if (fillAmount >= 0)
                {
                    if (!checkRecordStart) // Record Timer While 첫 시기 1번만 들어옴 => 녹음 기능 시작 버퍼링 때문에 Text : 실제 시간 맞지 않는 이유 때문
                    {
                        fillAmount = textInt / (float)recordTime;
                        recordSwitchSlider.value = fillAmount;
                        checkRecordStart = true;
                        //Debug.Log($"1/ {fillAmount}");
                        return;
                    }

                    fillAmount = fillAmount - (Time.deltaTime / (float)recordTime);
                    recordSwitchSlider.value = fillAmount;

                    //Debug.Log($"<color=yellow> {fillAmount}/ {recordSwitchSlider.value}</color>");

                }
            }
        }

        /// <summary>
        /// 녹음 타이머(시간 증가)
        /// </summary>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IEnumerator RecordTimerStart_Increase(float endTime)
        {
            TimeSpan tTime = TimeSpan.Zero;

            startButton.SetActive(false);

            if (resetDone)
            {
                startTime = DateTime.Now;
                resetDone = false;
            }

            int preTime = 0;
            int curTime = 0;
            int recordFillAmount;

            while (tTime.TotalSeconds <= endTime)
            {
                tTime = DateTime.Now - startTime;
                recordText.text = tTime.ToString(@"%s");
                recordFillAmount = int.Parse(recordText.text);

                curTime = int.Parse(recordText.text);
                if (curTime != preTime)
                {
                    timerAudio.Play();
                    preTime = curTime;
                }

                TimerSlider(recordFillAmount, endTime);

                recordingTime = string.Format("{0:00}:{1:00}", tTime.Seconds, tTime.Milliseconds);

                if (tTime.TotalSeconds >= 5)
                {
                    endButton.SetActive(true);
                }

                if (isFinish)
                {
                    yield break;
                }

                yield return null;
            }

            recordSlider.value = 1;

            yield return new WaitForSeconds(1.0f);
            VoiceRecoder.Instance.StopRecording();
        }

        /// <summary>
        /// 녹음 타이머(시간 감소)
        /// </summary>
        /// <param name="recordTime"></param>
        /// <returns></returns>
        public IEnumerator RecordTimerStart_Decrease(double recordTime)
        {
            TimeSpan tTime = TimeSpan.Zero;
            TimeSpan rTime = TimeSpan.FromSeconds(recordTime);

            if (resetDone)
            {
                startTime = DateTime.Now;
                resetDone = false;
            }

            int preTime = 0;
            int curTime = 0;
            int recordFillAmount;

            while (tTime.TotalSeconds >= 0)
            {
                tTime = rTime - (DateTime.Now - startTime);
                recordSwitchText.text = tTime.ToString(@"%s");
                recordFillAmount = int.Parse(recordSwitchText.text);

                curTime = int.Parse(recordSwitchText.text);
                if (curTime != preTime)
                {
                    timerAudio.Play();
                    preTime = curTime;
                }

                TimerSlider(recordFillAmount, recordTime);

                recordingTime = string.Format("{0:00}:{1:00}", tTime.Seconds, tTime.Milliseconds);

                if (isFinish)
                {
                    yield break;
                }

                yield return null;
            }

            recordSlider.value = 0;

            yield return new WaitForSeconds(1.0f);
            VoiceRecoder.Instance.StopRecording();

            StartCoroutine(TimerResetRoutine(true));
            // 시퀀스 이동
        }

        /// <summary>
        /// 버튼 활성화용 타이머 Slider
        /// </summary>
        /// <param name="timerOption"></param>
        private void TimerSlider(TimerOption timerOption)
        {
            if (fillAmount <= 1)
            {
                fillAmount = fillAmount + (Time.deltaTime / (timerOption.endTime));
                timerSlider.value = fillAmount;
            }
        }

        /// <summary>
        /// x초 버튼 활성화용 타이머
        /// </summary>
        /// <param name="timerOption"></param>
        /// <returns></returns>
        public IEnumerator TimerButtonStart(TimerOption timerOption)
        {
            TimeSpan tTime = TimeSpan.Zero;

            if (resetDone)
            {
                startTime = DateTime.Now;
                resetDone = false;
            }

            int preTime = 0;
            int curTime = 0;
            while (tTime.TotalSeconds <= timerOption.endTime)
            {
                tTime = DateTime.Now - startTime;
                TimerSlider(timerOption);
                text.text = tTime.ToString(@"%s");

                curTime = int.Parse(text.text);
                if (curTime != preTime)
                {
                    timerAudio.Play();
                    preTime = curTime;
                }

                if (tTime.TotalSeconds >= timerOption.limitTime)
                {
                    ActiveButtion(timerOption);
                }

                if (isFinish)
                {
                    yield break;
                }

                yield return null;
            }

            timerSlider.value = 1;

        }



        public IEnumerator TimerStart(TimerOption timerOption)
        {
            TimeSpan tTime = TimeSpan.Zero;

            if (resetDone)
            {
                startTime = DateTime.Now;
                resetDone = false;
            }

            int preTime = 0;
            int curTime = 0;
            while (tTime.TotalSeconds <= timerOption.endTime)
            {
                tTime = DateTime.Now - startTime;
                TimerSlider(timerOption);
                text.text = tTime.ToString(@"%s");

                curTime = int.Parse(text.text);
                if (curTime != preTime)
                {
                    timerAudio.Play();
                    preTime = curTime;
                }

                yield return null;
            }

            timerSlider.value = 1;

        }


        public void IsButtonClicked()
        {
            //TimerReset();
            StartCoroutine(TimerResetRoutine());
        }

        private void ActiveButtion(TimerOption timerOption)
        {
            if (timerOption.limitTime != 0)
            {
                GameObject targetButton = UIObject.transform.Find(timerOption.buttonPath).gameObject;
                targetButton.SetActive(true);
            }
        }

        // 스톱워치 리셋
        public void TimerReset()
        {
            isFinish = true;

            resetDone = true;
            Debug.Log("스톱워치 초기화");
        }

        /// <summary>
        /// 타이머 세팅 초기화
        /// </summary>
        /// <param name="isSwitching"> 버튼 <-> 슬라이더 Switching 타이머 여부 </param>
        /// <returns></returns>
        private IEnumerator TimerResetRoutine(bool isSwitching = false)
        {
            isFinish = true;
            resetDone = true;
            checkRecordStart = false;

            while (fadeCanvas.alpha <= 1)
            {
                if (fadeCanvas.alpha == 1) // Fade 완료될때까지 기다렸다가 초기화
                {
                    if (targetText != null && text.text != "0")
                    {
                        text.text = "0";
                        timerSlider.value = 0;
                    }

                    if (recordText.text != "0")
                    {
                        recordText.text = "0";
                        recordSlider.value = 0;
                    }

                    if (isSwitching)
                    {
                        if (recordSwitchText.text != "30")
                        {
                            recordSwitchText.text = "30";
                            recordSwitchSlider.value = 1;

                            recordSwitchTimer.gameObject.SetActive(false);
                            recordButton.gameObject.SetActive(true);
                        }
                    }

                }
                yield return null;

            }
        }


    }
}
