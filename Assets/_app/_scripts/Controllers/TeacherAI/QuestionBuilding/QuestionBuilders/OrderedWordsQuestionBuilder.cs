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

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            packs.Add(CreateSingleQuestionPackData());
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData()
        {
            var db = AppManager.Instance.DB;

            // Ordered words
            var correctAnswers = db.FindWordData(x => x.Category == category);
            correctAnswers.Sort((x, y) =>
                {
                    return x.ToString().CompareTo(y.ToString());
                }
            );

            return QuestionPackData.CreateFromCorrect(null, correctAnswers);
        }

    }
}