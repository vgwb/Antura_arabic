namespace EA4S.MissingLetter
{
    public class MissingLetterIntroductionState : IGameState
    {
        public MissingLetterIntroductionState(MissingLetterGame _game)
        {
            this.m_oGame = _game;
        }

        public void EnterState()
        {
            if(MissingLetterConfiguration.Instance.Variation == MissingLetterVariation.MissingLetter)
            {
                AudioManager.I.PlayDialog(Db.LocalizationDataId.MissingLetter_Title);
            }
            else
            {
                AudioManager.I.PlayDialog(Db.LocalizationDataId.MissingLetter_phrases_Title);
            }
        }

        public void ExitState()
        {
        }

        public void Update(float _delta)
        {
            m_fTimer -= _delta;

            if (m_fTimer < 0)
            {
                m_oGame.SetCurrentState(m_oGame.TutorialState);
            }
        }

        public void UpdatePhysics(float _delta)
        {
        }

        MissingLetterGame m_oGame;
        float m_fTimer = 2;
    }
}