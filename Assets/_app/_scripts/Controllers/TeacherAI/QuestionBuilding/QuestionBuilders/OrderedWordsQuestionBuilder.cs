using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class OrderedWordsQuestionBuilder : IQuestionBuilder
    {
        // focus: Words
        // pack history filter: only 1 pack
        // journey: enabled

        Db.WordDataCategory category;
        SelectionSeverity severity;

        public OrderedWordsQuestionBuilder(Db.WordDataCategory category, SelectionSeverity severity = SelectionSeverity.AsManyAsPossible)
        {
            this.category = category;
            this.severity = severity;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            packs.Add(CreateSingleQuestionPackData());
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;

            // Ordered words
            var words = teacher.wordAI.SelectData(
                 () => teacher.wordHelper.GetWordsByCategory(category, new WordFilters()),
                 new SelectionParameters(severity, 100)    // @todo: use a number that means 'all'
               );

            words.Sort((x, y) =>
                {
                    return x.ToString().CompareTo(y.ToString());
                }
            );

            if (ConfigAI.verboseTeacher)
            {
                string debugString = "Words: " + words.Count;
                foreach (var w in words) debugString += " " + w;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.CreateFromCorrect(null, words);
        }

    }
}