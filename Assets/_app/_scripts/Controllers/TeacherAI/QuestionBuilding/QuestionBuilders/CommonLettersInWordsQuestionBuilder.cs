using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class CommonLettersInWordQuestionBuilder : IQuestionBuilder
    {
        // focus: Letters & Words
        // pack history filter: TODO
        // journey: TODO

        private int nPacks;
        private int nMinCommonLetters;
        private int nMaxCommonLetters;
        private int nWrong;
        private int nWords;

        public CommonLettersInWordQuestionBuilder(int nPacks, int nMinCommonLetters = 1, int nMaxCommonLetters = 1, int nWrong = 0, int nWords = 1)
        {
            this.nPacks = nPacks;
            this.nMinCommonLetters = nMinCommonLetters;
            this.nMaxCommonLetters = nMaxCommonLetters;
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
                var words = teacher.wordHelper.GetAllWords(new WordFilters()).RandomSelect(nWords);
                var commonLetters = teacher.wordHelper.GetCommonLettersInWords(words.ToArray());

                if (commonLetters.Count < nMinCommonLetters || commonLetters.Count > nMaxCommonLetters)
                {
                    nAttempts--;
                    continue;
                }
                var nonCommonLetters = teacher.wordHelper.GetLettersNotIn(new LetterFilters(), commonLetters.ToArray()).RandomSelect(nWrong);

                // Debug
                if (ConfigAI.verboseTeacher)
                { 
                    string debugString = "--------- TEACHER: question pack result ---------";
                    debugString += "\nCommon letters: ";
                    foreach (var l in commonLetters) debugString += " " + l;
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