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
            game.gameUI.Initialize(false, false, true);
            game.gameUI.SetMaxLives(game.lives);
            AudioManager.I.PlayMusic(game.backgroundMusic);

            game.myLetters = new GameObject[game.rounds];
			for (int i = 0; i < game.rounds; ++i) {
				game.myLetters[i] = Object.Instantiate(game.m_LetterPrefab);
                game.myLetters[i].SetActive(true);
                game.myLetters[i].GetComponent<LetterObjectView>().Init(AppManager.Instance.Letters.GetRandomElement());
                game.myLetters[i].GetComponent<LLController>().movingToDestination = false;

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
				game.SetCurrentState(game.QuestionState);
            }
        }

        public void UpdatePhysics(float delta)
        {

             
        }

    }
                
}