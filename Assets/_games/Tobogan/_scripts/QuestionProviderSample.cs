namespace EA4S.Tobogan
{
    class QuestionProviderSample : IQuestionProvider
    {
        string description;
        IQuestion[] questions;

        int currentQuestion;

        public QuestionProviderSample()
        {
            currentQuestion = 0;

            description = "description of questions";

            QuestionSample[] newQuestions = new QuestionSample[4];

            for (int i = 0; i < newQuestions.Length; i++)
            {
                QuestionSentenceSample questionSentence = new QuestionSentenceSample(0, QuestionSentenceType.WORD);

                QuestionSentenceSample[] wrongAnswersSentence = new QuestionSentenceSample[3];

                for (int j = 0; j < wrongAnswersSentence.Length; j++)
                {
                    wrongAnswersSentence[j] = new QuestionSentenceSample(1 + j, QuestionSentenceType.LETTER);
                }

                QuestionSentenceSample correctAnswerSentence = new QuestionSentenceSample(5, QuestionSentenceType.LETTER);

                newQuestions[i] = new QuestionSample(questionSentence, wrongAnswersSentence, correctAnswerSentence);
            }

            questions = newQuestions;
        }

        public string GetDescription()
        {
            return description;
        }

        public IQuestion GetNextQuestion()
        {
            currentQuestion++;

            if (currentQuestion >= questions.Length)
                currentQuestion = 0;

            return questions[currentQuestion];
        }
    }
}
