using UnityEngine;

namespace EA4S
{
    public abstract class MiniGame : MiniGameBase, IGame
    {
        private OutcomeGameState OutcomeState;

        public int StarsScore { get; private set; }

        public IGameContext Context { get; private set; }
        public IWordProvider WordProvider { get; private set; }

        // When the game is ended, this event is raised
        public event GameResultAction OnGameEnded;

        /// <summary>
        /// Specify which is the first state of this game using this method
        /// </summary>
        protected abstract IGameState GetInitialState();

        /// <summary>
        /// Implement game's construction steps inside this method.
        /// </summary>
        protected abstract void OnInitialize(IGameContext context, int difficulty, IWordProvider wordProvider);

        GameStateManager stateManager = new GameStateManager();
        public GameStateManager StateManager { get { return stateManager; } }

        public void Initialize(IGameContext context, int difficulty, IWordProvider wordProvider)
        {
            Context = context;
            WordProvider = wordProvider;

            OutcomeState = new OutcomeGameState(this);

            base.Start();
            OnInitialize(context, difficulty, wordProvider);
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

        public void EndGame(int stars, int score)
        {
            StarsScore = stars;

            if (OnGameEnded != null)
                OnGameEnded(stars, score);

            this.SetCurrentState(OutcomeState);
        }
    }
}