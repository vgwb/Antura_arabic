using EA4S.API;
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

            // Fully ordered alphabet (@todo: check that it works!)
            var correctAnswers = teacher.wordHelper.GetAllRealLetters();
            correctAnswers.Sort((x, y) =>
                {
                    return x.ToString().CompareTo(y.ToString());
                }
            );

            return QuestionPackData.CreateFromCorrect(null, correctAnswers);
        }

    }
}