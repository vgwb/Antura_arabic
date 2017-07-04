using System.Collections.Generic;
using EA4S.Core;

namespace EA4S.Teacher
{
    /// <summary>
    /// Selects words in a given order
    /// * Correct answers: Ordered words
    /// </summary>
    public class OrderedWordsQuestionBuilder : IQuestionBuilder
    {
        // focus: Words
        // pack history filter: only 1 pack
        // journey: enabled

        private Database.WordDataCategory category;
        private QuestionBuilderParameters parameters;

        public QuestionBuilderParameters Parameters
        {
            get { return this.parameters; }
        }

        public OrderedWordsQuestionBuilder(Database.WordDataCategory category, QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();

            this.category = category;
            this.parameters = parameters;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            packs.Add(CreateSingleQuestionPackData());
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData()
        {
            var teacher = AppManager.I.Teacher;
            var vocabularyHelper = AppManager.I.VocabularyHelper;

            // Ordered words
            var words = teacher.VocabularyAi.SelectData(
                 () => vocabularyHelper.GetWordsByCategory(category, parameters.wordFilters),
                 new SelectionParameters(parameters.correctSeverity, getMaxData:true, useJourney:parameters.useJourneyForCorrect) 
               );

            // sort by id
            words.Sort((x, y) =>
                {
                    return x.Id.CompareTo(y.Id);
                }
            );

            if (ConfigAI.verboseQuestionPacks)
            {
                string debugString = "Words: " + words.Count;
                foreach (var w in words) debugString += " " + w;
                ConfigAI.AppendToTeacherReport(debugString);
            }

            return QuestionPackData.CreateFromCorrect(null, words);
        }

    }
}