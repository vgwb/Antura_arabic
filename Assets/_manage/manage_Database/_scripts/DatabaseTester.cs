using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using ArabicSupport;

namespace EA4S.Db.Management
{
    public class DatabaseTester : MonoBehaviour
    {
        public DatabaseLoader DBLoader;
        private DatabaseManager db;

        public Text OutputText;
        public TextMeshProUGUI OutputTextArabic;

        void Awake()
        {
            string playerId = "1";
            this.db = new DatabaseManager(playerId);
        }

        #region Main Actions

        public void ImportAll()
        {
            DBLoader.LoadDatabase();
            LogAllDataCounts();
        }

        public void RegenerateRuntimeDB()
        {
            db.RegenerateRuntimeDB();
        }

        #endregion

        #region Specific Logs

        public void LogAllDataCounts()
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

        public void LogAllLetterData()
        {
            LogAllData<LetterData>(db.FindAllLetterData());
        }

        public void LogAllWordData()
        {
            LogAllData<WordData>(db.FindAllWordData());
        }

        public void LogAllPhraseData()
        {
            LogAllData<PhraseData>(db.FindAllPhraseData());
        }

        public void LogAllPlaySessionData()
        {
            LogAllData<PlaySessionData>(db.FindAllPlaySessionData());
        }

        public void LogAllStageData()
        {
            LogAllData<StageData>(db.FindAllStageData());
        }

        public void LogAllLocalizationData()
        {
            LogAllData<LocalizationData>(db.FindAllLocalizationData());
        }

        public void LogAllMiniGameData()
        {
            LogAllData<MiniGameData>(db.FindAllMiniGameData());
        }

        public void LogAllAssessmentData()
        {
            LogAllData<AssessmentData>(db.FindAllAssessmentData());
        }

        public void LogAllLogData()
        {
            LogAllData<LogData>(db.FindAllLogData());
        }

        public void LogLetterById(string id)
        {
            IData data = db.GetLetterDataById(id);
            LogDataById(id, data);
        }

        public void LogWordById(string id)
        {
            IData data = db.GetWordDataById(id);
            LogDataById(id, data);
        }

        public void LogPhraseById(string id)
        {
            IData data = db.GetPhraseDataById(id);
            LogDataById(id, data);
        }

        public void LogMiniGameById(string id)
        {
            IData data = db.GetMiniGameDataById(id);
            LogDataById(id, data);
        }

        public void LogStageById(string id)
        {
            IData data = db.GetStageDataById(id);
            LogDataById(id, data);
        }

        public void LogPlaySessionById(string id)
        {
            IData data = db.GetPlaySessionDataById(id);
            LogDataById(id, data);
        }

        public void LogAssessmentById(string id)
        {
            IData data = db.GetAssessmentDataById(id);
            LogDataById(id, data);
        }

        public void LogLocalizationById(string id)
        {
            IData data = db.GetLocalizationDataById(id);
            LogDataById(id, data);
        }

        public void LogRewardById(string id)
        {
            IData data = db.GetRewardDataById(id);
            LogDataById(id, data);
        }

        public void LogLogDataById(string id)
        {
            IData data = db.GetLogDataById(id);
            LogDataById(id, data);
        }

        #endregion

        #region Log - specific tests

        public void LogArabicWord(string id)
        {
            var data = db.GetWordDataById(id);
            var arabic_text = data.Arabic;
            PrintArabicOutput(arabic_text);
        }

        public void LogActiveMinigames()
        {
            var all_minigames = db.FindAllMiniGameData();
            var active_minigames = db.FindAllActiveMinigames();
            PrintOutput(active_minigames.Count + " active minigames out of " + all_minigames.Count);
        }

        public void TestInsertLogData()
        {
            LogData newData = new LogData();
            newData.Id = UnityEngine.Random.Range(0f, 999).ToString();
            newData.Time = Time.time.ToString();
            newData.Session = UnityEngine.Random.Range(0, 10).ToString();
            newData.PlayerID = 1;
            newData.PlaySkill = LogPlaySkill.Musicality;
            newData.Type = LogDataType.Info;
            newData.Score = UnityEngine.Random.Range(0f, 10f);

            this.db.InsertLogData(newData);

            PrintOutput("Inserted new LogData: " + newData.ToString());
        }

        public void TestLINQLogData()
        {
            List<LogData> list = this.db.FindAllLogData(x => x.Score > 5f); 
            LogAllData(list);
        }

        #endregion

        #region Inner Logs

        public void LogAllData<T>(List<T> list) where T : IData
        {
            string output = "";
            foreach (var data in list)
            {
                output += (data.GetId() + ": " + data.ToString()) + "\n";
            }
            PrintOutput(output); ;
        }

        public void LogDataById(string id, IData data)
        {
            string output = "";
            if (data != null)
            { 
                output += (data.GetId() + ": " + data.ToString());
            }
            else
            {
                output += "No data with ID " + id;
            }
            PrintOutput(output);
        }

        #endregion

        #region Utilities

        void PrintOutput(string output)
        {
            Debug.Log(output);
            OutputText.text = output;
        }
   
        void PrintArabicOutput(string output)
        {
            string fixed_output = ReverseText(ArabicFixer.Fix(output));
            Debug.Log(fixed_output);
            OutputTextArabic.text = fixed_output;
        }

        string ReverseText(string text)
        {
            char[] cArray = text.ToCharArray();
            string reverse = String.Empty;
            for (int i = cArray.Length - 1; i > -1; i--)
            {
                reverse += cArray[i];
            }
            return reverse;
        }

        #endregion
    }
}
