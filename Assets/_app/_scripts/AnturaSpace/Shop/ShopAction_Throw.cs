using Antura.Core;

namespace Antura.AnturaSpace
{
    public class ShopAction_Throw : ShopAction
    {
        public ThrowableObject throwingObjectPrefabGO;

        public AnturaSpaceScene AnturaSpaceScene
        {
            get { return ((AnturaSpaceScene) AnturaSpaceScene.I); }
        }

        public override void PerformAction()
        {
            ThrowableObject thrownObject = AnturaSpaceScene.ThrowObject(throwingObjectPrefabGO);
            if (thrownObject != null)
            {
                thrownObject.OnDeath += RefreshAction;
            }
            CommitAction();
        }

        public override void PerformDrag()
        {
            ThrowableObject thrownObject = AnturaSpaceScene.DragObject(throwingObjectPrefabGO);
            if (thrownObject != null)
            {
                thrownObject.OnDeath += RefreshAction;
            }
            CommitAction();
        }

        public override bool IsLocked
        {
            get
            {
                if (base.IsLocked) return base.IsLocked;
                return !AnturaSpaceScene.CanSpawnMoreObjects;
            }
        }
    }
}