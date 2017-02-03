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
            string query = string.Format("SELECT * FROM " + typeof(MinigameScoreData).Name + " AND ElementId = '{0}'",  (int)miniGameCode);
            List<MinigameScoreData> minigame_scoreData_list = dbManager.FindDataByQuery<MinigameScoreData>(query);
            if (minigame_scoreData_list.Count == 0)
            {
                playerPerformance = ConfigAI.startingDifficultyForNewMiniGame;
            }
            else
            {
                float minigameScore = minigame_scoreData_list[0].Score;
                playerPerformance = minigameScore;
            }

            float performanceDifficulty = Mathf.Clamp01(Mathf.InverseLerp(AppConstants.minimumMiniGameScore, AppConstants.maximumMiniGameScore, playerPerformance));
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