using System.Collections.Generic;


namespace EA4S
{
    public class WordsWithLetterQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;
        private int nCorrect;
        private int nWrong;

        public WordsWithLetterQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
        }

        public int GetQuestionPackCount()
        {
            return nPacks;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            QuestionPackData pack = null;
            var teacher = AppManager.Instance.Teacher;
            var db = AppManager.Instance.DB;

            int nAttempts = 20;
            bool found = false;
            while(nAttempts > 0 && !found)
            {
                var letter = db.GetAllLetterData().RandomSelectOne();
                var correctWords = teacher.wordHelper.GetWordsWithLetter(letter.Id);
                //UnityEngine.Debug.Log("Trying letter " + letter + " found n words " + correctWords.Count + " out of " + db.GetAllWordData().Count);
                if (correctWords.Count < nCorrect)
                {
                    nAttempts--;
                    continue;
                }
                correctWords = correctWords.RandomSelect(nCorrect);
                var wrongWords = teacher.wordHelper.GetWordsNotIn(correctWords.ToArray()).RandomSelect(nWrong);
                pack = QuestionPackData.Create(letter, correctWords, wrongWords);
                found = true;
            }
            if (!found)
            {
                throw new System.Exception("Could not find enough data to build the required questions for the current journey position. Minigame should be aborted.");
            }

            return pack;
        }

    }
}