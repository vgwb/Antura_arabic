using System;

namespace EA4S.Assessment
{
    public class AssessmentQuestionState : IGameState
    {
        private AssessmentGame assessmentGame;

        public AssessmentQuestionState(AssessmentGame assessmentGame)
        {
            this.assessmentGame = assessmentGame;
        }

        public void EnterState()
        {
            // Enable popup widget
            var popupWidget = assessmentGame.Context.GetPopupWidget();
            popupWidget.Show();
            popupWidget.SetButtonCallback(OnPopupCloseRequested);
            popupWidget.SetMessage(GetDescription(), true);
        }

        private string GetDescription()
        {
            switch (assessmentGame.assessmentCode) {
                case AssessmentCode.MatchLettersToWord:
                    return LetterInWord.LetterInWordConfiguration.Instance.Description;

                case AssessmentCode.LetterShape:
                    return null;

                case AssessmentCode.WordsWithLetter:
                    return null;
            }

            return null;
        }

        void OnQuestionCompleted()
        {
            assessmentGame.SetCurrentState(assessmentGame.PlayState);
        }

        void OnPopupCloseRequested()
        {
            assessmentGame.SetCurrentState(assessmentGame.PlayState);
        }

        public void ExitState()
        {
            assessmentGame.Context.GetPopupWidget().Hide();
        }

        public void Update(float delta)
        {

        }

        public void UpdatePhysics(float delta)
        {

        }
    }
}