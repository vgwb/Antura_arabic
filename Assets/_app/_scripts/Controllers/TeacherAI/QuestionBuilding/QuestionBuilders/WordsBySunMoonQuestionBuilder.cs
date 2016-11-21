using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class WordsBySunMoonQuestionBuilder : IQuestionBuilder
    {
        // focus: Words
        // pack history filter: by default, all different
        // journey: enabled

        private int nPacks;
        SelectionSeverity severity;

        public WordsBySunMoonQuestionBuilder(int nPacks, SelectionSeverity severity = SelectionSeverity.AsManyAsPossible)
        {
            this.nPacks = nPacks;
            this.severity = severity;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            var teacher = AppManager.Instance.Teacher;

            var db = AppManager.Instance.DB;
            var choice1 = db.GetWordDataById("the_sun");
            var choice2 = db.GetWordDataById("the_moon");

            var wordFilters = new WordFilters();
            wordFilters.excludeArticles = false;

            var wordsWithArticle = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetWordsByArticle(Db.WordDataArticle.Determinative, wordFilters),
                new SelectionParameters(severity, nPacks)
                );

            foreach (var wordWithArticle in wordsWithArticle)
            {
                int articleLength = 2;
                var letterAfterArticle = teacher.wordHelper.GetLettersInWord(wordWithArticle)[articleLength];
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                switch (letterAfterArticle.SunMoon)
                {
                    case Db.LetterDataSunMoon.Sun:
                        correctWords.Add(choice1);
                        wrongWords.Add(choice2);
                        break;

                    case Db.LetterDataSunMoon.Moon:
                        correctWords.Add(choice2);
                        wrongWords.Add(choice1);
                        break;
                }

                // Debug
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

            return packs;
        }

    }
}