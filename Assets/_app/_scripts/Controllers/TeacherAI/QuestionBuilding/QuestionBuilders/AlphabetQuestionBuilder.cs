using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class AlphabetQuestionBuilder : IQuestionBuilder
    {
        public AlphabetQuestionBuilder(){}

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            packs.Add(CreateAlphabetQuestionPackData());
            return packs;
        }

        public QuestionPackData CreateAlphabetQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;

            // Parameters for builder's use of data
            var sParameters = new SelectionParameters(SelectionSeverity.AsManyAsPossible, 28);   // 28: letters in the alphabet

            // Fully ordered alphabet
            var alphabetLetters = teacher.wordAI.SelectLetters(() => teacher.wordHelper.GetAllRealLetters(), sParameters);
            alphabetLetters.Sort((x, y) =>
                {
                    return x.ToString().CompareTo(y.ToString());
                }
            );

            // Debug
            {
                string debugString = "Letters: " + alphabetLetters.Count;
                foreach (var l in alphabetLetters) debugString += " " + l;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.CreateFromCorrect(null, alphabetLetters);
        }

    }
}