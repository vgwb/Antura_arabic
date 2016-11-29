using UnityEngine;
using UnityEngine.UI;
using EA4S.Db;
using System.Collections.Generic;

namespace EA4S
{

    public class PlayerPanel : MonoBehaviour
    {
        public TextRender output;
        public BookGraph moodGraph;
        public BookGraph journeyGraph;

        void Start()
        {
            var str = "";

            str = "hello player (ID: " + AppManager.I.Player.Id + ")\n";
            str += "you're now in LB " + AppManager.I.Player.CurrentJourneyPosition + "\n";
            str += "your max LB is " + AppManager.I.Player.MaxJourneyPosition + "\n";
            str += "Mood " + AppManager.I.Player.MainMood + "\n";

            if (AppManager.I.Player.Precision != 0f) { str += "Precision " + AppManager.I.Player.Precision + "\n"; }
            if (AppManager.I.Player.Reaction != 0f) { str += "Reaction " + AppManager.I.Player.Reaction + "\n"; }
            if (AppManager.I.Player.Memory != 0f) { str += "Memory " + AppManager.I.Player.Memory + "\n"; }
            if (AppManager.I.Player.Logic != 0f) { str += "Logic " + AppManager.I.Player.Logic + "\n"; }
            if (AppManager.I.Player.Rhythm != 0f) { str += "Rhythm " + AppManager.I.Player.Rhythm + "\n"; }
            if (AppManager.I.Player.Musicality != 0f) { str += "Musicality " + AppManager.I.Player.Musicality + "\n"; }
            if (AppManager.I.Player.Sight != 0f) { str += "Sight " + AppManager.I.Player.Sight + "\n"; }

            // Number of play sessions
            var allPlaySessionInfos = AppManager.I.Teacher.scoreHelper.GetAllPlaySessionInfo();
            var unlockedPlaySessionInfos = allPlaySessionInfos.FindAll(x => x.unlocked);
            str += "Play sessions unlocked: " + unlockedPlaySessionInfos.Count + "\n";

            // Total elapsed time
            var db = AppManager.I.DB;
            var tableName = db.GetTableName<LogInfoData>();
            string query = "select * from \"" + tableName + "\"";
            List<LogInfoData> list = db.FindLogInfoDataByQuery(query);

            System.TimeSpan totalTimespan = new System.TimeSpan(0);
            bool foundStart = false;
            int startTimestamp = 0;
            int endTimestamp = 0;
            foreach (var infoData in list) {
                if (!foundStart && infoData.Event == InfoEvent.AppStarted) {
                    startTimestamp = infoData.Timestamp;
                    foundStart = true;
                } else if (foundStart && infoData.Event == InfoEvent.AppClosed) {
                    endTimestamp = infoData.Timestamp;
                    foundStart = false;

                    var deltaTimespan = GenericUtilities.FromTimestamp(endTimestamp) - GenericUtilities.FromTimestamp(startTimestamp);
                    totalTimespan += deltaTimespan;
                    //Debug.Log("TIME FOUND:"  + deltaTimespan.Days + " days " + deltaTimespan.Hours + " hours " + deltaTimespan.Minutes + " minutes " + deltaTimespan.Seconds + " seconds");
                }
            }
            str += "Total time elapsed: " + totalTimespan.Days + " days " + totalTimespan.Hours + " hours " + totalTimespan.Minutes + " minutes " + totalTimespan.Seconds + " seconds" + "\n";

            output.text = str;
            //AppManager.I.DB.GetLocalizationDataById("Game_Title").Arabic;
            //output.text += "\n" + AppManager.I.DB.GetLocalizationDataById("Game_Title2").Arabic;

            // Show moods
            int nMoods = 10;
            var latestMoods = AppManager.I.Teacher.GetLastMoodData(nMoods);
            float[] moodValues = latestMoods.ConvertAll(x => x.MoodValue).ToArray();
            moodGraph.SetValues(nMoods, AppConstants.maximumMoodValue, moodValues);

            // Show journey
            float[] journeyValues = unlockedPlaySessionInfos.ConvertAll(x => x.score).ToArray();
            //string[] journeyLabels = allPsInfo.ConvertAll(x => x.data.Id).ToArray();
            journeyGraph.SetValues(unlockedPlaySessionInfos.Count, 1f, journeyValues);
        }

    }
}