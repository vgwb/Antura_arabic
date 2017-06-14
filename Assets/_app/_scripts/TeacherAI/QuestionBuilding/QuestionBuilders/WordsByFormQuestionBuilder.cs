using System.Collections.Generic;
using EA4S.Core;
using EA4S.Helpers;

namespace EA4S.Teacher
{
    /// <summary>
    /// Categorize words based on their form
    /// * Question: Word to categorize
    /// * Correct answers: Correct form
    /// * Wrong answers: Wrong form
    /// </summary>
    public class WordsByFormQuestionBuilder : IQuestionBuilder
    {
        // focus: Words
        // pack history filter: by default, all different
        // journey: enabled

        private int nPacks;
        private QuestionBuilderParameters parameters;

        public QuestionBuilderParameters Parameters
        {
            get { return this.parameters; }
        }

        public WordsByFormQuestionBuilder(int nPacks, QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();

            this.nPacks = nPacks;
            this.parameters = parameters;

            // Forced parameters
            this.parameters.wordFilters.excludePluralDual = false;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            var teacher = AppManager.Instance.Teacher;
            var vocabularyHelper = AppManager.Instance.VocabularyHelper;

            var db = AppManager.Instance.DB;
            var choice1 = db.GetWordDataById("singular");
            var choice2 = db.GetWordDataById("plural");
            var choice3 = db.GetWordDataById("dual");

            int nPerType = nPacks / 3;

            var list_choice1 = teacher.VocabularyAi.SelectData(
                () => vocabularyHelper.GetWordsByForm(Database.WordDataForm.Singular, parameters.wordFilters),
                new SelectionParameters(parameters.correctSeverity, nPerType, useJourney: parameters.useJourneyForCorrect)
                );

            var list_choice2 = teacher.VocabularyAi.SelectData(
                () => vocabularyHelper.GetWordsByForm(Database.WordDataForm.Plural, parameters.wordFilters),
                new SelectionParameters(parameters.correctSeverity, nPerType, useJourney: parameters.useJourneyForCorrect)
                );

            var list_choice3 = teacher.VocabularyAi.SelectData(
                () => vocabularyHelper.GetWordsByForm(Database.WordDataForm.Dual, parameters.wordFilters),
                new SelectionParameters(parameters.correctSeverity, nPerType, useJourney: parameters.useJourneyForCorrect)
                );

            foreach (var word in list_choice1)
            {
                var correctWords = new List<Database.WordData>();
                var wrongWords = new List<Database.WordData>();
                correctWords.Add(choice1);
                wrongWords.Add(choice2);
                wrongWords.Add(choice3);

                var pack = QuestionPackData.Create(word, correctWords, wrongWords);
                packs.Add(pack);
            }

            foreach (var word in list_choice2)
            {
                var correctWords = new List<Database.WordData>();
                var wrongWords = new List<Database.WordData>();
                correctWords.Add(choice2);
                wrongWords.Add(choice1);
                wrongWords.Add(choice3);

                var pack = QuestionPackData.Create(word, correctWords, wrongWords);
                packs.Add(pack);
            }

            foreach (var word in list_choice3)
            {
                var correctWords = new List<Database.WordData>();
                var wrongWords = new List<Database.WordData>();
                correctWords.Add(choice3);
                wrongWords.Add(choice1);
                wrongWords.Add(choice2);

                var pack = QuestionPackData.Create(word, correctWords, wrongWords);
                packs.Add(pack);
            }

            // Shuffle the packs at the end
            packs.Shuffle();

            if (ConfigAI.verboseQuestionPacks)
            {
                foreach(var pack in packs)
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