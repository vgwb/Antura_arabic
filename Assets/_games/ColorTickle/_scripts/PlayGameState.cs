using UnityEngine;

namespace EA4S.ColorTickle
{
    public class PlayGameState : IGameState
    {
        ColorTickleGame game;
        float timer = 4;

        private ColorsUIManager m_ColorsUIManager;
        private bool m_Tickle;
        private float m_TickleTime = 1;

        public PlayGameState(ColorTickleGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            m_ColorsUIManager = game.m_ColorsCanvas.GetComponentInChildren<ColorsUIManager>();
            m_ColorsUIManager.SetBrushColor += SetBrushColor;

            game.currentLetter.GetComponent<TMPTextColoring>().OnTouchOutside += TicklesLetter;
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

           
            if (m_Tickle == true) {

                m_TickleTime -= delta;
                if (m_TickleTime < 0)
                {
                    m_Tickle = false;
                    game.currentLetter.GetComponent<Animator>().SetInteger("State", 0);
                }
            }
                
        }

        public void UpdatePhysics(float delta)
        {
        }

        private void SetBrushColor(Color color)
        {
            //game.m_ColorBrush = color;
            game.currentLetter.GetComponent<TMPTextColoring>().SetBrushColor(color);
            Debug.Log("New BrushColor :" + game.currentLetter);
        }

        private void TicklesLetter()
        {
            Debug.Log("Tickle");
            m_TickleTime = 1;
            m_Tickle = true;
            game.currentLetter.GetComponent<Animator>().SetInteger("State", 2);
        }
    }
}
