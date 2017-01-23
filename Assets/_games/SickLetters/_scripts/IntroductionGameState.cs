using UnityEngine;
using EA4S.Audio;
using EA4S.MinigamesCommon;

namespace EA4S.Minigames.SickLetters
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
            AudioManager.I.PlayDialogue(Db.LocalizationDataId.SickLetters_Title);
            //WidgetSubtitles.I.DisplaySentence(Db.LocalizationDataId.SickLetters_Title, 1.75f, true);
            game.antura.sleep();
            game.disableInput = true;
        }

        public void ExitState()
        {
            Debug.Log("exit intro");
            AudioManager.I.PlayDialogue(Db.LocalizationDataId.SickLetters_Intro);
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