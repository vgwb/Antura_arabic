using UnityEngine;
using UnityEngine.UI;
using ModularFramework.Helpers;

namespace EA4S.ColorTickle
{
    public class PlayGameState : IGameState
    {
        enum eHitState
        {
            HIT_NONE = 0, HIT_LETTERINSIDE_AND_BODY, HIT_LETTERINSIDE, HIT_LETTEROUTSIDE
        }

        ColorTickleGame game;
        
        #region PRIVATE MEMBERS

		private LetterObjectView m_currentLetter;

        private bool m_StartToPaint;

        private ColorsUIManager m_ColorsUIManager;
        private Button m_PercentageLetterColoredButton;

        private float m_PercentageLetterColored;
        private bool m_Tickle;
        private float m_TickleTime = 1;
        private eHitState m_HitState;
        private Vector2 m_LastBrushPosition;
        private float m_BurshDistanceCovered;
        private float m_DelayCheckVelocity;

        private float m_ClockTime;
		private int m_MaxLives;
        private int m_Lives;
        private bool m_LifeLost;
        private float m_TimeOutsideLetterwithBrush;

        private TMPTextColoring m_TMPTextColoringLetter;
        private SurfaceColoring m_SurfaceColoringLetter;

        private int m_Rounds;

        #endregion

