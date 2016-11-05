using System.Collections.Generic;

/*
namespace EA4S.MiniGameConfiguration
{
    public class FastCrowd_letter_MiniGameConfigurationRules : IMiniGameConfigurationRules
    {
        // Configuration
        private int packsCount = 10;

        private int wrongToSelect = 3;

        public int GetQuestionPackCount()
        {
            return packsCount;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;
            var db = AppManager.Instance.DB;

            Db.LetterData question = db.GetAllLetterData().RandomSelectOne();
            var wrongAnswers = teacher.wordHelper.GetLettersNotIn(question).RandomSelect(wrongToSelect);

            return QuestionPackData.CreateFromWrong(question, wrongAnswers);
        }

        public IQuestionPack CreateQuestionPack()
        {
            throw new System.Exception("DEPRECATED");
        }

    }
}*/