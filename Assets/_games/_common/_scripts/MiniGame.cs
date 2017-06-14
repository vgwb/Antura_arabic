using EA4S.Core;
using EA4S.UI;
using UnityEngine;

namespace EA4S.MinigamesCommon
{
    /// <summary>
    /// Base abstract class for all minigame in-scene managers.
    /// Main entry point for the logic of a minigame.
    /// </summary>
    // refactor: this could be merged with MiniGameBase
    // refactor: this could be better organized to signal what the minigame needs to access, and what the core needs
    public abstract class MiniGame : MiniGameBase, IGame
    {
        bool hasToPause;

        /// <summary>
        /// State reached when the minigame ends. 
        /// Exists regardless of the specific minigame.
        /// </summary>
        private OutcomeGameState OutcomeState;

        public int StarsScore { get; private set; }

        public IGameContext Context { get; private set; }

        /// <summary>
        /// Event raised whenever the game ends.
        /// </summary>
        public event GameResultAction OnGameEnded;

        /// <summary>
        /// Specify which is the first state of this game using this method
        /// </summary>
        protected abstract IState GetInitialState();

        /// <summary>
        /// Specify which is the game configuration class for this game
        /// </summary>
        protected abstract IGameConfiguration GetConfiguration();

        /// <summary>
        /// Implement game's construction steps inside this method.
        /// </summary>
        protected abstract void OnInitialize(IGameContext context);

        /// <summary>
        /// This must be called whenever the minigame ends.
        /// Called by the minigame logic.
        /// </summary>
        public void EndGame(int stars, int score)
        {
            StarsScore = stars;

            AppManager.Instance.NavigationManager.EndMinigame(stars);

            if (OnGameEnded != null)
                OnGameEnded(stars, score);

            // Log trace game result
            Context.GetLogManager().OnGameEnded(stars);

            this.SetCurrentState(OutcomeState);
        }

        /// <summary>
        /// Check if the game is in OutcomeState
        /// </summary>
        public bool IsEnded()
        {
            return stateManager.CurrentState == OutcomeState;
        }


        /// <summary>
        /// Access the GameStateManager that controls the minigame's FSM.
        /// </summary>
        public StateManager StateManager { get { return stateManager; } }
        StateManager stateManager = new StateManager();

        bool initialized = false;
        Vector3 oldGravity;

        /// <summary>
        /// Initializes the minigame with the given context.
        /// </summary>
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

            if ((AppManager.Instance.IsPaused || hasToPause) && !SceneTransitioner.IsShown && this.GetCurrentState() != OutcomeState)
                GlobalUI.PauseMenu.OpenMenu(true);
            hasToPause = false;

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

        void OnApplicationPause(bool pause)
        {
            if (pause)
                hasToPause = true;

        }
    }
}