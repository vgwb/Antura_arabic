namespace EA4S
{
    /// <summary>
    /// Add inside this class your TextIDs
    /// </summary>
    public class TextID
    {
        public static readonly TextID GAME_RESULT_RETRY = new TextID("game_result_retry");
        public static readonly TextID GAME_RESULT_FAIR = new TextID("game_result_fair");
        public static readonly TextID GAME_RESULT_GOOD = new TextID("game_result_good");
        public static readonly TextID GAME_RESULT_GREAT = new TextID("game_result_great");

        public static readonly TextID ASSESSMENT_START_A1 = new TextID("assessment_start_A1");
        public static readonly TextID ASSESSMENT_START_A2 = new TextID("assessment_start_A2");
        public static readonly TextID ASSESSMENT_START_A3 = new TextID("assessment_start_A3");
        public static readonly TextID ASSESSMENT_RESULT_INTRO = new TextID("assessment_result_intro");
        public static readonly TextID ASSESSMENT_RESULT_GOOD = new TextID("assessment_result_good");
        public static readonly TextID ASSESSMENT_RESULT_VERYGOOD = new TextID("assessment_result_verygood");
        public static readonly TextID ASSESSMENT_RESULT_RETRY = new TextID("assessment_result_retry");

        // ...

        public static TextID GetTextIDFromStars(int stars)
        {
            if (stars < 0)
                return GAME_RESULT_RETRY;

            switch (stars)
            {
                case 0:
                    return GAME_RESULT_RETRY;
                case 1:
                    return GAME_RESULT_FAIR;
                case 2:
                    return GAME_RESULT_GOOD;
                default:
                    return GAME_RESULT_GREAT;
            }
        }

        string id;

        /// <summary>
        /// Use TextID.{NAME} to provide text to other components
        /// </summary>
        private TextID(string id)
        {
            this.id = id;
        }

        public override string ToString()
        {
            return id;
        }
    }
}
