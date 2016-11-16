using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class SunMoonWordsQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;

        public SunMoonWordsQuestionBuilder(int nPacks)
        {
            this.nPacks = nPacks;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            var teacher = AppManager.Instance.Teacher;

            var db = AppManager.Instance.DB;
            var sunWord = db.GetWordDataById("the_sun");
            var moonWord = db.GetWordDataById("the_moon");

            var wordsWithArticle = teacher.wordHelper.GetWordsByArticle(Db.WordDataArticle.Determinative).RandomSelect(nPacks);
            foreach(var wordWithArticle in wordsWithArticle)
            {
                int articleLength = 2;
                var letterAfterArticle = teacher.wordHelper.GetLettersInWord(wordWithArticle)[articleLength];
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                switch (letterAfterArticle.SunMoon)
                {
                    case Db.LetterDataSunMoon.Sun:
                        correctWords.Add(sunWord);
                        wrongWords.Add(moonWord);
                        break;

                    case Db.LetterDataSunMoon.Moon:
                        correctWords.Add(moonWord);
                        wrongWords.Add(sunWord);
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