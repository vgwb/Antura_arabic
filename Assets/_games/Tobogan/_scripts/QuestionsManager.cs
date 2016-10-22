using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EA4S.Tobogan
{
    public class QuestionsManager
    {
        ToboganGame game;

        public bool Enabled = false;

        bool initialized = false;

        QuestionLivingLetter questionLivingLetter;
        QuestionLivingLetter draggingLetter;

        int questionLetterIndex;
        List<QuestionLivingLetter> livingLetters = new List<QuestionLivingLetter>();

        float nextQuestionTimer;
        float nextQuestiontime = 1f;
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

                game.Context.GetInputManager().onPointerDown += OnPointerDown;
                game.Context.GetInputManager().onPointerUp += OnPointerUp;
                game.Context.GetInputManager().onPointerDrag += OnPointerDrag;
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

            ILivingLetterData correctAnswer = null;

            var correctAnswers = questionPack.GetCorrectAnswers();
            var correctList = correctAnswers.ToList();
            correctAnswer = correctList[UnityEngine.Random.Range(0, correctList.Count)];

            var wrongAnswers = questionPack.GetWrongAnswers().ToList();

            // Shuffle wrong answers
            int n = wrongAnswers.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                var value = wrongAnswers[k];
                wrongAnswers[k] = wrongAnswers[n];
                wrongAnswers[n] = value;
            }

            game.pipesAnswerController.SetPipeAnswers(wrongAnswers, correctAnswer);
        }

        void CreateQuestionLivingLetters()
        {
            livingLetters.Clear();

            for (int i = 0; i < game.questionLivingLetterBox.lettersPosition.Length - 1; i++)
            {
                QuestionLivingLetter questionLetter = CreateQuestionLivingLetter();

                questionLetter.ClearQuestionText();
                questionLetter.PlayIdleAnimation();
                questionLetter.EnableCollider(false);

                questionLetter.GoToPosition(i);

                livingLetters.Add(questionLetter);
            }
        }

        QuestionLivingLetter CreateQuestionLivingLetter()
        {
            QuestionLivingLetter newQuestionLivingLetter = GameObject.Instantiate(game.questionLivingLetterPrefab).GetComponent<QuestionLivingLetter>();
            newQuestionLivingLetter.gameObject.SetActive(true);

            newQuestionLivingLetter.Initialize(game.tubesCamera, game.questionLivingLetterBox.upRightMaxPosition.position,
                game.questionLivingLetterBox.downLeftMaxPosition.position, game.questionLivingLetterBox.lettersPosition);
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
                    livingLetters[i].MoveToNextPosition(1f, OnQuestionLivingLetterOnPosition);
                }
                else
                {
                    livingLetters[i].MoveToNextPosition(1f, null);
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

            if (pipeAnswer != null && Enabled)
            {
                bool isCorrectAnswer = pipeAnswer.IsCorrectAnswer;

                if (isCorrectAnswer)
                    game.Context.GetAudioManager().PlaySound(Sfx.LetterHappy);
                else
                    game.Context.GetAudioManager().PlaySound(Sfx.LetterSad);


                if (onAnswered != null)
                    onAnswered(isCorrectAnswer);

                pipeAnswer.StopSelectedAnimation();

                questionLivingLetter.GoToFirstPostion();

                questionLetterIndex--;

                if (questionLetterIndex < 0)
                    questionLetterIndex = livingLetters.Count - 1;

                requestNextQueston = true;
                nextQuestionTimer = nextQuestiontime;
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

        void OnPointerDown()
        {
            if (Enabled && questionLivingLetter != null)
            {
                var pointerPosition = game.Context.GetInputManager().LastPointerPosition;
                var screenRay = game.tubesCamera.ScreenPointToRay(pointerPosition);

                RaycastHit hitInfo;
                if (questionLivingLetter.GetComponent<Collider>().Raycast(screenRay, out hitInfo, game.tubesCamera.farClipPlane))
                {
                    draggingLetter = questionLivingLetter;
                    questionLivingLetter.OnPointerDown(pointerPosition);
                }
            }
        }

        void OnPointerUp()
        {
            draggingLetter = null;

            if (questionLivingLetter != null)
                questionLivingLetter.OnPointerUp();
        }

        void OnPointerDrag()
        {
            if (draggingLetter != null && questionLivingLetter == draggingLetter)
            {
                var pointerPosition = game.Context.GetInputManager().LastPointerPosition;
                questionLivingLetter.OnPointerDrag(pointerPosition);
            }
        }
    }
}
