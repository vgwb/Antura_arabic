using EA4S.API;
using System.Collections.Generic;

namespace EA4S
{
    public class OrderedWordsQuestionBuilder : IQuestionBuilder
    {
        Db.WordDataCategory category;

        public OrderedWordsQuestionBuilder(Db.WordDataCategory _category)
        {
            this.category = _category;
        }

        public int GetQuestionPackCount()
        {
            return 1;
        }

        public QuestionPackData CreateQuestionPackData()
        {
            var db = AppManager.Instance.DB;

            // Ordered words (@todo: check that it works!)
            var correctAnswers = db.FindWordData(x => x.Category == category);
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