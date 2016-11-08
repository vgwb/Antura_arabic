using System.Collections.Generic;


namespace EA4S
{
    public class CommonLettersInWordQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private int nWords;

        public CommonLettersInWordQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, int nWords = 1)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.nWords = nWords;
        }

        public int GetQuestionPackCount()
        {
            return nPacks;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;
            var db = AppManager.Instance.DB;

            var commonLetters = db.GetAllLetterData().RandomSelect(nCorrect);
            var words = teacher.wordHelper.GetWordsWithLetters(commonLetters.ConvertAll(x => x.Id).ToArray()).RandomSelect(nWords);
            // @todo: make sure that two words can be found every time! maybe filter letters only by those that appear in multiple words?

            var nonCommonLetters = teacher.wordHelper.GetLettersNotInWords(words.ToArray()).RandomSelect(nWrong);

            return QuestionPackData.Create(words, commonLetters, nonCommonLetters);
        }

    }
}