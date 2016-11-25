#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using RND = UnityEngine.Random;

namespace EA4S.Db.Management
{
    public class DatabaseTester : MonoBehaviour
    {
        private DatabaseLoader dbLoader;
        private DatabaseManager dbManager;
        private TeacherAI teacherAI;
        private PlayerProfile playerProfile;

        public Text OutputText;
        public TextRender OutputTextArabic;

        public bool useTestDatabase;

        void Awake()
        {
            this.dbLoader = GetComponentInChildren<DatabaseLoader>();

            playerProfile = new PlayerProfile();
            playerProfile.CurrentJourneyPosition = new JourneyPosition(1, 2, 2);    // test
            this.dbManager = new DatabaseManager(useTestDatabase, playerProfile);

            teacherAI = new TeacherAI(dbManager, playerProfile);
        }

        #region Main Actions

        public void ImportAll()
        {
            dbLoader.LoadDatabase();
            DumpAllDataCounts();
        }

        public void RegenerateEnums()
        {
            dbLoader.RegenerateEnums();
        }

        public void RecreateDatabase()
        {
            dbLoader.RecreateDatabase();
        }

        public void CopyCurrentDatabaseForTesting()
        {
            dbLoader.CopyCurrentDatabaseForTesting();
        }

        #endregion

        #region Specific Logs

        public void DumpAllDataCounts()
        {
            var output = "";
            output += ("N letters: " + dbManager.GetAllLetterData().Count) + "\n";
            output += ("N words: " + dbManager.GetAllWordData().Count) + "\n";
            output += ("N phrases: " + dbManager.GetAllPhraseData().Count) + "\n";
            output += ("N minigames: " + dbManager.GetAllMiniGameData().Count) + "\n";
            output += ("N stages: " + dbManager.GetAllStageData().Count) + "\n";
            output += ("N learningblocks: " + dbManager.GetAllLearningBlockData().Count) + "\n";
            output += ("N playsessions: " + dbManager.GetAllPlaySessionData().Count) + "\n";
            output += ("N localizations: " + dbManager.GetAllLocalizationData().Count) + "\n";
            output += ("N rewards: " + dbManager.GetAllRewardData().Count) + "\n";
            PrintOutput(output);
        }

        public void DumpAllLetterData()
        {
            DumpAllData(dbManager.GetAllLetterData());
        }

        public void DumpAllWordData()
        {
            DumpAllData(dbManager.GetAllWordData());
        }

        public void DumpAllPhraseData()
        {
            DumpAllData(dbManager.GetAllPhraseData());
        }

        public void DumpAllPlaySessionData()
        {
            DumpAllData(dbManager.GetAllPlaySessionData());
        }

        public void DumpAllLearningBlockData()
        {
            DumpAllData(dbManager.GetAllLearningBlockData());
        }

        public void DumpAllStageData()
        {
            DumpAllData(dbManager.GetAllStageData());
        }

        public void DumpAllLocalizationData()
        {
            DumpAllData(dbManager.GetAllLocalizationData());
        }

        public void DumpAllMiniGameData()
        {
            DumpAllData(dbManager.GetAllMiniGameData());
        }

        public void DumpAllLogInfoData()
        {
            DumpAllData(dbManager.GetAllLogInfoData());
        }

        public void DumpAllLogLearnData()
        {
            DumpAllData(dbManager.GetAllLogLearnData());
        }

        public void DumpAllLogMoodData()
        {
            DumpAllData(dbManager.GetAllLogMoodData());
        }

        public void DumpAllLogPlayData()
        {
            DumpAllData(dbManager.GetAllLogPlayData());
        }

        public void DumpAllScoreData()
        {
            DumpAllData(dbManager.GetAllScoreData());
        }

        public void DumpLetterById(string id)
        {
            IData data = dbManager.GetLetterDataById(id);
            DumpDataById(id, data);
        }

        public void DumpWordById(string id)
        {
            var data = dbManager.GetWordDataById(id);
            var arabic_text = data.Arabic;
            PrintArabicOutput(arabic_text);
            DumpDataById(id, data);
        }

        public void DumpPhraseById(string id)
        {
            IData data = dbManager.GetPhraseDataById(id);
            DumpDataById(id, data);
        }

        public void DumpMiniGameByCode(MiniGameCode code)
        {
            IData data = dbManager.GetMiniGameDataByCode(code);
            DumpDataById(data.GetId(), data);
        }

