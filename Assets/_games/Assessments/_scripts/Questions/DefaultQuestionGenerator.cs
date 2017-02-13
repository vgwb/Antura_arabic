using EA4S.Helpers;
using EA4S.MinigamesAPI;
using Kore.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EA4S.Assessment
{
    public enum QuestionGeneratorState
    {
        Uninitialized,
        Initialized,
        QuestionFeeded,
        Completed
    }

    public enum DefaultQuestionType
    {
        Default = 0,
        MissingForm,
        VisibleForm
    }

    /// <summary>
    /// Question Generator for most assessments.
    /// </summary>
    public class DefaultQuestionGenerator : IQuestionGenerator
    {
        private IQuestionProvider provider;
        private QuestionGeneratorState state;
        private IQuestionPack currentPack;
        DefaultQuestionType config;

        public DefaultQuestionGenerator(    IQuestionProvider provider, AssessmentDialogues dialogues,
                                            AssessmentEvents events,
                                            DefaultQuestionType config)
        {
            this.provider = provider;
            this.dialogues = dialogues;
            this.config = config;

            if( AssessmentOptions.Instance.ReadQuestionAndAnswer)
                events.OnAllQuestionsAnswered = ReadQuestionAndReplyEvent;

            state = QuestionGeneratorState.Uninitialized;
            ClearCache();
        }

        public DefaultQuestionGenerator(    IQuestionProvider provider, AssessmentDialogues dialogues,
                                            AssessmentEvents events)

           : this( provider, dialogues, events, DefaultQuestionType.Default)
        {

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

            if (config != DefaultQuestionType.Default)
                return CustomQuestion();

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

        private IQuestion CustomQuestion()
        {
            var correct = currentPack.GetCorrectAnswers().ToList()[0];
            var correctAnsw = GenerateCorrectAnswer( correct);

            partialAnswers = new Answer[1];
            partialAnswers[0] = correctAnsw;
            totalAnswers.Add( correctAnsw);

            // Generate the question
            var question = GenerateCustomQuestion( currentPack.GetQuestion(), correct as LL_LetterData, correctAnsw);
            totalQuestions.Add( question);
            GeneratePlaceHolder( question, AssessmentOptions.Instance.AnswerType);
            return question;
        }

        private const string RemovedLetterChar = "_";

        private IQuestion GenerateCustomQuestion(ILivingLetterData question, LL_LetterData correctLetter, Answer answer)
        {
            Database.LetterForm questionLetterForm;
            LL_WordData word = question as LL_WordData;
            var wordGO = LivingLetterFactory.Instance.SpawnQuestion(word);

            string text = ArabicAlphabetHelper.GetWordWithMissingLetterText(
                            out questionLetterForm, word.Data,
                            correctLetter.Data, RemovedLetterChar);

            if (config == DefaultQuestionType.MissingForm)
            {
                //TODO save a chain of lambdas to restore original words
                // The restore coroutine should show poof and restore original text
                wordGO.Label.text = text;
            }
            
            wordGO.InstaShrink();
            (answer.Data() as LL_LetterData).Form = questionLetterForm;

            return new DefaultQuestion( wordGO, 1, dialogues);
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
