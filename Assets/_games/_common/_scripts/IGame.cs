
namespace EA4S
{
    public delegate void GameResultAction(int stars, int score);

    public interface IGame
    {
        event GameResultAction OnGameEnded;

        GameStateManager StateManager { get; }
    }
}