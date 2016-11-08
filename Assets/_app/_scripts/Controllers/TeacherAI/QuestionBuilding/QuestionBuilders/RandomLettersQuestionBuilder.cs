using System.Collections.Generic;

namespace EA4S
{

    public class RandomLettersQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private bool firstCorrectIsQuestion;

        public RandomLettersQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, bool firstCorrectIsQuestion = false)
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
            var db = AppManager.Instance.DB;

            var correctAnswers = teacher.wordHelper.GetAllRealLetters().RandomSelect(nCorrect);
            var question = firstCorrectIsQuestion ? correctAnswers[0] : null;
            var wrongAnswers = teacher.wordHelper.GetRealLettersNotIn(correctAnswers.ToArray()).RandomSelect(nWrong);

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }

    }
}