using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class AlphabetQuestionBuilder : IQuestionBuilder
    {
        // focus: Letters
        // pack history filter: forced - only 1 pack
        // journey: enabled

        private QuestionBuilderParameters parameters;

        public AlphabetQuestionBuilder(QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();

            this.parameters = parameters;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            packs.Add(CreateAlphabetQuestionPackData());
            return packs;
        }

        public QuestionPackData CreateAlphabetQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;

            // Fully ordered alphabet, only 1 pack
            var alphabetLetters = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetAllLetters(parameters.letterFilters),
                new SelectionParameters(parameters.correctSeverity, getAllData:true, useJourney: parameters.useJourneyForCorrect)
                );

            alphabetLetters.Sort((x, y) =>
                {
                    return x.ToString().CompareTo(y.ToString());
                }
            );

            if (ConfigAI.verboseTeacher)
            {
                string debugString = "Letters: " + alphabetLetters.Count;
                foreach (var l in alphabetLetters) debugString += " " + l;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.CreateFromCorrect(null, alphabetLetters);
        }

    }
}