
namespace EA4S.Teacher
{
    public static class ConfigAI
    {
        // Debug
        public static bool verboseTeacher = true;
        public static bool forceJourneyIgnore = true; // If true, the journey progression logic is turned off, so that all data is usable

        // General configuration
        public const int daysForMaximumRecentPlayMalus = 4;   // Days at which we get the maximum malus for a recent play weight
        public const int numberOfMinigamesPerPlaySession = 3;

        // Minigame selection weights
        public const float minigame_playSessionWeight = 1f;
        public const float minigame_recentPlayWeight = 1f;

        // Word selection weights
        public const float word_scoreWeight = 1f;
        public const float word_recentPlayWeight = 1f;

        // Difficulty selection weights
        public const float difficulty_weight_age = 1f;
        public const float difficulty_weight_journey = 1f;
        public const float difficulty_weight_performance = 1f;

        public const float startingDifficultyForNewMiniGame = 0f;

        // Application details
        public const float minimumAge = 7;
        public const float maximumAge = 15;

        public const float minimumStage = 1;
        public const float maximumStage = 6;

        public const float minimumMiniGameScore = -1;
        public const float maximumMiniGameScore = 1;

        public const int minimumMoodValue = 1;
        public const int maximumMoodValue = 5;


    }

}