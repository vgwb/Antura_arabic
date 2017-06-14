using System.Collections.Generic;
using EA4S.Core;
using EA4S.Helpers;

namespace EA4S.Teacher
{
    /// <summary>
    /// Categorize letters based on their type (vowel/consonant)
    /// * Question: Letter to categorize
    /// * Correct answers: Correct type
    /// * Wrong answers: Wrong type
    /// </summary>
    public class LettersByTypeQuestionBuilder : IQuestionBuilder
    {
        // focus: Letters
        // pack history filter: by default, all different
        // journey: enabled

        private int nPacks;
        private QuestionBuilderParameters parameters;

        public QuestionBuilderParameters Parameters
        {
            get { return this.parameters; }
        }

        public LettersByTypeQuestionBuilder(int nPacks, QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();

            this.nPacks = nPacks;
            this.parameters = parameters;

            // Forced filters
            this.parameters.letterFilters.excludeDiphthongs = true;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            var teacher = (AppManager.Instance as AppManager).Teacher;
            var vocabularyHelper = (AppManager.Instance as AppManager).VocabularyHelper;

            var db = (AppManager.Instance as AppManager).DB;
            var choice1 = db.GetWordDataById("consonant");
            var choice2 = db.GetWordDataById("vowel");

            int nPerType = nPacks / 2;

            var list_choice1 = teacher.VocabularyAi.SelectData(
                () => vocabularyHelper.GetConsonantLetter(parameters.letterFilters),
                new SelectionParameters(parameters.correctSeverity, nPerType, useJourney: parameters.useJourneyForCorrect)
                );

            var list_choice2 = teacher.VocabularyAi.SelectData(
                () => vocabularyHelper.GetVowelLetter(parameters.letterFilters),
                new SelectionParameters(parameters.correctSeverity, nPerType, useJourney: parameters.useJourneyForCorrect)
                );

            foreach (var data in list_choice1)
            {
                var correctWords = new List<Database.WordData>();
                var wrongWords = new List<Database.WordData>();
                correctWords.Add(choice1);
                wrongWords.Add(choice2);

                var pack = QuestionPackData.Create(data, correctWords, wrongWords);
                packs.Add(pack);
            }

            foreach (var data in list_choice2)
            {
                var correctWords = new List<Database.WordData>();
                var wrongWords = new List<Database.WordData>();
                correctWords.Add(choice2);
                wrongWords.Add(choice1);

                var pack = QuestionPackData.Create(data, correctWords, wrongWords);
                packs.Add(pack);
            }


            // Shuffle the packs at the end
            packs.Shuffle();

            if (ConfigAI.verboseQuestionPacks)
            {
                foreach (var pack in packs)
                {
                    string debugString = "--------- TEACHER: question pack result ---------";
                    debugString += "\nQuestion: " + pack.question;
                    debugString += "\nCorrect Word: " + pack.correctAnswers.Count;
                    foreach (var l in pack.correctAnswers) debugString += " " + l;
                    debugString += "\nWrong Word: " + pack.wrongAnswers.Count;
                    foreach (var l in pack.wrongAnswers) debugString += " " + l;
                    ConfigAI.AppendToTeacherReport(debugString);
                }
            }

            return packs;
        }

    }
}