namespace EA4S.Minigames.Maze
{
    public class MazeIntroState : IState
    {
        MazeGame game;

        //int currentShownTubes = 2;



        public MazeIntroState(MazeGame game)
        {
            this.game = game;


        }

        public void EnterState()
        {
            game.startGame();
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            game.timer.Update();
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}