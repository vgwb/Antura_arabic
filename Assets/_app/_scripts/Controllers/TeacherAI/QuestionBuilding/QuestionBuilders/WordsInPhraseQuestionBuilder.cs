using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class WordsInPhraseQuestionBuilder : IQuestionBuilder
    {
        // Focus: Words & Phrases
        // pack history filter: enabled
        // journey: enabled

        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private bool useAllCorrectWords;
        private bool usePhraseAnswersIfFound;
        private PackListHistory questionHistory;

        public WordsInPhraseQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0,
            bool useAllCorrectWords = false, bool usePhraseAnswersIfFound = false,
            PackListHistory questionHistory = PackListHistory.NoFilter)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.useAllCorrectWords = useAllCorrectWords;
            this.usePhraseAnswersIfFound = usePhraseAnswersIfFound;
            this.questionHistory = questionHistory;
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
            var teacher = AppManager.Instance.Teacher;

            // Get a phrase
            int nToUse = 1;
            var usablePhrases = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetPhrasesByCategory(Db.PhraseDataCategory.Question),
                    new SelectionParameters(SelectionSeverity.AsManyAsPossible, nToUse,
                        packListHistory: questionHistory, filteringIds: previousPacksIDs));
            var question = usablePhrases[0];

            // Get words related to the phrase
            var correctWords = new List<Db.WordData>();
            List<Db.WordData> relatedWords = null;
            if (usePhraseAnswersIfFound && question.Answers.Length > 0)
            {
                relatedWords = teacher.wordHelper.GetAnswersToPhrase(question);
            }
            else
            {
                relatedWords = teacher.wordHelper.GetWordsInPhrase(question);
            }

            correctWords.AddRange(relatedWords);
            if (!useAllCorrectWords) correctWords = correctWords.RandomSelect(nCorrect);

            var wrongWords = teacher.wordHelper.GetWordsNotIn(new WordFilters(), relatedWords.ToArray()).RandomSelect(nWrong);

            if (ConfigAI.verboseTeacher)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + question;
                debugString += "\nCorrect Answers: " + correctWords.Count;
                foreach (var l in correctWords) debugString += " " + l;
                debugString += "\nWrong Answers: " + wrongWords.Count;
                foreach (var l in wrongWords) debugString += " " + l;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.Create(question, correctWords, wrongWords);
        }


    }
}