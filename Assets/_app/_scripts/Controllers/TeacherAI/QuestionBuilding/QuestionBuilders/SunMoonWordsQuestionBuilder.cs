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

        public int GetQuestionPackCount()
        {
            return nPacks;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            var teacher = AppManager.Instance.Teacher;
            var db = AppManager.Instance.DB;

            var sunWord = db.GetWordDataById("sun");
            var moonWord = db.GetWordDataById("moon");

            int nPerType = nPacks / 2;
            var sunLetters = teacher.wordHelper.GetRealLettersBySunMoon(Db.LetterDataSunMoon.Sun).RandomSelect(nPerType);
            var moonLetters = teacher.wordHelper.GetRealLettersBySunMoon(Db.LetterDataSunMoon.Moon).RandomSelect(nPerType);

            foreach (var letter in sunLetters)
            {
                var correctWords = new List<Db.WordData>() { sunWord };
                var wrongWords = new List<Db.WordData>() { moonWord };
                var pack = QuestionPackData.Create(letter, correctWords, wrongWords);
                packs.Add(pack);
            }

            foreach (var letter in moonLetters)
            {
                var correctWords = new List<Db.WordData>() { moonWord };
                var wrongWords = new List<Db.WordData>() { sunWord };
                var pack = QuestionPackData.Create(letter, correctWords, wrongWords);
                packs.Add(pack);
            }

            return packs;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            throw new System.Exception("Moving to the new full-pack rules.");
        }

    }
}