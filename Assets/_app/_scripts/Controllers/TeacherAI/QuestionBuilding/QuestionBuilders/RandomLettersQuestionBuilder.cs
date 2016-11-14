using EA4S.Teacher;
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

            // Parameters for builder's use of data
            var sParameters = new SelectionParameters(SelectionSeverity.AsManyAsPossible);

            var correctLetters = teacher.wordAI.SelectLetters(() => teacher.wordHelper.GetAllLetters(), new SelectionParameters(SelectionSeverity.AsManyAsPossible, nCorrect));
            correctLetters = correctLetters.RandomSelect(nCorrect);

            var wrongLetters = teacher.wordAI.SelectLetters(() => teacher.wordHelper.GetLettersNotIn(correctLetters.ToArray()), new SelectionParameters(SelectionSeverity.AsManyAsPossible, nWrong, true));
            wrongLetters = wrongLetters.RandomSelect(nWrong);

            var question = firstCorrectIsQuestion ? correctLetters[0] : null;

            // Debug
            {
                string debugString = "Correct Letters: " + correctLetters.Count;
                foreach (var l in correctLetters) debugString += " " + l;
                debugString += "\nWrong Letters: " + wrongLetters.Count;
                foreach (var l in wrongLetters) debugString += " " + l;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.Create(question, correctLetters, wrongLetters);
        }

    }
}