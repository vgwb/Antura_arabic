using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class PhraseQuestionsQuestionBuilder : IQuestionBuilder
    {
        // Focus: Phrases
        // pack history filter: enabled
        // journey: enabled

        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private QuestionBuilderParameters parameters;

        public PhraseQuestionsQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0,
            QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();

            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.parameters = parameters;
        }

        private List<string> previousPacksIDs = new List<string>();

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            previousPacksIDs.Clear();
            List<QuestionPackData> packs = new List<QuestionPackData>();
            for (int pack_i = 0; pack_i < nPacks; pack_i++)
            {
                packs.Add(CreateSingleQuestionPackData());
            }
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData()
        {
            var teacher = AppManager.I.Teacher;

            // Get a question phrase at random
            int nToUse = 1;
            var usablePhrases = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetPhrasesByCategory(Db.PhraseDataCategory.Question, parameters.wordFilters, parameters.phraseFilters),
                    new SelectionParameters(parameters.correctSeverity, nToUse, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs));
            var question = usablePhrases[0];

            // Get the linked reply phrase
            var reply = teacher.wordHelper.GetLinkedPhraseOf(question);

            var correctAnswers = new List<Db.PhraseData>();
            correctAnswers.Add(reply);

            // Get random wrong phrases
            var wrongPhrases = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetPhrasesNotIn(parameters.wordFilters, parameters.phraseFilters, question, reply),
                    new SelectionParameters(parameters.correctSeverity, nWrong, useJourney: parameters.useJourneyForWrong,
                        packListHistory: parameters.wrongChoicesHistory, filteringIds: previousPacksIDs));

            if (ConfigAI.verboseTeacher)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + question;
                debugString += "\nCorrect Answers: " + correctAnswers.Count;
                foreach (var l in correctAnswers) debugString += " " + l;
                debugString += "\nWrong Answers: " + wrongPhrases.Count;
                foreach (var l in wrongPhrases) debugString += " " + l;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.Create(question, correctAnswers, wrongPhrases);
        }


    }
}