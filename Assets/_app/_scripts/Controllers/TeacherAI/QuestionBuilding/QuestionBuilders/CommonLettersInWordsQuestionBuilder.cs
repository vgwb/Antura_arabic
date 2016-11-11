using System.Collections.Generic;


namespace EA4S
{
    public class CommonLettersInWordQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private int nWords;

        public CommonLettersInWordQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, int nWords = 1)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.nWords = nWords;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            for (int pack_i = 0; pack_i < nPacks; pack_i++)
            {
                packs.Add(CreateSingleQuestionPackData());
            }
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData()
        {
            QuestionPackData pack = null;
            var teacher = AppManager.Instance.Teacher;
            //var db = AppManager.Instance.DB;

            int nAttempts = 20;
            bool found = false;
            while (nAttempts > 0 && !found)
            {
                var commonLetters = teacher.wordHelper.GetAllRealLetters().RandomSelect(nCorrect);
                var words = teacher.wordHelper.GetWordsWithLetters(commonLetters.ConvertAll(x => x.Id).ToArray());
                if (words.Count < nWords)
                {
                    nAttempts--;
                    continue;
                }
                words = words.RandomSelect(nWords);
                var nonCommonLetters = teacher.wordHelper.GetRealLettersNotInWords(words.ToArray()).RandomSelect(nWrong);

                //UnityEngine.Debug.Log("Found words " + words.Count + " for letters " + commonLetters.Count);

                pack = QuestionPackData.Create(words, commonLetters, nonCommonLetters);
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