using System;
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

        public Button confirmationYesButton;
        public Button confirmationNoButton;

        private Tween showShopPanelTween, showDragPanelTween, showConfirmationPanelTween;

        public void SetActions(ShopAction[] shopActions)
        {
            foreach (var shopAction in shopActions)
            {
                var shopActionUIgo = Instantiate(shopActionUIPrefab);
                shopActionUIgo.transform.SetParent(buttonsPivotTr);
                shopActionUIgo.transform.localScale = Vector3.one;
                shopActionUIgo.GetComponent<ShopActionUI>().SetAction(shopAction);
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

            // TODO: setup the confirmation panel for purchase
            confirmationYesButton.onClick.RemoveAllListeners();
            confirmationYesButton.onClick.AddListener(ShopDecorationsManager.I.ConfirmPurchase);
            confirmationNoButton.onClick.RemoveAllListeners();
            confirmationNoButton.onClick.AddListener(ShopDecorationsManager.I.CancelPurchase);

            showConfirmationPanelTween.PlayForward();
        }

        private void HandleDeleteConfirmationRequested()
        {
            showDragPanelTween.PlayBackwards();

            // TODO: setup the confirmation panel for deletion
            confirmationYesButton.onClick.RemoveAllListeners();
            confirmationYesButton.onClick.AddListener(ShopDecorationsManager.I.ConfirmDeletion);
            confirmationNoButton.onClick.RemoveAllListeners();
            confirmationNoButton.onClick.AddListener(ShopDecorationsManager.I.CancelDeletion);

            showConfirmationPanelTween.PlayForward();
        }

    }
}