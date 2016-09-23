// Written by Davide Barbieri <davide.barbieri AT ghostshark.it>
namespace EA4S
{
    public delegate void GameResult(bool won, int score);

    public interface IGame
    {
        GameStateManager StateManager { get; }

        event GameResult OnGameEnded;

        void Initialize(IGameContext context, int difficulty /*, World currentWorld*/);
    }
}