        public PlayGameState(ColorTickleGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            m_Rounds = game.rounds;
			m_ClockTime = game.clockTime;
			m_MaxLives = game.lives;

			//Init ColorCanvas and PercentageLetterColoredButton
			InitGameUI();
			ResetState ();

			m_currentLetter = game.myLetters[m_Rounds - 1];
			m_currentLetter.gameObject.SetActive (true);
            m_currentLetter.GetComponent<LLController>().movingToDestination = true;


            //Init the first letter
            InitLetter();
            
            game.anturaController.isEnable = true;
            game.anturaController.letterPosition = m_currentLetter.transform.position;
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            if (m_ClockTime < 0 || m_Lives <= 0)
            {
				DisableObjects ();
                game.SetCurrentState(game.ResultState);
                Debug.Log("You have lost");
            }
            else
            {
                if (m_StartToPaint)
                {
                    m_ClockTime -= delta;
                    game.gameUI.SetClockTime(m_ClockTime);
                                       
                    LifeCounter(delta);

                    TickleController(delta);

                    CheckBrushVelocity(delta);

                    CalcPercentageLetterColored();
					m_PercentageLetterColoredButton.GetComponentInChildren<Text>().text = Mathf.FloorToInt(m_PercentageLetterColored) + "%";

                    if (m_PercentageLetterColored >= 100)
                    {
                        if (m_Rounds <= 1)
                        {
                            m_currentLetter.Poof();
                            m_currentLetter.gameObject.SetActive(false);
                            DisableObjects ();
                            game.SetCurrentState(game.ResultState);
							Debug.Log("You win!!!!");
                        }
                        else
                        {
                            // Disable the letter just colored
                            m_currentLetter.Poof();
							m_currentLetter.gameObject.SetActive(false);
							ResetState ();
							game.gameUI.SetLives (m_MaxLives);
							--m_Rounds;
							m_currentLetter = game.myLetters[m_Rounds - 1];
							m_currentLetter.gameObject.SetActive (true);
                            m_currentLetter.GetComponent<LLController>().movingToDestination = true;
							// Initialize the next letter
							InitLetter();
                        }                  
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
			m_LifeLost = false;

			m_StartToPaint = false;
			m_HitState = eHitState.HIT_NONE;
			m_TimeOutsideLetterwithBrush = 0;
			m_Tickle = false;

			m_PercentageLetterColored = 0;
			m_PercentageLetterColoredButton.GetComponentInChildren<Text>().text = "0 %";
		}

		private void InitGameUI()
		{
			m_ColorsUIManager = game.colorsCanvas.GetComponentInChildren<ColorsUIManager>();
			m_ColorsUIManager.SetBrushColor += SetBrushColor;
			m_PercentageLetterColoredButton = GameObject.Find("PercentageButton").GetComponent<Button>();  
		}

		private void InitLetter()
		{
			m_TMPTextColoringLetter = m_currentLetter.GetComponent<TMPTextColoring>();
			m_SurfaceColoringLetter = m_currentLetter.GetComponent<SurfaceColoring>();
			m_TMPTextColoringLetter.OnShapeHit += ShapeTouched;
			m_SurfaceColoringLetter.OnBodyHit += BodyTouched;
//			m_TMPTextColoringLetter.enableColor = false;
//			m_SurfaceColoringLetter.enableColor = false;
			//BuildBrush ();
			SetBrushColor(m_ColorsUIManager.defaultColor);
			//m_currentLetter.GetComponentInChildren<Animator>().SetInteger("State", 1);
		}

		private void DisableObjects()
		{
            //m_currentLetter.GetComponentInChildren<Animator>().SetInteger("State", 1);
            m_currentLetter.SetState(LLAnimationStates.LL_still);
            m_ColorsUIManager.SetBrushColor -= SetBrushColor;
			m_TMPTextColoringLetter.OnShapeHit -= ShapeTouched;
			m_SurfaceColoringLetter.OnBodyHit -= BodyTouched;
			m_TMPTextColoringLetter.enableColor = false;
			m_SurfaceColoringLetter.enableColor = false;
			game.anturaController.isEnable = false;
		}


		private void BuildBrush(){
			ColoringParameters[] Brushes = m_currentLetter.GetComponents<ColoringParameters>();

			for (int i = 0; i < Brushes.Length; ++i)
			{
				Brushes [i].BuildBrush ();
			}
		}


        private void SetBrushColor(Color color)
        {
			ColoringParameters[] Brushes = m_currentLetter.GetComponents<ColoringParameters>();

			for (int i = 0; i < Brushes.Length; ++i)
			{
				if (Brushes [i].brushName.CompareTo ("BodyBrush") == 0) {
					Color brushColor = color;
					brushColor.r += (1 - color.r) * 0.4f;
					brushColor.g += (1 - color.g) * 0.4f;
					brushColor.b += (1 - color.b) * 0.4f;
					Brushes [i].SetBrushColor (brushColor);
				} 
				else {
					Brushes [i].SetBrushColor (color);
				}
			}
        }


		private void BodyTouched()
		{
			if (m_StartToPaint)
			{
				if (m_HitState != eHitState.HIT_LETTERINSIDE)
				{
					m_HitState = eHitState.HIT_LETTEROUTSIDE;
					TicklesLetter();
				}
				else
				{
					m_HitState = eHitState.HIT_LETTERINSIDE_AND_BODY;
					m_LifeLost = false;
				}
			}
		}

		private void ShapeTouched(bool bShapeHitted)
		{
			//when the hit is inside
			if (bShapeHitted)
			{
				// Call this function before we set m_HitState = HIT_LETTERINSIDE 
				TrackBrushDistanceCovered();
				m_HitState = eHitState.HIT_LETTERINSIDE;
				if (!m_StartToPaint)
				{
					m_StartToPaint = true;
					m_TMPTextColoringLetter.enableColor = true;
					m_SurfaceColoringLetter.enableColor = true;
				}
			}
			//when the hit is outside
			else
			{
				if (m_StartToPaint)
				{
					m_HitState = eHitState.HIT_LETTEROUTSIDE;
					TicklesLetter();
				}
			}
		}


        private void TicklesLetter()
        {           
            //Debug.Log("Tickle");
            m_TickleTime = 1;
            m_Tickle = true;
            //m_currentLetter.GetComponentInChildren<Animator>().SetInteger("State", game.animatorTickleState);
            m_currentLetter.SetState(LLAnimationStates.LL_dancing);
            //m_TMPTextColoringLetter.enableColor = false;
            //m_SurfaceColoringLetter.enableColor = false;          
        }

		private void TickleController(float delta)
		{
			if (m_Tickle)
			{
				m_TickleTime -= delta;
				if (m_TickleTime < 0 /*&& !m_currentLetter.GetComponentInChildren<Animator>().IsInTransition(0)*/)
				{
					m_HitState = eHitState.HIT_NONE;
					m_Tickle = false;
                    //m_currentLetter.GetComponentInChildren<Animator>().SetInteger("State", 1);
                    m_currentLetter.SetState(LLAnimationStates.LL_still);

                }
			}
		}


		private void LifeCounter(float delta)
		{
			if (m_HitState == eHitState.HIT_LETTEROUTSIDE)
			{
				m_TimeOutsideLetterwithBrush += delta;
				if (!m_LifeLost || m_TimeOutsideLetterwithBrush >= 2.0f)
				{
					// You can lose just one life when you color outside the letter
					// But if you keep color outside, after 1 second you lose one more life
					// After you color again inside the letter, if color outside, can lose one more life
					m_Lives--;
					m_LifeLost = true;
					m_TimeOutsideLetterwithBrush = 0.0f;
					game.gameUI.SetLives(m_Lives);
					Debug.Log("You have lost one life");
				}
			}
			else
			{
				m_LifeLost = false;
			}
		}

	
		private void CalcPercentageLetterColored()
		{
			float percentageRequiredToWin = m_TMPTextColoringLetter.percentageRequiredToWin;
			m_PercentageLetterColored = ((m_TMPTextColoringLetter.GetRachedCoverage () * 100.0f) / percentageRequiredToWin) * 100.0f ;
			if (m_PercentageLetterColored >= 100.0f) 
			{
				m_PercentageLetterColored = 100.0f;
			}
		}



        private void TrackBrushDistanceCovered()
        {
            if (m_HitState == eHitState.HIT_LETTERINSIDE_AND_BODY)
            {
                // if we already painted the letter the frame before
                Vector2 newBrushPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Vector2 diffBrushPositions = newBrushPosition - m_LastBrushPosition;
                m_LastBrushPosition = newBrushPosition;
                m_BurshDistanceCovered += diffBrushPositions.magnitude;
                //Debug.Log("Distance Covered:" + m_BurshDistanceCovered);
            }
            else
            {
                // if we didn't painted the letter the frame before
                m_LastBrushPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
        }
			
        private void CheckBrushVelocity(float delta)
        {
            m_DelayCheckVelocity += delta;
            // Every 0.5f seconds we check if the velocity of the brush is more than "m_BrushLimitVelocity" (pixel/seconds)
            // if it's more we tickles the letter
            if (m_DelayCheckVelocity >= 0.5f)
            {
                //Debug.Log("Distance Covered after 0.5s:" + m_BurshDistanceCovered);
                //Debug.Log("Velocity in pixel:" + m_BurshDistanceCovered / 0.5f);
                if (m_BurshDistanceCovered / 0.5f >= game.brushLimitVelocity)
                {
                    TicklesLetter();
                }
                m_BurshDistanceCovered = 0.0f;
                m_DelayCheckVelocity = 0.0f;
            }
        }
			
        #endregion
    }


}
