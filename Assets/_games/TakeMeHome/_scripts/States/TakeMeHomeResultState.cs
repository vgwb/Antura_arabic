using EA4S.Audio;
using UnityEngine;

namespace EA4S.Minigames.TakeMeHome
{

    public class TakeMeHomeResultState : IState {

        //TakeMeHomeGame game;

        public TakeMeHomeResultState(TakeMeHomeGame game)
		{
        //    this.game = game;
		}

		public void EnterState()
		{
            
        }

		public void ExitState()
		{
            //game.SetCurrentState(game.EndState);
        }

		public void Update(float delta)
		{
			
		}

		public void UpdatePhysics(float delta)
		{
		}
	}
}