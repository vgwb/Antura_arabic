using System;
using UnityEngine;
using EA4S.Database;
using System.Collections.Generic;
using System.Linq;
using EA4S.Helpers;
using EA4S.Rewards;
using EA4S.Utilities;
using UniRx;

namespace EA4S.Book
{

    public class PlayerPanel : MonoBehaviour
    {
        public InfoTable InfoTable;
        public GraphJourney journeyGraph;

        void Start()
        {
            InfoTable.Reset();

            // Level reached
            InfoTable.AddRow("Level", "", AppManager.I.Player.MaxJourneyPosition.GetShortTitle());

            // Unlocked / total PlaySessions
            var totalPlaySessions = AppManager.I.ScoreHelper.GetAllPlaySessionInfo();
            var totalPlaySessionsUnlocked = totalPlaySessions.FindAll(x => x.unlocked);
            InfoTable.AddRow("Unlocked Levels", "", totalPlaySessionsUnlocked.Count.ToString() + " / " + totalPlaySessions.Count.ToString());

            // Total elapsed time
            var totalTimespan = GetTotalApplicationTime();
            InfoTable.AddRow("Total application time", "", totalTimespan.Days + "d " + totalTimespan.Hours + "h " + totalTimespan.Minutes + "m");

            // total play time
            var totalPlayTime = GetTotalMiniGamePlayTime();
            InfoTable.AddRow("Total minigame time", "", totalPlayTime.Days + "d " + totalPlayTime.Hours + "h " + totalPlayTime.Minutes + "m");

            // Played Games
            InfoTable.AddRow("Number of games played", "", GetTotalMiniGamePlayInstances().ToString());

            // Total bones
            InfoTable.AddRow("Total bones collected", "", AppManager.I.Player.GetTotalNumberOfBones().ToString());

            // Total stars
            var totalStars = GetTotalMiniGameStars();
            InfoTable.AddRow("Total stars earned", "", totalStars.ToString());

            // unlocked / total REWARDS
            var totalRewards = GetTotalRewards();
            var totalRewardsUnlocked = GetTotalUnlockedRewards();
            InfoTable.AddRow("Antura rewards", "", totalRewardsUnlocked.ToString() + " / " + totalRewards);

            // unlocked / total Letters
            var totalLetters = GetTotalVocabularyData(VocabularyDataType.Letter);
            var totalLettersUnlocked = GetTotalVocabularyDataUnlocked(VocabularyDataType.Letter);
            InfoTable.AddRow("Unlocked Letters", "", totalLettersUnlocked.ToString() + " / " + totalLetters);

            // unlocked / total Words
            var totalWords = GetTotalVocabularyData(VocabularyDataType.Word);
            var totalWordsUnlocked = GetTotalVocabularyDataUnlocked(VocabularyDataType.Word);
            InfoTable.AddRow("Unlocked Words", "", totalWordsUnlocked.ToString() + " / " + totalWords);

            // unlocked / total Phrases
            var totalPhrases = GetTotalVocabularyData(VocabularyDataType.Phrase);
            var totalPhrasesUnlocked = GetTotalVocabularyDataUnlocked(VocabularyDataType.Phrase);
            InfoTable.AddRow("Unlocked Phrases", "", totalPhrasesUnlocked.ToString() + " / " + totalPhrases);

            // player UUID
            InfoTable.AddRow("Player Code", "", AppManager.I.Player.GetShortUuid());

            ////////////////////////////////////
            /// 
            // last lettert learnd
            //var lastLetterLearned = AppManager.I.ScoreHelper.GetLastLearnedLetterInfo();
            //InfoTable.AddRow("Last Letter", "", (lastLetterLearned != null ? lastLetterLearned.data.ToString() : ""));

            //if (AppManager.I.Player.Precision != 0f) { str += "Precision " + AppManager.I.Player.Precision + "\n"; }
            //if (AppManager.I.Player.Reaction != 0f) { str += "Reaction " + AppManager.I.Player.Reaction + "\n"; }
            //if (AppManager.I.Player.Memory != 0f) { str += "Memory " + AppManager.I.Player.Memory + "\n"; }
            //if (AppManager.I.Player.Logic != 0f) { str += "Logic " + AppManager.I.Player.Logic + "\n"; }
            //if (AppManager.I.Player.Rhythm != 0f) { str += "Rhythm " + AppManager.I.Player.Rhythm + "\n"; }
            //if (AppManager.I.Player.Musicality != 0f) { str += "Musicality " + AppManager.I.Player.Musicality + "\n"; }
            //if (AppManager.I.Player.Sight != 0f) { str += "Sight " + AppManager.I.Player.Sight + "\n"; }


            //Debug.Log("LAST LETTER: " + AppManager.I.ScoreHelper.GetLastLearnedLetterInfo().data);
            //Debug.Log("Total play times: " + GetMiniGamesTotalPlayTime().ToDebugString());
            //Debug.Log("Number of plays: " + GetMiniGamesNumberOfPlays().ToDebugString());

            // GRAPH
            //journeyGraph.Show(allPlaySessionInfos, unlockedPlaySessionInfos);
        }

        #region Queries

