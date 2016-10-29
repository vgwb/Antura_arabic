using UnityEngine;

namespace EA4S.ColorTickle
{
    public class PlayGameState : IGameState
    {
        //Temporary handling for animations (really row and bad)
        //This will be better handled as flags for body,letter inside, letter outside rather than enum
        enum eHitState
        {
            HIT_NONE = 0, HIT_LETTERINSIDE_AND_BODY, HIT_LETTEROUSIDE_AND_BODY, HIT_BODY_ONLY, HIT_LETTERINSIDE, HIT_LETTEROUTSIDE, HIT_EXIT
        }

        ColorTickleGame game;
        float timer = 4;

        private ColorsUIManager m_ColorsUIManager;
        private bool m_Tickle;
        private float m_TickleTime = 1;

        private eHitState m_eHitState = eHitState.HIT_NONE;

        public PlayGameState(ColorTickleGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            m_ColorsUIManager = game.m_ColorsCanvas.GetComponentInChildren<ColorsUIManager>();
            m_ColorsUIManager.SetBrushColor += SetBrushColor;

            game.currentLetter.GetComponent<TMPTextColoring>().OnShapeHit += ShapeTouched;
            game.currentLetter.GetComponent<SurfaceColoring>().OnBodyHit += BodyTouched;
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            //timer -= delta;
            //if (timer < 0)
            //{
            //    game.SetCurrentState(game.ResultState);
            //}


            /*if (m_Tickle == true) {

                m_TickleTime -= delta;
                if (m_TickleTime < 0 && !game.currentLetter.GetComponentInChildren<Animator>().IsInTransition(0))
                {
                    m_Tickle = false;
                    game.currentLetter.GetComponentInChildren<Animator>().SetInteger("State", 0);
                  
                }
            }*/
            if (m_eHitState!=eHitState.HIT_NONE)
            {
                m_eHitState = eHitState.HIT_EXIT;
                m_TickleTime -= delta;
                if (m_TickleTime < 0 && !game.currentLetter.GetComponentInChildren<Animator>().IsInTransition(0))
                {
                    m_eHitState = eHitState.HIT_NONE;
                    game.currentLetter.GetComponentInChildren<Animator>().SetInteger("State", 1);
                    game.currentLetter.GetComponent<TMPTextColoring>().enableColor = true;
                    game.currentLetter.GetComponent<SurfaceColoring>().enableColor = true;
                }


            }




        }

        public void UpdatePhysics(float delta)
        {
        }

        private void SetBrushColor(Color color)
        {
            //game.m_ColorBrush = color;
            game.currentLetter.GetComponent<ColoringParameters>().SetBrushColor(color);
            Debug.Log("New BrushColor :" + game.currentLetter);
        }

        private void TicklesLetter()
        {
            
            Debug.Log("Tickle");
            m_TickleTime = 1;
            //m_Tickle = true;
            game.currentLetter.GetComponentInChildren<Animator>().SetInteger("State", 10);
            game.currentLetter.GetComponent<TMPTextColoring>().enableColor = false;
            game.currentLetter.GetComponent<SurfaceColoring>().enableColor = false;
        }

        private void BodyTouched()
        {
            Debug.Log("LaunchedBodyHit " + m_eHitState);
            //when letter is already hit outside, tickle
            if (m_eHitState == eHitState.HIT_LETTEROUTSIDE)
            {
                m_eHitState = eHitState.HIT_LETTEROUSIDE_AND_BODY;
                TicklesLetter();
            }
            //when letter is already hit inside, do not tickle
            else if (m_eHitState == eHitState.HIT_LETTERINSIDE)
            {
                m_eHitState = eHitState.HIT_LETTERINSIDE_AND_BODY;
            }
            //when letter isn't hit, tickle
            else
            {
                m_eHitState = eHitState.HIT_BODY_ONLY;
                TicklesLetter();
            }
        }

        private void ShapeTouched(bool bShapeHitted)
        {
            //when the hit is inside
            if (bShapeHitted)
            {
                //when body was hitted
                if (m_eHitState==eHitState.HIT_BODY_ONLY)
                {
                    m_eHitState = eHitState.HIT_LETTERINSIDE_AND_BODY;
                }
                //when body wasn't hitted
                else
                {
                    m_eHitState = eHitState.HIT_LETTERINSIDE;
                }
            }
            //when the hit is outside
            else
            {
                //when body was hitted
                if (m_eHitState == eHitState.HIT_BODY_ONLY)
                {
                    m_eHitState = eHitState.HIT_LETTEROUSIDE_AND_BODY;
                    
                }
                //when body wasn't hitted
                else
                {
                    m_eHitState = eHitState.HIT_LETTEROUTSIDE;
                }

                TicklesLetter();
            }


        }


    }
}
