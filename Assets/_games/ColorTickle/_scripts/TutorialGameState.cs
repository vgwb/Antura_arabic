using UnityEngine;
using UnityEngine.UI;
using ModularFramework.Helpers;

namespace EA4S.ColorTickle
{
    public class TutorialGameState : IGameState
    {
        #region PRIVATE MEMBERS

        ColorTickleGame game;
        GameObject TutorialLetter;

        float m_PercentageLetterColored;

        // LL components
        LetterObjectView m_LetterObjectView;
        TMPTextColoring m_TMPTextColoringLetter;
        SurfaceColoring m_SurfaceColoringLetter;
        ColorTickle_LLController m_LLController;
        HitStateLLController m_HitStateLLController;


        #endregion

        public TutorialGameState(ColorTickleGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            m_PercentageLetterColored = 0.0f;

            //game.anturaController.OnStateChanged += AnturaInteractions;

            //Init the tutorial letter
            TutorialLetter = game.tutorialLetter;
            TutorialLetter.gameObject.SetActive(true);
            InitTutorialLetter();
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            CalcPercentageLetterColored();

            if (m_PercentageLetterColored >= 100)
            {
                game.anturaController.ForceAnturaToGoBack();//we completed the letter, antura turn back
                                                            //DisableAntura();
                m_LetterObjectView.Poof();
                TutorialLetter.SetActive(false);

                game.SetCurrentState(game.PlayState);
            }
        }


        public void UpdatePhysics(float delta)
        {
        }

        #region PRIVATE FUNCTIONS


        private void InitTutorialLetter()
        {
            m_LetterObjectView = TutorialLetter.GetComponent<LetterObjectView>();

            m_TMPTextColoringLetter = TutorialLetter.GetComponent<TMPTextColoring>();
            m_SurfaceColoringLetter = TutorialLetter.GetComponent<SurfaceColoring>();

            m_LLController = TutorialLetter.GetComponent<ColorTickle_LLController>();
            m_LLController.movingToDestination = true;

            m_HitStateLLController = TutorialLetter.GetComponent<HitStateLLController>();

            //m_HitStateLLController.LoseLife += LoseLife;
            //m_HitStateLLController.EnableAntura += EnableAntura;
            //game.anturaController.targetToLook = m_CurrentLetter.transform; //make antura look at the LL on rotations

            SetBrushColor(new Color(255, 0, 0, 255));
        }

        private void SetBrushColor(Color color)
        {
            m_TMPTextColoringLetter.brush.SetBrushColor(color); //give the exact color to the letter 

            Color brushColor = color;
            brushColor.r += (1 - color.r) * 0.5f;
            brushColor.g += (1 - color.g) * 0.5f;
            brushColor.b += (1 - color.b) * 0.5f;
            m_SurfaceColoringLetter.brush.SetBrushColor(brushColor); //give the desaturated color to the body
        }

        private void LoseLife()
        {
            game.anturaController.ForceAnturaToGoBack();//we tickled the letter, antura turn back
        }

        private void CalcPercentageLetterColored()
        {
            float percentageRequiredToWin = m_TMPTextColoringLetter.percentageRequiredToWin;
            m_PercentageLetterColored = ((m_TMPTextColoringLetter.GetRachedCoverage() * 100.0f) / percentageRequiredToWin) * 100.0f;
            if (m_PercentageLetterColored > 100.0f)
            {
                m_PercentageLetterColored = 100.0f;
            }
        }

        private void EnableAntura()
        {
            game.anturaController.TryLaunchAnturaDisruption();
        }

        private void AnturaReachedLetter()
        {
            m_LetterObjectView.SetState(LLAnimationStates.LL_still);
            m_LetterObjectView.HasFear = true;
            m_LetterObjectView.Crouching = true;
        }

        private void AnturaGoingAway()
        {
            m_LetterObjectView.HasFear = false;
            m_LetterObjectView.Crouching = false;
        }

        private void AnturaInteractions(AnturaContollerState eState)
        {
            if (eState == AnturaContollerState.BARKING) //Antura scared the LL
            {
                AnturaReachedLetter();
            }
            else if (eState == AnturaContollerState.COMINGBACK) //Antura is returning to his place
            {
                AnturaGoingAway();
            }
        }

        #endregion
    }

}

