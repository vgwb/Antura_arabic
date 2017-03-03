using System;
using UnityEngine;
using UnityEngine.UI;
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
            InfoTable.AddRow("لاعب", AppManager.I.Player.GetShortUuid(), "player UUID");
            InfoTable.AddRow("لاعب", AppManager.I.Player.CurrentJourneyPosition.ToString(), "current LB");
            var lastLetterLearned = AppManager.I.ScoreHelper.GetLastLearnedLetterInfo();
            InfoTable.AddRow("لاعب", (lastLetterLearned != null ? lastLetterLearned.data.ToString() : ""), "Last Letter");

            //var str = "";

            //str = "hello player (ID: " + AppManager.I.Player.Id + ")\n";
            //str += "you're now in LB " + AppManager.I.Player.CurrentJourneyPosition + "\n";
            //str += "your max LB is " + AppManager.I.Player.MaxJourneyPosition + "\n";
            //str += "Mood " + AppManager.I.Player.MainMood + "\n";

            //if (AppManager.I.Player.Precision != 0f) { str += "Precision " + AppManager.I.Player.Precision + "\n"; }
            //if (AppManager.I.Player.Reaction != 0f) { str += "Reaction " + AppManager.I.Player.Reaction + "\n"; }
            //if (AppManager.I.Player.Memory != 0f) { str += "Memory " + AppManager.I.Player.Memory + "\n"; }
            //if (AppManager.I.Player.Logic != 0f) { str += "Logic " + AppManager.I.Player.Logic + "\n"; }
            //if (AppManager.I.Player.Rhythm != 0f) { str += "Rhythm " + AppManager.I.Player.Rhythm + "\n"; }
            //if (AppManager.I.Player.Musicality != 0f) { str += "Musicality " + AppManager.I.Player.Musicality + "\n"; }
            //if (AppManager.I.Player.Sight != 0f) { str += "Sight " + AppManager.I.Player.Sight + "\n"; }

            // Number of play sessions
            var allPlaySessionInfos = AppManager.I.ScoreHelper.GetAllPlaySessionInfo();
            //Debug.Log(allPlaySessionInfos.ToDebugString());

            var unlockedPlaySessionInfos = allPlaySessionInfos.FindAll(x => x.unlocked);
            //str += "Play sessions unlocked: " + unlockedPlaySessionInfos.Count + "\n";
            InfoTable.AddRow("لاعب", unlockedPlaySessionInfos.Count.ToString(), "Play sessions unlocked");

            // Total elapsed time
            var totalTimespan = GetTotalElapsedTime();
            var timeElapsed = totalTimespan.Days + "d " + totalTimespan.Hours + "h " + totalTimespan.Minutes + "m " + totalTimespan.Seconds + "s";
            InfoTable.AddRow("لاعب", timeElapsed, "Total time elapsed");

            //AppManager.I.DB.GetLocalizationDataById("Game_Title").Arabic;
            //output.text += "\n" + AppManager.I.DB.GetLocalizationDataById("Game_Title2").Arabic;

            journeyGraph.Show(allPlaySessionInfos, unlockedPlaySessionInfos);

            //Debug.Log("LAST LETTER: " + AppManager.I.ScoreHelper.GetLastLearnedLetterInfo().data);
            //Debug.Log("Total play times: " + GetMiniGamesTotalPlayTime().ToDebugString());
            //Debug.Log("Number of plays: " + GetMiniGamesNumberOfPlays().ToDebugString());
        }

        #region Time Queries

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