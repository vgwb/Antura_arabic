namespace Antura.MinigamesCommon
{
    public delegate void GameResultAction(int stars, int score);

    /// <summary>
    /// Represents a minigame manager.
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Event raised whenever the game ends.
        /// </summary>
        event GameResultAction OnGameEnded;

        /// <summary>
        /// Access the GameStateManager that controls the FSM that controls the minigame flow.
        /// </summary>
        StateManager StateManager { get; }
    }
}