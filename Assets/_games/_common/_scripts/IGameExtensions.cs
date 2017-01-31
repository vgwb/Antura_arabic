namespace EA4S.MinigamesCommon
{
    public static class IGameExtensions
    {
        public static IState GetCurrentState(this IGame game)
        {
            return game.StateManager.CurrentState;
        }

        public static void SetCurrentState(this IGame game, IState state)
        {
            game.StateManager.CurrentState = state;
        }
    }
}