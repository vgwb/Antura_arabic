using EA4S.API;
using System.Collections.Generic;

namespace EA4S
{
    public class AlphabetQuestionBuilder : IQuestionBuilder
    {

        public AlphabetQuestionBuilder()
        {}

        public int GetQuestionPackCount()
        {
            return 1;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            //var db = AppManager.Instance.DB;
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

        public IQuestionPack CreateQuestionPack()
        {
            throw new System.Exception("DEPRECATED");
        }

    }
}