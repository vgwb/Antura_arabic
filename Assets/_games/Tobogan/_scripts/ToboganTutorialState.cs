using UnityEngine;

namespace EA4S.Tobogan
{
    public class ToboganTutorialState : IGameState
    {
        ToboganGame game;

        float delayStartTutorial;
        bool tutorialStarted;

        bool toPlayState;
        float toPlayStateTimer;

        bool pointerUp;

        public ToboganTutorialState(ToboganGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.questionsManager.onAnswered += OnAnswered;
            game.questionsManager.playerInputPointerUp += OnPointerUp;

            game.questionsManager.StartNewQuestion();
            game.questionsManager.Enabled = true;

            tutorialStarted = false;
            delayStartTutorial = 1f;

            toPlayState = false;
            toPlayStateTimer = 1f;

            pointerUp = true;
        }

        public void ExitState()
        {
            game.questionsManager.onAnswered -= OnAnswered;
            game.questionsManager.playerInputPointerUp -= OnPointerUp;

            TutorialUI.Clear(true);
        }

        public void Update(float delta)
        {
            if (!tutorialStarted)
            {
                delayStartTutorial += -delta;

                if (delayStartTutorial <= 0f)
                {
                    tutorialStarted = true;
                    TutorialDrawLine();
                }
            }

            if(toPlayState)
            {
                toPlayStateTimer -= delta;

                if(toPlayStateTimer <= 0f)
                {
                    toPlayState = false;
                    game.SetCurrentState(game.PlayState);
                }
            }

            if(pointerUp && tutorialStarted)
            {
                tutorialStarted = false;
                delayStartTutorial = 3f;
            }
        }

        public void UpdatePhysics(float delta) { }

        void OnAnswered(bool result)
        {
            if(result)
            {
                toPlayState = true;

                game.questionsManager.QuestionEnd();
                game.OnResult(true);
            }
            else
            {
                TutorialMarkNo();
            }
        }

        void OnPointerUp(bool pointerUp)
        {
            if (!pointerUp)
            {
                tutorialStarted = true;

                TutorialUI.Clear(false);
            }

            this.pointerUp = pointerUp;
        }

        void TutorialDrawLine()
        {
            Vector3 lineFrom = game.questionsManager.GetQuestionLivingLetter().letter.contentTransform.position;
            Vector3 lineTo = game.pipesAnswerController.GetCorrectPipeAnswer().tutorialPoint.position;

            TutorialUI.DrawLine(lineFrom, lineTo, TutorialUI.DrawLineMode.Finger);
        }

        void TutorialMarkNo()
        {
            Vector3 markNoPosition = game.pipesAnswerController.GetCurrentPipeAnswer().tutorialPoint.position;

            TutorialUI.MarkNo(markNoPosition);
        }
    }
}