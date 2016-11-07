using System.Collections.Generic;


namespace EA4S
{
    public class WordsWithLetterQuestionBuilder : IQuestionBuilder
    {
        private int nPacks = 10;
        private int nCorrect = 3;
        private int nWrong = 3;

        public WordsWithLetterQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
        }

        public int GetQuestionPackCount()
        {
            return nPacks;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;
            var db = AppManager.Instance.DB;

            Db.LetterData question = db.GetAllLetterData().RandomSelectOne();
            var correctAnswers = teacher.wordHelper.GetWordsWithLetter(question.Id).RandomSelect(nCorrect);
            var wrongAnswers = teacher.wordHelper.GetWordsNotIn(correctAnswers.ToArray()).RandomSelect(nWrong);

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }

    }
}