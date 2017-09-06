
using System;

namespace Antura.AnturaSpace
{
    public class ShopAction_UnlockDecoration : ShopAction
    {
        public ShopDecorationObject UnlockableDecorationObject;

        public override void PerformAction()
        {
            switch (ShopDecorationsManager.I.ShopContext)
            {
                case ShopContext.Shopping:
                    ShopDecorationsManager.I.PrepareNewDecorationPlacement(UnlockableDecorationObject);
                    break;
                case ShopContext.Placement:
                    // Back to shop
                    ShopDecorationsManager.I.CancelPlacement();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void InitialiseLockedState()
        {
            base.InitialiseLockedState();

            if (!ShopDecorationsManager.I.HasSlotsForDecoration(UnlockableDecorationObject))
            {
                SetLocked(true);
            }
        }
    }
}