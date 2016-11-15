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
            var sunWord = db.GetWordDataById("sun");
            var moonWord = db.GetWordDataById("moon");

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
                var pack = QuestionPackData.Create(wordWithArticle, correctWords, wrongWords);
                packs.Add(pack);
            }

            return packs;
        }

    }
}