using EA4S.MinigamesAPI;
using Kore.Coroutines;
using System;
using System.Collections;
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

        public DefaultQuestionGenerator(    IQuestionProvider provider, AssessmentDialogues dialogues,
                                            AssessmentEvents events)
        {
            this.provider = provider;
            this.dialogues = dialogues;

            if( AssessmentOptions.Instance.ReadQuestionAndAnswer)
                events.OnAllQuestionsAnswered = ReadQuestionAndReplyEvent;

            state = QuestionGeneratorState.Uninitialized;
            ClearCache();
        }

        IEnumerator ReadQuestionAndReplyEvent()
        {
            yield return Koroutine.Nested( 
                    dialogues.PlayLetterDataCoroutine( cacheQuestionToRead));

            yield return Wait.For( 0.3f);

            yield return Koroutine.Nested(
                    dialogues.PlayLetterDataCoroutine( cacheAnswerToRead));
        }

        public void InitRound()
        {
            if (state != QuestionGeneratorState.Uninitialized && state != QuestionGeneratorState.Completed)
                throw new InvalidOperationException( "Cannot initialized");

            state = QuestionGeneratorState.Initialized;
            ClearCache();
        }

        private void ClearCache()
        {
            totalAnswers = new List< Answer>();
            totalQuestions = new List< IQuestion>();
            partialAnswers = null;
        }

        public void CompleteRound()
        {
            if (state != QuestionGeneratorState.Initialized)
                throw new InvalidOperationException( "Not Initialized");

            state = QuestionGeneratorState.Completed;
        }

        public Answer[] GetAllAnswers()
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

        public Answer[] GetNextAnswers()
        {
            if (state != QuestionGeneratorState.QuestionFeeded)
                throw new InvalidOperationException("Not Initialized");

            state = QuestionGeneratorState.Initialized;
            return partialAnswers;
        }

        List< Answer> totalAnswers;
        List< IQuestion> totalQuestions;
        Answer[] partialAnswers;
        private AssessmentDialogues dialogues;

        public IQuestion GetNextQuestion()
        {
            if (state != QuestionGeneratorState.Initialized)
                throw new InvalidOperationException( "Not Initialized");

            state = QuestionGeneratorState.QuestionFeeded;

            currentPack = provider.GetNextQuestion();

            List< Answer> answers = new List< Answer>();
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

            int correctCount = 0;
            foreach (var correct in currentPack.GetCorrectAnswers())
            {
                var correctAnsw = GenerateCorrectAnswer( correct);
                correctCount++;
                answers.Add( correctAnsw);
                totalAnswers.Add( correctAnsw);
            }

            partialAnswers = answers.ToArray();

            // Generate the question
            var question = GenerateQuestion( questionData, correctCount);
            totalQuestions.Add( question);

            // Generate placeholders
            foreach (var correct in currentPack.GetCorrectAnswers())
                GeneratePlaceHolder( question, AssessmentOptions.Instance.AnswerType);

            return question;
        }

        ILivingLetterData cacheQuestionToRead;
        ILivingLetterData cacheAnswerToRead;

        private IQuestion GenerateQuestion( ILivingLetterData data, int correctCount)
        {
            cacheQuestionToRead = data;
            if (AssessmentOptions.Instance.ShowQuestionAsImage)
                data = new LL_ImageData( data.Id);

            var q = LivingLetterFactory.Instance.SpawnQuestion( data);

            if (AssessmentOptions.Instance.QuestionAnsweredFlip)
                q.GetComponent< StillLetterBox>().HideHiddenQuestion();

            return new DefaultQuestion( q, correctCount, dialogues);
        }

        private Answer GenerateWrongAnswer( ILivingLetterData wrongAnswer)
        {
            return
            LivingLetterFactory.Instance.SpawnAnswer( wrongAnswer, false, dialogues);
        }

        private void GeneratePlaceHolder( IQuestion question, LivingLetterDataType dataType)
        {
            var placeholder = LivingLetterFactory.Instance.SpawnPlaceholder( dataType);
            placeholder.InstaShrink();
            question.TrackPlaceholder( placeholder.gameObject);
        }

        private Answer GenerateCorrectAnswer( ILivingLetterData correctAnswer)
        {
            cacheAnswerToRead = correctAnswer;

            return
            LivingLetterFactory.Instance.SpawnAnswer( correctAnswer, true, dialogues);
        }
    }
}
