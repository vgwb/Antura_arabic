using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class WordsWithLetterQuestionBuilder : IQuestionBuilder
    {
        // focus: Words & Letters
        // pack history filter: TODO
        // journey: TODO

        private int nPacks;
        private int nCorrect;
        private int nWrong;

        public WordsWithLetterQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
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

            int nAttempts = 20;
            bool found = false;
            while(nAttempts > 0 && !found)
            {
                var letter = teacher.wordHelper.GetAllLetters(new LetterFilters()).RandomSelectOne();
                var correctWords = teacher.wordHelper.GetWordsWithLetter(letter.Id);
                //UnityEngine.Debug.Log("Trying letter " + letter + " found n words " + correctWords.Count + " out of " + db.GetAllWordData().Count);
                if (correctWords.Count < nCorrect)
                {
                    nAttempts--;
                    continue;
                }
                correctWords = correctWords.RandomSelect(nCorrect);
                var wrongWords = teacher.wordHelper.GetWordsNotIn(new WordFilters(), correctWords.ToArray()).RandomSelect(nWrong);
                pack = QuestionPackData.Create(letter, correctWords, wrongWords);
                found = true;

                if (ConfigAI.verboseTeacher)
                {
                    string debugString = "--------- TEACHER: question pack result ---------";
                    debugString += "\nQuestion: " + letter;
                    debugString += "\nCorrect Answers: " + correctWords.Count;
                    foreach (var l in correctWords) debugString += " " + l;
                    debugString += "\nWrong Answers: " + wrongWords.Count;
                    foreach (var l in wrongWords) debugString += " " + l;
                    UnityEngine.Debug.Log(debugString);
                }

            }
            if (!found)
            {
                throw new System.Exception("Could not find enough data to build the required questions for the current journey position. Minigame should be aborted.");
            }

            return pack;
        }

    }
}