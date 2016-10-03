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

            questionLivingLetter = livingLetters[livingLetters.Count - 1];

            questionLivingLetter.SetQuestionText(questionPack.GetQuestion());
            questionLivingLetter.PlayIdleAnimation();

            ILivingLetterData corretcAnswer = null;

            foreach (ILivingLetterData correct in questionPack.GetCorrectAnswers())
            {
                corretcAnswer = correct;
            }

            game.pipesAnswerController.SetPipeAnswers(questionPack.GetWrongAnswers(), corretcAnswer);
        }

        void CreateQuestionLivingLetters()
        {
            /*
            questionLivingLetter = GetQuestionLivingLetter();
            questionLivingLetter.onMouseUpLetter += CheckAnswer;

            questionLivingLetter.transform.localPosition = game.questionLivingLetterBox.letterEndPosition.localPosition;
            questionLivingLetter.transform.rotation = game.questionLivingLetterBox.letterEndPosition.rotation;
            questionLivingLetter.Initialize(game.tubesCamera, game.questionLivingLetterBox.letterEndPosition.position,
                game.questionLivingLetterBox.upRightMaxPosition.localPosition, game.questionLivingLetterBox.downLeftMaxPosition.localPosition);

            livingLetters.Clear();
            Transform[] lettersPosition = game.questionLivingLetterBox.lettersPosition;

            //QuestionLivingLetter startQuestionLetter = GetQuestionLivingLetter();
            //startQuestionLetter.onMouseUpLetter += CheckAnswer;

            //startQuestionLetter.transform.localPosition = game.questionLivingLetterBox.letterStartPosition.localPosition;
            //startQuestionLetter.transform.rotation = game.questionLivingLetterBox.letterStartPosition.rotation;
            //startQuestionLetter.Initialize(game.tubesCamera, game.questionLivingLetterBox.letterEndPosition.position,
            //    game.questionLivingLetterBox.upRightMaxPosition.localPosition, game.questionLivingLetterBox.downLeftMaxPosition.localPosition);


            //standbyLivingLetters.Add(startQuestionLetter);
            */

            livingLetters.Clear();
            Transform[] lettersPosition = game.questionLivingLetterBox.lettersPosition;

            for (int i = 0; i < lettersPosition.Length - 1; i++)
            {
                QuestionLivingLetter questionLetter = GetQuestionLivingLetter();

                questionLetter.transform.localPosition = lettersPosition[i].localPosition;
                questionLetter.transform.rotation = lettersPosition[i].rotation;
                questionLetter.Initialize(game.tubesCamera, lettersPosition[lettersPosition.Length - 1].position,
                game.questionLivingLetterBox.upRightMaxPosition.localPosition, game.questionLivingLetterBox.downLeftMaxPosition.localPosition);

                livingLetters.Add(questionLetter);
            }
        }

        QuestionLivingLetter GetQuestionLivingLetter()
        {
            QuestionLivingLetter newQuestionLivingLetter = GameObject.Instantiate(game.questionLivingLetterPrefab).GetComponent<QuestionLivingLetter>();
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
            Transform[] lettersPosition = game.questionLivingLetterBox.lettersPosition;

            for (int i = 1; i < lettersPosition.Length; i++)
            {
                if (i == livingLetters.Count - 1)
                    livingLetters[i - 1].TransformTo(lettersPosition[i], 1f, OnQuestionLivingLetterOnPosition);
                else
                    livingLetters[i - 1].TransformTo(lettersPosition[i], 1f, null);
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
                    onAnswered(pipeAnswer.IsCorrectAnswer);

                pipeAnswer.StopSelectedAnimation();

                questionLivingLetter.transform.localPosition = game.questionLivingLetterBox.lettersPosition[0].localPosition;
                questionLivingLetter.transform.eulerAngles = game.questionLivingLetterBox.lettersPosition[0].eulerAngles;

                List<QuestionLivingLetter> newLivingLetters = new List<QuestionLivingLetter>();

                newLivingLetters.Add(livingLetters[livingLetters.Count - 1]);

                for (int i = 0; i < livingLetters.Count - 1; i++)
                {
                    newLivingLetters.Add(livingLetters[i]);
                }

                livingLetters = newLivingLetters;

                requestNextQueston = true;
                nextQuestionTimer = 1f;
                game.pipesAnswerController.HidePipes();

            }
        }

        public void Update(float delta)
        {
            if (requestNextQueston)
            {
                nextQuestionTimer -= delta;

                if (nextQuestionTimer <= 0f)
                {
                    StartNewQuestion();
                    requestNextQueston = false;
                }
            }
        }
    }
}
