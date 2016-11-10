namespace EA4S.Maze
{
    public class MazeIntroState : IGameState
    {
        MazeGameManager game;

        int currentShownTubes = 2;



        public MazeIntroState(MazeGameManager game)
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