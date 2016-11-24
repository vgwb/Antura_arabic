using UnityEngine;

namespace EA4S.MixedLetters
{
    public class QuestionGameState : IGameState
    {
        MixedLettersGame game;

        public QuestionGameState(MixedLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.Context.GetPopupWidget().Show(OnQuestionCompleted, Db.LocalizationDataId.Assessment_Complete_1, true, null);
        }

        public void ExitState()
        {
            game.Context.GetPopupWidget().Hide();
        }

        void OnQuestionCompleted()
        {
            game.SetCurrentState(game.PlayState);
        }

        public void Update(float delta)
        {

        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
