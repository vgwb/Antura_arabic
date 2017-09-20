namespace Antura.AnturaSpace
{
    public class ShopAction_Throw : ShopAction
    {
        public ThrowableObject throwingObjectPrefabGO;

        public override void PerformAction()
        {
            //SetLocked(true);
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
    }
}