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
        
        // return aswer result
        public event Action<bool> onAnswered;
        public event Action<bool> playerInputPointerUp;

        bool sunMoonGameVariation;
        bool playWhenDragged = true;
        bool playWhenEnter = true;


        public QuestionsManager(ToboganGame game)
        {
            this.game = game;
        }

        public void Initialize()
        {
            if (!initialized)
            {
                initialized = true;

                game.pipesAnswerController.Initialize(game);
                CreateQuestionLivingLetters();

                questionLetterIndex = livingLetters.Count - 1;

                game.Context.GetInputManager().onPointerDown += OnPointerDown;
                game.Context.GetInputManager().onPointerUp += OnPointerUp;
                game.Context.GetInputManager().onPointerDrag += OnPointerDrag;
            }
        }

        public void StartNewQuestion()
        {
            sunMoonGameVariation = ToboganVariation.SunMoon == ToboganConfiguration.Instance.Variation;

            IQuestionPack nextQuestionPack = null;
            
            if(sunMoonGameVariation)
            {
                nextQuestionPack = game.SunMoonQuestions.GetNextQuestion();
            }
            else
            {
                nextQuestionPack = ToboganConfiguration.Instance.Questions.GetNextQuestion();
            }

            UpdateQuestion(nextQuestionPack);
            PrepareLettersToAnswer();
        }

        public void SetDraggingAudio(bool enabled)
        {
            playWhenDragged = enabled;
            for (int i = 0; i < livingLetters.Count; i++)
                livingLetters[i].playWhenDragged = enabled;
        }

        public void SetEnteringAudio(bool enabled)
        {
            playWhenEnter = enabled;
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

            game.pipesAnswerController.SetPipeAnswers(wrongAnswers, correctAnswer, sunMoonGameVariation);
        }

        void CreateQuestionLivingLetters()
        {
            livingLetters.Clear();

            for (int i = 0; i < game.questionLivingLetterBox.lettersPosition.Length - 1; i++)
            {
                QuestionLivingLetter questionLetter = CreateQuestionLivingLetter();

                questionLetter.ClearQuestionText();
                questionLetter.PlayStillAnimation();
                questionLetter.EnableCollider(false);

                questionLetter.GoToPosition(i);
                questionLetter.playWhenDragged = playWhenDragged;

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

            var shadow = GameObject.Instantiate(game.shadowPrefab);
            shadow.transform.SetParent(newQuestionLivingLetter.transform);
            shadow.gameObject.SetActive(true);
            shadow.Initialize(newQuestionLivingLetter.transform, game.pipesAnswerController.basePosition.position.y);

            return newQuestionLivingLetter;
        }

        void ResetLetters()
        {
            for (int i = 0; i < livingLetters.Count; i++)
            {
                livingLetters[i].ClearQuestionText();
                livingLetters[i].PlayStillAnimation();
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

            if (playWhenEnter)
                game.Context.GetAudioManager().PlayLetterData(questionLivingLetter.letter.Data, true);

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
            }
        }

        public void QuestionEnd()
        {
            questionLivingLetter.GoToFirstPostion();

            questionLetterIndex--;

            if (questionLetterIndex < 0)
                questionLetterIndex = livingLetters.Count - 1;

            game.pipesAnswerController.HidePipes();
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
                    if (playerInputPointerUp != null)
                        playerInputPointerUp(false);

                    draggingLetter = questionLivingLetter;
                    questionLivingLetter.OnPointerDown(pointerPosition);
                }
            }
        }

        void OnPointerUp()
        {
            if (playerInputPointerUp != null)
                playerInputPointerUp(true);

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

        public QuestionLivingLetter GetQuestionLivingLetter()
        {
            return questionLivingLetter;
        }
    }
}
