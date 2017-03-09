using System.Collections.Generic;
using EA4S.Database;
using EA4S.Helpers;
using UnityEngine;

namespace EA4S.Teacher
{
    /// <summary>
    /// Selects letter forms inside words
    /// * Question: Word
    /// * Correct answers: Letter contained in the word (set by minigame: correct form)
    /// * Wrong answers: (set by minigame: wrong forms)
    /// * Different packs: same Letter will be in all packs, but with different forms
    /// @note: the use of forms (correct/uncorrect check) is performed by the minigame itself
    /// </summary>
    public class LetterFormsInWordsQuestionBuilder : IQuestionBuilder
    {
        // focus: Letters & Words
        // pack history filter: enabled
        // journey: enabled

        private int nPacksPerRound;
        private int nRounds;
        private int maximumWordLength;
        private WordDataCategory category;
        private bool forceUnseparatedLetters;
        private QuestionBuilderParameters parameters;

        public QuestionBuilderParameters Parameters
        {
            get { return this.parameters; }
        }

        public LetterFormsInWordsQuestionBuilder(int nPacksPerRound, int nRounds,
            WordDataCategory category = WordDataCategory.None,
            int maximumWordLength = 20, bool forceUnseparatedLetters = true,
            QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();
            this.nPacksPerRound = nPacksPerRound;
            this.nRounds = nRounds;
            this.category = category;
            this.maximumWordLength = maximumWordLength;
            this.forceUnseparatedLetters = forceUnseparatedLetters;
            this.parameters = parameters;

            // Forced parameters
            this.parameters.letterFilters.excludeDiacritics = LetterFilters.ExcludeDiacritics.All;
        }

        private List<string> previousPacksIDs_words = new List<string>();
        private List<string> previousPacksIDs_letters = new List<string>();

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            // HACK: the game may need unseparated letters
            if (forceUnseparatedLetters) AppManager.I.VocabularyHelper.ForceUnseparatedLetters = true;

            previousPacksIDs_words.Clear();
            previousPacksIDs_letters.Clear();
            List<QuestionPackData> packs = new List<QuestionPackData>();

            for (int round_i = 0; round_i < nRounds; round_i++)
            {
                // First, choose a letter
                var teacher = AppManager.I.Teacher;
                var vocabulary = AppManager.I.VocabularyHelper;
                var usableLetters = teacher.VocabularyAi.SelectData(
                    () => FindEligibleLettersAndForms(minFormsAppearing:2, maxWordLength: maximumWordLength),  
                        new SelectionParameters(parameters.correctSeverity, 1, useJourney: parameters.useJourneyForCorrect,
                            packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_letters));
                var letter = usableLetters[0];

                // Determine what forms the letter appears in
                List<LetterForm> usableForms = lettersAndForms[letter];
                //Debug.Log("N USABLE FORMS: " + usableForms.Count + " for letter " + letter);

                // Packs are reduced to the number of available forms, if needed
                int nPacksFound = Mathf.Min(usableForms.Count, nPacksPerRound);

                // Randomly choose some forms (one per pack) and create packs
                for (int pack_i = 0; pack_i < nPacksFound; pack_i++)
                {
                    var form = RandomHelper.RandomSelectOne(usableForms);
                    usableForms.Remove(form);
                    packs.Add(CreateSingleQuestionPackData(letter, form));
                }
            }

            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData(LetterData letter, LetterForm form)
        {
            var teacher = AppManager.I.Teacher;

            // Find a word with the letter in that form
            var usableWords = teacher.VocabularyAi.SelectData(
                () => FindEligibleWords(maximumWordLength, letter, form),
                    new SelectionParameters(parameters.correctSeverity, 1, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_words));
            var question = usableWords[0];

            // Place the correct letter
            var correctAnswers = new List<LetterData>();
            correctAnswers.Add(letter);

            if (ConfigAI.verboseQuestionPacks)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + question;
                debugString += "\nCorrect Answers: " + correctAnswers.Count;
                foreach (var l in correctAnswers) debugString += " " + l;
                ConfigAI.AppendToTeacherReport(debugString);
            }

            return QuestionPackData.Create(question, correctAnswers, new List<LetterData>());
        }

