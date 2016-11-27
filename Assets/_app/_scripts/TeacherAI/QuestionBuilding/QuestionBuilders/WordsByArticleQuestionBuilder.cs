using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class WordsByArticleQuestionBuilder : IQuestionBuilder
    {
        // focus: Words
        // pack history filter: by default, all different
        // journey: enabled

        private int nPacks;
        private QuestionBuilderParameters parameters;

        public WordsByArticleQuestionBuilder(int nPacks, QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();

            this.nPacks = nPacks;
            this.parameters = parameters;

            // Forced parameters
            this.parameters.wordFilters.excludeArticles = false;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            var teacher = AppManager.I.Teacher;

            var db = AppManager.I.DB;
            var choice1 = db.GetWordDataById("with_article");
            var choice2 = db.GetWordDataById("without_article");

            int nPerType = nPacks / 2;

            var list_choice1 = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetWordsByArticle(Db.WordDataArticle.Determinative, parameters.wordFilters),
                new SelectionParameters(parameters.correctSeverity, nPerType, useJourney:parameters.useJourneyForCorrect)
                );

            var list_choice2 = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetWordsByArticle(Db.WordDataArticle.None, parameters.wordFilters),
                new SelectionParameters(parameters.wrongSeverity, nPerType, useJourney: parameters.useJourneyForCorrect)
                );

            foreach (var wordWithArticle in list_choice1)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(choice1);
                wrongWords.Add(choice2);

                if (ConfigAI.verboseTeacher)
                {
                    string debugString = "--------- TEACHER: question pack result ---------";
                    debugString += "\nQuestion: " + wordWithArticle;
                    debugString += "\nCorrect Word: " + correctWords.Count;
                    foreach (var l in correctWords) debugString += " " + l;
                    debugString += "\nWrong Word: " + wrongWords.Count;
                    foreach (var l in wrongWords) debugString += " " + l;
                    UnityEngine.Debug.Log(debugString);
                }

                var pack = QuestionPackData.Create(wordWithArticle, correctWords, wrongWords);
                packs.Add(pack);
            }

            foreach (var wordWithoutArticle in list_choice2)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(choice2);
                wrongWords.Add(choice1);

                if (ConfigAI.verboseTeacher)
                {
                    string debugString = "--------- TEACHER: question pack result ---------";
                    debugString += "\nQuestion: " + wordWithoutArticle;
                    debugString += "\nCorrect Word: " + correctWords.Count;
                    foreach (var l in correctWords) debugString += " " + l;
                    debugString += "\nWrong Word: " + wrongWords.Count;
                    foreach (var l in wrongWords) debugString += " " + l;
                    UnityEngine.Debug.Log(debugString);
                }

                var pack = QuestionPackData.Create(wordWithoutArticle, correctWords, wrongWords);
                packs.Add(pack);
            }

            return packs;
        }

    }
}