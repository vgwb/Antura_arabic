using EA4S.MinigamesAPI;
using EA4S.MinigamesAPI.Sample;
using EA4S.MinigamesCommon;
using EA4S.Teacher;

namespace EA4S.Balloons
{
    public enum BalloonsVariation : int
    {
        Spelling = 1,
        Words = 2,
        Letter = 3,
        Counting = 4,

    }

    public class BalloonsConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }

        public IQuestionProvider Questions { get; set; }

        #region Game configurations

        public float Difficulty { get; set; }

        public BalloonsVariation Variation { get; set; }

        #endregion

        /////////////////
        // Singleton Pattern
        static BalloonsConfiguration instance;

        public static BalloonsConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new BalloonsConfiguration();
                return instance;
            }
        }

        /////////////////

        private BalloonsConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE

            Questions = new SampleQuestionProvider();

            Variation = BalloonsVariation.Spelling;

            Context = new MinigamesGameContext(MiniGameCode.Balloons_spelling, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.5f;
        }

        #region external configuration call

        public static void SetConfiguration(float _difficulty, int _variation)
        {
            instance = new BalloonsConfiguration()
            {
                Difficulty = _difficulty,
                Variation = (BalloonsVariation)_variation,
            };
        }

        public IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 6;
            int nCorrect = 3;
            int nWrong = 8;

            var builderParams = new Teacher.QuestionBuilderParameters();

            switch (Variation)
            {
                case BalloonsVariation.Spelling:
                    builderParams.wordFilters.excludeColorWords = true;
                    builderParams.wordFilters.requireDrawings = true;
                    builder = new LettersInWordQuestionBuilder(nPacks, useAllCorrectLetters:true, nWrong:nWrong, parameters: builderParams);
                    break;
                case BalloonsVariation.Words:
                    builderParams.wordFilters.excludeColorWords = true;
                    builderParams.wordFilters.requireDrawings = true;
                    builder = new RandomWordsQuestionBuilder(nPacks, 1, nWrong, firstCorrectIsQuestion:true, parameters:builderParams);
                    break;
                case BalloonsVariation.Letter:
                    builder = new WordsWithLetterQuestionBuilder(nPacks, nPacksPerRound:1, forceUnseparatedLetters:true, nCorrect:nCorrect, nWrong:nWrong);
                    break;  
                case BalloonsVariation.Counting:
                    builder = new OrderedWordsQuestionBuilder(Database.WordDataCategory.Number); 
                    break;
            }

            return builder;
        }

        public MiniGameLearnRules SetupLearnRules()
        {
            var rules = new MiniGameLearnRules();
            // example: a.minigameVoteSkewOffset = 1f;
            return rules;
        }


        #endregion
    }
}
