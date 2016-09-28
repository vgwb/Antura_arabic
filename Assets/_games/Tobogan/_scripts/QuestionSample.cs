using System.Collections.Generic;

namespace EA4S.Tobogan
{
    class QuestionSample : IQuestion
    {
        IQuestionSentence questionSentence;
        IEnumerable<IQuestionSentence> wrongAnswersSentence;
        IQuestionSentence correctAnswerSentence;

        public QuestionSample(IQuestionSentence questionSentence, IEnumerable<IQuestionSentence> wrongAnswersSentence, IQuestionSentence correctAnswerSentence)
        {
            this.questionSentence = questionSentence;
            this.wrongAnswersSentence = wrongAnswersSentence;
            this.correctAnswerSentence = correctAnswerSentence;
        }

        public IQuestionSentence GetQuestion()
        {
            return questionSentence;
        }

        public IEnumerable<IQuestionSentence> GetWrongAnswers()
        {
            return wrongAnswersSentence;
        }

        public IQuestionSentence GetCorrectAnswer()
        {
            return correctAnswerSentence;
        }
    }
}
