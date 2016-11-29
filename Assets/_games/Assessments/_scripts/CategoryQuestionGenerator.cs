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
        private List<ILivingLetterData>[] answersBuckets;
        private ICategoryProvider categoryProvider;

        public CategoryQuestionGenerator( IQuestionProvider provider, ICategoryProvider categoryProvider, int maxAnsw, int rounds)
        {
            Debug.Log("CategoryQuestionGenerator CONSTRUCTOR");
            this.provider = provider;
            state = QuestionGeneratorState.Uninitialized;
            numberOfMaxAnswers = maxAnsw;
            numberOfCategories = categoryProvider.GetCategories();
            Debug.Log("numberOfCategories" + numberOfCategories);
            numberOfRounds = rounds;
            answersBuckets = new List<ILivingLetterData>[ numberOfCategories];
            this.categoryProvider = categoryProvider;
            for (int i = 0; i < numberOfCategories; i++)
                answersBuckets[i] = new List<ILivingLetterData>();
                        
            ClearCache();
            FillBuckets();
        }

        private void FillBuckets()
        {
            // We need to aggregate answers before so we can later generate Questions
            int max = numberOfRounds * numberOfMaxAnswers;
            for (int i = 0; i < max; i++)
            {
                var pack = provider.GetNextQuestion();
                foreach( var answ in pack.GetCorrectAnswers())
                {
                    for (int j = 0; j < numberOfCategories; j++)
                        if (categoryProvider.Category(j) == answ.TextForLivingLetter)
                        {
                            Debug.Log("AddedQuestion: "+ pack.GetQuestion().TextForLivingLetter+
                                      "  -- In Category:" + answ.TextForLivingLetter);
                            answersBuckets[j].Add(pack.GetQuestion());
                        }
                }
            }

            if (numberOfCategories > 0)
                category1 = answersBuckets[0].Count;

            if (numberOfCategories > 1)
                category2 = answersBuckets[1].Count;

            if (numberOfCategories > 2)
                category3 = answersBuckets[2].Count;

            Debug.Log("category1" + category1);
            Debug.Log("category2" + category2);
            Debug.Log("category3" + category3);
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
        }

        private void ClearCache()
        {
            totalAnswers = new List< IAnswer>();
            totalQuestions = new List< IQuestion>();
            partialAnswers = null;
            currentCategory = 0;
            category1 = 0;
            category2 = 0;
            category3 = 0;

            if (numberOfCategories == 2)
                DistributeRandom2();
            else
                DistributeRandom3();

            MakeDistributionConsistent();
        }

        private void DistributeRandom3()
        {
            for (int i = 0; i < numberOfMaxAnswers; i++)
                switch (UnityEngine.Random.Range(1, 3))
                {
                    case 1: category1++; break;
                    case 2: category2++; break;
                    case 3: category3++; break;
                }
        }

        private void DistributeRandom2()
        {
            for (int i = 0; i < numberOfMaxAnswers; i++)
                if (UnityEngine.Random.Range( 1, 3)== 1)
                    category1++;
                else
                    category2++;
        }


        // Use buckets only if they have enough elements
        private void MakeDistributionConsistent()
        {
            Debug.Log("MakeDistributionConsistent");
            if (category1 > answersBuckets[0].Count)
            {
                var diff = category1 - answersBuckets[0].Count;
                category1 -= diff;
                category2 += diff;
            }

            Debug.Log("AnswersBuckets: " + answersBuckets + " -- answersBuckets[1]: " + answersBuckets[1]);
            if (numberOfCategories > 1 )
            if (category2 > answersBuckets[1].Count)
            {
                var diff = category2 - answersBuckets[1].Count;
                category2 -= diff;
                category3 += diff;
            }

            if(numberOfCategories>2)
            if (category3 > answersBuckets[2].Count)
            {
                var diff = category3 - answersBuckets[2].Count;
                category3 -= diff;
            }

            const string errorMessage = "Cannot have all categories empty! Inconsistent with input parameters";

            if (category1 + category2 + category3 > 0)
                return;

            if ( numberOfCategories == 3 && category3 <= 0)
                throw new InvalidOperationException( errorMessage);

            if ( numberOfCategories == 2 && category2 <= 0)
                throw new InvalidOperationException( errorMessage);

            if ( numberOfCategories == 1 && category1 <= 0)
                throw new InvalidOperationException( errorMessage);
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

        List<IAnswer> totalAnswers;
        List<IQuestion> totalQuestions;
        IAnswer[] partialAnswers;

        private int currentCategory;
        private int category1;
        private int category2;
        private int category3;

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
                amount = category1;
            else
            if (currentCategory == 1)
                amount = category2;
            else
            if (currentCategory == 2)
                amount = category3;

            List< IAnswer> answers = new List< IAnswer>();

            int correctCount = 0;
            for(int i=0; i<amount; i++)
            {
                var answer = answersBuckets[currentCategory].Pull();
                var correctAnsw = GenerateCorrectAnswer( answer);

                correctCount++;
                answers.Add( correctAnsw);
                totalAnswers.Add( correctAnsw);
                Debug.Log("Added answer");
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
            var q = categoryProvider.SpawnCustomObject( currentCategory, true);
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
