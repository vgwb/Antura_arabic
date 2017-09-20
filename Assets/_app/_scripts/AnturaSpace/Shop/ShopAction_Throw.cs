using Antura.Core;

namespace Antura.AnturaSpace
{
    public class ShopAction_Throw : ShopAction
    {
        public ThrowableObject throwingObjectPrefabGO;

        public override void PerformAction()
        {
            ((AnturaSpaceScene)AnturaSpaceScene.I).ThrowObject(throwingObjectPrefabGO);
            // TODO: attach to the ball's lifetime. Wwhen it dies, unlock this action
            CommitAction();
        }

        public override void PerformDrag()
        {
            ((AnturaSpaceScene) AnturaSpaceScene.I).DragObject(throwingObjectPrefabGO);
            base.PerformDrag();
            CommitAction();
        }

        public override bool IsLocked
        {
            get
            {
                 if (base.IsLocked) return base.IsLocked;
                return ((AnturaSpaceScene) AnturaSpaceScene.I).CanSpawnMoreObjects;
            }
        }
    }
}