
namespace EA4S.AnturaSpace
{
    public class AnturaState : IState
    {
        protected AnturaSpaceManager controller;

        public AnturaState(AnturaSpaceManager controller)
        {
            this.controller = controller;
        }

        public virtual void EnterState()
        {
         
        }

        public virtual void ExitState()
        {
         
        }

        public virtual void Update(float delta)
        {
         
        }

        public virtual void UpdatePhysics(float delta)
        {
        }
    }
}