        public void DumpStageById(string id)
        {
            IData data = dbManager.GetStageDataById(id);
            DumpDataById(id, data);
        }

        public void DumpPlaySessionById(string id)
        {
            IData data = dbManager.GetPlaySessionDataById(id);
            DumpDataById(id, data);
        }

        public void DumpLocalizationById(string id)
        {
            IData data = dbManager.GetLocalizationDataById(id);
            DumpDataById(id, data);
        }

        public void DumpRewardById(string id)
        {
            IData data = dbManager.GetRewardDataById(id);
            DumpDataById(id, data);
        }

        public void DumpLogDataById(string id)
        {
            IData data = dbManager.GetLogInfoDataById(id);
            DumpDataById(id, data);
        }


        public void DumpArabicWord(string id)
        {
            var data = dbManager.GetWordDataById(id);
            var arabic_text = data.Arabic;
            PrintArabicOutput(arabic_text);
        }

        public void DumpActiveMinigames()
        {
            var all_minigames = dbManager.GetAllMiniGameData();
            var active_minigames = dbManager.GetActiveMinigames();
            PrintOutput(active_minigames.Count + " active minigames out of " + all_minigames.Count);
        }

        #endregion

        #region Test Insert Log Data

        public void PopulateDatabaseRandomly()
        {
            for (int i = 0; i < RND.Range(10, 20); i++) TestInsertLogInfoData();
            for (int i = 0; i < RND.Range(10, 20); i++) TestInsertLogLearnData();
            for (int i = 0; i < RND.Range(10, 20); i++) TestInsertLogMoodData();
            for (int i = 0; i < RND.Range(10, 20); i++) TestInsertLogPlayData();
            for (int i = 0; i < RND.Range(40, 60); i++) TestInsertScoreData();
        }

        public void TestInsertLogInfoData()
        {
            var newData = new LogInfoData();
            newData.Session = UnityEngine.Random.Range(0, 10).ToString();
            newData.Timestamp = GenericUtilities.GetTimestampForNow();

            newData.Event = InfoEvent.Book;
            newData.Parameters = "test:1";

            this.dbManager.Insert(newData);
            PrintOutput("Inserted new LogInfoData: " + newData.ToString());
        }

        public void TestInsertLogLearnData()
        {
            var newData = new LogLearnData();
            newData.Session = UnityEngine.Random.Range(0, 10).ToString();
            newData.Timestamp = GenericUtilities.GetTimestampForNow();

            newData.PlaySession = "1.1.1";
            newData.MiniGame = MiniGameCode.Assessment_LetterShape;

            bool useLetter = RND.value > 0.5f;
            newData.TableName = useLetter ? "LetterData" : "WordData";
            newData.ElementId = useLetter
                ? RandomHelper.GetRandom(dbManager.GetAllLetterData()).GetId()
                : RandomHelper.GetRandom(dbManager.GetAllWordData()).GetId();

            newData.Score = RND.Range(-1f, 1f);

            this.dbManager.Insert(newData);
            PrintOutput("Inserted new LogLearnData: " + newData.ToString());
        }

        public void TestInsertLogMoodData()
        {
            var newData = new LogMoodData();
            newData.Session = UnityEngine.Random.Range(0, 10).ToString();
            newData.Timestamp = GenericUtilities.GetTimestampForNow();

            newData.MoodValue = RND.Range(0, 20);

            this.dbManager.Insert(newData);
            PrintOutput("Inserted new LogMoodData: " + newData.ToString());
        }

        public void TestInsertLogPlayData()
        {
            var newData = new LogPlayData();
            newData.Session = UnityEngine.Random.Range(0, 10).ToString();
            newData.Timestamp = GenericUtilities.GetRelativeTimestampFromNow(-RND.Range(0, 5));

            newData.PlaySession = "1.1.1";
            newData.MiniGame = MiniGameCode.Balloons_counting;
            newData.Score = RND.Range(0, 1f);
            newData.PlayEvent = RND.value > 0.5f ? PlayEvent.GameFinished : PlayEvent.Skill;
            newData.PlaySkill = PlaySkill.Logic;
            newData.RawData = "TEST";

            this.dbManager.Insert(newData);
            PrintOutput("Inserted new LogPlayData: " + newData.ToString());
        }

