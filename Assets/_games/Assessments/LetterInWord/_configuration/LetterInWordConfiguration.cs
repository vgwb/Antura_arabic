using System;
using UnityEngine;

namespace EA4S.IdentifyLetter
{
    public class LetterInWordConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        //public IQuestionProvider RandomQuestions { get; set; }
        public float Difficulty { get; set; }
        public int SimultaneosQuestions { get; set; }
        public int Rounds { get; set; }

        /////////////////
        // Singleton Pattern
        static LetterInWordConfiguration instance;
        public static LetterInWordConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new LetterInWordConfiguration();
                return instance;
            }
        }

        public string Description { get { return "Missing description"; } private set { } }

        public IQuestionProvider Questions { get; set; }

        /////////////////

        private LetterInWordConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new SampleQuestionProvider();
            Context = new SampleGameContext();
            SimultaneosQuestions = 2;
            Rounds = 2;
        }

        IQuestionBuilder builder = null;

        public IQuestionBuilder SetupBuilder()
        {
            int nCorrect = Difficulty > 0.6f ? 2 : 1;
            int nWrong = Difficulty > 0.3f ? 1 : 0;

            builder = new LettersInWordQuestionBuilder(Rounds, nCorrect, nWrong);
            return builder;
        }

        public MiniGameLearnRules SetupLearnRules()
        {
            var rules = new MiniGameLearnRules();
            // example: a.minigameVoteSkewOffset = 1f;
            return rules;
        }
    }
}