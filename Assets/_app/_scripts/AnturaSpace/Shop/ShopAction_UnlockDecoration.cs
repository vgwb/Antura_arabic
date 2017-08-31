
namespace Antura.AnturaSpace
{
    public class ShopAction_UnlockDecoration : ShopAction
    {
        public override void PerformAction()
        {
            ShopDecorationsManager.I.UnlockNewDecoration();
        }

        public override void InitialiseLockedState()
        {
            base.InitialiseLockedState();
            if (!ShopDecorationsManager.I.HasDecorationsToUnlock) SetLocked(true);
        }
    }
}