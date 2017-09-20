
using System;

namespace Antura.AnturaSpace
{
    public class ShopAction_UnlockDecoration : ShopAction
    {
        public ShopDecorationObject UnlockableDecorationObject;

        public override void PerformAction()
        {
            //ShopDecorationsManager.I.PrepareNewDecorationPlacement(UnlockableDecorationObject);
        }

        public override void PerformDrag()
        {
            ShopDecorationsManager.I.CreateAndStartDragPlacement(UnlockableDecorationObject);
            ShopDecorationsManager.I.OnPurchaseComplete += CommitAction;
        }

        public override void CancelAction()
        {
            // Back to shop
           // ShopDecorationsManager.I.CancelPlacement();
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