        public void TestInsertScoreData()
        {
            int rndTableValue = RND.Range(0, 7);
            DbTables rndTable = DbTables.Letters;
            string rndId = "";
            switch (rndTableValue) {
                case 0:
                    rndTable = DbTables.Letters;
                    rndId = RandomHelper.GetRandom(dbManager.GetAllLetterData()).GetId();
                    break;
                case 1:
                    rndTable = DbTables.Words;
                    rndId = RandomHelper.GetRandom(dbManager.GetAllWordData()).GetId();
                    break;
                case 2:
                    rndTable = DbTables.Phrases;
                    rndId = RandomHelper.GetRandom(dbManager.GetAllPhraseData()).GetId();
                    break; ;
                case 3:
                    rndTable = DbTables.MiniGames;
                    rndId = RandomHelper.GetRandom(dbManager.GetAllMiniGameData()).GetId();
                    break;
                case 4:
                    rndTable = DbTables.PlaySessions;
                    rndId = RandomHelper.GetRandom(dbManager.GetAllPlaySessionData()).GetId();
                    break;
                case 5:
                    rndTable = DbTables.Stages;
                    rndId = RandomHelper.GetRandom(dbManager.GetAllStageData()).GetId();
                    break;
                case 6:
                    rndTable = DbTables.Rewards;
                    rndId = RandomHelper.GetRandom(dbManager.GetAllRewardData()).GetId();
                    break;
            }

            var lastAccessTimestamp = GenericUtilities.GetRelativeTimestampFromNow(-RND.Range(0, 5));
            var score = RND.Range(0f, 1f);

            this.dbManager.Debug_UpdateScoreData(rndTable, rndId, score, lastAccessTimestamp);
            PrintOutput("Inserted (or replaced) new ScoreData: " + lastAccessTimestamp.ToString());
        }

        #endregion

        #region Test Query Log Data

        // Test that uses a simple select/where expression on a single table
        public void TestLINQLogData()
        {
            List<LogInfoData> list = this.dbManager.FindLogInfoData(x => x.Timestamp > 1000);
            DumpAllData(list);
        }

        // Test query: get all MoodData, ordered by MoodValue
        public void TestQuery_SingleTable1()
        {
            var tableName = this.dbManager.GetTableName<LogMoodData>();
            string query = "select * from \"" + tableName + "\" order by MoodValue";
            List<LogMoodData> list = this.dbManager.FindLogMoodDataByQuery(query);
            DumpAllData(list);
        }

        // Test query: get number of LogPlayData for a given PlaySession with a high enough score
        public void TestQuery_SingleTable2()
        {
            var tableName = this.dbManager.GetTableName<LogPlayData>();
            string targetPlaySessionId = "\"5\"";
            string query = "select * from \"" + tableName + "\" where Session = " + targetPlaySessionId;
            List<LogPlayData> list = this.dbManager.FindLogPlayDataByQuery(query);
            PrintOutput("Number of play data for PlaySession " + targetPlaySessionId + ": " + list.Count);
        }


        public class TestQueryResult
        {
            public int MoodValue { get; set; }
        }

        // Test query: get just the MoodValues (with a custom result class) from all LogMoodData entries
        public void TestQuery_SingleTable3()
        {
            SQLite.TableMapping resultMapping = new SQLite.TableMapping(typeof(TestQueryResult));

            string targetPlaySessionId = "\"5\"";
            string query = "select LogMoodData.MoodValue from LogMoodData where LogMoodData.Session = " + targetPlaySessionId;
            List<object> list = this.dbManager.FindCustomDataByQuery(resultMapping, query);

            string output = "Test values N: " + list.Count + "\n";
            foreach (var obj in list) {
                output += ("Test value: " + (obj as TestQueryResult).MoodValue) + "\n";
            }
            PrintOutput(output);
        }


        // Test query: join LogMoodData and LogPlayData by PlayerId (fake), match where they have the same PlayerId, return MoodData
        public void TestQuery_JoinTables()
        {
            SQLite.TableMapping resultMapping = new SQLite.TableMapping(typeof(TestQueryResult));

            string query = "select LogMoodData.MoodValue from LogMoodData inner join LogPlayData on LogMoodData.Session = LogPlayData.Session";
            List<object> list = this.dbManager.FindCustomDataByQuery(resultMapping, query);

            string output = "Test values N: " + list.Count + "\n";
            foreach (var obj in list) {
                output += ("Test value: " + (obj as TestQueryResult).MoodValue) + "\n";
            }
            PrintOutput(output);
        }
        #endregion

