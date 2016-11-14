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

        public ToboganTutorialState(ToboganGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.questionsManager.onAnswered += OnAnswered;

            game.questionsManager.StartNewQuestion();

            tutorialStarted = false;
            delayStartTutorial = 2f;

            toPlayState = false;
            toPlayStateTimer = 1f;
        }

        public void ExitState()
        {
            game.questionsManager.onAnswered -= OnAnswered;
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
                TutorialDrawLine();
            }
        }

        void TutorialDrawLine()
        {
            Vector3 lineFrom = game.questionsManager.GetQuestionLivingLetter().letter.innerTransform.position;
            Vector3 lineTo = game.pipesAnswerController.GetCorrectPipeAnswer().tutorialPoint.position; 

            TutorialUI.DrawLine(lineFrom, lineTo, TutorialUI.DrawLineMode.Arrow)
                .OnComplete(delegate () { game.questionsManager.Enabled = true; });
        }

        void TutorialMarkNo()
        {
            Vector3 markNoPosition = game.pipesAnswerController.GetCurrentPipeAnswer().tutorialPoint.position;

            TutorialUI.MarkNo(markNoPosition);
        }
    }
}