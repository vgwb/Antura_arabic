using System.Collections.Generic;
using EA4S.Database;
using EA4S.Helpers;

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

        private int nRounds;
        private int nPacksPerRound;
        private int nCorrect;
        private bool useAllCorrectLetters;
        private int nWrong;
        private int maximumWordLength;
        private bool packsUsedTogether;
        private WordDataCategory category;
        private bool forceUnseparatedLetters;
        private QuestionBuilderParameters parameters;

        public QuestionBuilderParameters Parameters
        {
            get { return this.parameters; }
        }

        public LettersInWordQuestionBuilder(
            int nRounds, int nPacksPerRound = 1, int nCorrect = 1, int nWrong = 0,
            bool useAllCorrectLetters = false, Database.WordDataCategory category = Database.WordDataCategory.None,
            int maximumWordLength = 20, bool forceUnseparatedLetters = false,
            QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();
            this.nRounds = nRounds;
            this.nPacksPerRound = nPacksPerRound;
            this.packsUsedTogether = nPacksPerRound > 1;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.useAllCorrectLetters = useAllCorrectLetters;
            this.category = category;
            this.maximumWordLength = maximumWordLength;
            this.forceUnseparatedLetters = forceUnseparatedLetters;
            this.parameters = parameters;
        }

        private List<string> previousPacksIDs_words = new List<string>();
        private List<string> previousPacksIDs_letters = new List<string>();

        private List<string> currentRoundIDs_letters = new List<string>();
        private List<string> currentRoundIDs_words = new List<string>();

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            // HACK: the game may need unseparated letters
            if (forceUnseparatedLetters) AppManager.I.VocabularyHelper.ForceUnseparatedLetters = true;

            previousPacksIDs_words.Clear();
            previousPacksIDs_letters.Clear();
            List<QuestionPackData> packs = new List<QuestionPackData>();
            for (int round_i = 0; round_i < nRounds; round_i++)
            {
                // At each round, we must make sure to not repeat some words / letters
                currentRoundIDs_letters.Clear();
                currentRoundIDs_words.Clear();

                for (int pack_i = 0; pack_i < nPacksPerRound; pack_i++)
                {
                    packs.Add(CreateSingleQuestionPackData(pack_i));
                }
            }
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData(int inRoundPackIndex)
        {
            var teacher = AppManager.I.Teacher;
            var vocabularyHelper = AppManager.I.VocabularyHelper;

            // Choose a single eligible word
            var usableWords = teacher.VocabularyAi.SelectData(
                () => FindEligibleWords(maxWordLength: maximumWordLength),
                    new SelectionParameters(parameters.correctSeverity, 1, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_words));
            var wordQuestion = usableWords[0];
            currentRoundIDs_words.Add(wordQuestion.Id);
            //UnityEngine.Debug.LogWarning("Chosen word: " + question);

            // Get letters of that word
            var wordLetters = vocabularyHelper.GetLettersInWord(wordQuestion);
            //UnityEngine.Debug.LogWarning("Found letters: " + wordLetters.ToArray().ToDebugString());

            bool useJourneyForLetters = parameters.useJourneyForCorrect;
            // @note: we force journey in this case to be off so that all letters can be found
            // @note: we also force the journey if the packs must be used together, as the data filters for journey clash with the new filter
            if (useAllCorrectLetters || packsUsedTogether) useJourneyForLetters = false;

            // Get some letters (from that word)
            var correctLetters = teacher.VocabularyAi.SelectData(
                () => FindCorrectLetters(wordQuestion, wordLetters),
                 new SelectionParameters(parameters.correctSeverity, nCorrect, getMaxData:useAllCorrectLetters, 
                    useJourney: useJourneyForLetters, filteringIds: previousPacksIDs_letters));
            currentRoundIDs_letters.AddRange(correctLetters.ConvertAll(w => w.Id));

            // Get some wrong letters (not from that word, nor other words, nor previous letters)
            // Only for the first pack of the round
            List<LetterData> wrongLetters = new List<LetterData>();
            if (inRoundPackIndex == 0)
            {
                wrongLetters = teacher.VocabularyAi.SelectData(
                () => FindWrongLetters(wordQuestion, wordLetters),
                    new SelectionParameters(
                        parameters.wrongSeverity, nWrong, useJourney: parameters.useJourneyForWrong,
                        journeyFilter: SelectionParameters.JourneyFilter.UpToFullCurrentStage));
                currentRoundIDs_letters.AddRange(wrongLetters.ConvertAll(w => w.Id));
            }

            if (ConfigAI.verboseQuestionPacks)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + wordQuestion;
                debugString += "\nCorrect Answers: " + correctLetters.Count;
                foreach (var l in correctLetters) debugString += " " + l;
                debugString += "\nWrong Answers: " + wrongLetters.Count;
                foreach (var l in wrongLetters) debugString += " " + l;
                ConfigAI.AppendToTeacherReport(debugString);
            }

            return QuestionPackData.Create(wordQuestion, correctLetters, wrongLetters);
        }

        public List<WordData> FindEligibleWords(int maxWordLength)
        {
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            List<WordData> eligibleWords = new List<WordData>();
            foreach(var word in vocabularyHelper.GetWordsByCategory(category, parameters.wordFilters))
            {
                // HACK: Skip the problematic words (for now)
                if (vocabularyHelper.ProblematicWordIds.Contains(word.Id)) continue;

                // Check max length
                if (word.Letters.Length > maxWordLength) continue;

                // Avoid using words that contain previously chosen letters
                if (vocabularyHelper.WordContainsAnyLetter(word, currentRoundIDs_letters)) continue;

                // Avoid using words that have ONLY letters that appeared in previous words
                if (vocabularyHelper.WordHasAllLettersInCommonWith(word, currentRoundIDs_words)) continue;

                eligibleWords.Add(word);
            }
            //UnityEngine.Debug.Log("Eligible words: " + eligibleWords.Count + " out of " + teacher.VocabularyHelper.GetWordsByCategory(category, parameters.wordFilters).Count);
            return eligibleWords;
        }

        public List<LetterData> FindCorrectLetters(WordData selectedWord, List<LetterData> wordLetters)
        {
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            List<LetterData> eligibleLetters = new List<LetterData>();
            var bad_words = new List<string>(currentRoundIDs_words);
            bad_words.Remove(selectedWord.Id);
            foreach (var letter in wordLetters)
            {
                // Avoid using letters that appeared in previous words
                if (vocabularyHelper.LetterContainedInAnyWord(letter, bad_words)) continue;

                eligibleLetters.Add(letter);
            }
            return eligibleLetters;
        }

        public List<LetterData> FindWrongLetters(WordData selectedWord, List<LetterData> wordLetters)
        {
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            List<LetterData> noWordLetters = vocabularyHelper.GetLettersNotIn(parameters.letterFilters, wordLetters.ToArray());
            List<LetterData> eligibleLetters = new List<LetterData>();
            var bad_words = new List<string>(currentRoundIDs_words);
            bad_words.Remove(selectedWord.Id);
            foreach (var letter in noWordLetters)
            {
                // Avoid using letters that appeared in previous words
                if (vocabularyHelper.LetterContainedInAnyWord(letter, bad_words)) continue;

                eligibleLetters.Add(letter);
            }
            return eligibleLetters;
        }

    }
}