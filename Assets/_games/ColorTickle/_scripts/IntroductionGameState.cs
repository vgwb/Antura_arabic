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
            game.colorsCanvas.gameObject.SetActive(false);

            AudioManager.I.PlayMusic(game.backgroundMusic);

            game.myLetters = new GameObject[game.rounds];
			for (int i = 0; i < game.rounds; ++i) {
				game.myLetters[i] = Object.Instantiate(game.m_LetterPrefab);
                game.myLetters[i].SetActive(true);
                // HACK fix for the automatic reset of the color after update at Unity 5.4.2
                game.myLetters[i].GetComponent<LetterObjectView>().Lable.color = Color.white;
                game.myLetters[i].GetComponent<LetterObjectView>().Init(AppManager.Instance.Teacher.GetAllTestLetterDataLL().GetRandomElement());
                game.myLetters[i].GetComponent<ColorTickle_LLController>().movingToDestination = false;
            }

            LL_LetterData LLdata = new LL_LetterData("alef");
            game.tutorialLetter = Object.Instantiate(game.m_LetterPrefab);
            game.tutorialLetter.SetActive(true);
            // HACK fix for the automatic reset of the color after update at Unity 5.4.2
            game.tutorialLetter.GetComponent<LetterObjectView>().Lable.color = Color.white;
            game.tutorialLetter.GetComponent<LetterObjectView>().Init(LLdata);
            game.tutorialLetter.GetComponent<ColorTickle_LLController>().movingToDestination = false;
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