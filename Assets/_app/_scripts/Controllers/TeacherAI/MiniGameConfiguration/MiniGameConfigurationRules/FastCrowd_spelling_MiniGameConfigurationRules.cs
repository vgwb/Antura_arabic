using System.Collections.Generic;
/*
namespace EA4S.MiniGameConfiguration
{
    // @note: this is almost the same as Egg!!!! may merge them :D
    public class FastCrowd_spelling_MiniGameConfigurationRules : IMiniGameConfigurationRules
    {
        // Configuration
        private int packsCount = 10;

        private int nWrongToSelect = 7;
        private Db.WordDataCategory wordDataCategory = Db.WordDataCategory.BodyPart;

        public int GetQuestionPackCount()
        {
            return packsCount;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;
            var db = AppManager.Instance.DB;

            var question = teacher.wordHelper.GetWordsByCategory(wordDataCategory).RandomSelectOne();
            var correctAnswers = teacher.wordHelper.GetLettersInWord(question.GetId());
            var wrongAnswers = teacher.wordHelper.GetLettersNotIn(correctAnswers.ToArray()).RandomSelect(nWrongToSelect);

            return QuestionPackData.CreateFromWrong(question, wrongAnswers);
        }

        public IQuestionPack CreateQuestionPack()
        {
            throw new System.Exception("DEPRECATED");
        }

    }
}*/