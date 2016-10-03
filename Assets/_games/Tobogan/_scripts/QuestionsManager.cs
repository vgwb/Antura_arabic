using UnityEngine;
using System.Collections.Generic;
using System;
namespace EA4S.Tobogan
{
    public class QuestionsManager
    {
        ToboganGame game;

        bool initialized = false;

        QuestionLivingLetter questionLivingLetter;
        int questionLetterIndex;
        List<QuestionLivingLetter> livingLetters = new List<QuestionLivingLetter>();

        float nextQuestionTimer;
        bool requestNextQueston;

        // return aswer result
        public event Action<bool> onAnswered;

        public QuestionsManager(ToboganGame game)
        {
            this.game = game;
        }

        public void Initialize()
        {
            if (!initialized)
            {
                initialized = true;

                game.pipesAnswerController.Initialize();
                CreateQuestionLivingLetters();

                questionLetterIndex = livingLetters.Count - 1;

                nextQuestionTimer = 0f;
                requestNextQueston = false;
            }
        }

        public void StartNewQuestion()
        {
            var nextQuestionPack = ToboganConfiguration.Instance.PipeQuestions.GetNextQuestion();

            UpdateQuestion(nextQuestionPack);
            PrepareLettersToAnswer();
        }

        void UpdateQuestion(IQuestionPack questionPack)
        {
            ResetLetters();

            questionLivingLetter = livingLetters[questionLetterIndex];

            questionLivingLetter.SetQuestionText(questionPack.GetQuestion());

            ILivingLetterData corretcAnswer = null;

            foreach (ILivingLetterData correct in questionPack.GetCorrectAnswers())
            {
                corretcAnswer = correct;
            }

            game.pipesAnswerController.SetPipeAnswers(questionPack.GetWrongAnswers(), corretcAnswer);
        }

        void CreateQuestionLivingLetters()
        {
            livingLetters.Clear();

            for (int i = 0; i < game.questionLivingLetterBox.lettersPosition.Length - 1; i++)
            {
                QuestionLivingLetter questionLetter = GetQuestionLivingLetter();

                questionLetter.GoToPosition(i);

                livingLetters.Add(questionLetter);
            }
        }

        QuestionLivingLetter GetQuestionLivingLetter()
        {
            QuestionLivingLetter newQuestionLivingLetter = GameObject.Instantiate(game.questionLivingLetterPrefab).GetComponent<QuestionLivingLetter>();
            newQuestionLivingLetter.Initialize(game.tubesCamera, game.questionLivingLetterBox.upRightMaxPosition.localPosition,
                game.questionLivingLetterBox.downLeftMaxPosition.localPosition, game.questionLivingLetterBox.lettersPosition);
            newQuestionLivingLetter.transform.SetParent(game.questionLivingLetterBox.transform);
            newQuestionLivingLetter.onMouseUpLetter += CheckAnswer;

            return newQuestionLivingLetter;
        }

        void ResetLetters()
        {
            for (int i = 0; i < livingLetters.Count; i++)
            {
                livingLetters[i].ClearQuestionText();
                livingLetters[i].PlayIdleAnimation();
                livingLetters[i].EnableCollider(false);
            }
        }

        void PrepareLettersToAnswer()
        {
            for (int i = 0; i < livingLetters.Count; i++)
            {
                if (i == livingLetters.Count - 1)
                {
                    livingLetters[i].MoveToNextPosition(1, OnQuestionLivingLetterOnPosition);
                }
                else
                {
                    livingLetters[i].MoveToNextPosition(1, null);
                }
            }
        }

        void OnQuestionLivingLetterOnPosition()
        {
            questionLivingLetter.EnableCollider(true);
        }

        void CheckAnswer()
        {
            PipeAnswer pipeAnswer = game.pipesAnswerController.GetCurrentPipeAnswer();

            if (pipeAnswer != null)
            {
                if (onAnswered != null)
                {
                    onAnswered(pipeAnswer.IsCorrectAnswer);

                    pipeAnswer.StopSelectedAnimation();

                    questionLivingLetter.GoToFirstPostion();

                    questionLetterIndex--;

                    if (questionLetterIndex < 0)
                        questionLetterIndex = livingLetters.Count - 1;

                    requestNextQueston = true;
                    nextQuestionTimer = 1f;
                }
            }
        }

        public void Update(float delta)
        {
            if (requestNextQueston)
            {
                nextQuestionTimer -= delta;

                if (nextQuestionTimer > 0f)
                {
                    StartNewQuestion();
                    requestNextQueston = false;
                }
            }
        }
    }
}
