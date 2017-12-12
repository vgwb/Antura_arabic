using Antura.LivingLetters;
using Antura.Teacher;

namespace Antura.Minigames.MakeFriends
{
    public enum MakeFriendsDifficulty
    {
        EASY,
        MEDIUM,
        HARD
    }

    public enum MakeFriendsVariation
    {
        LetterForm = MiniGameCode.MakeFriends_letterform
    }

    public class MakeFriendsConfiguration : AbstractGameConfiguration
    {
        public MakeFriendsVariation Variation { get; set; }

        public override void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (MakeFriendsVariation)code;
        }

        // Singleton Pattern
        static MakeFriendsConfiguration instance;
        public static MakeFriendsConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new MakeFriendsConfiguration();
                return instance;
            }
        }

        private MakeFriendsConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new MakeFriendsQuestionProvider();
            Context = new MinigamesGameContext(MiniGameCode.MakeFriends_letterform, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0f;
            TutorialEnabled = true;
        }

        public override IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nMinCommonLetters = 1;
            int nMaxCommonLetters = 1;
            int nWrong = 5;
            int nWords = 2;

            var builderParams = new QuestionBuilderParameters();
            builder = new CommonLettersInWordQuestionBuilder(nPacks, nMinCommonLetters, nMaxCommonLetters, nWrong, nWords, parameters: builderParams);

            return builder;
        }

        public override MiniGameLearnRules SetupLearnRules()
        {
            var rules = new MiniGameLearnRules();
            // example: a.minigameVoteSkewOffset = 1f;
            return rules;
        }


        #region Difficulty Choice

        private const float EASY_THRESHOLD = 0f;
        private const float MEDIUM_THRESHOLD = 0.3f;
        private const float HARD_THRESHOLD = 0.7f;

        public MakeFriendsDifficulty DifficultyChoice
        {
            get
            {
                // GameManager Override
                if (MakeFriendsGame.Instance.overrideDifficulty)
                {
                    switch (MakeFriendsGame.Instance.difficultySetting)
                    {
                        case MakeFriendsDifficulty.EASY:
                            Difficulty = EASY_THRESHOLD;
                            break;

                        case MakeFriendsDifficulty.MEDIUM:
                            Difficulty = MEDIUM_THRESHOLD;
                            break;

                        case MakeFriendsDifficulty.HARD:
                            Difficulty = HARD_THRESHOLD;
                            break;
                    }
                }

                // SRDebugger Override
#if SRDebuggerEnabled
                if (SROptions.Current.MakeFriendsUseDifficulty)
                {
                    switch (SROptions.Current.MakeFriendsDifficulty)
                    {
                        case MakeFriendsVariation.EASY:
                            Difficulty = EASY_THRESHOLD;
                            break;

                        case MakeFriendsVariation.MEDIUM:
                            Difficulty = MEDIUM_THRESHOLD;
                            break;

                        case MakeFriendsVariation.HARD:
                            Difficulty = HARD_THRESHOLD;
                            break;
                    }
                }
#endif
                // Get Variation based on Difficulty
                MakeFriendsDifficulty variation;
                if (Difficulty < MEDIUM_THRESHOLD)
                {
                    variation = MakeFriendsDifficulty.EASY;
                }
                else if (Difficulty < HARD_THRESHOLD)
                {
                    variation = MakeFriendsDifficulty.MEDIUM;
                }
                else
                {
                    variation = MakeFriendsDifficulty.HARD;
                }

                return variation;
            }
        }

        #endregion
    }
}