// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/24

using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    public class AnturaSpaceUI : MonoBehaviour
    {
        public int MaxItems = 9;
        public LayerMask RewardsLayer;
        [Header("References")]
        public UIButton BtOpenModsPanel;
        public RectTransform CategoriesContainer, ItemsContainer, SwatchesContainer;
        public AnturaSpaceItemButton BtItemMain;

        bool isModsPanelOpen;
        AnturaSpaceCategoryButton[] btsCategories;
        AnturaSpaceItemButton[] btsItems;
        AnturaSpaceSwatchButton[] btsSwatches;
        List<Transform> rewardsContainers;
        List<RewardItem> currRewardItems;
        Tween showCategoriesTween, showItemsTween, showSwatchesTween;

        #region Unity

        void Start()
        {
            btsCategories = CategoriesContainer.GetComponentsInChildren<AnturaSpaceCategoryButton>(true);
            btsSwatches = ItemsContainer.GetComponentsInChildren<AnturaSpaceSwatchButton>(true);
            SelectCategory(AnturaSpaceCategoryButton.AnturaSpaceCategory.Unset);
            // Create items
            rewardsContainers = new List<Transform>();
            btsItems = new AnturaSpaceItemButton[MaxItems];
            btsItems[0] = BtItemMain;
            for (int i = 1; i < MaxItems; ++i) {
                AnturaSpaceItemButton item = Instantiate(BtItemMain);
                item.transform.SetParent(BtItemMain.transform.parent, false);
                item.Setup();
                btsItems[i] = item;
                rewardsContainers.Add(item.RewardContainer);
            }
            BtItemMain.Setup();

            const float duration = 0.3f;
            showCategoriesTween = CategoriesContainer.DOAnchorPosY(150, duration).From().SetEase(Ease.OutBack).SetAutoKill(false).Pause()
                .OnRewind(()=> CategoriesContainer.gameObject.SetActive(false));
            showItemsTween = ItemsContainer.DOAnchorPosX(-350, duration).From().SetEase(Ease.OutBack).SetAutoKill(false).Pause()
                .OnRewind(()=> ItemsContainer.gameObject.SetActive(false));
            showSwatchesTween = SwatchesContainer.DOAnchorPosY(-100, duration).From().SetEase(Ease.OutBack).SetAutoKill(false).Pause()
                .OnRewind(()=> SwatchesContainer.gameObject.SetActive(false));

            CategoriesContainer.gameObject.SetActive(false);
            ItemsContainer.gameObject.SetActive(false);
            SwatchesContainer.gameObject.SetActive(false);

            // Listeneres
            BtOpenModsPanel.Bt.onClick.AddListener(()=> OnClick(BtOpenModsPanel));
            foreach (var bt in btsCategories) {
                var b = bt;
                b.Bt.onClick.AddListener(()=> OnClickCategory(b));
            }
            foreach (var bt in btsItems) {
                var b = bt;
                b.Bt.onClick.AddListener(()=> OnClickItem(b));
            }
            foreach (var bt in btsSwatches) {
                var b = bt;
                b.Bt.onClick.AddListener(()=> OnClickSwatch(b));
            }
        }

        void OnDestroy()
        {
            showCategoriesTween.Kill();
            showItemsTween.Kill();
            showSwatchesTween.Kill();
            BtOpenModsPanel.Bt.onClick.RemoveAllListeners();
            foreach (var bt in btsCategories) bt.Bt.onClick.RemoveAllListeners();
            foreach (var bt in btsItems) bt.Bt.onClick.RemoveAllListeners();
            foreach (var bt in btsSwatches) bt.Bt.onClick.RemoveAllListeners();
        }

        #endregion

        #region Methods

        void ToggleModsPanel()
        {
            isModsPanelOpen = !isModsPanelOpen;
            if (isModsPanelOpen) {
                CategoriesContainer.gameObject.SetActive(true);
                showCategoriesTween.PlayForward();
            } else {
                SelectCategory(AnturaSpaceCategoryButton.AnturaSpaceCategory.Unset);
                showCategoriesTween.PlayBackwards();
                showItemsTween.PlayBackwards();
                showSwatchesTween.PlayBackwards();
            }
        }

        void SelectCategory(AnturaSpaceCategoryButton.AnturaSpaceCategory _category)
        {
            // Toggle buttons
            foreach (AnturaSpaceCategoryButton bt in btsCategories) {
                if (bt.Category == _category) bt.Toggle(true, true);
                else bt.Toggle(false);
            }
            if (_category == AnturaSpaceCategoryButton.AnturaSpaceCategory.Unset) return;

            // Get rewards list
            switch (_category) {
            case AnturaSpaceCategoryButton.AnturaSpaceCategory.Texture:
                currRewardItems = RewardSystemManager.GetRewardItemsByRewardType(RewardTypes.texture, rewardsContainers);
                break;
            case AnturaSpaceCategoryButton.AnturaSpaceCategory.Decal:
                currRewardItems = RewardSystemManager.GetRewardItemsByRewardType(RewardTypes.decal, rewardsContainers);
                break;
            default:
                if (_category == AnturaSpaceCategoryButton.AnturaSpaceCategory.Ears) {
                    currRewardItems = RewardSystemManager.GetRewardItemsByRewardType(RewardTypes.reward, rewardsContainers, "EAR_L");
                    currRewardItems.AddRange(RewardSystemManager.GetRewardItemsByRewardType(RewardTypes.reward, rewardsContainers, "EAR_R"));
                } else currRewardItems = RewardSystemManager.GetRewardItemsByRewardType(RewardTypes.reward, rewardsContainers, _category.ToString());
                break;
            }
            // Hide non-existent items
            for (int i = currRewardItems.Count - 1; i < btsItems.Length; ++i) btsItems[i].gameObject.SetActive(false);
            // Setup and show items
            RewardItem selectedRewardItemData = null;
            for (int i = 0; i < currRewardItems.Count; ++i) {
                RewardItem itemData = currRewardItems[i];
                AnturaSpaceItemButton item = btsItems[i];
                item.Data = itemData;
                item.RewardContainer.gameObject.SetLayerRecursive(GenericUtilities.LayerMaskToIndex(RewardsLayer));
                item.Lock(itemData == null);
                if (itemData != null) {
                    item.SetAsNew(itemData.IsNew);
                    item.Toggle(itemData.IsSelected);
                    if (itemData.IsSelected) selectedRewardItemData = itemData;
                }
            }
            showItemsTween.PlayForward();

            // Select current reward
            if (selectedRewardItemData != null) {
                
            }
        }

        #endregion

        #region Callbacks

        void OnClick(UIButton _bt)
        {
            _bt.AnimateClick();

            if (_bt == BtOpenModsPanel) ToggleModsPanel();
        }

        void OnClickCategory(AnturaSpaceCategoryButton _bt)
        {
            _bt.AnimateClick();

            SelectCategory(_bt.Category);
        }

        void OnClickItem(AnturaSpaceItemButton _bt)
        {
            _bt.AnimateClick();
        }

        void OnClickSwatch(AnturaSpaceSwatchButton _bt)
        {
            _bt.AnimateClick();
        }

        #endregion
    }
}