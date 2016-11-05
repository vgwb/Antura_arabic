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

        float timer = 1f;


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

			game.myLetters = new LetterObjectView[game.rounds];
			for (int i = 0; i < game.rounds; ++i) {
				game.myLetters[i] = LetterObjectView.Instantiate(game.m_LetterPrefab);
				game.myLetters[i].Init(AppManager.Instance.Letters.GetRandomElement());
				game.myLetters[i].gameObject.SetActive (false);
			}
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