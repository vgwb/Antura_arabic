
using System;

namespace Antura.AnturaSpace
{
    public class ShopAction_UnlockDecoration : ShopAction
    {
        public ShopDecorationObject UnlockableDecorationObject;

        void Awake()
        {
            iconSprite = UnlockableDecorationObject.iconSprite;
        }

        public override void PerformDrag()
        {
            ShopDecorationsManager.I.CreateAndStartDragPlacement(UnlockableDecorationObject, bonesCost);
            ShopDecorationsManager.I.OnPurchaseComplete += CommitAction;
            ShopDecorationsManager.I.OnPurchaseCancelled += CancelAction;
        }

        protected override void CommitAction()
        {
            base.CommitAction();
            ShopDecorationsManager.I.OnPurchaseComplete -= CommitAction;
            ShopDecorationsManager.I.OnPurchaseCancelled -= CancelAction;
        }

        public override void CancelAction()
        {
            base.CancelAction();
            ShopDecorationsManager.I.OnPurchaseComplete -= CommitAction;
            ShopDecorationsManager.I.OnPurchaseCancelled -= CancelAction;
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