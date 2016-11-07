using System.Collections.Generic;


namespace EA4S
{
    public class LettersInWordQuestionBuilder : IQuestionBuilder
    {
        // Configuration
        private int nPacks;
        private int nWrong;
        private Db.WordDataCategory category;

        public LettersInWordQuestionBuilder(int nPacks, int nWrong = 0, Db.WordDataCategory category = Db.WordDataCategory.None)
        {
            this.nPacks = nPacks;
            this.nWrong = nWrong;
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

            var correctAnswers = teacher.wordHelper.GetLettersInWord(question);
            var wrongAnswers = teacher.wordHelper.GetLettersNotIn(correctAnswers.ToArray()).RandomSelect(nWrong);

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }

        public IQuestionPack CreateQuestionPack()
        {
            throw new System.Exception("DEPRECATED");
        }

    }
}