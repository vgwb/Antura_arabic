using System;
using UnityEngine;
using EA4S.Database;
using System.Collections.Generic;
using EA4S.Helpers;
using EA4S.Utilities;

namespace EA4S.Book
{

    public class PlayerPanel : MonoBehaviour
    {
        public InfoTable InfoTable;
        public GraphJourney journeyGraph;

        void Start()
        {
            InfoTable.Reset();

            // Stage reached
            InfoTable.AddRow("Stage reached", "", AppManager.I.Player.MaxJourneyPosition.GetShortTitle());

            // unlocked / total PlaySessions
            var totalPlaySessions = AppManager.I.ScoreHelper.GetAllPlaySessionInfo();
            var totalPlaySessionsUnlocked = totalPlaySessions.FindAll(x => x.unlocked);
            InfoTable.AddRow("Levels Unlocked", "", totalPlaySessionsUnlocked.Count.ToString() + " / " + totalPlaySessions.Count.ToString());

            // Total elapsed time
            var totalTimespan = GetTotalElapsedTime();
            InfoTable.AddRow("Started playing", "", totalTimespan.Days + "d " + totalTimespan.Hours + "h " + totalTimespan.Minutes + "m");

            // total play time
            var totalPlayTime = GetTotalPlayTime();
            InfoTable.AddRow("Total Playtime playing", "", totalPlayTime.Days + "d " + totalPlayTime.Hours + "h " + totalPlayTime.Minutes + "m");

            // Played Games
            InfoTable.AddRow("Played Games", "", GetTotalNumberOfPlays().ToString());

            // Total bones
            InfoTable.AddRow("Total bones", "", AppManager.I.Player.GetTotalNumberOfBones().ToString());

            // Total stars
            var totalStars = 0;
            InfoTable.AddRow("Total stars", "", totalStars.ToString());

            // unlocked / total REWARDS
            var totalRewards = 200;
            var totalRewardsUnlocked = 0;
            InfoTable.AddRow("Antura Rewards", "", totalRewardsUnlocked.ToString() + " / " + totalRewards);

            // unlocked / total Letters
            var totalLetters = 100;
            var totalLettersUnlocked = 0;
            InfoTable.AddRow("Letters Unlocked", "", totalLettersUnlocked.ToString() + " / " + totalLetters);

            // unlocked / total Words
            var totalWords = 100;
            var totalWordsUnlocked = 0;
            InfoTable.AddRow("Words Unlocked", "", totalWordsUnlocked.ToString() + " / " + totalWords);

            // unlocked / total Phrases
            var totalPhrases = 100;
            var totalPhrasesUnlocked = 0;
            InfoTable.AddRow("Phrases Unlocked", "", totalPhrasesUnlocked.ToString() + " / " + totalPhrases);

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

        TimeSpan GetTotalElapsedTime()
        {
            string query = "select * from \"" + typeof(LogInfoData).Name + "\"";
            var list = AppManager.I.DB.FindDataByQuery<LogInfoData>(query);

            System.TimeSpan totalTimespan = new System.TimeSpan(0);
            bool foundStart = false;
            int startTimestamp = 0;
            foreach (var infoData in list) {
                if (!foundStart && infoData.Event == InfoEvent.AppSessionStart) {
                    startTimestamp = infoData.Timestamp;
                    foundStart = true;
                } else if (foundStart && infoData.Event == InfoEvent.AppSessionEnd) {
                    var endTimestamp = infoData.Timestamp;
                    foundStart = false;

                    var deltaTimespan = GenericHelper.FromTimestamp(endTimestamp) - GenericHelper.FromTimestamp(startTimestamp);
                    totalTimespan += deltaTimespan;
                    //Debug.Log("TIME FOUND:"  + deltaTimespan.Days + " days " + deltaTimespan.Hours + " hours " + deltaTimespan.Minutes + " minutes " + deltaTimespan.Seconds + " seconds");
                }
            }
            return totalTimespan;
        }

        TimeSpan GetTotalPlayTime()
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

        int GetTotalNumberOfPlays()
        {
            int total = 0;
            string query = "select * from " + typeof(LogMiniGameScoreData).Name;
            var list = AppManager.I.DB.FindDataByQuery<LogMiniGameScoreData>(query);

            foreach (var data in list) {
                total++;
            }
            return total;
        }

        Dictionary<MiniGameCode, int> GetMiniGamesNumberOfPlays()
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