using System.Collections.Generic;

namespace EA4S.Teacher
{
    /// <summary>
    /// Fake question builder used in development.
    /// </summary>
    public class EmptyQuestionBuilder : IQuestionBuilder
    {

        public EmptyQuestionBuilder()
        {
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            return new List<QuestionPackData>();
        }

        public QuestionBuilderParameters Parameters
        {
            get { return null; }
        }
    }
}