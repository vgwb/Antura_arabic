using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    /// <summary>
    /// Handles the selection of what words a minigame should use, given a playsession
    /// </summary>
    public class WordSelectionAI 
    {
        // Configuration
        private const float SCORE_WEIGHT = 1f;
        private const float RECENT_PLAY_WEIGHT = 1f;

        private const int DAYS_FOR_MAXIMUM_RECENT_PLAY_MALUS = 4;   // Days at which we get the maximum malus for a recent play weight

        // References
        private DatabaseManager dbManager;
        private PlayerProfile playerProfile;

        // Inner state
        private List<string> currentAlreadyPickedLetterIds = new List<string>();

        public WordSelectionAI(DatabaseManager _dbManager, PlayerProfile _playerProfile)
        {
            this.dbManager = _dbManager;
            this.playerProfile = _playerProfile;
        }

        public void InitialiseNewPlaySession()
        {
            currentAlreadyPickedLetterIds.Clear();
        }

        public List<Db.WordData> PerformSelection(string playSessionId, int numberToSelect)
        {
            Db.PlaySessionData playSessionData = dbManager.GetPlaySessionDataById(playSessionId);
            List<Db.ScoreData> word_scoreData_list = dbManager.FindScoreDataByQuery("SELECT * FROM ScoreData WHERE TableName = 'Words'");

            List<Db.WordData> selectedWordData_list = new List<Db.WordData>();

            int nRemainingToSelect = numberToSelect;

            // First check current Words
            //UnityEngine.Debug.Log("Selecting " + nRemainingToSelect + " MAIN words");
            SelectWordsFrom(playSessionData.Words, selectedWordData_list, word_scoreData_list, ref nRemainingToSelect);

            // ... if it's not enough, check previous Words
            if (nRemainingToSelect > 0)
            {
                //UnityEngine.Debug.Log("Selecting " + nRemainingToSelect + " PREVIOUS words");
                SelectWordsFrom(playSessionData.Words_previous, selectedWordData_list, word_scoreData_list, ref nRemainingToSelect);
            }

            // ... if that's still not enough, check words from past sessions
            if (nRemainingToSelect > 0)
            {
                //UnityEngine.Debug.Log("Selecting " + nRemainingToSelect + " PAST words");
                SelectWordsFrom(GetAllWordIdsFromPreviousPlaySessions(playSessionData), selectedWordData_list, word_scoreData_list, ref nRemainingToSelect);
            }

            // ... if that's still not enough, there is some issue.
            if (nRemainingToSelect > 0)
            {
                UnityEngine.Debug.LogWarning("Warning: could not find enough words for play session " + playSessionId + " (found " + (numberToSelect - nRemainingToSelect) + " out of " + (numberToSelect) + ")");
            }

            return selectedWordData_list;
        }

        void SelectWordsFrom(string[] currentWordIds, List<Db.WordData> selectedWordData_list, List<Db.ScoreData> word_scoreData_list, ref int nRemainingToSelect)
        {
            List<Db.WordData> wordData_list = new List<Db.WordData>();
            List<float> weights_list = new List<float>();
            foreach (var word_Id in currentWordIds)
            {
                if (currentAlreadyPickedLetterIds.Contains(word_Id))
                {
                    continue;
                }

                float cumulativeWeight = 0;
                var word_scoreData = word_scoreData_list.Find(x => x.ElementId == word_Id);
                float currentWordScore = 0;
                int daysSinceLastScore = 0;
                if (word_scoreData != null)
                {
                    var timespanFromLastScoreToNow = GenericUtilities.GetTimeSpanBetween(word_scoreData.LastAccessTimestamp, GenericUtilities.GetTimestampForNow());
                    daysSinceLastScore = timespanFromLastScoreToNow.Days;
                    currentWordScore = word_scoreData.Score;
                }

                // Score Weight [0,1]: higher the lower the score [-1,1] is
                var scoreWeight = 0.5f * (1 - currentWordScore);
                cumulativeWeight += scoreWeight * SCORE_WEIGHT;

                // Always skip letters that have a score weight of zero
                if (scoreWeight == 0)
                {
                    continue;
                }

                // RecentPlay Weight  [1,0]: higher the more in the past we saw that word
                const float dayLinerWeightDecrease = 1f / DAYS_FOR_MAXIMUM_RECENT_PLAY_MALUS;
                float weightMalus = daysSinceLastScore * dayLinerWeightDecrease;
                float recentPlayWeight = 1f - UnityEngine.Mathf.Min(1, weightMalus);
                cumulativeWeight += recentPlayWeight * RECENT_PLAY_WEIGHT;

                //UnityEngine.Debug.Log("Word " + word_Id + " score: " + currentWordScore + " days " + daysSinceLastScore);

                // Save cumulative weight
                if (cumulativeWeight <= 0)
                {
                    continue;
                }
                weights_list.Add(cumulativeWeight);

                // Add the data to the list
                var wordData = dbManager.GetWordDataById(word_Id);
                wordData_list.Add(wordData);
            }

            //UnityEngine.Debug.Log("Number of words: " + wordData_list.Count);

            // Select some words
            if (wordData_list.Count > 0) {
                int nToSelectFromCurrentList = UnityEngine.Mathf.Min(wordData_list.Count, nRemainingToSelect);
                var chosenWords = RandomHelper.RouletteSelectNonRepeating(wordData_list, weights_list, nToSelectFromCurrentList);
                selectedWordData_list.AddRange(chosenWords);
                nRemainingToSelect -= nToSelectFromCurrentList;
                currentAlreadyPickedLetterIds.AddRange(chosenWords.ConvertAll<string>(x => x.Id));
            }
        }

        string[] GetAllWordIdsFromPreviousPlaySessions(Db.PlaySessionData current_ps)
        {
            // @note: this assumes that all play sessions are correctly ordered
            var all_ps_list = dbManager.GetAllPlaySessionData();
            int index_current_ps = all_ps_list.IndexOf(current_ps);

            List<string> allWordIds = new List<string>();
            for (int prev_ps_id = 0; prev_ps_id < index_current_ps; prev_ps_id++)
            {
                allWordIds.AddRange(all_ps_list[prev_ps_id].Words);
                allWordIds.AddRange(all_ps_list[prev_ps_id].Words_previous);
            }

            return allWordIds.ToArray();
        }

    }
}