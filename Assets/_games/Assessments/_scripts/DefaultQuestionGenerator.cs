using System;
using System.Collections.Generic;
using UnityEngine;

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
            ClearCache();
        }

        public void InitRound()
        {
            if (state != QuestionGeneratorState.Uninitialized && state != QuestionGeneratorState.Completed)
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
            Debug.Log("GetNextQuestion");
            if (state != QuestionGeneratorState.Initialized)
                throw new InvalidOperationException( "Not Initialized");

            state = QuestionGeneratorState.QuestionFeeded;

            currentPack = provider.GetNextQuestion();

            List< IAnswer> answers = new List< IAnswer>();
            ILivingLetterData questionData = currentPack.GetQuestion();

            //____________________________________
            //Prepare answers for next method call
            //____________________________________


            foreach (var wrong in currentPack.GetWrongAnswers())
            {
                var wrongAnsw = GenerateWrongAnswer( wrong);

                answers.Add( wrongAnsw);
                totalAnswers.Add( wrongAnsw);
            }

            Debug.Log("PRE");
            int correctCount = 0;
            foreach (var correct in currentPack.GetCorrectAnswers())
            {
                var correctAnsw = GenerateCorrectAnswer( correct);
                Debug.Log("Added");
                correctCount++;
                answers.Add( correctAnsw);
                totalAnswers.Add( correctAnsw);
            }
            Debug.Log("POST");

            partialAnswers = answers.ToArray();

            // Generate the question
            var question = GenerateQuestion( questionData, correctCount);
            totalQuestions.Add( question);

            // Generate placeholders
            foreach (var correct in currentPack.GetCorrectAnswers())
                GeneratePlaceHolder( question);

            return question;
        }

        private IQuestion GenerateQuestion( ILivingLetterData data, int correctCount)
        {   
            if(AssessmentConfiguration.Instance.ShowQuestionAsImage)
                data = new LL_ImageData(data.Id);

            var q = LivingLetterFactory.Instance.SpawnQuestion( data);
            return new DefaultQuestion( q, correctCount);
        }

        private IAnswer GenerateWrongAnswer( ILivingLetterData wrongAnswer)
        {
            return new DefaultAnswer( 
                LivingLetterFactory.Instance.SpawnAnswer( wrongAnswer)

                //wrong
                , false);
        }

        private void GeneratePlaceHolder( IQuestion question)
        {
            var placeholder = LivingLetterFactory.Instance.SpawnCustomElement( CustomElement.Placeholder).transform;
            placeholder.localPosition = new Vector3( 0, 5, 0);
            placeholder.localScale = Vector3.zero;
            question.TrackPlaceholder( placeholder.gameObject);
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
