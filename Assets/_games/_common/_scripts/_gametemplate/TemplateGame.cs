using UnityEngine;

namespace EA4S.Tobogan
{
    public abstract class TemplateGame : MiniGameBase, IGame
    {
        // When the game is ended, this event is raised
        public event GameResult OnGameEnded;

        /// <summary>
        /// Specify which is the first state of this game using this method
        /// </summary>
        protected abstract IGameState GetInitialState();

        /// <summary>
        /// Implement game's construction steps inside this method
        /// </summary>
        protected abstract void OnInitialize(IGameContext context, int difficulty);

        GameStateManager stateManager = new GameStateManager();
        public GameStateManager StateManager { get { return stateManager; } }

        public void Initialize(IGameContext context, int difficulty)
        {
            base.Start();
            OnInitialize(context, difficulty);
            this.SetCurrentState(GetInitialState());
        }

        /// <summary>
        /// Do not override Update/FixedUpdate; just implement Update and UpdatePhysics inside game states
        /// </summary>
        void Update()
        {
            stateManager.Update(Time.deltaTime);
        }

        void FixedUpdate()
        {
            stateManager.UpdatePhysics(Time.fixedDeltaTime);
        }

        protected void EndGame(bool won, int score)
        {
            this.SetCurrentState(null);
            if (OnGameEnded != null)
                OnGameEnded(won, score);
        }
    }
}