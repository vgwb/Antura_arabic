using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class EmptyQuestionBuilder : IQuestionBuilder
    {

        public EmptyQuestionBuilder()
        {
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            return new List<QuestionPackData>();
        }

    }
}