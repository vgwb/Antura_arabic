using System;
using UnityEngine;

namespace EA4S.IdentifyLetter
{
    public class LetterShapeConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        //public IQuestionProvider RandomQuestions { get; set; }
        public float Difficulty { get; set; }
        public int SimultaneosQuestions { get; set; }
        public int Rounds { get; set; }

        /////////////////
        // Singleton Pattern
        static LetterShapeConfiguration instance;
        public static LetterShapeConfiguration Instance {
            get {
                if (instance == null)
                    instance = new LetterShapeConfiguration();
                return instance;
            }
        }

        public string Description { get { return "Missing description"; } private set { } }

        public IQuestionProvider Questions { get; set; }

        /////////////////

        private LetterShapeConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new SampleQuestionProvider();
            Context = new SampleGameContext();
            SimultaneosQuestions = 2;
            Rounds = 2;
        }

        public IQuestionBuilder SetupBuilder()
        {
            throw new NotImplementedException();
        }

        public MiniGameLearnRules SetupLearnRules()
        {
            var rules = new MiniGameLearnRules();
            // example: a.minigameVoteSkewOffset = 1f;
            return rules;
        }
    }
}
