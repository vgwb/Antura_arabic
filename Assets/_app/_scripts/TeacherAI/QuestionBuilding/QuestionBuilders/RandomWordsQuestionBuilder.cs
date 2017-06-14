using System.Collections.Generic;
using EA4S.Core;

namespace EA4S.Teacher
{
    /// <summary>
    /// Selects words at random
    /// * Question: The word to find
    /// * Correct answers: The correct word
    /// * Wrong answers: Wrong words
    /// </summary>
    public class RandomWordsQuestionBuilder : IQuestionBuilder
    {
        // Focus: Words
        // pack history filter: parameterized
        // journey: enabled

        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private bool firstCorrectIsQuestion;
        private Database.WordDataCategory category;
        private QuestionBuilderParameters parameters;

        public QuestionBuilderParameters Parameters
        {
            get { return this.parameters; }
        }
        public RandomWordsQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, 
            bool firstCorrectIsQuestion = false, Database.WordDataCategory category = Database.WordDataCategory.None, QuestionBuilderParameters parameters = null)
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
            var teacher = AppManager.Instance.Teacher;
            var vocabularyHelper = AppManager.Instance.VocabularyHelper;

            var correctWords = teacher.VocabularyAi.SelectData(
                () => vocabularyHelper.GetWordsByCategory(category, parameters.wordFilters), 
                    new SelectionParameters(parameters.correctSeverity, nCorrect, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds:previousPacksIDs)
                );

            var wrongWords = teacher.VocabularyAi.SelectData(
                () => vocabularyHelper.GetWordsNotIn(parameters.wordFilters, correctWords.ToArray()), 
                    new SelectionParameters(parameters.wrongSeverity, nWrong, useJourney: parameters.useJourneyForWrong,
                        packListHistory: parameters.wrongChoicesHistory, filteringIds: previousPacksIDs,
                        journeyFilter: SelectionParameters.JourneyFilter.UpToFullCurrentStage)
                );

            var question = firstCorrectIsQuestion ? correctWords[0] : null;

            if (ConfigAI.verboseQuestionPacks)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nCorrect Words: " + correctWords.Count;
                foreach (var l in correctWords) debugString += " " + l;
                debugString += "\nWrong Words: " + wrongWords.Count;
                foreach (var l in wrongWords) debugString += " " + l;
                ConfigAI.AppendToTeacherReport(debugString);
            }

            return QuestionPackData.Create(question, correctWords, wrongWords);
        }

    }
}