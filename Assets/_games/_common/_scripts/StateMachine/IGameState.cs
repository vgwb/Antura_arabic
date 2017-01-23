namespace EA4S
{
    /// <summary>
    /// Represents a game state in the GameStateManager
    /// </summary>
    public interface IGameState
    {
        void EnterState();
        void ExitState();

        void Update(float delta);
        void UpdatePhysics(float delta);
    }
}