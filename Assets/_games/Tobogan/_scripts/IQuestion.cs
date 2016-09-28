using System.Collections.Generic;

namespace EA4S.Tobogan
{
    interface IQuestion
    {
        IQuestionSentence GetQuestion();
        IEnumerable<IQuestionSentence> GetWrongAnswers();
        IQuestionSentence GetCorrectAnswer();
    }
}
