using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ModularFramework.Helpers;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.ColorTickle
{
    public class IntroductionGameState : IGameState
    {
        ColorTickleGame game;

        float timer = 1;

        bool m_FirstLetter = true;

        public IntroductionGameState(ColorTickleGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            IGameContext gameCotext = ColorTickleConfiguration.Instance.Context;
            game.gameUI = gameCotext.GetOverlayWidget();
            game.gameUI.Initialize(false, true, true);
            game.gameUI.SetMaxLives(game.lives);
            game.gameUI.SetClockDuration(game.clockTime);

            if (m_FirstLetter)
            {
                game.currentLetter = LetterObjectView.Instantiate(game.m_LetterPrefab);
                m_FirstLetter = false;
            }
            else
            {
                //Destroy previous Letter
                //Istantiate a new Letter 
            }

            game.currentLetter.Init(AppManager.Instance.Letters.GetRandomElement());

            //game.currentLetter.GetComponent<TMPTextColoring>().enableColor = false;
            //game.currentLetter.GetComponent<SurfaceColoring>().enableColor = false;
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.SetCurrentState(game.PlayState);
            }
        }

        public void UpdatePhysics(float delta)
        {

             
        }

    }
                
}