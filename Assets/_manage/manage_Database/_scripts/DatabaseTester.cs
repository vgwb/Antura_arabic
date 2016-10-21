using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace EA4S.Db.Management
{
    public class DatabaseTester : MonoBehaviour
    {
#if UNITY_EDITOR
        public DatabaseLoader DBLoader;
#endif
        private DatabaseManager db;

        public Text OutputText;
        public TextMeshProUGUI OutputTextArabic;

        void Awake()
        {
            this.db = new DatabaseManager();
        }

        #region Main Actions

        public void ImportAll()
        {
#if UNITY_EDITOR
            DBLoader.LoadDatabase();
            DumpAllDataCounts();
#endif
        }

        #endregion

        #region Test static data

        public void DumpAllDataCounts()
        {
            string output = "";
            output += ("N letters: " + db.FindAllLetterData().Count) + "\n";
            output += ("N words: " + db.FindAllWordData().Count) + "\n";
            output += ("N phrases: " + db.FindAllPhraseData().Count) + "\n";
            output += ("N minigames: " + db.FindAllMiniGameData().Count) + "\n";
            output += ("N stages: " + db.FindAllStageData().Count) + "\n";
            output += ("N playsessions: " + db.FindAllPlaySessionData().Count) + "\n";
            output += ("N assessments: " + db.FindAllAssessmentData().Count) + "\n";
            output += ("N localizations: " + db.FindAllLocalizationData().Count) + "\n";
            output += ("N rewards: " + db.FindAllRewardData().Count) + "\n";
            PrintOutput(output);
        }

        public void DumpAllLetterData()
        {
            DumpAllData(db.FindAllLetterData());
        }

        public void DumpAllWordData()
        {
            DumpAllData(db.FindAllWordData());
        }

        public void DumpAllPhraseData()
        {
            DumpAllData(db.FindAllPhraseData());
        }

        public void DumpAllPlaySessionData()
        {
            DumpAllData(db.FindAllPlaySessionData());
        }

        public void DumpAllStageData()
        {
            DumpAllData(db.FindAllStageData());
        }

        public void DumpAllLocalizationData()
        {
            DumpAllData(db.FindAllLocalizationData());
        }

        public void DumpAllMiniGameData()
        {
            DumpAllData(db.FindAllMiniGameData());
        }

        public void DumpAllAssessmentData()
        {
            DumpAllData(db.FindAllAssessmentData());
        }

        public void DumpAllLogInfoData()
        {
            DumpAllData(db.FindAllLogInfoData());
        }

        public void DumpLetterById(string id)
        {
            IData data = db.GetLetterDataById(id);
            DumpDataById(id, data);
        }

        public void DumpWordById(string id)
        {
            var data = db.GetWordDataById(id);
            var arabic_text = data.Arabic;
            PrintArabicOutput(arabic_text);
            DumpDataById(id, data);
        }

        public void DumpPhraseById(string id)
        {
            IData data = db.GetPhraseDataById(id);
            DumpDataById(id, data);
        }

        public void DumpMiniGameByCode(MiniGameCode code)
        {
            IData data = db.GetMiniGameDataByCode(code);
            DumpDataById(data.GetId(), data);
        }

        public void DumpStageById(string id)
        {
            IData data = db.GetStageDataById(id);
            DumpDataById(id, data);
        }

        public void DumpPlaySessionById(string id)
        {
            IData data = db.GetPlaySessionDataById(id);
            DumpDataById(id, data);
        }

        public void DumpAssessmentById(string id)
        {
            IData data = db.GetAssessmentDataById(id);
            DumpDataById(id, data);
        }

        public void DumpLocalizationById(string id)
        {
            IData data = db.GetLocalizationDataById(id);
            DumpDataById(id, data);
        }

        public void DumpRewardById(string id)
        {
            IData data = db.GetRewardDataById(id);
            DumpDataById(id, data);
        }

        public void DumpLogDataById(string id)
        {
            IData data = db.GetLogInfoDataById(id);
            DumpDataById(id, data);
        }

        public void DumpArabicWord(string id)
        {
            var data = db.GetWordDataById(id);
            var arabic_text = data.Arabic;
            PrintArabicOutput(arabic_text);
        }

        public void DumpActiveMinigames()
        {
            var all_minigames = db.FindAllMiniGameData();
            var active_minigames = db.FindAllActiveMinigames();
            PrintOutput(active_minigames.Count + " active minigames out of " + all_minigames.Count);
        }

        #endregion

        #region Test Insert Log Data

        public void TestInsertLogInfoData()
        {
            var newData = new LogInfoData();
            newData.Id = UnityEngine.Random.Range(0f, 999).ToString();
            newData.Time = Time.time.ToString();
            newData.Session = UnityEngine.Random.Range(0, 10).ToString();
            newData.PlayerID = 1;
            newData.Score = UnityEngine.Random.Range(0f, 10f);

            this.db.InsertLogInfoData(newData);
            PrintOutput("Inserted new LogInfoData: " + newData.ToString());
        }

        public void TestInsertLogLearnData()
        {
            var newData = new LogLearnData();
            newData.Id = UnityEngine.Random.Range(0f, 999).ToString();
            newData.Time = Time.time.ToString();
            newData.Session = UnityEngine.Random.Range(0, 10).ToString();
            newData.PlayerID = 1;
            newData.Score = UnityEngine.Random.Range(0f, 10f);

            this.db.InsertLogLearnData(newData);
            PrintOutput("Inserted new LogLearnData: " + newData.ToString());
        }

        public void TestInsertLogMoodData()
        {
            var newData = new LogMoodData();
            newData.Id = UnityEngine.Random.Range(0f, 999).ToString();
            newData.Time = Time.time.ToString();
            newData.Session = UnityEngine.Random.Range(0, 10).ToString();
            newData.PlayerID = 1;
            newData.MoodValue = UnityEngine.Random.Range(0, 20);

            this.db.InsertLogMoodData(newData);
            PrintOutput("Inserted new LogMoodData: " + newData.ToString());
        }

        public void TestInsertLogPlayData()
        {
            var newData = new LogPlayData();
            newData.Id = UnityEngine.Random.Range(0f, 999).ToString();
            newData.Time = Time.time.ToString();
            newData.Session = UnityEngine.Random.Range(0, 10).ToString();
            newData.PlayerID = 1;

            this.db.InsertLogPlayData(newData);
            PrintOutput("Inserted new LogPlayData: " + newData.ToString());
        }

        public void TestInsertLogScoreData()
        {
            var newData = new LogScoreData();
            newData.Id = UnityEngine.Random.Range(0f, 999).ToString();
            newData.Time = Time.time.ToString();
            newData.Session = UnityEngine.Random.Range(0, 10).ToString();
            newData.PlayerID = 1;

            this.db.InsertLogScoreData(newData);
            PrintOutput("Inserted new LogScoreData: " + newData.ToString());
        }

        #endregion

        #region Test Query Log Data

        // Test that uses a simple select/where expression on a single table
        public void TestLINQLogData()
        {
            List<LogInfoData> list = this.db.FindAllLogInfoData(x => x.Score > 5f);
            DumpAllData(list);
        }

        // Test query: get all MoodData, ordered by MoodValue
        public void TestQuery_SingleTable1()
        {
            var tableName = this.db.GetTableName<LogMoodData>();
            string query = "select * from \"" + tableName + "\" order by MoodValue";
            List<LogMoodData> list = this.db.FindLogMoodDataByQuery(query);
            DumpAllData(list);
        }

        // Test query: get number of LogPlayData for a given PlaySession with a high enough score
        public void TestQuery_SingleTable2()
        {
            var tableName = this.db.GetTableName<LogPlayData>();
            string targetPlaySessionId = "\"5\"";
            string query = "select * from \"" + tableName + "\" where Session = " + targetPlaySessionId;
            List<LogPlayData> list = this.db.FindLogPlayDataByQuery(query);
            PrintOutput("Number of play data for PlaySession " + targetPlaySessionId + ": " + list.Count);
        }


        public class TestQueryResult
        {
            public int MoodValue { get; set; }
        }

        // Test query: get just the MoodValues (with a custom result class) from all LogMoodData entries
        public void TestQuery_SingleTable3()
        {
            var tableName = this.db.GetTableName<LogMoodData>();
            SQLite.TableMapping resultMapping = new SQLite.TableMapping(typeof(TestQueryResult));

            string targetPlaySessionId = "\"5\"";
            string query = "select MoodValue from \"" + tableName + "\" where Session = " + targetPlaySessionId;
            List<object> list = this.db.FindCustomDataByQuery(resultMapping, query);

            string output = "Test values N: " + list.Count;
            foreach(var obj in list)
            {
                output += ("Test value: " + (obj as TestQueryResult).MoodValue) + "\n";
            }
            PrintOutput(output);
        }


        // Test query: join LogMoodData and LogPlayData by PlayerId (fake), match where they have the same PlayerId, return MoodData
        public void TestQuery_JoinTables()
        {
            SQLite.TableMapping resultMapping = new SQLite.TableMapping(typeof(TestQueryResult));

            string query = "select MoodValue FROM LogMoodData JOIN LogPlayData ON LogMoodData.PlayerId = LogPlayData.PlayerId";
            List<object> list = this.db.FindCustomDataByQuery(resultMapping, query);

            string output = "";
            foreach (var obj in list)
            {
                output += ("Test value: " + (obj as TestQueryResult).MoodValue) + "\n";
            }
            PrintOutput(output);
        }

        #endregion

        #region Profiles

        public void LoadProfile(int profileId)
        {
            this.db.LoadProfile(profileId);
            PrintOutput("Loading profile " + profileId);
        }

        public void CreateCurrentProfile()
        {
            this.db.CreateProfile();
            PrintOutput("Creating tables for selected profile");
        }

        public void DeleteCurrentProfile()
        {
            this.db.DropProfile();
            PrintOutput("Deleting tables for current selected profile");
        }

        #endregion



        #region Inner Dumps

        public void DumpAllData<T>(List<T> list) where T : IData
        {
            string output = "";
            foreach (var data in list) {
                output += (data.GetId() + ": " + data.ToString()) + "\n";
            }
            PrintOutput(output); ;
        }

        public void DumpDataById(string id, IData data)
        {
            string output = "";
            if (data != null) {
                output += (data.GetId() + ": " + data.ToString());
            } else {
                output += "No data with ID " + id;
            }
            PrintOutput(output);
        }

        #endregion

        #region Utilities

        void PrintOutput(string output)
        {
            Debug.Log(output);
            OutputText.text = output.Substring(0,Mathf.Min(1000,output.Length));
        }

        void PrintArabicOutput(string output)
        {
            //Debug.Log(fixed_output);
            OutputTextArabic.text = ArabicAlphabetHelper.PrepareStringForDisplay(output);
        }

        #endregion
    }
}
