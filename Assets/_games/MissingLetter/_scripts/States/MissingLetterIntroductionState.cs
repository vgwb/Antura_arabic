using Antura.Audio;

namespace Antura.Minigames.MissingLetter
{
    public class MissingLetterIntroductionState : FSM.IState
    {
        public MissingLetterIntroductionState(MissingLetterGame _game)
        {
            this.m_oGame = _game;
        }

        public void EnterState()
        {
            AudioManager.I.PlayDialogue(MissingLetterConfiguration.Instance.TitleLocalizationId);
        }

        public void ExitState()
        {
        }

        public void Update(float _delta)
        {
            m_fTimer -= _delta;

            if (m_fTimer < 0) {
                if (m_oGame.TutorialEnabled) {
                    m_oGame.SetCurrentState(m_oGame.TutorialState);
                } else {
                    m_oGame.SetCurrentState(m_oGame.QuestionState);
                }
            }
        }

        public void UpdatePhysics(float _delta)
        {
        }

        MissingLetterGame m_oGame;
        float m_fTimer = 2;
    }
}