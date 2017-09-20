using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.AnturaSpace
{
    public class ShopActionsPanelUI : MonoBehaviour
    {
        public GameObject shopActionUIPrefab;
        public Transform buttonsPivotTr;

        public RectTransform shopPanel;
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
            showShopPanelTween = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause()
                .Append(shopPanel.DOAnchorPosY(150, duration).From().SetEase(Ease.OutBack))
                .Join(dragPanel.DOAnchorPosY(-830, duration));
            showDragPanelTween = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(shopPanel.DOAnchorPosY(-830, duration).From().SetEase(Ease.OutBack))
                .Join(dragPanel.DOAnchorPosY(150, duration));
            showConfirmationPanelTween =
                confirmationPanel.DOAnchorPosY(-350, duration).From().SetEase(Ease.OutBack).SetAutoKill(false).Pause();

            ShopDecorationsManager.I.OnContextChange += HandleContextChange;
            ShopDecorationsManager.I.OnConfirmationRequested += HandleConfirmationRequested;
            confirmationYesButton.onClick.AddListener(HandleConfirmationYes);
            confirmationNoButton.onClick.AddListener(HandleConfirmationNo);
        }

        private void HandleContextChange(ShopContext shopContext)
        {
            switch (shopContext)
            {
                case ShopContext.Shopping:
                    showShopPanelTween.PlayForward();
                    break;
                case ShopContext.Placement:
                    showDragPanelTween.PlayForward();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("shopContext", shopContext, null);
            }
        }

        private void HandleConfirmationRequested()
        {
            showConfirmationPanelTween.PlayForward();
        }

        private void HandleConfirmationYes()
        {
            ShopDecorationsManager.I.ConfirmPurchase();
        }
        private void HandleConfirmationNo()
        {
            ShopDecorationsManager.I.CancelPurchase();
        }

    }
}