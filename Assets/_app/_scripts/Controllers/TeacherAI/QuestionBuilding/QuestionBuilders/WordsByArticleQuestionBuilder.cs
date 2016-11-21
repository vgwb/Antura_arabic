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
        SelectionSeverity severity;

        public WordsByArticleQuestionBuilder(int nPacks, SelectionSeverity severity = SelectionSeverity.AsManyAsPossible)
        {
            this.nPacks = nPacks;
            this.severity = severity;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            var teacher = AppManager.Instance.Teacher;

            var db = AppManager.Instance.DB;
            var choice1 = db.GetWordDataById("with_article");
            var choice2 = db.GetWordDataById("without_article");

            int nPerType = nPacks / 2;

            var wordFilters = new WordFilters();
            wordFilters.excludeArticles = false;

            var list_choice1 = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetWordsByArticle(Db.WordDataArticle.Determinative, wordFilters),
                new SelectionParameters(severity, nPerType)
                );

            var list_choice2 = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetWordsByArticle(Db.WordDataArticle.None, wordFilters),
                new SelectionParameters(severity, nPerType)
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