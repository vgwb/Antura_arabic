using System;
using System.Collections.Generic;

namespace EA4S.Assessment
{
    public enum QuestionGeneratorState
    {
        Uninitialized,
        Initialized,
        QuestionFeeded,
        Completed
    }

    /// <summary>
    /// Question Generator for most assessments.
    /// </summary>
    public class DefaultQuestionGenerator : IQuestionGenerator
    {
        private IQuestionProvider provider;
        private QuestionGeneratorState state;
        private IQuestionPack currentPack;
        private QuestionType questionType;

        public DefaultQuestionGenerator( IQuestionProvider provider, QuestionType type)
        {
            this.provider = provider;
            state = QuestionGeneratorState.Uninitialized;
            questionType = type;
            ClearCache();
        }

        public void InitRound()
        {
            if (state != QuestionGeneratorState.Uninitialized || state != QuestionGeneratorState.Completed)
                throw new InvalidOperationException("Cannot initialized");

            state = QuestionGeneratorState.Initialized;
            ClearCache();
        }

        private void ClearCache()
        {
            totalAnswers = new List< IAnswer>();
            totalQuestions = new List< IQuestion>();
            partialAnswers = null;
        }

        public void CompleteRound()
        {
            if (state != QuestionGeneratorState.Initialized)
                throw new InvalidOperationException("Not Initialized");

            state = QuestionGeneratorState.Completed;
        }

        public IAnswer[] GetAllAnswers()
        {
            if (state != QuestionGeneratorState.Completed)
                throw new InvalidOperationException("Not Completed");

            return totalAnswers.ToArray();
        }

        public IQuestion[] GetAllQuestions()
        {
            if (state != QuestionGeneratorState.Completed)
                throw new InvalidOperationException("Not Completed");

            return totalQuestions.ToArray();
        }

        public IAnswer[] GetNextAnswers()
        {
            if (state != QuestionGeneratorState.QuestionFeeded)
                throw new InvalidOperationException("Not Initialized");

            state = QuestionGeneratorState.Initialized;
            return partialAnswers;
        }

        List< IAnswer> totalAnswers;
        List< IQuestion> totalQuestions;
        IAnswer[] partialAnswers;

        public IQuestion GetNextQuestion()
        {
            if (state != QuestionGeneratorState.Initialized)
                throw new InvalidOperationException( "Not Initialized");

            state = QuestionGeneratorState.QuestionFeeded;

            currentPack = provider.GetNextQuestion();

            List< IAnswer> answers = new List< IAnswer>();
            ILivingLetterData question = currentPack.GetQuestion();

            //____________________________________
            //Prepare answers for next method call
            //____________________________________

            foreach (var wrong in currentPack.GetWrongAnswers())
            {
                var wrongAnsw = GenerateWrongAnswer( wrong);

                answers.Add( wrongAnsw);
                totalAnswers.Add( wrongAnsw);
            }

            int correctCount = 0;
            foreach (var correct in currentPack.GetCorrectAnswers())
            {
                var correctAnsw = GenerateCorrectAnswer( correct);

                correctCount++;
                answers.Add( correctAnsw);
                totalAnswers.Add( correctAnsw);
            }

            partialAnswers = answers.ToArray();

            return GenerateQuestion( question, correctCount);
        }

        private IQuestion GenerateQuestion( ILivingLetterData data, int correctCount)
        {
            var q = LivingLetterFactory.Instance.SpawnQuestion( data);
            return new DefaultQuestion( q, correctCount, questionType);
        }

        private IAnswer GenerateWrongAnswer( ILivingLetterData wrongAnswer)
        {
            return new DefaultAnswer( 
                LivingLetterFactory.Instance.SpawnAnswer( wrongAnswer)

                //wrong
                , false);
        }

        private IAnswer GenerateCorrectAnswer( ILivingLetterData correctAnswer)
        {
            return new DefaultAnswer(
                LivingLetterFactory.Instance.SpawnAnswer( correctAnswer)

                //correct
                , true);
        }
    }
}
