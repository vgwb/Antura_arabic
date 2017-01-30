﻿using System.Collections.Generic;

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
        private Database.WordDataCategory category;
        private QuestionBuilderParameters parameters;

        public LettersInWordQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, 
            bool useAllCorrectLetters = false, Database.WordDataCategory category = Database.WordDataCategory.None,
            int maximumWordLength = 20,
            QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.useAllCorrectLetters = useAllCorrectLetters;
            this.category = category;
            this.maximumWordLength = maximumWordLength;
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

            // Get a word
            var usableWords = teacher.VocabularyAi.SelectData(
                () => FindEligibleWords(maxWordLength: this.maximumWordLength),
                    new SelectionParameters(parameters.correctSeverity, 1, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs));
            var question = usableWords[0];

            // Get letters of that word
            var wordLetters = teacher.wordHelper.GetLettersInWord(question);

            bool useJourneyForLetters = parameters.useJourneyForCorrect; 
            if (useAllCorrectLetters) useJourneyForLetters = false;  // @note: we force journey in this case to be off so that all letters can be found

            var correctAnswers = teacher.VocabularyAi.SelectData(
                () => wordLetters,
                 new SelectionParameters(parameters.correctSeverity, nCorrect, getMaxData:useAllCorrectLetters, 
                 useJourney: useJourneyForLetters));  

            var wrongAnswers = teacher.VocabularyAi.SelectData(
                () => teacher.wordHelper.GetLettersNotIn(parameters.letterFilters, wordLetters.ToArray()),
                    new SelectionParameters(parameters.wrongSeverity, nWrong, useJourney: parameters.useJourneyForWrong));

            if (ConfigAI.verboseTeacher)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + question;
                debugString += "\nCorrect Answers: " + correctAnswers.Count;
                foreach (var l in correctAnswers) debugString += " " + l;
                debugString += "\nWrong Answers: " + wrongAnswers.Count;
                foreach (var l in wrongAnswers) debugString += " " + l;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }

        public List<Database.WordData> FindEligibleWords(int maxWordLength)
        {
            var teacher = AppManager.I.Teacher;
            List<Database.WordData> eligibleWords = new List<Database.WordData>();
            foreach(var word in teacher.wordHelper.GetWordsByCategory(category, parameters.wordFilters))
            {
                if (word.Letters.Length <= maxWordLength)
                {
                    eligibleWords.Add(word);
                }
            }
            //UnityEngine.Debug.Log("Eligible words: " + eligibleWords.Count + " out of " + teacher.wordHelper.GetWordsByCategory(category, parameters.wordFilters).Count);
            return eligibleWords;
        }


    }
}