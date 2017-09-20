using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.AnturaSpace
{
    public class ShopPanelUI : MonoBehaviour
    {
        public GameObject shopActionUIPrefab;
        public Transform buttonsPivotTr;

        public RectTransform purchasePanel;
        public RectTransform dragPanel;
        public RectTransform confirmationPanel;
        public ShopConfirmationPanelUI confirmationPanelUI;

        public Button confirmationYesButton;
        public Button confirmationNoButton;

        private List<ShopActionUI> actionUIs;    

        private Tween showShopPanelTween, showDragPanelTween, showConfirmationPanelTween;

        public void SetActions(ShopAction[] shopActions)
        {
            actionUIs = new List<ShopActionUI>();
            foreach (var shopAction in shopActions)
            {
                var shopActionUIgo = Instantiate(shopActionUIPrefab);
                shopActionUIgo.transform.SetParent(buttonsPivotTr);
                shopActionUIgo.transform.localScale = Vector3.one;
                var actionUI = shopActionUIgo.GetComponent<ShopActionUI>();
                actionUI.SetAction(shopAction);
                actionUIs.Add(actionUI);
            }
        }

        private void Start()
        {
            const float duration = 0.3f;
            showShopPanelTween =DOTween.Sequence() .SetAutoKill(false) .Pause()
                    .Append(purchasePanel.DOAnchorPosY(-350, duration).From().SetEase(Ease.OutBack));
            showDragPanelTween = DOTween.Sequence().SetAutoKill(false).Pause()
                    .Append(dragPanel.DOAnchorPosY(-350, duration).From().SetEase(Ease.OutBack));
            showConfirmationPanelTween =
                confirmationPanel.DOAnchorPosY(-350, duration).From().SetEase(Ease.OutBack).SetAutoKill(false).Pause();

            ShopDecorationsManager.I.OnContextChange += HandleContextChange;
            ShopDecorationsManager.I.OnPurchaseConfirmationRequested += HandlePurchaseConfirmationRequested;
            ShopDecorationsManager.I.OnDeleteConfirmationRequested += HandleDeleteConfirmationRequested;
          
        }


        private void OnEnable()
        {
            HandleContextChange(ShopContext.Purchase);
        }

        private void HandleContextChange(ShopContext shopContext)
        {
            switch (shopContext)
            {
                case ShopContext.Purchase:
                    showShopPanelTween.PlayForward();
                    showDragPanelTween.PlayBackwards();
                    showConfirmationPanelTween.PlayBackwards();
                    break;
                case ShopContext.NewPlacement:
                    showShopPanelTween.PlayBackwards();
                    showDragPanelTween.PlayBackwards();
                    showConfirmationPanelTween.PlayBackwards();
                    break;
                case ShopContext.MovingPlacement:
                    showShopPanelTween.PlayBackwards();
                    showDragPanelTween.PlayForward();
                    showConfirmationPanelTween.PlayBackwards();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("shopContext", shopContext, null);
            }
        }

        private void HandlePurchaseConfirmationRequested()
        {
            showDragPanelTween.PlayBackwards();

            confirmationPanelUI.SetupForPurchase();
            confirmationYesButton.onClick.RemoveAllListeners();
            confirmationYesButton.onClick.AddListener(ShopDecorationsManager.I.ConfirmPurchase);
            confirmationNoButton.onClick.RemoveAllListeners();
            confirmationNoButton.onClick.AddListener(ShopDecorationsManager.I.CancelPurchase);

            showConfirmationPanelTween.PlayForward();
        }

        private void HandleDeleteConfirmationRequested()
        {
            showDragPanelTween.PlayBackwards();

            confirmationPanelUI.SetupForDeletion();
            confirmationYesButton.onClick.RemoveAllListeners();
            confirmationYesButton.onClick.AddListener(ShopDecorationsManager.I.ConfirmDeletion);
            confirmationNoButton.onClick.RemoveAllListeners();
            confirmationNoButton.onClick.AddListener(ShopDecorationsManager.I.CancelDeletion);

            showConfirmationPanelTween.PlayForward();
        }

        public void UpdateAllActionButtons()
        {
            foreach (var actionUI in actionUIs)
            {
                actionUI.UpdateAction();
            }
        }
    }
}