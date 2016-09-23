// Written by Davide Barbieri <davide.barbieri AT ghostshark.it>
namespace EA4S
{

    /// <summary>
    /// Manages game states/phases
    /// </summary>
    public class GameStateManager
    {
        IGameState currentState;
        public IGameState CurrentState
        {
            get
            {
                return currentState;
            }

            set
            {
                if (currentState != null)
                    currentState.ExitState();

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