        TimeSpan GetTotalApplicationTime()
        {
            string query = "select * from \"" + typeof(LogInfoData).Name + "\"";
            var list = AppManager.I.DB.FindDataByQuery<LogInfoData>(query);

            System.TimeSpan totalTimespan = new System.TimeSpan(0);
            bool foundStart = false;
            int startTimestamp = 0;
            foreach (var infoData in list) {
                if (!foundStart && infoData.Event == InfoEvent.AppSessionStart) {
                    startTimestamp = infoData.Timestamp;
                    //Debug.Log("START: " + infoData.Timestamp);
                    foundStart = true;
                } else if (foundStart && infoData.Event == InfoEvent.AppSessionEnd) {
                    var endTimestamp = infoData.Timestamp;
                    foundStart = false;
                    //Debug.Log("END: " + infoData.Timestamp);

                    var deltaTimespan = GenericHelper.GetTimeSpanBetween(startTimestamp, endTimestamp);
                    totalTimespan += deltaTimespan;
                    //Debug.Log("TIME FOUND:"  + deltaTimespan.Days + " days " + deltaTimespan.Hours + " hours " + deltaTimespan.Minutes + " minutes " + deltaTimespan.Seconds + " seconds");
                }
            }

            // Time up to now
            if (foundStart)
            {
                var deltaTimespan = GenericHelper.GetTimeSpanBetween(startTimestamp, GenericHelper.GetTimestampForNow());
                totalTimespan += deltaTimespan;
                //Debug.Log("TIME UP TO NOW:" + deltaTimespan.Days + " days " + deltaTimespan.Hours + " hours " + deltaTimespan.Minutes + " minutes " + deltaTimespan.Seconds + " seconds");
            }
            return totalTimespan;
        }

        TimeSpan GetTotalMiniGamePlayTime()
        {
            float totalSeconds = 0f;
            string query = "select * from " + typeof(MiniGameScoreData).Name;
            var list = AppManager.I.DB.FindDataByQuery<MiniGameScoreData>(query);

            foreach (var data in list) {
                totalSeconds += data.TotalPlayTime;
            }
            TimeSpan t = TimeSpan.FromSeconds(totalSeconds);
            return t;
        }

        Dictionary<MiniGameCode, float> GetMiniGamesTotalPlayTime()
        {
            Dictionary<MiniGameCode, float> dict = new Dictionary<MiniGameCode, float>();
            string query = "select * from " + typeof(MiniGameScoreData).Name;
            var list = AppManager.I.DB.FindDataByQuery<MiniGameScoreData>(query);

            foreach (var data in list) {
                dict[data.MiniGameCode] = data.TotalPlayTime;
            }
            return dict;
        }

        int GetTotalMiniGamePlayInstances()
        {
            int total = 0;
            string query = "select * from " + typeof(LogMiniGameScoreData).Name;
            var list = AppManager.I.DB.FindDataByQuery<LogMiniGameScoreData>(query);

            foreach (var data in list) {
                total++;
            }
            return total;
        }

        int GetTotalMiniGameStars()
        {
            string query = "select * from " + typeof(MiniGameScoreData).Name;
            var list = AppManager.I.DB.FindDataByQuery<MiniGameScoreData>(query);
            var totalStars = list.Sum(data => data.Stars);
            return totalStars;
        }

        int GetTotalVocabularyData(VocabularyDataType dataType)
        {
            int count = 0;
            switch (dataType)
            {
                case VocabularyDataType.Letter:
                    count = AppManager.I.DB.GetAllLetterData().Count;
                    break;
                case VocabularyDataType.Word:
                    count = AppManager.I.DB.GetAllWordData().Count;
                    break;
                case VocabularyDataType.Phrase:
                    count = AppManager.I.DB.GetAllPhraseData().Count;
                    break;
            }
            return count;
        }

        int GetTotalVocabularyDataUnlocked(VocabularyDataType dataType)
        {
            if (AppManager.I.Player.IsDemoUser) return GetTotalVocabularyData(dataType);
            string query = "select * from " + typeof(VocabularyScoreData).Name + " where VocabularyDataType='" + (int)dataType + "'";
            var list = AppManager.I.DB.FindDataByQuery<VocabularyScoreData>(query);
            return list.Count(data => data.Unlocked);
        }

        int GetTotalRewards()
        {
            return RewardSystemManager.GetTotalRewardsCount();
        }

        int GetTotalUnlockedRewards()
        {
            return RewardSystemManager.GetUnlockedRewardsCount();
        }

        Dictionary<MiniGameCode, int> GetNumberOfPlaysByMiniGame()
        {
            Dictionary<MiniGameCode, int> dict = new Dictionary<MiniGameCode, int>();
            string query = "select * from " + typeof(LogMiniGameScoreData).Name;
            var list = AppManager.I.DB.FindDataByQuery<LogMiniGameScoreData>(query);

            foreach (var data in list) {
                if (!dict.ContainsKey(data.MiniGameCode)) {
                    dict[data.MiniGameCode] = 0;
                }
                dict[data.MiniGameCode]++;
            }
            return dict;
        }

        #endregion

    }
}