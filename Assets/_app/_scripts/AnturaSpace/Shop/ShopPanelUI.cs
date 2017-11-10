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

        public Transform bottomButtonsPivotTr;
        public Transform sideButtonsPivotTr;

        public RectTransform purchasePanelBottom;
        public RectTransform purchasePanelSide;
        public RectTransform purchasePanelAlwaysAvailableUI;
        public RectTransform dragPanel;
        public RectTransform confirmationPanel;
        public ShopConfirmationPanelUI confirmationPanelUI;

        public Button confirmationYesButton;
        public Button confirmationNoButton;

        private List<ShopActionUI> actionUIs;

        private Tween showShopPanelTween,
            showDragPanelTween,
            showConfirmationPanelTween,
            showPurchasePanelAlwaysAvailableTween;


        public void SetActions(ShopAction[] shopActions)
        {
            actionUIs = new List<ShopActionUI>();
            foreach (var shopAction in shopActions)
            {
                var shopActionUIgo = Instantiate(shopActionUIPrefab);
                var parentTr = shopAction.IsOnTheSide ? sideButtonsPivotTr : bottomButtonsPivotTr;
                shopActionUIgo.transform.SetParent(parentTr);
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
                    .Append(purchasePanelBottom.DOAnchorPosY(-350, duration).From().SetEase(Ease.OutBack))
                    .Join(purchasePanelSide.DOAnchorPosX(1250, duration).From().SetEase(Ease.OutBack));
            showDragPanelTween = DOTween.Sequence().SetAutoKill(false).Pause()
                    .Append(dragPanel.DOAnchorPosY(-350, duration).From().SetEase(Ease.OutBack));
            showConfirmationPanelTween =
                confirmationPanel.DOAnchorPosY(-350, duration).From().SetEase(Ease.Linear).SetAutoKill(false).Pause();
            showPurchasePanelAlwaysAvailableTween =
                purchasePanelAlwaysAvailableUI.DOAnchorPosX(200, duration)
                    .From()
                    .SetEase(Ease.OutBack)
                    .SetAutoKill(false);

            ShopDecorationsManager.I.OnContextChange += HandleContextChange;
            ShopDecorationsManager.I.OnPurchaseConfirmationRequested += HandlePurchaseConfirmationRequested;
            ShopDecorationsManager.I.OnDeleteConfirmationRequested += HandleDeleteConfirmationRequested;
            ShopPhotoManager.I.OnPhotoConfirmationRequested += HandlePhotoConfirmationRequested;
        }


        private void OnEnable()
        {
            HandleContextChange(ShopContext.Purchase);
        }

        private void HandleContextChange(ShopContext shopContext)
        {
            //Debug.Log("CONTEXT: " + shopContext);
            switch (shopContext)
            {
                case ShopContext.Purchase:
                    showPurchasePanelAlwaysAvailableTween.PlayForward();
                    showShopPanelTween.PlayForward();
                    showDragPanelTween.PlayBackwards();
                    showConfirmationPanelTween.PlayBackwards();
                    break;
                case ShopContext.NewPlacement:
                    showPurchasePanelAlwaysAvailableTween.PlayBackwards();
                    showShopPanelTween.PlayBackwards();
                    showDragPanelTween.PlayBackwards();
                    showConfirmationPanelTween.PlayBackwards();
                    break;
                case ShopContext.MovingPlacement:
                    showPurchasePanelAlwaysAvailableTween.PlayBackwards();
                    showShopPanelTween.PlayBackwards();
                    showDragPanelTween.PlayForward();
                    showConfirmationPanelTween.PlayBackwards();
                    break;
                case ShopContext.SpecialAction:
                    showShopPanelTween.PlayBackwards();
                    showDragPanelTween.PlayBackwards();
                    showConfirmationPanelTween.PlayBackwards();
                    break;
                case ShopContext.Closed:
                    showPurchasePanelAlwaysAvailableTween.PlayForward();
                    showConfirmationPanelTween.PlayBackwards();
                    break;
                case ShopContext.Customization:
                    showPurchasePanelAlwaysAvailableTween.PlayBackwards();
                    showConfirmationPanelTween.PlayBackwards();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("shopContext", shopContext, null);
            }
        }

        private void HandlePurchaseConfirmationRequested()
        {
            showDragPanelTween.PlayBackwards();
            showPurchasePanelAlwaysAvailableTween.PlayBackwards();

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
            showPurchasePanelAlwaysAvailableTween.PlayBackwards();

            confirmationPanelUI.SetupForDeletion();
            confirmationYesButton.onClick.RemoveAllListeners();
            confirmationYesButton.onClick.AddListener(ShopDecorationsManager.I.ConfirmDeletion);
            confirmationNoButton.onClick.RemoveAllListeners();
            confirmationNoButton.onClick.AddListener(ShopDecorationsManager.I.CancelDeletion);

            showConfirmationPanelTween.PlayForward();
        }

        private void HandlePhotoConfirmationRequested()
        {
            showShopPanelTween.PlayBackwards();
            showDragPanelTween.PlayBackwards();
            showPurchasePanelAlwaysAvailableTween.PlayBackwards();

            confirmationPanelUI.SetupForPhoto();
            confirmationYesButton.onClick.RemoveAllListeners();
            confirmationYesButton.onClick.AddListener(ShopPhotoManager.I.ConfirmPhoto);
            confirmationNoButton.onClick.RemoveAllListeners();
            confirmationNoButton.onClick.AddListener(ShopPhotoManager.I.CancelPhoto);

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