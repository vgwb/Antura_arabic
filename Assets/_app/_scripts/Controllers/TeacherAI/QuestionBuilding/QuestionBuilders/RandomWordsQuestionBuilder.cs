using System.Collections.Generic;

namespace EA4S
{
    public class RandomWordsQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private bool firstCorrectIsQuestion;

        private Db.WordDataCategory wordCategory = Db.WordDataCategory.BodyPart;

        public RandomWordsQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, bool firstCorrectIsQuestion = false)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.firstCorrectIsQuestion = firstCorrectIsQuestion;
        }

        public int GetQuestionPackCount()
        {
            return nPacks;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;

            var correctAnswers = teacher.wordHelper.GetWordsByCategory(wordCategory).RandomSelect(nCorrect);
            var question = firstCorrectIsQuestion ? correctAnswers[0] : null;
            var wrongAnswers = teacher.wordHelper.GetWordsNotIn(correctAnswers.ToArray()).RandomSelect(nWrong);

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }

    }
}