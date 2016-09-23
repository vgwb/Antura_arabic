using UnityEngine;

namespace EA4S.Tobogan
{
    public abstract class TemplateGame : MiniGameBase, IGame
    {
        GameStateManager stateManager = new GameStateManager();
        public GameStateManager StateManager { get { return stateManager; } }

        public abstract IGameState GetInitialState();
        public abstract void Initialize();

        protected sealed override void Start()
        {
            base.Start();
            Initialize();
            this.SetCurrentState(GetInitialState());
        }

        void Update()
        {
            stateManager.Update(Time.deltaTime);
        }

        void FixedUpdate()
        {
            stateManager.UpdatePhysics(Time.fixedDeltaTime);
        }
    }
}