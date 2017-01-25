namespace EA4S.MinigamesCommon
{
    public static class IGameExtensions
    {
        public static IGameState GetCurrentState(this IGame game)
        {
            return game.StateManager.CurrentState;
        }

        public static void SetCurrentState(this IGame game, IGameState state)
        {
            game.StateManager.CurrentState = state;
        }
    }
}