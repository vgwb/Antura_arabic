using EA4S.Audio;

namespace EA4S.Minigames.Scanner
{
	public class ScannerResultState : IState
	{
		ScannerGame game;

		float timer = 2;

		public ScannerResultState(ScannerGame game)
		{
			this.game = game;
		}

		public void EnterState()
		{
			
		}

		public void ExitState()
		{
		}

		public void Update(float delta)
		{
			timer -= delta;

			if (timer < 0)
            {
				game.EndGame(game.CurrentStars, game.CurrentScoreRecord);
                game.Context.GetOverlayWidget().Initialize(false, false, false);
                AudioManager.I.PlayMusic(Music.Relax);

                if (game.CurrentStars == 0)
                    AudioManager.I.PlayDialogue("Reward_0Star");
                else
                    AudioManager.I.PlayDialogue("Reward_" + game.CurrentStars + "Star_" + UnityEngine.Random.Range(1, 4));
            }
		}

		public void UpdatePhysics(float delta)
		{
		}
	}
}