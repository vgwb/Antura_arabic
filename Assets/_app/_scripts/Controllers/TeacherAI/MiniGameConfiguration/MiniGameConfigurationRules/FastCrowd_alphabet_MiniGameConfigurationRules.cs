using EA4S.API;
using System.Collections.Generic;
/*
namespace EA4S.MiniGameConfiguration
{
    public class FastCrowd_alphabet_MiniGameConfigurationRules : IMiniGameConfigurationRules
    {
        // Configuration
        private int packsCount = 1;
         
        public int GetQuestionPackCount()
        {
            return packsCount;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            var db = AppManager.Instance.DB;

            // Fully ordered alphabet (@todo: check that it works!)
            var correctAnswers = db.GetAllLetterData();
            correctAnswers.Sort((x, y) =>
            {
                return x.ToString().CompareTo(y.ToString());
            }
            );

            return QuestionPackData.CreateFromCorrect(null, correctAnswers);
        }

        public IQuestionPack CreateQuestionPack()
        {
            throw new System.Exception("DEPRECATED");
        }

    }
}*/