using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class ArticleWordsQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;

        public ArticleWordsQuestionBuilder(int nPacks)
        {
            this.nPacks = nPacks;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            var teacher = AppManager.Instance.Teacher;

            var db = AppManager.Instance.DB;

            var withArticleWord = db.GetWordDataById("with_article");
            var withoutArticleWord = db.GetWordDataById("without_article");

            int nPerType = nPacks / 2;
            var wordsWithArticle = teacher.wordHelper.GetWordsByArticle(Db.WordDataArticle.Determinative).RandomSelect(nPerType);
            var wordsWithoutArticle = teacher.wordHelper.GetWordsByArticle(Db.WordDataArticle.None).RandomSelect(nPerType);

            foreach (var wordWithArticle in wordsWithArticle)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(withArticleWord);
                wrongWords.Add(withoutArticleWord);

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

            foreach (var wordWithoutArticle in wordsWithoutArticle)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(withoutArticleWord);
                wrongWords.Add(withArticleWord);

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