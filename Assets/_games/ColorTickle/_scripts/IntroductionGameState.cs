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

        

        public IntroductionGameState(ColorTickleGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.currentLetter = LetterObjectView.Instantiate(game.m_LetterPrefab);
            game.currentLetter.Init(AppManager.Instance.Letters.GetRandomElement());
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