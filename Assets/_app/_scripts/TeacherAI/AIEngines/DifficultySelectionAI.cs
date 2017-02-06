using System.Collections.Generic;
using EA4S.Core;
using EA4S.Database;
using EA4S.Profile;
using UnityEngine;

namespace EA4S.Teacher
{
    /// <summary>
    /// Handles the selection of the difficulty to use for a given minigame
    /// </summary>
    public class DifficultySelectionAI
    {

        // References
        private DatabaseManager dbManager;
        private PlayerProfile playerProfile;

        // Weights
        private float ageWeightContribution = ConfigAI.difficulty_weight_age;                   // Higher age -> higher difficulty
        private float performanceWeightContribution = ConfigAI.difficulty_weight_performance;   // Higher performance -> higher difficulty

        public DifficultySelectionAI(DatabaseManager _dbManager)
        {
            dbManager = _dbManager;
        }

        public void SetPlayerProfile(PlayerProfile _playerProfile)
        {
            playerProfile = _playerProfile;
        }

        public float SelectDifficulty(MiniGameCode miniGameCode)
        {
            float totalWeight = ageWeightContribution + performanceWeightContribution;

            // Age
            var playerAge = playerProfile.Age;
            float ageDifficulty = Mathf.Clamp01(Mathf.InverseLerp(AppConstants.minimumAge, AppConstants.maximumAge, playerAge));
            float weightedAgeDifficulty = ageDifficulty * ageWeightContribution / totalWeight;

            // Performance
            float playerPerformance;
            string query = string.Format("SELECT * FROM " + typeof(MinigameScoreData).Name + " WHERE ElementId = '{0}'",  (int)miniGameCode);
            List<MinigameScoreData> minigame_scoreData_list = dbManager.FindDataByQuery<MinigameScoreData>(query);
            if (minigame_scoreData_list.Count == 0)
            {
                playerPerformance = ConfigAI.startingDifficultyForNewMiniGame;
            }
            else
            {
                // We use a custom logic to define the difficulty:
                // - start from performance = 0
                // - get last N scores for the minigame
                // - a score of 0 diminishes the performance
                // - a score of 1 does not change it 
                // - a score of 2 or 3 increases it

                // TODO: query on last X minigame logged scores
                List<int> scores = new List<int>() {1,0,2,2,0,1,3};

                // Diminish to create the wegiths [-1, 0, 1, 2]
                for (var i = 0; i < scores.Count; i++)
                    scores[i] -= 1;

                // Compute the performance for these minigames starting from zero and adding values
                const float scorePointsContribution = 0.3f;
                playerPerformance = 0f;
                for (var i = 0; i < scores.Count; i++)
                {
                    playerPerformance += scores[i] * scorePointsContribution;
                }
                playerPerformance = Mathf.Clamp01(playerPerformance);

                //int minigameScore = minigame_scoreData_list[0].Score;
                //playerPerformance = minigameScore;
            }

            float performanceDifficulty = playerPerformance;
            float weightedPerformanceDifficulty = performanceDifficulty * performanceWeightContribution / totalWeight;

            // Total
            float totalDifficulty = weightedAgeDifficulty + weightedPerformanceDifficulty;

            // Debug log
            if (ConfigAI.verboseTeacher) {
                string debugString = "-----  TEACHER: Selected Difficulty: " + totalDifficulty + " -----";
                debugString += "\n From Age (C " + ageWeightContribution + "): " + ageDifficulty + " xw(" + weightedAgeDifficulty + ")";
                debugString += "\n From Performance (C " + performanceWeightContribution + "): " + performanceDifficulty + " xw(" + weightedPerformanceDifficulty + ")";
                Debug.Log(debugString);
            }

            return totalDifficulty;
        }

    }
}