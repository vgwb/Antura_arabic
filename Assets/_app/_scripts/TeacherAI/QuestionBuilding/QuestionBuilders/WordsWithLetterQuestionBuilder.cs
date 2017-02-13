using System.Collections.Generic;

namespace EA4S.Teacher
{
    /// <summary>
    /// Selects words given a letter
    /// * Question: The letter
    /// * Correct answers: Words with the letter
    /// * Wrong answers: Words without the letter
    /// </summary>
    public class WordsWithLetterQuestionBuilder : IQuestionBuilder
    {
        // focus: Words & Letters
        // pack history filter: enabled
        // journey: enabled

        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private bool packsUsedTogether;
        private QuestionBuilderParameters parameters;

        public QuestionBuilderParameters Parameters
        {
            get { return this.parameters; }
        }

        public WordsWithLetterQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0,
              bool packsUsedTogether = false, 
              QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.packsUsedTogether = packsUsedTogether;
            this.parameters = parameters;
        }

        private List<string> previousPacksIDs_letters = new List<string>();
        private List<string> previousPacksIDs_words = new List<string>();

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            previousPacksIDs_letters.Clear();
            previousPacksIDs_words.Clear();
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
            var vocabularyHelper = AppManager.I.VocabularyHelper;

            bool useJourneyForLetters = parameters.useJourneyForCorrect;
            // @note: we also force the journey if the packs must be used together, as the data filters for journey clash with the new filter
            if (packsUsedTogether) useJourneyForLetters = false;

            // Get a letter
            var usableLetters = teacher.VocabularyAi.SelectData(
              () => FindEligibleLetters(atLeastNWords: nCorrect),
                new SelectionParameters(parameters.correctSeverity, 1, useJourney: useJourneyForLetters,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_letters));
            var commonLetter = usableLetters[0];

            // Get words with the letter 
            // (but without the previous letters)
            var correctWords = teacher.VocabularyAi.SelectData(
                () => FindEligibleWords(commonLetter),
                    new SelectionParameters(parameters.correctSeverity, nCorrect, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_words));

            // Get words without the letter
            var wrongWords = teacher.VocabularyAi.SelectData(
                () => vocabularyHelper.GetWordsNotIn(parameters.wordFilters, correctWords.ToArray()),
                    new SelectionParameters(parameters.wrongSeverity, nWrong, useJourney: parameters.useJourneyForWrong));

            var pack = QuestionPackData.Create(commonLetter, correctWords, wrongWords);

            if (ConfigAI.verboseQuestionPacks)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + commonLetter;
                debugString += "\nCorrect Answers: " + correctWords.Count;
                foreach (var l in correctWords) debugString += " " + l;
                debugString += "\nWrong Answers: " + wrongWords.Count;
                foreach (var l in wrongWords) debugString += " " + l;
                ConfigAI.AppendToTeacherReport(debugString);
            }

            return pack;
        }

        private List<Database.LetterData> FindEligibleLetters(int atLeastNWords)
        {
            List<Database.LetterData> eligibleLetters = new List<Database.LetterData>();
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            var allLetters = vocabularyHelper.GetAllLetters(parameters.letterFilters);
            foreach(var letter in allLetters)
            {
                // Check number of words
                int nWords = vocabularyHelper.GetWordsWithLetter(parameters.wordFilters, letter.Id).Count;
                if (nWords < atLeastNWords) continue;

                // Avoid using letters that appeared in previous words
                if (packsUsedTogether && vocabularyHelper.AnyWordContainsLetter(letter, previousPacksIDs_words)) continue;

                eligibleLetters.Add(letter);
            }
            return eligibleLetters;
        }

        private List<Database.WordData> FindEligibleWords(Database.LetterData commonLetter)
        {
            List<Database.WordData> eligibleWords = new List<Database.WordData>();
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            var words = vocabularyHelper.GetWordsWithLetter(parameters.wordFilters, commonLetter.Id);
            foreach(var w in words)
            {
                // Not words that have one of the previous letters
                if (packsUsedTogether && vocabularyHelper.WordContainsAnyLetter(w, previousPacksIDs_letters)) continue;

                eligibleWords.Add(w);
            }
            return eligibleWords;
        }

    }
}