namespace Antura.AnturaSpace
{
    public class ShopAction_Ball : ShopAction
    {
        public override void PerformAction()
        {
            SetLocked(true);
            // TODO: enable the ball in the scene
            // TODO: attach to the ball's lifetime. Wwhen it dies, unlock this action
        }

    }
}