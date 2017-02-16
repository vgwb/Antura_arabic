using EA4S.Helpers;
using EA4S.MinigamesAPI;
using Kore.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        // ##################################
        //             INIT
        // ##################################

        public DefaultQuestionGenerator(    IQuestionProvider provider, AssessmentAudioManager dialogues,
                                            AssessmentEvents events,
                                            DefaultQuestionType config)
        {
            this.provider = provider;
            this.dialogues = dialogues;
            this.config = config;

            if (config == DefaultQuestionType.MissingForm)
                events.OnAllQuestionsAnswered = CompleteWordsWithForm;

            if ( AssessmentOptions.Instance.ReadQuestionAndAnswer)
                events.OnAllQuestionsAnswered = ReadQuestionAndReplyEvent;

            state = QuestionGeneratorState.Uninitialized;
            ClearCache();
        }

        public DefaultQuestionGenerator(    IQuestionProvider provider, AssessmentAudioManager dialogues,
                                            AssessmentEvents events)

           : this( provider, dialogues, events, DefaultQuestionType.Default)
        {

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
            cacheWordsToComplete = new List< StillLetterBox>();
            cacheCompleteWords = new List< string>();
            partialAnswers = null;
        }

        // ##################################
        //             EVENTS
        // ##################################

        List< StillLetterBox> cacheWordsToComplete;
        List< string> cacheCompleteWords;

        private void AddCompleteWordEvent( string completeText, StillLetterBox target)
        {
            cacheWordsToComplete.Add( target);
            cacheCompleteWords.Add( completeText);
        }

        private IEnumerator CompleteWordsWithForm()
        {
            yield return Wait.For( 0.5f);

            StillLetterBox[] boxes = cacheWordsToComplete.ToArray();
            string[] words = cacheCompleteWords.ToArray();

            Debug.Assert( boxes.Length == words.Length);
            
            for(int i=0; i<boxes.Length; i++)
            {
                dialogues.PlayPoofSound();
                boxes[i].Poof();
                boxes[i].Label.text = words[i];
                yield return Wait.For( AssessmentOptions.Instance.TimeToShowCompleteWord);
            }

            yield return Wait.For( 0.3f);
        }

        ILivingLetterData cacheQuestionToRead;
        ILivingLetterData cacheAnswerToRead;

        IEnumerator ReadQuestionAndReplyEvent()
        {
            yield return Koroutine.Nested( 
                    dialogues.PlayLetterDataCoroutine( cacheQuestionToRead));

            yield return Wait.For( 0.3f);

            yield return Koroutine.Nested(
                    dialogues.PlayLetterDataCoroutine( cacheAnswerToRead));
        }

        // ##################################
        //           IMPLEMENTATION
        // ##################################

        public void CompleteRound()
        {
            if (state != QuestionGeneratorState.Initialized)
                throw new InvalidOperationException( "Not Initialized");

            state = QuestionGeneratorState.Completed;
        }

        public Answer[] GetAllAnswers()
        {
            if (state != QuestionGeneratorState.Completed)
                throw new InvalidOperationException( "Not Completed");

            return totalAnswers.ToArray();
        }

        public IQuestion[] GetAllQuestions()
        {
            if (state != QuestionGeneratorState.Completed)
                throw new InvalidOperationException( "Not Completed");

            return totalQuestions.ToArray();
        }

        public Answer[] GetNextAnswers()
        {
            if (state != QuestionGeneratorState.QuestionFeeded)
                throw new InvalidOperationException( "Not Initialized");

            state = QuestionGeneratorState.Initialized;
            return partialAnswers;
        }

        List< Answer> totalAnswers;
        List< IQuestion> totalQuestions;
        Answer[] partialAnswers;
        private AssessmentAudioManager dialogues;

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

        // ##################################
        //           AUXILIARY METHODS
        // ##################################

        private IQuestion CustomQuestion()
        {
            var correct = currentPack.GetCorrectAnswers().ToList()[0];
            
            // Generate the question (and set form for correct letter)
            var question = GenerateCustomQuestion( currentPack.GetQuestion(), correct as LL_LetterData);
            totalQuestions.Add( question);
            GeneratePlaceHolder( question, AssessmentOptions.Instance.AnswerType);

            // Generate Answer after having setted the correct form
            var correctAnsw = GenerateCorrectAnswer( correct);
            partialAnswers = new Answer[1];
            partialAnswers[0] = correctAnsw;
            totalAnswers.Add( correctAnsw);

            return question;
        }

        private const string RemovedLetterChar = "_";

        private IQuestion GenerateCustomQuestion(ILivingLetterData question, LL_LetterData correctLetter)
        {
            LL_WordData word = question as LL_WordData;
            var wordGO = LivingLetterFactory.Instance.SpawnQuestion(word);

            var partsToRemove = ArabicAlphabetHelper.FindLetter(AppManager.I.DB, word.Data, correctLetter.Data);
            partsToRemove.Shuffle(); //pick a random letter

            string text = ArabicAlphabetHelper.GetWordWithMissingLetterText(
                word.Data, partsToRemove[0], RemovedLetterChar);

            if (config == DefaultQuestionType.MissingForm)
            {
                wordGO.Label.text = text;
                AddCompleteWordEvent(word.TextForLivingLetter, wordGO);
            }

            wordGO.InstaShrink();
            correctLetter.Form = partsToRemove[0].letterForm;

            return new DefaultQuestion(wordGO, 1, dialogues);
        }



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
