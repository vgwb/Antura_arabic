using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class RandomLettersQuestionBuilder : IQuestionBuilder
    {
        // focus: Letters
        // pack history filter: parameterized
        // journey: enabled

        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private bool firstCorrectIsQuestion;
        private QuestionBuilderParameters parameters;

        public RandomLettersQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, bool firstCorrectIsQuestion = false, QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();

            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.firstCorrectIsQuestion = firstCorrectIsQuestion;
            this.parameters = parameters;
        }

        private List<string> previousPacksIDs = new List<string>();

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            previousPacksIDs.Clear();

            List<QuestionPackData> packs = new List<QuestionPackData>();
            for (int pack_i = 0; pack_i < nPacks; pack_i++)
            {
                var pack = CreateSingleQuestionPackData();
                packs.Add(pack);
            }
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData()
        {
            var teacher = AppManager.I.Teacher;

            var correctLetters = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetAllLetters(parameters.letterFilters),
                    new SelectionParameters(parameters.correctSeverity, nCorrect, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs)
                );

            var wrongLetters = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetLettersNotIn(parameters.letterFilters, correctLetters.ToArray()),
                    new SelectionParameters(parameters.wrongSeverity, nWrong, useJourney: parameters.useJourneyForWrong,
                     packListHistory: parameters.wrongChoicesHistory, filteringIds: previousPacksIDs)
                );

            var question = firstCorrectIsQuestion ? correctLetters[0] : null;

            if (ConfigAI.verboseTeacher)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nCorrect Letters: " + correctLetters.Count;
                foreach (var l in correctLetters) debugString += " " + l;
                debugString += "\nWrong Letters: " + wrongLetters.Count;
                foreach (var l in wrongLetters) debugString += " " + l;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.Create(question, correctLetters, wrongLetters);
        }

    }
}