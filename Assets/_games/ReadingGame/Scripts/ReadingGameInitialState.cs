using System;
using UnityEngine;

namespace EA4S.ReadingGame
{
    public class ReadingGameInitialState : IGameState
    {
        ReadingGameGame game;

        float timer = 2;

        bool introCompleted = false;

        public ReadingGameInitialState(ReadingGameGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            timer = 2;

            if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.ReadAndAnswer)
            {
                game.Context.GetAudioManager().PlayDialogue(Db.LocalizationDataId.ReadingGame_Title, () => { introCompleted = true; });
            }
            else if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.AlphabetSong)
            {
                game.Context.GetAudioManager().PlayDialogue(Db.LocalizationDataId.AlphabetSong_Title, () => { introCompleted = true; });
            }
            else
            {
                introCompleted = true;
            }

            if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.ReadAndAnswer)
                game.Context.GetAudioManager().PlayMusic(Music.Theme8);
        }


        public void ExitState()
        {
           
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0 && introCompleted)
                game.SetCurrentState(game.QuestionState);
        }

        public void UpdatePhysics(float delta)
        {

        }
    }
}