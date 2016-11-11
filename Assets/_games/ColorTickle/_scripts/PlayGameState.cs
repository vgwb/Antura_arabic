using UnityEngine;
using UnityEngine.UI;
using ModularFramework.Helpers;

namespace EA4S.ColorTickle
{
    public class PlayGameState : IGameState
    {
        #region PRIVATE MEMBERS

        ColorTickleGame game;
        GameObject m_CurrentLetter;

		int m_MaxLives;
        int m_Lives;
        int m_Rounds;
        float m_Stars;

        Button m_PercentageLetterColoredButton;
        float m_PercentageLetterColored;

        ColorsUIManager m_ColorsUIManager;

        // LL components
        LetterObjectView m_LetterObjectView;
        TMPTextColoring m_TMPTextColoringLetter;
        SurfaceColoring m_SurfaceColoringLetter;
        ColorTickle_LLController m_LLController;
        HitStateLLController m_HitStateLLController;

        bool m_NextLetter = false;
        #endregion

        public PlayGameState(ColorTickleGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            m_Rounds = game.rounds;
			m_MaxLives = game.lives;
            m_Stars = 3.0f;

			//Init ColorCanvas and PercentageLetterColoredButton
			InitGameUI();

			ResetState ();

            game.anturaController.OnStateChanged += AnturaInteractions;
            
            //Init the first letter
            m_CurrentLetter = game.myLetters[m_Rounds - 1];
			m_CurrentLetter.gameObject.SetActive (true);         
            InitLetter();       
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            if (m_Rounds <= 0)
            {
                game.m_Stars = Mathf.RoundToInt(m_Stars);
                game.SetCurrentState(game.ResultState);
            }
            else
            {
                CalcPercentageLetterColored();
                m_PercentageLetterColoredButton.GetComponentInChildren<Text>().text = Mathf.FloorToInt(m_PercentageLetterColored) + "%";

                if (m_PercentageLetterColored >= 100 || m_Lives <=0)
                {
                    //DisableAntura();
                    m_LetterObjectView.Poof();
                    m_CurrentLetter.SetActive(false);
                    --m_Rounds;
                    if (m_Rounds > 0)
                    {
                        ResetState();
                        m_ColorsUIManager.ChangeButtonsColor();
                        m_CurrentLetter = game.myLetters[m_Rounds - 1];
                        m_CurrentLetter.gameObject.SetActive(true);
                        // Initialize the next letter
                        InitLetter();
                    }
                }
            }
        }


        public void UpdatePhysics(float delta)
        {
        }

        #region PRIVATE FUNCTIONS

		private void ResetState(){		
			m_Lives = m_MaxLives;
            game.gameUI.SetLives(m_MaxLives);
            m_PercentageLetterColored = 0;
            m_PercentageLetterColoredButton.GetComponentInChildren<Text>().text = "0 %";
        }

		private void InitGameUI()
		{
            m_ColorsUIManager = game.colorsCanvas.GetComponentInChildren<ColorsUIManager>();
            m_ColorsUIManager.SetBrushColor += SetBrushColor;
			m_PercentageLetterColoredButton = m_ColorsUIManager.percentageColoredButton;
		}

		private void InitLetter()
		{
            m_LetterObjectView = m_CurrentLetter.GetComponent<LetterObjectView>();

            m_TMPTextColoringLetter = m_CurrentLetter.GetComponent<TMPTextColoring>();
            m_SurfaceColoringLetter = m_CurrentLetter.GetComponent<SurfaceColoring>();

            m_LLController = m_CurrentLetter.GetComponent<ColorTickle_LLController>();
            m_LLController.movingToDestination = true;

            m_HitStateLLController = m_CurrentLetter.GetComponent<HitStateLLController>();
            m_HitStateLLController.LoseLife += LoseLife;

            m_HitStateLLController.EnableAntura += EnableAntura;
            game.anturaController.targetToLook = m_CurrentLetter.transform;

            SetBrushColor(m_ColorsUIManager.defaultColor);     
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
            m_Lives--;
            m_Stars -= 0.3f;
            game.gameUI.SetLives(m_Lives);
            Debug.Log("Lives : " + m_Lives);
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

        /// <summary>
        /// This is called by Antura controller with the change state event to apply any
        /// needed interactions.
        /// </summary>
        /// <param name="eState">Current state for Antura</param>
        private void AnturaInteractions( AnturaContollerState eState)
        {
            //For now scare the LL by make it crouch 
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

#region DEPRECATED FUNCTIONS 

//private void TrackBrushDistanceCovered()
//{
//    if (m_HitState == eHitState.HIT_LETTERINSIDE_AND_BODY)
//    {
//        // if we already painted the letter the frame before
//        Vector2 newBrushPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
//        Vector2 diffBrushPositions = newBrushPosition - m_LastBrushPosition;
//        m_LastBrushPosition = newBrushPosition;
//        m_BurshDistanceCovered += diffBrushPositions.magnitude;
//        //Debug.Log("Distance Covered:" + m_BurshDistanceCovered);
//    }
//    else
//    {
//        // if we didn't painted the letter the frame before
//        m_LastBrushPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
//    }
//}

//private void CheckBrushVelocity(float delta)
//{
//    m_DelayCheckVelocity += delta;
//    // Every 0.5f seconds we check if the velocity of the brush is more than "m_BrushLimitVelocity" (pixel/seconds)
//    // if it's more we tickles the letter
//    if (m_DelayCheckVelocity >= 0.5f)
//    {
//        //Debug.Log("Distance Covered after 0.5s:" + m_BurshDistanceCovered);
//        //Debug.Log("Velocity in pixel:" + m_BurshDistanceCovered / 0.5f);
//        if (m_BurshDistanceCovered / 0.5f >= game.brushLimitVelocity)
//        {
//            TicklesLetter();
//        }
//        m_BurshDistanceCovered = 0.0f;
//        m_DelayCheckVelocity = 0.0f;
//    }
//}

#endregion