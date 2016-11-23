namespace EA4S.TakeMeHome
{
    public class TakeMeHomeTutorialIntroState : IGameState
    {

        TakeMeHomeGame game;
        
        public TakeMeHomeTutorialIntroState(TakeMeHomeGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            //create a random LL and make it move:
            AudioManager.I.PlayDialog("TakeMeHome_Title",()=> {
                
                AudioManager.I.PlayDialog("TakeMeHome_Intro", () => {
                    
                    game.spawnLetteAtTube();
                });

               
            });
            
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            if (game.currentLetter != null && !game.currentLetter.isMoving)
            {
                game.currentLetter.isDraggable = true;
                game.SetCurrentState(game.TutorialPlayState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