        List<LetterData> lettersWithManyForms = new List<LetterData>();
        Dictionary<LetterData, List<LetterForm>> lettersAndForms = new Dictionary<LetterData, List<LetterForm>>();

        List<LetterData> FindEligibleLettersAndForms(int minFormsAppearing, int maxWordLength)
        {
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            List<LetterData> eligibleLetters = new List<LetterData>();

            if (lettersAndForms.Count == 0)
            {
                var allWords = AppManager.I.Teacher.VocabularyAi.SelectData(
                    () => vocabularyHelper.GetWordsByCategory(category, parameters.wordFilters),
                        new SelectionParameters(parameters.correctSeverity, getMaxData:true, useJourney: parameters.useJourneyForCorrect,
                            packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_words));

                // The chosen letter should actually have words that contain it in different forms.
                // This can be quite slow, so we do this only at the start.
                var allLetters = vocabularyHelper.GetAllLetters(parameters.letterFilters);
                foreach (var letterData in allLetters)
                {
                    int nFormsAppearing = 0;
                    var availableForms = new List<LetterForm>();
                    foreach (var form in letterData.GetAvailableForms())
                    {
                        foreach (var wordData in allWords)
                        {
                            if (WordIsFine(wordData, letterData, form, maxWordLength))
                            {
                                nFormsAppearing++;
                                availableForms.Add(form);
                                break;
                            }
                        }
                    }
                    if (nFormsAppearing >= minFormsAppearing)
                    {
                        lettersWithManyForms.Add(letterData);
                        lettersAndForms[letterData] = availableForms;
                        //Debug.Log("Letter " + letterData + " is cool as it appears " + nFormsAppearing);
                    }
                }
            }

            eligibleLetters = lettersWithManyForms;

            return eligibleLetters;
        }

        Dictionary<KeyValuePair<LetterData,LetterForm>, List<WordData>> eligibleWordsForLetters = new Dictionary<KeyValuePair<LetterData, LetterForm>, List<WordData>>();

        public List<WordData> FindEligibleWords(int maxWordLength, LetterData containedLetter, LetterForm form)
        {
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            List<WordData> eligibleWords = new List<WordData>();

            var pair = new KeyValuePair<LetterData, LetterForm>(containedLetter, form);
            if (!eligibleWordsForLetters.ContainsKey(pair))
            {
                foreach (var word in vocabularyHelper.GetWordsByCategory(category, parameters.wordFilters))
                {
                    if (!WordIsFine(word, containedLetter, form, maxWordLength)) continue;
                    eligibleWords.Add(word);
                    //Debug.Log("Letter: " + containedLetter + " form: " + form + " Word: " + word);
                }
                eligibleWordsForLetters[pair] = eligibleWords;
            }
            eligibleWords = eligibleWordsForLetters[pair];

            //Debug.LogWarning("Eligible words: " + eligibleWords.Count + " out of " + vocabularyHelper.GetWordsByCategory(category, parameters.wordFilters).Count);
            return eligibleWords;
        }

        private bool WordIsFine(WordData word, LetterData containedLetter, LetterForm form, int maxWordLength)
        {
            // Check max length
            if (word.Letters.Length > maxWordLength) return false;

            // Check that it contains the letter only once
            if (WordContainsLetterTimes(word, containedLetter) > 1) return false;

            // Check that it contains a letter in the correct form
            if (!WordContainsLetterWithForm(word, containedLetter, form)) return false;

            return true;
        }

        private int WordContainsLetterTimes(WordData selectedWord, LetterData containedLetter)
        {
            List<LetterData> wordLetters = AppManager.I.VocabularyHelper.GetLettersInWord(selectedWord);
            int count = 0;
            foreach (var letter in wordLetters)
                if (letter == containedLetter)
                    count++;
            return count;
        }

        private bool WordContainsLetterWithForm(WordData selectedWord, LetterData containedLetter, LetterForm selectedForm)
        {
            foreach (var l in ArabicAlphabetHelper.FindLetter(AppManager.I.DB, selectedWord, containedLetter))
                if (l.letterForm == selectedForm)
                    return true;
            return false;
        }

    }
}