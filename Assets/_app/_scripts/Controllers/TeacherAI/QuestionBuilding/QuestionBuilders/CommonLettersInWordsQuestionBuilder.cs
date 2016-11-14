using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class CommonLettersInWordQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;
        private int nCommonLetters;
        private int nWrong;
        private int nWords;

        public CommonLettersInWordQuestionBuilder(int nPacks, int nCommonLetters = 1, int nWrong = 0, int nWords = 1)
        {
            this.nPacks = nPacks;
            this.nCommonLetters = nCommonLetters;
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
                var commonLetters = teacher.wordHelper.GetAllLetters().RandomSelect(nCommonLetters);
                var words = teacher.wordHelper.GetWordsWithLetters(commonLetters.ConvertAll(x => x.Id).ToArray());
                if (words.Count < nWords)
                {
                    nAttempts--;
                    continue;
                }
                words = words.RandomSelect(nWords);
                var nonCommonLetters = teacher.wordHelper.GetLettersNotInWords(words.ToArray()).RandomSelect(nWrong);

                // Debug
                if (ConfigAI.verboseTeacher)
                { 
                    string debugString = "For letter " + commonLetters[0];
                    debugString += "\nWord0: " + words[0];
                    foreach (var l in words[0].Letters) debugString += " " + l;
                    debugString += "\nWord1: " + words[1];
                    foreach (var l in words[1].Letters) debugString += " " + l;
                    UnityEngine.Debug.Log(debugString);
                }

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