using Antura.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.AnturaSpace
{
    public class ShopActionUI : MonoBehaviour
    {
        public Image iconUI;
        public TextMeshProUGUI amountUI;
        public Button buttonUI;

        private ShopAction shopAction;

        public void SetAction(ShopAction shopAction)
        {
            this.shopAction = shopAction;
            iconUI.sprite = shopAction.iconSprite;
            amountUI.text = shopAction.bonesCost.ToString();
            buttonUI.interactable = !shopAction.IsLocked;
        }

        public void Update()
        {
            // TODO: react to any spending of bones!
            buttonUI.interactable = !shopAction.IsLocked;
        }

        public void OnClick()
        {
            if (ShopDecorationsManager.I.ShopContext != ShopContext.Shopping)
            {
                return;
            }

            if (AppManager.I.Player.GetTotalNumberOfBones() >= shopAction.bonesCost) {
                AppManager.I.Player.RemoveBones(shopAction.bonesCost);
                shopAction.PerformAction();
            }
        }

        public void OnDrag()
        {
            if (ShopDecorationsManager.I.ShopContext != ShopContext.Shopping)
            {
                return;
            }

            if (AppManager.I.Player.GetTotalNumberOfBones() >= shopAction.bonesCost)
            {
                AppManager.I.Player.RemoveBones(shopAction.bonesCost);
                shopAction.PerformDrag();
            }
        }

    }
}