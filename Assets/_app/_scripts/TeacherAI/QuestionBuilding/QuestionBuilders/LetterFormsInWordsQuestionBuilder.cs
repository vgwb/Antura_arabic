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

        private int nPacks;
        private int maximumWordLength;
        private WordDataCategory category;
        private QuestionBuilderParameters parameters;

        public LetterFormsInWordsQuestionBuilder(int nPacks,
            WordDataCategory category = WordDataCategory.None,
            int maximumWordLength = 20,
            QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();
            this.nPacks = nPacks;
            this.category = category;
            this.maximumWordLength = maximumWordLength;
            this.parameters = parameters;

            // Forced parameters
            this.parameters.letterFilters.excludeDiacritics = true;
            this.parameters.letterFilters.excludeLetterVariations = true;
        }

        private List<string> previousPacksIDs_words = new List<string>();
        private List<string> previousPacksIDs_letters = new List<string>();

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            previousPacksIDs_words.Clear();
            previousPacksIDs_letters.Clear();
            List<QuestionPackData> packs = new List<QuestionPackData>();

            // First, choose a letter
            var teacher = AppManager.I.Teacher;
            var vocabulary = AppManager.I.VocabularyHelper;
            var usableLetters = teacher.VocabularyAi.SelectData(
                () => vocabulary.GetAllLetters(parameters.letterFilters),
                    new SelectionParameters(parameters.correctSeverity, 1, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_letters));
            var letter = usableLetters[0];
            // @todo: the chosen letter should actually have words that contain it in different forms... VERY HARD FILTER!

            // Determine what forms the letter appears in
            List<LetterForm> usableForms = new List<LetterForm>();
            foreach (var form in GenericHelper.SortEnums<LetterForm>())
            {
                if (form == LetterForm.None) continue;
                if (letter.GetUnicode(form, false) != "") usableForms.Add(form);
            }
            //Debug.Log("N USABLE FORMS: " + usableForms.Count + " for letter " + letter);

            // Packs are reduced to the number of available forms, if needed
            nPacks = Mathf.Min(usableForms.Count,nPacks);

            // Randomly choose some forms (one per pack) and create packs
            for (int pack_i = 0; pack_i < nPacks; pack_i++)
            {
                var form = RandomHelper.RandomSelectOne(usableForms);
                usableForms.Remove(form);
                packs.Add(CreateSingleQuestionPackData(letter, form));
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

            if (ConfigAI.verboseTeacher)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + question;
                debugString += "\nCorrect Answers: " + correctAnswers.Count;
                foreach (var l in correctAnswers) debugString += " " + l;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.Create(question, correctAnswers, new List<LetterData>());
        }

        public List<WordData> FindEligibleWords(int maxWordLength, LetterData containedLetter, LetterForm form)
        {
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            List<WordData> eligibleWords = new List<WordData>();
            foreach(var word in vocabularyHelper.GetWordsByCategory(category, parameters.wordFilters))
            {
                // Check max length
                if (word.Letters.Length > maxWordLength) continue;

                // Check that it contains the letter only once
                if (WordContainsLetterTimes(word, containedLetter) > 1) continue;

                // Check that it contains a letter in the correct form
                if (!WordContainsLetterWithForm(word, containedLetter, form)) continue;

                eligibleWords.Add(word);
            }
            //UnityEngine.Debug.Log("Eligible words: " + eligibleWords.Count + " out of " + teacher.VocabularyHelper.GetWordsByCategory(category, parameters.wordFilters).Count);
            return eligibleWords;
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
            return ArabicAlphabetHelper.GetLetterShapeInWord(selectedWord, containedLetter) == selectedForm;
        }

    }
}