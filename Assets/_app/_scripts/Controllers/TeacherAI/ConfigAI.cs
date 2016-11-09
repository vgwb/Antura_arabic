
namespace EA4S.Teacher
{
    public static class ConfigAI
    {
        public static bool verboseTeacher = true;

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

        // General configuration
        public const int daysForMaximumRecentPlayMalus = 4;   // Days at which we get the maximum malus for a recent play weight

    }

}