        #region Teacher

        public void Teacher_LastNMoods()
        {
            var list = teacherAI.GetLastMoodData(10);

            string output = "Latest 10 moods:\n";
            foreach (var data in list) output += GenericUtilities.FromTimestamp(data.Timestamp) + ": " + data.ToString() + "\n";
            PrintOutput(output);
        }

        public void Teacher_LatestScores()
        {
            var scores = teacherAI.scoreHelper.GetLatestScoresForMiniGame(MiniGameCode.Balloons_counting, 3);

            string output = "Scores:\n";
            foreach (var score in scores) output += score.ToString() + "\n";
            PrintOutput(output);
        }

        public void Teacher_AllPlaySessionScores()
        {
            var list = teacherAI.scoreHelper.GetCurrentScoreForAllPlaySessions();

            string output = "All play session scores:\n";
            foreach (var data in list) output += data.ElementId + ": " + data.Score + "\n";
            PrintOutput(output);
        }

        public void Teacher_FailedAssessmentLetters()
        {
            var list = teacherAI.GetFailedAssessmentLetters(MiniGameCode.Assessment_LetterShape);

            string output = "Failed letters for assessment 'Letters':\n";
            foreach (var data in list) output += data.ToString() + "\n";
            PrintOutput(output);
        }

        public void Teacher_FailedAssessmentWords()
        {
            var list = teacherAI.GetFailedAssessmentWords(MiniGameCode.Assessment_LetterShape);

            string output = "Failed words for assessment 'Letters':\n";
            foreach (var data in list) output += data.ToString() + "\n";
            PrintOutput(output);
        }

        public void Teacher_ScoreHistoryCurrentJourneyPosition()
        {
            var list = teacherAI.GetScoreHistoryForCurrentJourneyPosition();

            string output = "Score history for the current journey position (" + playerProfile.CurrentJourneyPosition.ToString() + ") in the PlayerProfile:\n";
            foreach (var data in list) output += GenericUtilities.FromTimestamp(data.Timestamp) + ": " + data.Score + "\n";
            PrintOutput(output);
        }

        // Deprecated
        public void Teacher_LettersOfWord()
        {
            Debug.LogWarning("Deprecated function.");
            /*var wordDataId = dbManager.GetWordDataByRandom().GetId();
            var list = teacherAI.GetLettersInWord(wordDataId);

            string output = list.Count + " letters in word " + wordDataId + ":\n";
            foreach (var data in list) output += data.Id + "\n";
            PrintOutput(output);*/
        }

        public void Teacher_PerformMiniGameSelection()
        {
            var currentJourneyPositionId = playerProfile.CurrentJourneyPosition.ToString();
            var list = teacherAI.SelectMiniGamesForPlaySession(currentJourneyPositionId, 2);

            string output = "Minigames selected (" + currentJourneyPositionId + "):\n";
            foreach (var data in list) output += data.Code + "\n";
            PrintOutput(output);
        }

        public void Teacher_PerformWordSelection()
        {
            Debug.LogWarning("Deprecated use, word selection is now built-in with more logic.");
            /*
            var currentJourneyPositionId = playerProfile.CurrentJourneyPosition.ToString();

            int nTests = 7;
            int nWordsPerTest = 2;
            string output = "";
            output = "Words selected (" + currentJourneyPositionId + "):\n";
            for (int i = 0; i < nTests; i++) {
                var list = teacherAI.SelectWordsForPlaySession(currentJourneyPositionId, nWordsPerTest);
                foreach (var data in list) output += data.Id + " ";
                output += "\n";
            }

            PrintOutput(output);
            */
        }

        #endregion

        #region Profiles

        public void LoadProfile(int profileId)
        {
            this.dbManager.LoadDynamicDbForPlayerProfile(profileId);
            PrintOutput("Loading profile " + profileId);
        }

        public void CreateCurrentProfile()
        {
            this.dbManager.CreateProfile();
            PrintOutput("Creating tables for selected profile");
        }

        public void DeleteCurrentProfile()
        {
            this.dbManager.DropProfile();
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
            PrintOutput(output);
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
            OutputText.text = output.Substring(0, Mathf.Min(1000, output.Length));
        }

        void PrintArabicOutput(string output)
        {
            OutputTextArabic.text = output;//ArabicAlphabetHelper.PrepareStringForDisplay(output);
        }

        #endregion
    }
}
#endif
