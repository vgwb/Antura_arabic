namespace EA4S.TakeMeHome
{
    public class TakeMeHomeTutorialPlayState : IGameState
    {

        TakeMeHomeGame game;
        TakeMeHomeTube tube;
        float timer = 3;

        public TakeMeHomeTutorialPlayState(TakeMeHomeGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            
            //create a random LL and make it move:
            TakeMeHomeConfiguration.Instance.Context.GetAudioManager().PlayDialogue(Db.LocalizationDataId.TakeMeHome_Tuto, ()=> {
                game.currentLetter.sayLetter();
            });

            //find which tube belongs to current letter:
            foreach (UnityEngine.GameObject go in this.game.activeTubes)
            {
                if(go.name == "tube_"+ this.game.currentTube)
                {
                    tube = go.GetComponent<TakeMeHomeTube>();
                    break;
                }
            }

            if (tube == null) return;

            timer = 2;
            

            
        }

        private void showTutorial()
        {
            TutorialUI.Clear(false);
            TutorialUI.DrawLine(this.game.currentLetter.transform.position - new UnityEngine.Vector3(0,-2.5f,0), tube.cubeInfo.transform.position, TutorialUI.DrawLineMode.FingerAndArrow,false,true);
           
        }

        public void ExitState()
        {
            
        }

        public void Update(float delta)
        {
            //do not do anything while dragging:
            if(this.game.currentLetter.dragging || !this.game.currentLetter.isDraggable)
            {
                
                return;
            }

            if (!game.currentLetter.dragging && game.currentLetter.collidedTubes.Count > 0)
            {
                //check if we should do anything:
                game.SetCurrentState(game.TutorialResetState);
                return;
            }

            timer -= delta;
            if (timer < 0)
            {
                timer = 5;
                showTutorial();
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
