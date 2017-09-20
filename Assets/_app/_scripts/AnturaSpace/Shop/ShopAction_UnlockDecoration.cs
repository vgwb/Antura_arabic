
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
            ShopDecorationsManager.I.CreateAndStartDragPlacement(UnlockableDecorationObject, bonesCost);
            ShopDecorationsManager.I.OnPurchaseComplete += CommitAction;
        }

        public override void CancelAction()
        {
            // Back to shop
           // ShopDecorationsManager.I.CancelPlacement();
        }

        public override bool IsLocked
        {
            get
            {
                if (base.IsLocked) return base.IsLocked;
                return !ShopDecorationsManager.I.HasSlotsForDecoration(UnlockableDecorationObject);
            }
        }

    }
}