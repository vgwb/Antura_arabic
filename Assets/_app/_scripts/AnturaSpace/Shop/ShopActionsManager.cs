using Antura.Core;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopActionsManager : MonoBehaviour
    {
        [Header("Debug")]
        public bool addDebugBones = false;

        public ShopPanelUI ShopPanelUi;
        public ShopDecorationsManager ShopDecorationsManager;

        private ShopAction[] shopActions;

        void Start()
        {
            if (addDebugBones) {
                AppManager.I.Player.AddBones(50);
            }

            // Setup the decorations manager
            var shopState = AppManager.I.Player.CurrentShopState;
            ShopDecorationsManager.Initialise(shopState);
            ShopDecorationsManager.OnContextChange += (x) => UpdateAllActions();

            // Setup actions
            shopActions = GetComponentsInChildren<ShopAction>();
            foreach (var shopAction in shopActions)
            {
                shopAction.OnActionCommitted += HandleActionPerformed;
                shopAction.OnActionRefreshed += HandleActionRefreshed;
            }

            ShopPanelUi.SetActions(shopActions);
            ShopPanelUi.UpdateAllActionButtons();
        }

        private void HandleActionPerformed()
        {
            UpdateAllActions();
        }

        private void HandleActionRefreshed()
        {
            UpdateAllActions();
        }

        void UpdateAllActions()
        {
            ShopPanelUi.UpdateAllActionButtons();
        }

    }
}