// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/24

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    public class AnturaSpaceUI : MonoBehaviour
    {
        public int MaxItems = 10;
        public LayerMask RewardsLayer;
        public bool FlipRewards = true;
        [Header("References")]
        public UIButton BtOpenModsPanel;
        public RectTransform CategoriesContainer, ItemsContainer, SwatchesContainer;
        public AnturaSpaceItemButton BtItemMain;

        bool isModsPanelOpen;
        AnturaSpaceCategoryButton[] btsCategories;
        AnturaSpaceItemButton[] btsItems;
        AnturaSpaceSwatchButton[] btsSwatches;
        List<Transform> rewardsContainers;
        List<Transform> rewardsImagesContainers; // Passed with texture and decal reward types
        RewardTypes currRewardType;
        List<RewardItem> currRewardDatas;
        List<RewardColorItem> currSwatchesDatas;
        Tween showCategoriesTween, showItemsTween, showSwatchesTween;

        #region Unity

        void Start()
        {
            btsCategories = CategoriesContainer.GetComponentsInChildren<AnturaSpaceCategoryButton>(true);
            btsSwatches = SwatchesContainer.GetComponentsInChildren<AnturaSpaceSwatchButton>(true);
            SelectCategory(AnturaSpaceCategoryButton.AnturaSpaceCategory.Unset);
            // Create items
            rewardsContainers = new List<Transform>();
            rewardsImagesContainers = new List<Transform>();
            btsItems = new AnturaSpaceItemButton[MaxItems];
            btsItems[0] = BtItemMain;
            rewardsContainers.Add(BtItemMain.RewardContainer);
            rewardsImagesContainers.Add(BtItemMain.RewardImage.transform);
            for (int i = 1; i < MaxItems; ++i) {
                AnturaSpaceItemButton item = Instantiate(BtItemMain);
                item.transform.SetParent(BtItemMain.transform.parent, false);
                item.Setup();
                btsItems[i] = item;
                rewardsContainers.Add(item.RewardContainer);
                rewardsImagesContainers.Add(item.RewardImage.transform);
            }
            BtItemMain.Setup();

            const float duration = 0.3f;
            showCategoriesTween = CategoriesContainer.DOAnchorPosY(150, duration).From().SetEase(Ease.OutBack).SetAutoKill(false).Pause()
                .OnRewind(()=> CategoriesContainer.gameObject.SetActive(false));
            showItemsTween = ItemsContainer.DOAnchorPosX(-350, duration).From().SetEase(Ease.OutBack).SetAutoKill(false).Pause()
                .OnRewind(() => {
                    ItemsContainer.gameObject.SetActive(false);
                    // Clear items containers children
                    foreach (Transform container in rewardsContainers) {
                        foreach (Transform child in container) Destroy(child.gameObject);
                    }
                });
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
            this.StopAllCoroutines();
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
            this.StopAllCoroutines();
            // Toggle buttons
            foreach (AnturaSpaceCategoryButton bt in btsCategories) {
                if (bt.Category == _category) bt.Toggle(true, true);
                else bt.Toggle(false);
            }
            if (_category == AnturaSpaceCategoryButton.AnturaSpaceCategory.Unset) return;

            showItemsTween.Rewind();
            this.StartCoroutine(CO_SelectCategory(_category));
        }
        IEnumerator CO_SelectCategory(AnturaSpaceCategoryButton.AnturaSpaceCategory _category)
        {
            // Get rewards list
            currRewardType = CategoryToRewardType(_category);
            bool useImages = _category == AnturaSpaceCategoryButton.AnturaSpaceCategory.Texture || _category == AnturaSpaceCategoryButton.AnturaSpaceCategory.Decal;
            foreach (AnturaSpaceItemButton item in btsItems) item.SetImage(!useImages);
            if (_category == AnturaSpaceCategoryButton.AnturaSpaceCategory.Ears) {
                currRewardDatas = RewardSystemManager.GetRewardItemsByRewardType(currRewardType, rewardsContainers, "EAR_L");
                List<Transform> altRewardContainers = new List<Transform>(rewardsContainers);
                altRewardContainers.RemoveRange(0, currRewardDatas.Count);
                currRewardDatas.AddRange(RewardSystemManager.GetRewardItemsByRewardType(currRewardType, altRewardContainers, "EAR_R"));
            } else {
                currRewardDatas = RewardSystemManager.GetRewardItemsByRewardType(currRewardType, useImages ? rewardsImagesContainers : rewardsContainers, _category.ToString());
            }
            yield return null;

            // Hide non-existent items
            for (int i = currRewardDatas.Count - 1; i < btsItems.Length; ++i) btsItems[i].gameObject.SetActive(false);
            // Setup and show items
            RewardItem selectedRewardData = null;
            for (int i = 0; i < currRewardDatas.Count; ++i) {
                RewardItem rewardData = currRewardDatas[i];
                AnturaSpaceItemButton item = btsItems[i];
                item.Data = rewardData;
                item.Lock(rewardData == null);
                if (rewardData != null) {
                    if (!useImages) {
                        item.RewardContainer.gameObject.SetLayerRecursive(GenericUtilities.LayerMaskToIndex(RewardsLayer));
                        CameraHelper.FitRewardToUICamera(item.RewardContainer.GetChild(0), item.RewardCamera, FlipRewards);
                    }
                    item.SetAsNew(rewardData.IsNew);
                    item.Toggle(rewardData.IsSelected);
                    if (rewardData.IsSelected) selectedRewardData = rewardData;
                }
            }

            ItemsContainer.gameObject.SetActive(true);
            showItemsTween.PlayForward();

            // Select eventual reward
            if (selectedRewardData != null) SelectReward(selectedRewardData);
        }

        void SelectReward(RewardItem _rewardData)
        {
            showSwatchesTween.Rewind();

            foreach (AnturaSpaceItemButton item in btsItems) item.Toggle(item.Data == _rewardData);
            currSwatchesDatas = RewardSystemManager.SelectRewardItem(_rewardData.ID, currRewardType);
            if (currSwatchesDatas.Count == 0) {
                Debug.Log("No color swatches for the selected reward!");
                return;
            }

            // Hide non-existent swatches
            Debug.Log(currSwatchesDatas.Count + " > " + btsSwatches.Length);
            for (int i = currSwatchesDatas.Count - 1; i < btsSwatches.Length; ++i) btsSwatches[i].gameObject.SetActive(false);
            // Setup and show swatches
            RewardColorItem selectedSwatchData = null;
            for (int i = 0; i < currSwatchesDatas.Count; ++i) {
                RewardColorItem swatchData = currSwatchesDatas[i];
                AnturaSpaceSwatchButton swatch = btsSwatches[i];
                swatch.Data = swatchData;
                swatch.Lock(swatchData == null);
                if (swatchData != null) {
                    swatch.SetAsNew(swatchData.IsNew);
                    swatch.Toggle(swatchData.IsSelected);
                    swatch.SetColors(GenericUtilities.HexToColor(swatchData.Color1RGB), GenericUtilities.HexToColor(swatchData.Color2RGB));
                    if (swatchData.IsSelected) selectedSwatchData = swatchData;
                }
            }

            SwatchesContainer.gameObject.SetActive(true);
            showSwatchesTween.PlayForward();

            // Select eventual color
            if (selectedSwatchData != null) SelectSwatch(selectedSwatchData);
        }

        void SelectSwatch(RewardColorItem _colorData)
        {
            foreach (AnturaSpaceSwatchButton item in btsSwatches) item.Toggle(item.Data == _colorData);
            RewardSystemManager.SelectRewardColorItem(_colorData.ID, currRewardType);
        }

        #endregion

        #region Helpers

        RewardTypes CategoryToRewardType(AnturaSpaceCategoryButton.AnturaSpaceCategory _category)
        {
            switch (_category) {
            case AnturaSpaceCategoryButton.AnturaSpaceCategory.Texture:
                return RewardTypes.texture;
            case AnturaSpaceCategoryButton.AnturaSpaceCategory.Decal:
                return RewardTypes.decal;
            default:
                return RewardTypes.reward;
            }
        }

        #endregion

        #region Callbacks

        void OnClick(UIButton _bt)
        {
            if (_bt == BtOpenModsPanel) ToggleModsPanel();
        }

        void OnClickCategory(AnturaSpaceCategoryButton _bt)
        {
            SelectCategory(_bt.Category);
        }

        void OnClickItem(AnturaSpaceItemButton _bt)
        {
            SelectReward(_bt.Data);
        }

        void OnClickSwatch(AnturaSpaceSwatchButton _bt)
        {
            SelectSwatch(_bt.Data);
        }

        #endregion
    }
}