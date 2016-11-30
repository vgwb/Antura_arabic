using System;
using TMPro;
using UnityEngine;
using System.Collections;

namespace EA4S.SickLetters
{
    public class IntroductionGameState : IGameState
    {
        SickLettersGame game;

        float timer = 2.5f;
        public IntroductionGameState(SickLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            Debug.Log("enter intro");
            AudioManager.I.PlayDialog(Db.LocalizationDataId.SickLetters_Title);
            //WidgetSubtitles.I.DisplaySentence(Db.LocalizationDataId.SickLetters_Title, 1.75f, true);
            game.antura.sleep();
            game.disableInput = true;
        }

        public void ExitState()
        {
            Debug.Log("exit intro");
            AudioManager.I.PlayDialog(Db.LocalizationDataId.SickLetters_Intro);
            //WidgetSubtitles.I.DisplaySentence(Db.LocalizationDataId.SickLetters_Intro, 5.75f, true);
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