using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class LettersInWordQuestionBuilder : IQuestionBuilder
    {
        // focus: Letters & Words
        // pack history filter: TODO
        // journey: TODO

        private int nPacks;
        private int nCorrect;
        private bool useAllCorrectLetters;
        private int nWrong;
        private Db.WordDataCategory category;
        private bool drawingNeeded;

        public LettersInWordQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, 
            bool useAllCorrectLetters = false, Db.WordDataCategory category = Db.WordDataCategory.None, bool drawingNeeded = false)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.useAllCorrectLetters = useAllCorrectLetters;
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

            // Get the word
            var wordFilters = new WordFilters();
            var question = teacher.wordHelper.GetWordsByCategory(category, wordFilters).RandomSelectOne();

            // Get letters of that word
            var wordLetters = teacher.wordHelper.GetLettersInWord(question);

            var correctAnswers = new List<Db.LetterData>(wordLetters);
            if (!useAllCorrectLetters) correctAnswers = wordLetters.RandomSelect(nCorrect);

            var wrongAnswers = teacher.wordHelper.GetLettersNotIn(new LetterFilters(), wordLetters.ToArray()).RandomSelect(nWrong);

            if (ConfigAI.verboseTeacher)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + question;
                debugString += "\nCorrect Answers: " + correctAnswers.Count;
                foreach (var l in correctAnswers) debugString += " " + l;
                debugString += "\nWrong Answers: " + wrongAnswers.Count;
                foreach (var l in wrongAnswers) debugString += " " + l;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }


    }
}