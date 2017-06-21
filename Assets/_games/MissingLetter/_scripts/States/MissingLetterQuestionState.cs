using EA4S.MinigamesCommon;

namespace EA4S.Minigames.MissingLetter
{
    public class MissingLetterQuestionState : IState
    {
        
        public MissingLetterQuestionState(MissingLetterGame _game)
        {
            this.M_oGgame = _game;
        }

        public void EnterState()
        {
        }

        public void ExitState()
        {
            M_oGgame.Context.GetPopupWidget().Hide();
        }

        void OnQuestionCompleted()
        {
            M_oGgame.SetCurrentState(M_oGgame.TutorialState);
        }

        public void Update(float delta)
        {
            M_oGgame.SetCurrentState(M_oGgame.TutorialState);
        }

        public void UpdatePhysics(float delta)
        {
        }

        MissingLetterGame M_oGgame;
    }
}
