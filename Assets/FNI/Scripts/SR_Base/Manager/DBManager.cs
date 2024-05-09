/// 작성자: 고승로
/// 작성일: 2021-03-12
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace FNI
{
    public class DBManager : MonoBehaviour
    {
        #region Singleton
        private static DBManager _instance;
        public static DBManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<DBManager>();
                }
                return _instance;
            }
        }
        #endregion

        public string DataFolderName { get => dataFolderName; }
        private string dataFolderName = "";

        private string scoreDataPath = "";

        private List<Score> scoreList = new List<Score>();
        public List<Score> ScoreList { get => scoreList; }

        private List<Duration> durationList = new List<Duration>();

        [SerializeField] private Text debugText;


        /// <summary>
        /// 사전검사 CSV파일을 생성합니다.
        /// </summary>
        /// <param name="path">.csv 문자를 포함한 경로</param>
        /// <param name="year"> 콘텐츠 년도
        public void WriteCSV(string path, bool isFirst, int year)
        {
            try
            {
                StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8);

                if (isFirst)
                    writer.WriteLine("이름 (코드), 설명, 검사 완료 시간, 선택한 점수, 해당 점수 내용");

                writer.Write(GetScore(year));

                //if (!isFirst)
                //{
                //    writer.Write("\n\r ");

                //}


                writer.Close();
                //writer.Dispose();
            }
            catch (Exception ex)
            {
                Debug.Log($"Data 저장 경로 Null => <color=red>[{path}]</color>사후 검사 저장 Skip\n{ex}");
            }

        }

        #region 미사용
        /// <summary>
        /// 생성된 사전검사 CSV파일에 사후검사 정보를 덮어씁니다.
        /// </summary>
        /// <param name="path">.csv 문자를 포함한 경로</param>
        public void WritePostCSV(string path)
        {
            try
            {
                StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8);

                writer.Write(GetScore(1));

                writer.Close();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                Debug.Log($"Data 저장 경로 Null : {path} => 사후 검사 저장 Skip/ {ex}");
            }
        }

        /// <summary>
        /// CSV파일에 저장될 데이터 - 사후검사용 +음성 녹음 데이터 취합(Post Tension)
        /// </summary>
        /// <returns></returns>
        private string CollectData()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(GetScore(1));
            builder.Append("\n\r");

            return builder.ToString();
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"> 해당 년도 차수</param>
        /// <returns></returns>
        private string GetScore(int year)
        {
            StringBuilder builder = new StringBuilder();

            string _scoreChange = "";
            float changingValue;
            float _changingValue;

            string preMinusPost = "";

            for (int i = 0; i < scoreList.Count; i++)
            {
                Score targetScoreData = scoreList[i];
                //Debug.Log($"<color=yellow>ID: {targetScoreData.ID}/ isWritten {targetScoreData.isWritten}</color>");

                switch (year)
                {
                    case 1: // 1차년도 Score 데이터
                        List<Score> pre = scoreList.FindAll(x => x.ID.Contains($"Pre")); // [0] : Pre Feeling, [1] : Pre Tension

                        if (targetScoreData.isWritten == false)
                        {
                            if (targetScoreData.ID.Contains("Pre")) // 사전검사
                            {
                                builder.Append(targetScoreData.ToCSV());
                                builder.Append("\n\r");

                                //if (ScoreList.Count !=1 && targetScoreData.ID == pre[1].ID)
                                //    builder.Append("\n\r");
                            }
                            else // 사후검사 (WritePostCSV => CollectData 함수에서 들어옴!)
                            {
                                if (targetScoreData.ID.Contains("Feeling")) // Feeling
                                {
                                    builder.Append(targetScoreData.ToCSV());
                                    builder.Append("\n\r");

                                    changingValue = pre[0].score - targetScoreData.score;
                                    _changingValue = Math.Abs(changingValue); // 절댓값

                                    if (pre[0].score > targetScoreData.score)
                                        _scoreChange = "감소";
                                    else if (targetScoreData.score > pre[0].score)
                                        _scoreChange = "증가";
                                    else
                                        _scoreChange = "";

                                    preMinusPost = $"<Feeling Score>, 사전-사후 평가 점수 변화값, {_changingValue} {_scoreChange}";

                                    builder.Append(preMinusPost);
                                    builder.Append("\n\r");
                                }
                                else // Tension 
                                {
                                    builder.Append(targetScoreData.ToCSV());
                                    builder.Append("\n\r");

                                    changingValue = pre[1].score - targetScoreData.score;
                                    _changingValue = Math.Abs(changingValue); // 절댓값

                                    if (pre[1].score > targetScoreData.score)
                                        _scoreChange = "감소";
                                    else if (targetScoreData.score > pre[1].score)
                                        _scoreChange = "증가";
                                    else
                                        _scoreChange = "";

                                    preMinusPost = $"<Tension Score>, 사전-사후 평가 점수 변화값 =>, {_changingValue} {_scoreChange}";

                                    builder.Append(preMinusPost);
                                    builder.Append("\n\r");

                                    //음성 녹음 파일 관련 데이터
                                    for (int j = 0; j < durationList.Count; j++)
                                    {
                                        Duration targetDuraData = durationList[j];

                                        if (targetDuraData.isWritten == false)
                                        {
                                            //Debug.Log($"<color=cyan> {targetDuraData.ID} </color>");
                                            builder.Append(targetDuraData.ToCSV());
                                            builder.Append("\n\r");

                                            targetDuraData.isWritten = true;
                                            durationList[j] = targetDuraData;
                                        }
                                    }
                                }
                            }
                            targetScoreData.isWritten = true;
                            ScoreList[i] = targetScoreData;
                            //Debug.Log($"<color=magenta>ID: {targetScoreData.ID}/ isWritten {targetScoreData.isWritten}</color>");
                        }
                        break;
                    case 2: // 2차년도 Score 데이터
                        if (targetScoreData.isWritten == false)
                        {
                            builder.Append(targetScoreData.ToCSV());
                            builder.Append("\n\r");

                            //음성 녹음 파일 관련 데이터
                            //for (int j = 0; j < durationList.Count; j++)
                            //{
                            //    Duration targetDuraData = durationList[j];

                            //    if (targetDuraData.isWritten == false)
                            //    {
                            //        //Debug.Log($"<color=cyan> {targetDuraData.ID} </color>");
                            //        builder.Append(targetDuraData.ToCSV());
                            //        builder.Append("\n\r");

                            //        targetDuraData.isWritten = true;
                            //        durationList[j] = targetDuraData;
                            //    }
                            //}

                            targetScoreData.isWritten = true;
                            ScoreList[i] = targetScoreData;
                            //Debug.Log($"<color=magenta>ID: {targetScoreData.ID}/ isWritten {targetScoreData.isWritten}</color>");
                        }
                        break;
                }
            }

            return builder.ToString();
        }



        // 스코어 리스트에 사용자 점수를 추가
        public void AddScore(Score add)
        {
            scoreList.Add(add);

            //foreach (var item in scoreList)
            //{
            //    item.Log();
            //}
        }

        public void AddDuration(Duration add)
        {
            durationList.Add(add);
        }

        // 사용자 데이터를 초기화 - 메인메뉴에서 Quit 했을때만 호출
        public void ResetData()
        {
            scoreList.Clear();
            durationList.Clear();

            dataFolderName = "";
        }

        // 설문 점수가 저장될 경로와 이름을 지정해서 csv 파일을 생성합니다.
        public void ScoreCheckSceneID()
        {
            string testTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string _scoreData;
            string curSceneID = Main.curSceneID;

            switch (curSceneID)
            {
                case var a when curSceneID.Contains("Pre Feeling"): // 1차년도
                    CreateDataFolder();
                    _scoreData = dataFolderName + "/Score/Data_" + testTime + ".csv";
                    WriteCSV(_scoreData, true, 1);
                    scoreDataPath = _scoreData;
                    break;
                case var a when curSceneID.Contains("Pre Tension"):
                    WriteCSV(scoreDataPath, false, 1);
                    break;
                case var a when curSceneID.Contains("Post Feeling"):
                    WriteCSV(scoreDataPath, false, 1);
                    break;
                case var a when curSceneID.Contains("Post Tension"):
                    WriteCSV(scoreDataPath, false, 1);
                    //scoreDataPath = "";
                    break;
                case var a when curSceneID.Contains("Assessment_00"): // 2차년도
                    CreateDataFolder();
                    _scoreData = dataFolderName + "/Score/Data_" + testTime + ".csv";
                    WriteCSV(_scoreData, true, 2);
                    scoreDataPath = _scoreData;
                    break;
                case var a when curSceneID.Contains("Assessment_01"):
                    WriteCSV(scoreDataPath, false, 2);
                    break;
                case var a when curSceneID.Contains("Assessment_02"):
                    WriteCSV(scoreDataPath, false, 2);
                    break;
                case var a when curSceneID.Contains("Assessment_03"):
                    WriteCSV(scoreDataPath, false, 2);
                    break;
                case var a when curSceneID.Contains("Assessment_04"):
                    WriteCSV(scoreDataPath, false, 2);
                    break;

            }
        }

        // 음성 녹음될 파일 이름 지정
        public string CheckSceneID()
        {
            string testTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string recordName = "Recording_" + testTime;

            return recordName;
        }

        // 사용자 Data가 저장될 폴더를 생성합니다.
        private void CreateDataFolder()
        {
            if (dataFolderName == "")
            {
#if UNITY_EDITOR
                // 에디터 데이터 저장 경로
                string path = Application.dataPath + "/../Data/" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
#else
                // 빌드 데이터 저장 경로
                string path = Application.persistentDataPath + "/Data/" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
#endif
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                dataFolderName = path;
                Debug.Log($"Data 폴더 생성 : <color=yellow>{dataFolderName}</color>");

                // 설문 점수를 저장될 폴더를 생성합니다.
                if (!Directory.Exists(dataFolderName + "/Score"))
                {
                    Directory.CreateDirectory(dataFolderName + "/Score");
                }
            }
        }
    }

    [System.Serializable]

    public struct Score
    {
        public DateTime inputTime;
        public string ID;
        public float score;
        public string text;

        public bool isWritten;

        public string ToCSV()
        {
            return $"{ID} - Score, {ID} 설문 점수, {inputTime.ToString("HH:mm:ss")}, {score}, {text}";
        }

        public void Log()
        {
            Debug.Log($"{inputTime} , {ID}, {score}");
        }
    }

    [System.Serializable]
    public struct Duration
    {
        public DateTime inputTime;
        public string ID;
        public string length;

        public bool isWritten;

        public string ToCSV()
        {
            return $"<{ID} Recording>, 총 말한 시간 => {length}, , , {inputTime.ToString("HH:mm:ss")}";
        }
    }
}