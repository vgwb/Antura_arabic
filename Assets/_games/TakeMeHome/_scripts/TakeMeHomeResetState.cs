namespace EA4S.TakeMeHome
{
	public class TakeMeHomeResetState : IGameState {

		TakeMeHomeGame game;
		bool win;

		public TakeMeHomeResetState(TakeMeHomeGame game)
		{
			this.game = game;
			win = false;
		}

		public void EnterState()
		{
           

            if (game.currentLetter.collidedTubes.Count == 0) {
				win = false;
				game.currentLetter.respawn = true;
				return;
			}

			int tubeIndex = int.Parse(game.currentLetter.collidedTubes[game.currentLetter.collidedTubes.Count-1].name.Substring (5));

			win = false;
			if (tubeIndex == game.currentTube) {
                TakeMeHomeConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Win);
				//AudioManager.I.PlaySfx (Sfx.Win);
				win = true;
				game.IncrementScore ();
                TutorialUI.MarkYes(game.currentLetter.transform.position + new UnityEngine.Vector3(0,0,-5), TutorialUI.MarkSize.Big);
			} else {
                TakeMeHomeConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Lose);
                TutorialUI.MarkNo(game.currentLetter.transform.position + new UnityEngine.Vector3(0, 0, -5), TutorialUI.MarkSize.Big);
            }

			game.currentLetter.followTube (win);
		}

		public void ExitState()
		{
			game.currentLetter.isDraggable = true;
		}

		public void Update(float delta)
		{
			if (!game.currentLetter.isMoving) {
				if(!win)
					game.SetCurrentState (game.PlayState);
				else
					game.SetCurrentState (game.IntroductionState);
			}
		}

		public void UpdatePhysics(float delta)
		{
		}


	}
}
