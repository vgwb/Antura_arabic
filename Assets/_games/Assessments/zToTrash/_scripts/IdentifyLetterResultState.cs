using UnityEngine;

namespace EA4S.IdentifyLetter
{
    public class IdentifyLetterResultState : IGameState
    {
        IdentifyLetterGame game;

        float timer = 0.7f;
        public IdentifyLetterResultState(IdentifyLetterGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.Context.GetAudioManager().PlayMusic(Music.Relax);
        }


        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0) {
                game.EndGame(0, 0);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
