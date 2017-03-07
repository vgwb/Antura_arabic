using EA4S.Audio;
using EA4S.MinigamesCommon;

namespace EA4S.Minigames.MissingLetter
{
    public class MissingLetterIntroductionState : IState
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