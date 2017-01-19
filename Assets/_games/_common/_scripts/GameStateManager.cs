namespace EA4S
{
    /// <summary>
    /// Implements a Finite State Machine (FSM).
    /// This is used to control minigame flow, and may also be used as a general purpose FSM.
    /// </summary>
    /// refactor: we may remove the *Game* prefix, as this is used as a general FSM
    public class GameStateManager
    {
        // Use to prevent recursive calls to ExitState, when the transition happens in ExitState
        bool isInExitTransition = false;

        IGameState currentState;
        public IGameState CurrentState
        {
            get
            {
                return currentState;
            }

            set
            {
                if (!isInExitTransition && currentState != null)
                {
                    isInExitTransition = true;
                    currentState.ExitState();
                    isInExitTransition = false;
                }

                currentState = value;

                if (currentState != null)
                    currentState.EnterState();
            }
        }

        public void Update(float delta)
        {
            if (currentState != null)
                currentState.Update(delta);
        }

        public void UpdatePhysics(float delta)
        {
            if (currentState != null)
                currentState.UpdatePhysics(delta);
        }
    }
}