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
                game.Context.GetAudioManager().PlayDialogue(TextID.READINGGAME_TITLE, () => { introCompleted = true; });
            }
            else if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.AlphabetSong)
            {
                game.Context.GetAudioManager().PlayDialogue(TextID.ALPHABETSONG_TITLE, () => { introCompleted = true; });
            }
            else
            {
                introCompleted = true;
            }
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