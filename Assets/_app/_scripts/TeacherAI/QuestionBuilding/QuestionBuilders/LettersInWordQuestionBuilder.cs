using System.Collections.Generic;
using EA4S.Database;

namespace EA4S.Teacher
{
    /// <summary>
    /// Selects letters inside a word
    /// * Question: Word
    /// * Correct answers: Letters contained in the word
    /// * Wrong answers: Letters not contained in the word
    /// </summary>
    public class LettersInWordQuestionBuilder : IQuestionBuilder
    {
        // focus: Letters & Words
        // pack history filter: enabled
        // journey: enabled

        private int nPacks;
        private int nCorrect;
        private bool useAllCorrectLetters;
        private int nWrong;
        private int maximumWordLength;
        private bool packsUsedTogether;
        private WordDataCategory category;
        private QuestionBuilderParameters parameters;

        public QuestionBuilderParameters Parameters
        {
            get { return this.parameters; }
        }

        public LettersInWordQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, 
            bool useAllCorrectLetters = false, Database.WordDataCategory category = Database.WordDataCategory.None,
            int maximumWordLength = 20,
            bool packsUsedTogether = false,
            QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.useAllCorrectLetters = useAllCorrectLetters;
            this.category = category;
            this.maximumWordLength = maximumWordLength;
            this.packsUsedTogether = packsUsedTogether;
            this.parameters = parameters;
        }

        private List<string> previousPacksIDs_words = new List<string>();
        private List<string> previousPacksIDs_letters = new List<string>();

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            previousPacksIDs_words.Clear();
            previousPacksIDs_letters.Clear();
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

            // Choose a single eligible word
            var usableWords = teacher.VocabularyAi.SelectData(
                () => FindEligibleWords(maxWordLength: maximumWordLength),
                    new SelectionParameters(parameters.correctSeverity, 1, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_words));
            var question = usableWords[0];
            //UnityEngine.Debug.LogWarning("Chosen word: " + question);

            // Get letters of that word
            var wordLetters = vocabularyHelper.GetLettersInWord(question);
            //UnityEngine.Debug.LogWarning("Found letters: " + wordLetters.Count);

            bool useJourneyForLetters = parameters.useJourneyForCorrect;
            // @note: we force journey in this case to be off so that all letters can be found
            // @note: we also force the journey if the packs must be used together, as the data filters for journey clash with the new filter
            if (useAllCorrectLetters || packsUsedTogether) useJourneyForLetters = false;

            // Get some letters (from that word)
            var correctAnswers = teacher.VocabularyAi.SelectData(
                () => FindEligibleLetters(question, wordLetters),
                 new SelectionParameters(parameters.correctSeverity, nCorrect, getMaxData:useAllCorrectLetters, 
                 useJourney: useJourneyForLetters, filteringIds: previousPacksIDs_letters));

            // Get some wrong letters (not from that word)
            var wrongAnswers = teacher.VocabularyAi.SelectData(
                () => vocabularyHelper.GetLettersNotIn(parameters.letterFilters, wordLetters.ToArray()),
                    new SelectionParameters(parameters.wrongSeverity, nWrong, useJourney: parameters.useJourneyForWrong));

            if (ConfigAI.verboseQuestionPacks)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + question;
                debugString += "\nCorrect Answers: " + correctAnswers.Count;
                foreach (var l in correctAnswers) debugString += " " + l;
                debugString += "\nWrong Answers: " + wrongAnswers.Count;
                foreach (var l in wrongAnswers) debugString += " " + l;
                ConfigAI.AppendToTeacherReport(debugString);
            }

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }

        public List<WordData> FindEligibleWords(int maxWordLength)
        {
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            List<WordData> eligibleWords = new List<WordData>();
            foreach(var word in vocabularyHelper.GetWordsByCategory(category, parameters.wordFilters))
            {
                // Check max length
                if (word.Letters.Length > maxWordLength) continue;

                // Avoid using words that contain previously chosen letters
                if (packsUsedTogether && vocabularyHelper.WordContainsAnyLetter(word, previousPacksIDs_letters)) continue;

                // Avoid using words that have ONLY letters that appeared in previous words
                if (packsUsedTogether && vocabularyHelper.WordHasAllLettersInCommonWith(word, previousPacksIDs_words)) continue;

                eligibleWords.Add(word);
            }
            //UnityEngine.Debug.Log("Eligible words: " + eligibleWords.Count + " out of " + teacher.VocabularyHelper.GetWordsByCategory(category, parameters.wordFilters).Count);
            return eligibleWords;
        }

        public List<LetterData> FindEligibleLetters(WordData selectedWord, List<LetterData> wordLetters)
        {
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            List<LetterData> eligibleLetters = new List<LetterData>();
            var bad_words = previousPacksIDs_words;
            bad_words.Remove(selectedWord.Id);
            foreach (var letter in wordLetters)
            {
                // Avoid using letters that appeared in previous words
                if (packsUsedTogether && vocabularyHelper.LetterContainedInAnyWord(letter, bad_words)) continue;

                eligibleLetters.Add(letter);
            }
            return eligibleLetters;
        }


    }
}