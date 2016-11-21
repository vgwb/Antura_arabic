namespace EA4S.TakeMeHome
{
    public class TakeMeHomeTutorialResetState : IGameState
    {

        TakeMeHomeGame game;
        bool win;

        public TakeMeHomeTutorialResetState(TakeMeHomeGame game)
        {
            this.game = game;
            win = false;
        }

        public void EnterState()
        {


            if (game.currentLetter.lastTube == null)
            {
                win = false;
                game.currentLetter.respawn = true;
                return;
            }

            int tubeIndex = int.Parse(game.currentLetter.lastTube.name.Substring(5));

            win = false;
            if (tubeIndex == game.currentTube)
            {
                AudioManager.I.PlaySfx(Sfx.Win);
                win = true;
                TutorialUI.MarkYes(game.currentLetter.transform.position + new UnityEngine.Vector3(0, 0, -5));
            }
            else {
                AudioManager.I.PlaySfx(Sfx.Lose);
                TutorialUI.MarkNo(game.currentLetter.transform.position + new UnityEngine.Vector3(0, 0, -5));
            }

            game.currentLetter.followTube(win);
        }

        public void ExitState()
        {
            game.currentLetter.isDraggable = true;
        }

        public void Update(float delta)
        {
            if (!game.currentLetter.isMoving)
            {
                if (!win)
                    game.SetCurrentState(game.TutorialPlayState);
                else
                {
                    game.SetCurrentState(game.IntroductionState);
                    game.initUI();
                }
                    
            }
        }

        public void UpdatePhysics(float delta)
        {
        }


    }
}
