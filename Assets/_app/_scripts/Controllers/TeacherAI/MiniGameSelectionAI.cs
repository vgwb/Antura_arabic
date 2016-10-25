using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    public class MiniGameSelectionAI
    {
        private DatabaseManager dbManager;
        private PlayerProfile playerProfile;

        public MiniGameSelectionAI(DatabaseManager _dbManager, PlayerProfile _playerProfile)
        {
            this.dbManager = _dbManager;
            this.playerProfile = _playerProfile;
        }

        public struct WeightedMiniGameData
        {
            public Db.MiniGameData data;
            public float weight;

            public WeightedMiniGameData(Db.MiniGameData _data)
            {
                this.data = _data;
                weight = 0;
            }
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

            // Create the weighted mini game list
            List<WeightedMiniGameData> weightedMiniGames_list = new List<WeightedMiniGameData>();
            minigame_data_list.ConvertAll<WeightedMiniGameData>(x => new WeightedMiniGameData(x));
            foreach (var data in minigame_data_list)
            {
                var weightedMiniGame = new WeightedMiniGameData();
                weightedMiniGame.data = data;
                weightedMiniGames_list.Add(weightedMiniGame);
            }

            // Retrieve the current score data (state) for each minigame (from the dynamic DB)
            List<Db.ScoreData> minigame_score_list = dbManager.FindScoreDataByQuery("SELECT * FROM ScoreData WHERE TableName = 'MiniGames'");

            // Determine the final weight for each minigame
            foreach (var weightedData in weightedMiniGames_list)
            {
                var minigameData = weightedData.data;
                float cumulativeWeight = 0;

                // Consider PlaySession Weight
                float playsessionWeight = playsession_weights_dict[minigameData.Code];
                cumulativeWeight += playsessionWeight;

                // Retrieve the score data
                var minigame_scoredata = minigame_score_list.Find(x => x.ElementId == minigameData.GetId());

                // Consider RecentPlay Weight
                const float dayLinerWeightDecrease = -0.25f;
                //  var timespanFromLastScoreToNow = GenericUtilites.GetTimeSpanBetween(minigame_scoredata.LastAccessTimestamp, GenericUtilites.GetTimestampForNow());
                //  int daysSinceLastScore = timespanFromLastScoreToNow.Days;
                //  float recentPlayWeight = daysSinceLastScore * dayLinerWeightDecrease;
               // cumulativeWeight += recentPlayWeight;
            }

            // Choose N minigames based on these weights (without re-immission)
            float total_weight = 0;

            return new List<Db.MiniGameData>();
        }
    }
}