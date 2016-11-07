using System.Collections.Generic;

namespace EA4S
{

    public class RandomLettersQuestionBuilder : IQuestionBuilder
    {
        // Configuration
        private int packsCount = 4;
        private int nWrong = 7;

        public RandomLettersQuestionBuilder(int packsCount, int nCorrect, int nWrong)
        {
            this.packsCount = packsCount;
            this.nWrong = nWrong;
        }

        private Db.WordDataCategory wordDataCategory = Db.WordDataCategory.BodyPart;

        public int GetQuestionPackCount()
        {
            return packsCount;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;

            var question = teacher.wordHelper.GetWordsByCategory(wordDataCategory).RandomSelectOne();
            var correctAnswers = teacher.wordHelper.GetLettersInWord(question.GetId());
            var wrongAnswers = teacher.wordHelper.GetLettersNotIn(correctAnswers.ToArray()).RandomSelect(nWrong);

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }


        public IQuestionPack CreateQuestionPack()
        {
            throw new System.Exception("DEPRECATED");
        }

    }
}