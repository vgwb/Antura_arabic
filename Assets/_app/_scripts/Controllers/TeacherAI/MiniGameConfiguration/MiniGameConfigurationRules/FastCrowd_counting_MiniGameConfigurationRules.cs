using System.Collections.Generic;

namespace EA4S.MiniGameConfiguration
{
    public class FastCrowd_counting_MiniGameConfigurationRules : IMiniGameConfigurationRules
    {
        // Configuration
        private int packsCount = 10;
        private Db.WordDataCategory wordCategory = Db.WordDataCategory.BodyPart;
        private int wrongEntriesNumber = 10;

        public int GetQuestionPackCount()
        {
            return packsCount;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;

            var question = teacher.wordHelper.GetWordsByCategory(wordCategory).RandomSelectOne();
            var correctAnswers = new List<Db.WordData>() { question };
            var wrongAnswers = teacher.wordHelper.GetWordsNotIn(correctAnswers.ToArray()).RandomSelect(wrongEntriesNumber);

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }

        public IQuestionPack CreateQuestionPack()
        {
            throw new System.Exception("DEPRECATED");
        }

    }
}