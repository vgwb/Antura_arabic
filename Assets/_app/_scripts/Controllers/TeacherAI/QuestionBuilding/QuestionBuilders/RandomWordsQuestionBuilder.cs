using EA4S.Teacher;
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

            var correctWords = teacher.wordAI.SelectWords(() => teacher.wordHelper.GetWordsByCategory(category, drawingNeeded), new SelectionParameters(SelectionSeverity.AsManyAsPossible, nCorrect));
            correctWords = correctWords.RandomSelect(nCorrect);

            var wrongWords = teacher.wordAI.SelectWords(() => teacher.wordHelper.GetWordsNotIn(correctWords.ToArray()), new SelectionParameters(SelectionSeverity.AsManyAsPossible, nCorrect));
            wrongWords = wrongWords.RandomSelect(nWrong);

            var question = firstCorrectIsQuestion ? correctWords[0] : null;

            return QuestionPackData.Create(question, correctWords, wrongWords);
        }

    }
}