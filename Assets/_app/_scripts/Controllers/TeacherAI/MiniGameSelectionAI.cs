using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    /// <summary>
    /// Handles the selection of what minigames to play during a playsession
    /// </summary>
    public class MiniGameSelectionAI
    {
        // Configuration
        private const float PLAY_SESSION_WEIGHT = 1f;
        private const float RECENT_PLAY_WEIGHT = 1f;

        private const int DAYS_FOR_MAXIMUM_RECENT_PLAY_MALUS = 4;   // Days at which we get the maximum malus for a recent play weight

        // References
        private DatabaseManager dbManager;
        private PlayerProfile playerProfile;

        public MiniGameSelectionAI(DatabaseManager _dbManager, PlayerProfile _playerProfile)
        {
            this.dbManager = _dbManager;
            this.playerProfile = _playerProfile;
        }

        public void InitialiseNewPlaySession()
        {
            // Nothing to be done here
        }

        public List<Db.MiniGameData> PerformSelection(string playSessionId, int numberToSelect)
        {
            Dictionary<MiniGameCode, float> playsession_weights_dict = new Dictionary<MiniGameCode, float>();

            // Get all minigames ids for the given playsession (from PlaySessionData)
            // ... also, keep the weights around
            Db.PlaySessionData playSessionData = dbManager.GetPlaySessionDataById(playSessionId);
            List<string> minigame_id_list = new List<string>();
            foreach(var minigameInPlaySession in playSessionData.Minigames)
            {
                minigame_id_list.Add(minigameInPlaySession.MiniGameCode.ToString());
                playsession_weights_dict[minigameInPlaySession.MiniGameCode] = minigameInPlaySession.Weight;
            }

            // Get all minigame data, filter by availability (from the static DB)
            List<Db.MiniGameData> minigame_data_list = dbManager.FindMiniGameData(x => x.Available && minigame_id_list.Contains(x.GetId()));

            // Create the weights list too
            List<float> weights_list = new List<float>(minigame_data_list.Count);

            // Retrieve the current score data (state) for each minigame (from the dynamic DB)
            List<Db.ScoreData> minigame_score_list = dbManager.FindScoreDataByQuery("SELECT * FROM ScoreData WHERE TableName = 'MiniGames'");

            //UnityEngine.Debug.Log("M GAME SCORE LIST: " + minigame_score_list.Count);
            //foreach(var l in minigame_score_list) UnityEngine.Debug.Log(l.ElementId);

            // Determine the final weight for each minigame
            string debugString = "";
            foreach (var minigame_data in minigame_data_list)
            {
                float cumulativeWeight = 0;
                var minigame_scoredata = minigame_score_list.Find(x => x.ElementId == minigame_data.GetId());
                int daysSinceLastScore = 0;
                if (minigame_scoredata != null)
                {
                    var timespanFromLastScoreToNow = GenericUtilities.GetTimeSpanBetween(minigame_scoredata.LastAccessTimestamp, GenericUtilities.GetTimestampForNow());
                    daysSinceLastScore = timespanFromLastScoreToNow.Days;
                }
                debugString += minigame_data.Code + " --- \t";

                // PlaySession Weight [0,1]
                float playSessionWeight = playsession_weights_dict[minigame_data.Code] / 100f; //  [0-100]
                cumulativeWeight += playSessionWeight * PLAY_SESSION_WEIGHT;
                debugString += " PSw: " + playSessionWeight * PLAY_SESSION_WEIGHT +"("+playSessionWeight+")";

                // RecentPlay Weight  [1,0]
                const float dayLinerWeightDecrease = 1f/DAYS_FOR_MAXIMUM_RECENT_PLAY_MALUS;
                float weightMalus = daysSinceLastScore * dayLinerWeightDecrease;
                float recentPlayWeight = 1f - UnityEngine.Mathf.Min(1, weightMalus);
                cumulativeWeight += recentPlayWeight * RECENT_PLAY_WEIGHT;
                debugString += " RPw: " + recentPlayWeight * RECENT_PLAY_WEIGHT + "(" + recentPlayWeight + ")";

                // Save cumulative weight
                weights_list.Add(cumulativeWeight);
                debugString += " TOTw: " + cumulativeWeight;
                debugString += "\n";
            }
            UnityEngine.Debug.Log(debugString);

            // Choose N minigames based on these weights
            var selectedMiniGameData = RandomHelper.RouletteSelectNonRepeating<Db.MiniGameData>(minigame_data_list, weights_list, numberToSelect);

            return selectedMiniGameData;
        }
    }
}