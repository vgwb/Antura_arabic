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

        public DefaultQuestionGenerator( IQuestionProvider provider)
        {
            this.provider = provider;
            state = QuestionGeneratorState.Uninitialized;
        }

        public void InitRound()
        {
            if (state != QuestionGeneratorState.Uninitialized || state != QuestionGeneratorState.Completed)
                throw new InvalidOperationException("Cannot initialized");

            state = QuestionGeneratorState.Initialized;
        }

        private void ClearCache()
        {

        }

        public void CompleteRound()
        {
            if (state != QuestionGeneratorState.Initialized)
                throw new InvalidOperationException("Not Initialized");

            state = QuestionGeneratorState.Completed;
        }

        public IAnswer[] GetAllAnswers()
        {
            return null;
        }

        public IQuestion[] GetAllQuestions()
        {
            return null;
        }

        public IAnswer[] GetNextAnswers()
        {
            return null;
        }

        public IQuestion GetNextQuestion()
        {
            if (state != QuestionGeneratorState.Initialized)
                throw new InvalidOperationException( "Not Initialized");

            state = QuestionGeneratorState.QuestionFeeded;

            currentPack = provider.GetNextQuestion();

            List< ILivingLetterData> wrongAnswers = new List< ILivingLetterData>();
            List< ILivingLetterData> correctAnswers = new List< ILivingLetterData>();
            ILivingLetterData question = currentPack.GetQuestion();

            foreach (var wrong in currentPack.GetWrongAnswers())
                wrongAnswers.Add( wrong);

            foreach (var correct in currentPack.GetCorrectAnswers())
                correctAnswers.Add( correct);

            return null;
        }
    }
}
