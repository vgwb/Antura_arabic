
namespace EA4S.Teacher
{
    public static class ConfigAI
    {
        // Minigame selection weights
        public const float minigame_playSessionWeight = 1f;
        public const float minigame_recentPlayWeight = 1f;

        // Word selection weights
        public const float word_scoreWeight = 1f;
        public const float word_recentPlayWeight = 1f;

        // General configuration
        public const int daysForMaximumRecentPlayMalus = 4;   // Days at which we get the maximum malus for a recent play weight
        
    }

}