using EA4S.Audio;
using EA4S.MinigamesCommon;

namespace EA4S.Minigames.Scanner
{
	public class ScannerIntroductionState : IState
	{
		ScannerGame game;

		float timer = 2f;
		public ScannerIntroductionState(ScannerGame game)
		{
			this.game = game;
		}

		public void EnterState()
		{
            AudioManager.I.PlayDialogue(Database.LocalizationDataId.Scanner_Title);
        }

		public void ExitState()
		{
            AudioManager.I.PlayDialogue(Database.LocalizationDataId.Scanner_Intro);
        }

		public void Update(float delta)
		{
			timer -= delta;

			if (timer < 0)
			{
				game.SetCurrentState(game.PlayState);
				return;
			}
		}

		public void UpdatePhysics(float delta)
		{
		}
	}
}
