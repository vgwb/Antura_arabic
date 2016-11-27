using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{

    public class RandomWordsQuestionBuilder : IQuestionBuilder
    {
        // Focus: Words
        // pack history filter: parameterized
        // journey: enabled

        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private bool firstCorrectIsQuestion;
        private Db.WordDataCategory category;
        private QuestionBuilderParameters parameters;

        public RandomWordsQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, 
            bool firstCorrectIsQuestion = false, Db.WordDataCategory category = Db.WordDataCategory.None, QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();

            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.firstCorrectIsQuestion = firstCorrectIsQuestion;
            this.category = category;
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

            var correctWords = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetWordsByCategory(category, parameters.wordFilters), 
                    new SelectionParameters(parameters.correctSeverity, nCorrect, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds:previousPacksIDs)
                );

            var wrongWords = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetWordsNotIn(parameters.wordFilters, correctWords.ToArray()), 
                    new SelectionParameters(parameters.wrongSeverity, nWrong, useJourney: parameters.useJourneyForWrong,
                        packListHistory: parameters.wrongChoicesHistory, filteringIds: previousPacksIDs)
                );

            var question = firstCorrectIsQuestion ? correctWords[0] : null;

            if (ConfigAI.verboseTeacher)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nCorrect Words: " + correctWords.Count;
                foreach (var l in correctWords) debugString += " " + l;
                debugString += "\nWrong Words: " + wrongWords.Count;
                foreach (var l in wrongWords) debugString += " " + l;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.Create(question, correctWords, wrongWords);
        }

    }
}