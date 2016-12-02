using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// Question Generator for asessments that show Image
    /// </summary>
    public class ImageQuestionGenerator : IQuestionGenerator
    {
        private IQuestionProvider provider;
        private QuestionGeneratorState state;
        private IQuestionPack currentPack;
        private bool missingLetter;

        public ImageQuestionGenerator( IQuestionProvider provider , bool missingLetter)
        {
            this.provider = provider;
            this.missingLetter = missingLetter;
            state = QuestionGeneratorState.Uninitialized;
            ClearCache();
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
            totalAnswers = new List< IAnswer>();
            totalQuestions = new List< IQuestion>();
            partialAnswers = null;
        }

        public void CompleteRound()
        {
            if (state != QuestionGeneratorState.Initialized)
                throw new InvalidOperationException( "Not Initialized");

            state = QuestionGeneratorState.Completed;
        }

        public IAnswer[] GetAllAnswers()
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

        public IAnswer[] GetNextAnswers()
        {
            if (state != QuestionGeneratorState.QuestionFeeded)
                throw new InvalidOperationException( "Not Initialized");

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
            ILivingLetterData questionData = currentPack.GetQuestion();

            //____________________________________
            //Prepare answers for next method call
            //____________________________________

            if ( missingLetter)
            {
                // ### MISSING LETTER ###
                foreach (var wrong in currentPack.GetWrongAnswers())
                {
                    var wrongAnsw = GenerateWrongAnswer( wrong);

                    answers.Add( wrongAnsw);
                    totalAnswers.Add( wrongAnsw);
                }

                var correct = currentPack.GetCorrectAnswers().ToList()[ 0];
                var correctAnsw = GenerateCorrectAnswer( correct);

                answers.Add( correctAnsw);
                totalAnswers.Add( correctAnsw);

                partialAnswers = answers.ToArray();

                // Generate the question
                var question = GenerateMissingLetterQuestion( questionData, correct);
                totalQuestions.Add( question);
                GeneratePlaceHolder( question);
                return question;
            }
            else
            {
                // ### ORDER LETTERS ###
                foreach (var correct in currentPack.GetCorrectAnswers())
                {
                    var correctAnsw = GenerateCorrectAnswer( correct);
                    answers.Add( correctAnsw);
                    totalAnswers.Add( correctAnsw);
                }

                partialAnswers = answers.ToArray();

                // Generate the question
                var question = GenerateQuestion(questionData);
                totalQuestions.Add(question);

                return question;
            }
        }

        private IQuestion GenerateQuestion( ILivingLetterData data)
        {
            if (AssessmentConfiguration.Instance.ShowQuestionAsImage)
                data = new LL_ImageData( data.Id);

            var q = LivingLetterFactory.Instance.SpawnQuestion( data);
            return new DefaultQuestion( q, 0);
        }

        private const string RemovedLetterChar = "_";

        private IQuestion GenerateMissingLetterQuestion( ILivingLetterData data, ILivingLetterData letterToRemove)
        {
            var imageData = new LL_ImageData( data.Id);
            LL_WordData word = (LL_WordData)data;
            LL_LetterData letter = (LL_LetterData)letterToRemove;
            var Letters = ArabicAlphabetHelper.ExtractLetterDataFromArabicWord( word.Data.Arabic);
            
            int index = 0;
            for (; index < Letters.Count; ++index)
            {
                if (Letters[index].Id == letter.Id)
                {
                    break;
                }
            }

            var wordGO = LivingLetterFactory.Instance.SpawnQuestion( word);
            wordGO.Label.text = wordGO.Label.text.Remove( index, 1);
            wordGO.Label.text = wordGO.Label.text.Insert( index, RemovedLetterChar);

            return new ImageQuestion( wordGO, imageData);
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
