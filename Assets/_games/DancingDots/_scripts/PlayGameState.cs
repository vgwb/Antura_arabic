namespace EA4S.DancingDots
{
    public class PlayGameState : IGameState
    {
        DancingDotsGame game;

        float timer;
        int alarmIsTriggered = 0;
        IAudioSource alarmSound;

        public PlayGameState(DancingDotsGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            AudioManager.I.PlayDialogue("DancingDots_Intro", ()=> { game.disableInput = false; });
            this.game.dancingDotsLL.contentGO.SetActive(true);
            game.StartRound();
            timer = game.gameDuration;
        }

        public void ExitState()
        {
            game.DancingDotsEndGame();
        }

        public void Update(float delta)
        {
            if (!game.isTutRound)
            {
                timer -= delta;
                game.Context.GetOverlayWidget().SetClockTime(timer);
            }

            if (timer < 0)
            {
                if (alarmSound != null)
                {
                    alarmSound.Stop();
                    alarmSound = null;
                }
                game.SetCurrentState(game.ResultState);
                AudioManager.I.PlayDialogue("Keeper_TimeUp");
            }

            else if (alarmIsTriggered == 0 && timer < 20)
            {
                alarmIsTriggered = 1;
                AudioManager.I.PlayDialogue("Keeper_Time_" + UnityEngine.Random.Range(1, 4));
            }
            else if (alarmIsTriggered == 1 && timer < 4)
            {
                alarmIsTriggered = 2;
                alarmSound = game.Context.GetAudioManager().PlaySound(Sfx.DangerClockLong);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
