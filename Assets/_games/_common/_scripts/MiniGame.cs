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

            NavigationManager.I.EndMinigame(stars);

            if (OnGameEnded != null)
                OnGameEnded(stars, score);

            // Log trace game result
            Context.GetLogManager().OnMiniGameResult(stars);

            this.SetCurrentState(OutcomeState);
        }

        GameStateManager stateManager = new GameStateManager();
        public GameStateManager StateManager { get { return stateManager; } }

        bool initialized = false;
        Vector3 oldGravity;

        void Initialize(IGameContext context)
        {
            Context = context;

            OutcomeState = new OutcomeGameState(this);

            base.Start();
            OnInitialize(context);
            this.SetCurrentState(GetInitialState());

            oldGravity = Physics.gravity;
            Physics.gravity = GetGravity();
            initialized = true;
        }

        void OnDestroy()
        {
            if (initialized)
                Physics.gravity = oldGravity;

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
            var audioManager = Context.GetAudioManager();

            // TODO: move this outside this method (actually it is useless with the current implementation of PauseMenu)
            inputManager.Enabled = !(GlobalUI.PauseMenu.IsMenuOpen);

            inputManager.Update(Time.deltaTime);
            audioManager.Update();

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Alpha0))
                EndGame(0, 0);
            else if (Input.GetKeyDown(KeyCode.Alpha1))
                EndGame(1, 1);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                EndGame(2, 2);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                EndGame(3, 3);
#endif
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

        public virtual Vector3 GetGravity()
        {
            return Vector3.up * (-80);
        }
    }
}