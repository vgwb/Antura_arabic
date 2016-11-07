namespace EA4S.MissingLetter
{
    public class MissingLetterResultState : IGameState
    {
        MissingLetterGame game;

        bool goToEndGame;

        float timer = 4;

        public MissingLetterResultState(MissingLetterGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {

            game.m_RoundManager.Terminate();
            timer = 1;
            goToEndGame = true;

            game.Context.GetAudioManager().PlayMusic(Music.Relax);
            game.Context.GetOverlayWidget().Initialize(false, false, false);

            //if (game.isTimesUp)
            //{
            //    goToEndGame = true;

            //    //game.Context.GetPopupWidget().ShowTimeUp(OnPopupTimeUpCloseRequested);
            //}
        }

        //void OnPopupTimeUpCloseRequested()
        //{
        //    game.Context.GetPopupWidget().Hide();
        //    timer = 1;
        //    goToEndGame = true;
        //}

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            if (goToEndGame)
                timer -= delta;

            if (timer < 0)
            {
                game.EndGame(game.CurrentStars, game.mCurrentScore);
            }

        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
