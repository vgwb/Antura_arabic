using System.Collections.Generic;

namespace EA4S
{
    public class RandomWordsQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private bool firstCorrectIsQuestion;
        private Db.WordDataCategory category;
        private bool drawingNeeded;

        public RandomWordsQuestionBuilder(int nPacks, int nCorrect = 1,  int nWrong = 0, bool firstCorrectIsQuestion = false, Db.WordDataCategory category = Db.WordDataCategory.None, bool drawingNeeded = false)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.firstCorrectIsQuestion = firstCorrectIsQuestion;
            this.category = category;
            this.drawingNeeded = drawingNeeded;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            for (int pack_i = 0; pack_i < nPacks; pack_i++)
            {
                packs.Add(CreateSingleQuestionPackData());
            }
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;

            var correctAnswers = teacher.wordHelper.GetWordsByCategory(category, drawingNeeded).RandomSelect(nCorrect);
            var question = firstCorrectIsQuestion ? correctAnswers[0] : null;
            var wrongAnswers = teacher.wordHelper.GetWordsNotIn(correctAnswers.ToArray()).RandomSelect(nWrong);

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }

    }
}