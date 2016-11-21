namespace EA4S.MissingLetter
{
    public class MissingLetterResultState : IGameState
    {

        public MissingLetterResultState(MissingLetterGame _game)
        {
            this.m_oGame = _game;
        }

        public void EnterState()
        {

            m_oGame.m_oRoundManager.Terminate();
            m_fTimer = 1;
            m_bGoToEndGame = true;

            m_oGame.Context.GetAudioManager().PlayMusic(Music.Relax);
            m_oGame.Context.GetOverlayWidget().Initialize(false, false, false);

        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            if (m_bGoToEndGame)
                m_fTimer -= delta;

            if (m_fTimer < 0)
            {
                m_oGame.EndGame(m_oGame.m_iCurrentStars, m_oGame.m_iCurrentScore);
            }

        }

        public void UpdatePhysics(float delta)
        {
        }

        MissingLetterGame m_oGame;
        bool m_bGoToEndGame;
        float m_fTimer = 4;
    }
}
