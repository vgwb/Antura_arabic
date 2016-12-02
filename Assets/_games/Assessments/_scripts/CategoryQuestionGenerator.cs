using System;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// Question Generator for assessments that requires to categorize something
    /// </summary>
    public class CategoryQuestionGenerator : IQuestionGenerator
    {
        private IQuestionProvider provider;
        private QuestionGeneratorState state;
        private IQuestionPack currentPack;
        private int numberOfCategories;
        private int numberOfMaxAnswers;
        private int numberOfRounds;
        private List< ILivingLetterData>[] answersBuckets;
        private ICategoryProvider categoryProvider;

        public CategoryQuestionGenerator( IQuestionProvider provider, ICategoryProvider categoryProvider, int maxAnsw, int rounds)
        {
            this.provider = provider;
            state = QuestionGeneratorState.Uninitialized;
            numberOfMaxAnswers = maxAnsw;
            numberOfCategories = categoryProvider.GetCategories();
            numberOfRounds = rounds;
            answersBuckets = new List< ILivingLetterData>[ 3];
            this.categoryProvider = categoryProvider;
            for (int i = 0; i < 3; i++)
                answersBuckets[i] = new List< ILivingLetterData>();
                        
            ClearCache();
            FillBuckets();
        }

        // Unluckily the question provider can be used only in the following mode
        // because of how API was designed.
        private void TakeAnswersFromBuckets()
        {
            category1ForThisRound = 0;
            category2ForThisRound = 0;
            category3ForThisRound = 0;

            Debug.Log("Bucket1:" + answersBuckets[0].Count);
            Debug.Log("Bucket2:" + answersBuckets[1].Count);
            Debug.Log("Bucket3:" + answersBuckets[2].Count);

            int picksThisRound = numberOfMaxAnswers;
            int totalAnswers = answersBuckets[0].Count + answersBuckets[1].Count + answersBuckets[2].Count;

            while ( picksThisRound > 0 && totalAnswers>0)
            {
                int pickFromBucketN = -1;

                //ok as long as we have 10 or less buckets
                // try to be fair (but never use infinite loop.)
                for( int i=0; i<1000000; i++)
                {
                    int temp = UnityEngine.Random.Range( 0, 3);
                    if ( answersBuckets[ temp].Count > 0)
                    {
                        pickFromBucketN = temp;
                        break;
                    }

                }

                if (pickFromBucketN == -1)
                {
                    //and use a little bias if computation took to long.
                    for(int i=0; i<3; i++)
                    {
                        if (answersBuckets[i].Count > 0)
                        {
                            pickFromBucketN = i;
                            break;
                        }
                    }
                }

                if (pickFromBucketN == -1)
                    throw new InvalidOperationException( "buckets empty");

                picksThisRound--;
                totalAnswers--;

                switch (pickFromBucketN)
                {
                    case 0: category1ForThisRound++; break;
                    case 1: category2ForThisRound++; break;
                    default: category3ForThisRound++; break;
                }

            }

            if ( picksThisRound == numberOfMaxAnswers)
                throw new InvalidOperationException( "buckets empty");
        }

        private void FillBuckets()
        {
            // We need to aggregate answers before so we can later generate Questions
            int max = numberOfRounds * numberOfMaxAnswers;
            for (int i = 0; i < max; i++)
            {
                var pack = provider.GetNextQuestion();
                foreach (var answ in pack.GetCorrectAnswers())
                    for (int j = 0; j < numberOfCategories; j++)
                    {
                        Debug.Log("##CATEGORY:"+ answ.TextForLivingLetter);
                        if (categoryProvider.Compare( j, answ))
                        {
                            Debug.Log("##ADDED");
                            answersBuckets[j].Add(pack.GetQuestion());
                        }
                    }
            }
        }

        private IAnswer GenerateCorrectAnswer( ILivingLetterData correctAnswer)
        {
            return new DefaultAnswer(
                LivingLetterFactory.Instance.SpawnAnswer( correctAnswer)

                //correct
                , true);
        }

        public void InitRound()
        {
            if (state != QuestionGeneratorState.Uninitialized && state != QuestionGeneratorState.Completed)
                throw new InvalidOperationException( "Cannot initialized");

            state = QuestionGeneratorState.Initialized;
            ClearCache();
            TakeAnswersFromBuckets();
        }

        private void ClearCache()
        {
            totalAnswers = new List< IAnswer>();
            totalQuestions = new List< IQuestion>();
            partialAnswers = null;
            currentCategory = 0;
        }

        public void CompleteRound()
        {
            if (state != QuestionGeneratorState.Initialized)
                throw new InvalidOperationException( "Not Initialized");

            state = QuestionGeneratorState.Completed;
            currentCategory = 0;
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

        List<IAnswer> totalAnswers;
        List<IQuestion> totalQuestions;
        IAnswer[] partialAnswers;

        private int currentCategory;

        // Categories
        private int category1ForThisRound;
        private int category2ForThisRound;
        private int category3ForThisRound;

        public IQuestion GetNextQuestion()
        {
            if (state != QuestionGeneratorState.Initialized)
                throw new InvalidOperationException( "Not Initialized");

            state = QuestionGeneratorState.QuestionFeeded;

            //____________________________________
            //Prepare answers for next method call
            //____________________________________

            // Assumption: Here each category have enough elements
            int amount = 0;
            if (currentCategory == 0)
                amount = category1ForThisRound;
            else
            if (currentCategory == 1)
                amount = category2ForThisRound;
            else
            if (currentCategory == 2)
                amount = category3ForThisRound;

            List< IAnswer> answers = new List< IAnswer>();

            int correctCount = 0;
            for(int i=0; i<amount; i++)
            {
                // If crashed here => not enough buckets => Because teacher cannot find enough data
                // Session number X.X.XX should probably raised a bit.
                var answer = answersBuckets[currentCategory].Pull();
                var correctAnsw = GenerateCorrectAnswer( answer);

                correctCount++;
                answers.Add( correctAnsw);
                totalAnswers.Add( correctAnsw);
            }

            partialAnswers = answers.ToArray();

            // Generate the question
            var question = GenerateQuestion( correctCount);
            totalQuestions.Add( question);

            // Generate placeholders
            for (int i=0; i<numberOfMaxAnswers; i++)
                GeneratePlaceHolder( question);

            currentCategory++;
            return question;
        }

        private IQuestion GenerateQuestion( int correctCount)
        {
            var q = categoryProvider.SpawnCustomObject( currentCategory);
            return new CategoryQuestion( q, correctCount);
        }

        private void GeneratePlaceHolder( IQuestion question)
        {
            var placeholder = LivingLetterFactory.Instance.SpawnCustomElement( CustomElement.Placeholder).transform;
            placeholder.localPosition = new Vector3( 0, 5, 0);
            placeholder.localScale = Vector3.zero;
            question.TrackPlaceholder( placeholder.gameObject);
        }
    }
}
