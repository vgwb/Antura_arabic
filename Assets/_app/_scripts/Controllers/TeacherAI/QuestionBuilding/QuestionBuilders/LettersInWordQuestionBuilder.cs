using System.Collections.Generic;


namespace EA4S
{
    public class LettersInWordQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;
        private int nCorrect;
        private bool useAllCorrects;
        private int nWrong;
        private Db.WordDataCategory category;

        public LettersInWordQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, bool useAllCorrects = false, Db.WordDataCategory category = Db.WordDataCategory.None)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.useAllCorrects = useAllCorrects;
            this.category = category;
        }


        public int GetQuestionPackCount()
        {
            return nPacks;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;
            var db = AppManager.Instance.DB;

            Db.WordData question = null;
            if (category != Db.WordDataCategory.None) question = teacher.wordHelper.GetWordsByCategory(category).RandomSelectOne();
            else question = db.GetAllWordData().RandomSelectOne();

            var wordLetters = teacher.wordHelper.GetLettersInWord(question);

            var correctAnswers = new List<Db.LetterData>(wordLetters);
            if (!this.useAllCorrects) correctAnswers = wordLetters.RandomSelect(nCorrect);

            var wrongAnswers = teacher.wordHelper.GetLettersNotIn(wordLetters.ToArray()).RandomSelect(nWrong);

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }

    }
}