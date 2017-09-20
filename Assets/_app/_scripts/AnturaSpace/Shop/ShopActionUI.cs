using Antura.Core;
using Antura.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.AnturaSpace
{
    public class ShopActionUI : MonoBehaviour
    {
        public Image iconUI;
        public TextMeshProUGUI amountUI;
        public UIButton buttonUI;

        private ShopAction shopAction;

        public void SetAction(ShopAction shopAction)
        {
            this.shopAction = shopAction;
            iconUI.sprite = shopAction.iconSprite;
            amountUI.text = shopAction.bonesCost.ToString();
            UpdateAction();
        }

        public void UpdateAction()
        {
            bool isLocked = shopAction.IsLocked;
            buttonUI.Lock(isLocked);
        }

        public void OnClick()
        {
            if (ShopDecorationsManager.I.ShopContext == ShopContext.Purchase)
            {
                if (AppManager.I.Player.GetTotalNumberOfBones() >= shopAction.bonesCost)
                {
                    shopAction.PerformAction();
                }
            }
        }

        public void OnDrag()
        {
            if (ShopDecorationsManager.I.ShopContext == ShopContext.Purchase)
            {
                if (AppManager.I.Player.GetTotalNumberOfBones() >= shopAction.bonesCost)
                {
                    shopAction.PerformDrag();
                }
            }
        }

    }
}