using System;
using UnityEngine;

namespace EA4S
{
    public abstract class MiniGame : MiniGameBase, IGame
    {
        private OutcomeGameState OutcomeState;

        public int StarsScore { get; private set; }

        public IGameContext Context { get; private set; }

        // When the game is ended, this event is raised
        public event GameResultAction OnGameEnded;

        /// <summary>
        /// Specify which is the first state of this game using this method
        /// </summary>
        protected abstract IGameState GetInitialState();

        /// <summary>
        /// Specify which is the game configuration class for this game
        /// </summary>
        protected abstract IGameConfiguration GetConfiguration();

        /// <summary>
        /// Implement game's construction steps inside this method.
        /// </summary>
        protected abstract void OnInitialize(IGameContext context);

        public void EndGame(int stars, int score)
        {
            StarsScore = stars;

            if (OnGameEnded != null)
                OnGameEnded(stars, score);

            this.SetCurrentState(OutcomeState);
        }

        GameStateManager stateManager = new GameStateManager();
        public GameStateManager StateManager { get { return stateManager; } }

        void Initialize(IGameContext context)
        {
            Context = context;

            OutcomeState = new OutcomeGameState(this);

            base.Start();
            OnInitialize(context);
            this.SetCurrentState(GetInitialState());
        }

        void OnDestroy()
        {
            if (Context != null)
                Context.Reset();
        }

        /// <summary>
        /// Do not override Update/FixedUpdate; just implement Update and UpdatePhysics inside game states
        /// </summary>
        void Update()
        {
            stateManager.Update(Time.deltaTime);

            var inputManager = Context.GetInputManager();

            // TODO: move this outside this method (actually it is useless with the current implementation of PauseMenu)
            inputManager.Enabled = !(GlobalUI.PauseMenu.IsMenuOpen);

            inputManager.Update();
        }

        void FixedUpdate()
        {
            stateManager.UpdatePhysics(Time.fixedDeltaTime);
        }

        protected override void Start()
        {
            base.Start();

            Initialize(GetConfiguration().Context);
        }
    }
}