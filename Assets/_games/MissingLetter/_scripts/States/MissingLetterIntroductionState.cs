using Antura.Audio;
using Antura.Minigames;

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
            if (MissingLetterConfiguration.Instance.Variation == MissingLetterVariation.MissingLetter)
            {
                AudioManager.I.PlayDialogue(Database.LocalizationDataId.MissingLetter_Title);
            }
            else if (MissingLetterConfiguration.Instance.Variation == MissingLetterVariation.MissingForm)
            {
                AudioManager.I.PlayDialogue(Database.LocalizationDataId.MissingLetter_forms_Title);
            }
            else
            {
                AudioManager.I.PlayDialogue(Database.LocalizationDataId.MissingLetter_phrases_Title);
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
                if (m_oGame.TutorialEnabled)
                {
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