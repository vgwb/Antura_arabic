using System.Collections.Generic;
using EA4S.Database;
using EA4S.Helpers;

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
    public class LetterFormsInWordQuestionBuilder : IQuestionBuilder
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

        public LetterFormsInWordQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, 
            bool useAllCorrectLetters = false, WordDataCategory category = WordDataCategory.None,
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

            // First, choose a letter
            var teacher = AppManager.I.Teacher;
            var vocabulary = AppManager.I.VocabularyHelper;
            var usableLetters = teacher.VocabularyAi.SelectData(
                () => vocabulary.GetAllLetters(parameters.letterFilters),
                    new SelectionParameters(parameters.correctSeverity, 1, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_letters));
            var letter = usableLetters[0];

            // Determine what forms the letter appears in
            List<LetterPosition> usableForms = new List<LetterPosition>();
            foreach (var form in GenericHelper.SortEnums<LetterPosition>())
            {
                if (form == LetterPosition.None) continue;
                if (letter.GetUnicode(form, false) != "") usableForms.Add(form);
            }

            // Packs are reducted to the number of available forms, if needed
            nPacks = UnityEngine.Mathf.Min(usableForms.Count,nPacks);

            // Randomly choose some forms (one per pack) and create packs
            for (int pack_i = 0; pack_i < nPacks; pack_i++)
            {
                var form = RandomHelper.RandomSelectOne(usableForms);
                usableForms.Remove(form);
                packs.Add(CreateSingleQuestionPackData(letter, form));
            }
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData(LetterData letter, LetterPosition form)
        {
            var teacher = AppManager.I.Teacher;
            var vocabularyHelper = AppManager.I.VocabularyHelper;

            // Find a word with the letter in that position
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

            return QuestionPackData.Create(question, correctAnswers, null);
        }

        public List<WordData> FindEligibleWords(int maxWordLength, LetterData containedLetter, LetterPosition form)
        {
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            List<WordData> eligibleWords = new List<WordData>();
            foreach(var word in vocabularyHelper.GetWordsByCategory(category, parameters.wordFilters))
            {
                // Check max length
                if (word.Letters.Length > maxWordLength) continue;



                eligibleWords.Add(word);
            }
            //UnityEngine.Debug.Log("Eligible words: " + eligibleWords.Count + " out of " + teacher.VocabularyHelper.GetWordsByCategory(category, parameters.wordFilters).Count);
            return eligibleWords;
        }

        public List<LetterData> FindEligibleLetters(WordData selectedWord, List<LetterData> wordLetters)
        {
            List<LetterData> eligibleLetters = new List<LetterData>();
            foreach (var letter in wordLetters)
            {
                // Avoid using letters that appeared in previous words
//                if (packsUsedTogether && LetterContainedInAnyWord(selectedWord, letter, previousPacksIDs_words)) continue;

                eligibleLetters.Add(letter);
            }
            return eligibleLetters;
        }

    }
}