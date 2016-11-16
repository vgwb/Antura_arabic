namespace EA4S.ReadingGame
{
    public abstract class AnturaState : IGameState
    {
        protected ReadingGameAntura antura;

        public AnturaState(ReadingGameAntura antura)
        {
            this.antura = antura;
        }

        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void Update(float delta);
        public abstract void UpdatePhysics(float delta);
    }
}
