using Antura.Core;
using Antura.Dog;
using Antura.Extensions;
using Antura.Helpers;
using Antura.Rewards;
using Antura.UI;
using System;
using DG.DeInspektor.Attributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.AnturaSpace.UI
{
    /// <summary>
    /// General controller of the UI in the Antura Space scene.
    /// </summary>
    public class AnturaSpaceUI : MonoBehaviour
    {
        public int MaxItems = 10;
        public LayerMask RewardsLayer;

        [DeToggleButton]
        public bool FlipRewards = true;

        [DeToggleButton]
        public bool HideLockedSwatchesColors;

        [Header("References")]
        public AnturaSpaceModsButton BtOpenModsPanel;
        public UIButton BtBonesShop;

        public UIButton BTRemoveMods;
        public RectTransform CategoriesContainer, ItemsContainer, SwatchesContainer;
        public AnturaSpaceItemButton BtItemMain;
        public RectTransform ShopPanelContainer;
        public TMPro.TextMeshProUGUI bonesNumber;


        public event System.Action onEnterCustomization;
        public event System.Action onExitCustomization;

        public delegate void AnturaSpaceUIEvent(string _id);

        public static event AnturaSpaceUIEvent onRewardCategorySelectedInCustomization;

        public static event AnturaSpaceUIEvent onRewardSelectedInCustomization;
        //        public static event AnturaSpaceUIEvent onRewardColorSelectedInCustomization;

        public static AnturaSpaceUI I { get; private set; }
        public bool IsModsPanelOpen { get; private set; }
        //public bool IsShopPanelOpen { get; private set; }

        bool isTutorialMode;
        AnturaSpaceCategoryButton[] btsCategories;
        AnturaSpaceItemButton[] btsItems;
        AnturaSpaceSwatchButton[] btsSwatches;
        List<Transform> rewardsContainers;
        List<Transform> rewardsImagesContainers; // Passed with texture and decal reward types
        RewardTypes currRewardType;
        List<RewardItem> currRewardDatas;
        List<RewardColorItem> currSwatchesDatas;
        AnturaSpaceCategoryButton.AnturaSpaceCategory currCategory;
        Tween showCategoriesTween, showShopTween, showItemsTween, showSwatchesTween;

        #region Unity

        int bonesCount = -1;

        public int BonesCount {
            get { return bonesCount; }
            set {
                if (value == bonesCount) {
                    return;
                }

                bonesCount = value;
                bonesNumber.text = value.ToString();
            }
        }

        void Awake()
        {
            I = this;
        }

        public void Initialise()
        {
            btsCategories = CategoriesContainer.GetComponentsInChildren<AnturaSpaceCategoryButton>(true);
            btsSwatches = SwatchesContainer.GetComponentsInChildren<AnturaSpaceSwatchButton>(true);
            SelectCategory(AnturaSpaceCategoryButton.AnturaSpaceCategory.Unset);
            BtOpenModsPanel.SetAsNew(AppManager.I.Player.ThereIsSomeNewReward());

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
            showCategoriesTween = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(CategoriesContainer.DOAnchorPosY(150, duration).From().SetEase(Ease.OutBack))
                .Join(BtBonesShop.RectT.DOAnchorPosY(-830, duration))
                .OnRewind(() => CategoriesContainer.gameObject.SetActive(false));
            showShopTween = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(ShopPanelContainer.DOAnchorPosY(-830, duration).From().SetEase(Ease.OutBack))
                .Join(BtOpenModsPanel.RectT.DOAnchorPosY(150, duration))
                .OnRewind(() => ShopPanelContainer.gameObject.SetActive(false));
            showItemsTween = ItemsContainer.DOAnchorPosX(-350, duration).From().SetEase(Ease.OutBack).SetAutoKill(false).Pause()
                .OnRewind(() => {
                    ItemsContainer.gameObject.SetActive(false);
                    // Clear items containers children
                    foreach (Transform container in rewardsContainers) {
                        foreach (Transform child in container) Destroy(child.gameObject);
                    }
                });
            showSwatchesTween = SwatchesContainer.DOAnchorPosY(-100, duration).From().SetEase(Ease.OutBack).SetAutoKill(false).Pause()
                .OnRewind(() => SwatchesContainer.gameObject.SetActive(false));

            CategoriesContainer.gameObject.SetActive(false);
            ShopPanelContainer.gameObject.SetActive(false);
            ItemsContainer.gameObject.SetActive(false);
            SwatchesContainer.gameObject.SetActive(false);

            // Listeneres
            BtOpenModsPanel.Bt.onClick.AddListener(() => OnClick(BtOpenModsPanel));
            BtBonesShop.Bt.onClick.AddListener(() => OnClick(BtBonesShop));
            BTRemoveMods.Bt.onClick.AddListener(() => OnClick(BTRemoveMods));
            foreach (var bt in btsCategories) {
                var b = bt;
                b.Bt.onClick.AddListener(() => OnClickCategory(b));
            }
            foreach (var bt in btsItems) {
                var b = bt;
                b.Bt.onClick.AddListener(() => OnClickItem(b));
            }
            foreach (var bt in btsSwatches) {
                var b = bt;
                b.Bt.onClick.AddListener(() => OnClickSwatch(b));
            }
        }

        void OnDestroy()
        {
            if (I == this) I = null;
            // TODO: is better move this in "exit scene" method?
            AnturaModelManager.I.SaveAnturaCustomization();
            StopAllCoroutines();
            showCategoriesTween.Kill();
            showItemsTween.Kill();
            showSwatchesTween.Kill();
            BtOpenModsPanel.Bt.onClick.RemoveAllListeners();
            BTRemoveMods.Bt.onClick.RemoveAllListeners();
            foreach (var bt in btsCategories) {
                bt.Bt.onClick.RemoveAllListeners();
            }
            foreach (var bt in btsItems) {
                bt.Bt.onClick.RemoveAllListeners();
            }
            foreach (var bt in btsSwatches) {
                bt.Bt.onClick.RemoveAllListeners();
            }
        }

        #endregion

        #region Public Methods

        public void ToggleShopPanel()
        {
            if (ShopDecorationsManager.I.ShopContext == ShopContext.Closed)
            {
                ShopPanelContainer.gameObject.SetActive(true);
                ShopDecorationsManager.I.SetContextPurchase();
                showShopTween.PlayForward();
            }
            else
            {
                ShopDecorationsManager.I.SetContextClosed();
                showShopTween.PlayBackwards();
            }
        }

        public void ToggleModsPanel()
        {
            if (IsModsPanelOpen && isTutorialMode) return;

            IsModsPanelOpen = !IsModsPanelOpen;
            if (IsModsPanelOpen)
            {
                BtOpenModsPanel.SetAsNew(false);
                CategoriesContainer.gameObject.SetActive(true);
                showCategoriesTween.PlayForward();
                RefreshCategories();

                ShopDecorationsManager.I.SetContextCustomization();

                if (onEnterCustomization != null) {
                    onEnterCustomization();
                }
            }
            else
            {
                BtOpenModsPanel.SetAsNew(AppManager.I.Player.ThereIsSomeNewReward());
                SelectCategory(AnturaSpaceCategoryButton.AnturaSpaceCategory.Unset);
                showCategoriesTween.PlayBackwards();
                showItemsTween.PlayBackwards();
                showSwatchesTween.PlayBackwards();

                ShopDecorationsManager.I.SetContextClosed();

                if (onExitCustomization != null) {
                    onExitCustomization();
                }
            }
        }

        /// <summary>
        /// Activates or deactivates the tutorial mode for the customization UI.
        /// If active, only the first new category and the first new item will be selectable.
        /// </summary>
        public void SetTutorialMode(bool activate)
        {
            isTutorialMode = activate;
            BtOpenModsPanel.AutoAnimateClick = !activate;
        }

        /// <summary>
        /// Returns the first category button marked as NEW (meaning it has new content).
        /// Return NULL if the mods panel is not open.
        /// </summary>
        public AnturaSpaceCategoryButton GetNewCategoryButton()
        {
            if (!IsModsPanelOpen) {
                Debug.LogWarning("AnturaSpaceUI.GetNewCategoryButton > Mods Panel is not open");
                return null;
            }
            foreach (AnturaSpaceCategoryButton bt in btsCategories) {
                if (bt.IsNew) {
                    return bt;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the first item button marked as NEW (meaning it has new content).
        /// Return NULL if the mods panel is not open or a category is not selected.
        /// </summary>
        public AnturaSpaceItemButton GetNewItemButton()
        {
            if (!IsModsPanelOpen || !ItemsContainer.gameObject.activeSelf) {
                Debug.LogWarning("AnturaSpaceUI.GetNewItemButton > Mods Panel is not open or category is not selected");
                return null;
            }
            foreach (var bt in btsItems) {
                if (bt.IsNew) {
                    return bt;
                }
            }
            return null;
        }

        #endregion

        #region Methods

        public void ShowBonesButton(bool show)
        {
            BtBonesShop.gameObject.SetActive(show);
        }

        void SelectCategory(AnturaSpaceCategoryButton.AnturaSpaceCategory _category)
        {
            StopAllCoroutines();
            // Save configuration
            //AnturaModelManager.Instance.SaveAnturaCustomization();

            // Toggle buttons
            foreach (AnturaSpaceCategoryButton bt in btsCategories) {
                if (bt.Category == _category) {
                    bt.Toggle(true, true);
                } else {
                    bt.Toggle(false);
                }
            }
            if (_category == AnturaSpaceCategoryButton.AnturaSpaceCategory.Unset) {
                return;
            }

            showItemsTween.Rewind();
            StartCoroutine(CO_SelectCategory(_category));
        }

        IEnumerator CO_SelectCategory(AnturaSpaceCategoryButton.AnturaSpaceCategory _category)
        {
            BTRemoveMods.gameObject.SetActive(false);

            // Get rewards list
            currCategory = _category;
            currRewardType = CategoryToRewardType(_category);
            bool useImages = _category == AnturaSpaceCategoryButton.AnturaSpaceCategory.Texture ||
                             _category == AnturaSpaceCategoryButton.AnturaSpaceCategory.Decal;
            foreach (var item in btsItems) {
                item.SetImage(!useImages);
            }
            ReloadRewardsDatas();
            yield return null;

            RewardItem selectedRewardData = RefreshItems();
            ItemsContainer.gameObject.SetActive(true);
            showItemsTween.PlayForward();

            // Select eventual reward
            if (selectedRewardData != null) {
                SelectReward(selectedRewardData);
            } else {
                showSwatchesTween.Rewind();
            }
        }

        void SelectReward(RewardItem _rewardData)
        {
            showSwatchesTween.Rewind();
            bool isTextureOrDecal = currCategory == AnturaSpaceCategoryButton.AnturaSpaceCategory.Texture
                                    || currCategory == AnturaSpaceCategoryButton.AnturaSpaceCategory.Decal;
            BTRemoveMods.gameObject.SetActive(!isTextureOrDecal && _rewardData != null);
            if (_rewardData == null) {
                foreach (AnturaSpaceItemButton item in btsItems) {
                    item.Toggle(false);
                }
                if (currCategory == AnturaSpaceCategoryButton.AnturaSpaceCategory.Ears) {
                    AnturaModelManager.I.ClearLoadedRewardInCategory("EAR_L");
                    AnturaModelManager.I.ClearLoadedRewardInCategory("EAR_R");
                } else {
                    AnturaModelManager.I.ClearLoadedRewardInCategory(currCategory.ToString());
                }
                return;
            }

            currSwatchesDatas = RewardSystemManager.SelectRewardItem(_rewardData.ID, currRewardType);
            if (currSwatchesDatas.Count == 0) {
                Debug.Log("No color swatches for the selected reward!");
                return;
            }

            // Hide non-existent swatches
            for (int i = currSwatchesDatas.Count - 1; i < btsSwatches.Length; ++i) btsSwatches[i].gameObject.SetActive(false);
            // Setup and show swatches
            RewardColorItem selectedSwatchData = null;
            for (int i = 0; i < currSwatchesDatas.Count; ++i) {
                RewardColorItem swatchData = currSwatchesDatas[i];
                AnturaSpaceSwatchButton swatch = btsSwatches[i];
                swatch.gameObject.SetActive(true);
                swatch.Data = swatchData;
                if (swatchData != null) {
                    swatch.SetAsNew(!swatchData.IsSelected && swatchData.IsNew);
                    swatch.Toggle(swatchData.IsSelected);
                    swatch.SetColors(GenericHelper.HexToColor(swatchData.Color1RGB), GenericHelper.HexToColor(swatchData.Color2RGB));
                    if (swatchData.IsSelected) {
                        selectedSwatchData = swatchData;
                    }
                } else {
                    swatch.Toggle(false);
                }
                swatch.Lock(swatchData == null);
            }

            SwatchesContainer.gameObject.SetActive(true);
            showSwatchesTween.PlayForward();

            // Select eventual color
            if (selectedSwatchData != null) {
                SelectSwatch(selectedSwatchData);
            }

            ReloadRewardsDatas();
            RefreshCategories();
            RefreshItems(true);
        }

        void SelectSwatch(RewardColorItem _colorData)
        {
            foreach (var item in btsSwatches) {
                item.Toggle(item.Data == _colorData);
            }
            if (_colorData != null) {
                RewardSystemManager.SelectRewardColorItem(_colorData.ID, currRewardType);
            } else {
                Debug.Log("SelectSwatch > _colorData is NULL!");
            }
        }

        void ReloadRewardsDatas()
        {
            bool useImages = currCategory == AnturaSpaceCategoryButton.AnturaSpaceCategory.Texture ||
                             currCategory == AnturaSpaceCategoryButton.AnturaSpaceCategory.Decal;
            if (currCategory == AnturaSpaceCategoryButton.AnturaSpaceCategory.Ears) {
                currRewardDatas = RewardSystemManager.GetRewardItemsByRewardType(currRewardType, rewardsContainers, "EAR_L");
                List<Transform> altRewardContainers = new List<Transform>(rewardsContainers);
                altRewardContainers.RemoveRange(0, currRewardDatas.Count);
                currRewardDatas.AddRange(RewardSystemManager.GetRewardItemsByRewardType(currRewardType, altRewardContainers, "EAR_R"));
            } else {
                currRewardDatas = RewardSystemManager.GetRewardItemsByRewardType(currRewardType,
                    useImages ? rewardsImagesContainers : rewardsContainers, currCategory.ToString());
            }
        }

        void RefreshCategories()
        {
            foreach (AnturaSpaceCategoryButton btCat in btsCategories) {
                bool isNew;
                switch (btCat.Category) {
                    case AnturaSpaceCategoryButton.AnturaSpaceCategory.Ears:
                        isNew = AppManager.I.Player.RewardCategoryContainsNewElements(CategoryToRewardType(btCat.Category), "EAR_L")
                                || AppManager.I.Player.RewardCategoryContainsNewElements(CategoryToRewardType(btCat.Category), "EAR_R");
                        break;
                    case AnturaSpaceCategoryButton.AnturaSpaceCategory.Decal:
                    case AnturaSpaceCategoryButton.AnturaSpaceCategory.Texture:
                        isNew = AppManager.I.Player.RewardCategoryContainsNewElements(CategoryToRewardType(btCat.Category));
                        break;
                    default:
                        isNew = AppManager.I.Player.RewardCategoryContainsNewElements(CategoryToRewardType(btCat.Category),
                            btCat.Category.ToString());
                        break;
                }
                btCat.SetAsNew(isNew);
            }
        }

        // Returns eventual selected reward item
        RewardItem RefreshItems(bool toggleOnly = false)
        {
            bool useImages = currCategory == AnturaSpaceCategoryButton.AnturaSpaceCategory.Texture ||
                             currCategory == AnturaSpaceCategoryButton.AnturaSpaceCategory.Decal;
            // Hide non-existent items
            for (int i = currRewardDatas.Count - 1; i < btsItems.Length; ++i) {
                btsItems[i].gameObject.SetActive(false);
            }
            // Setup and show items
            RewardItem selectedRewardData = null;
            for (int i = 0; i < currRewardDatas.Count; ++i) {
                RewardItem rewardData = currRewardDatas[i];
                AnturaSpaceItemButton item = btsItems[i];
                item.gameObject.SetActive(true);
                item.Data = rewardData;
                if (rewardData != null) {
                    if (!useImages && !toggleOnly) {
                        item.RewardContainer.gameObject.SetLayerRecursive(GenericHelper.LayerMaskToIndex(RewardsLayer));
                        CameraHelper.FitRewardToUICamera(item.RewardContainer.GetChild(0), item.RewardCamera, FlipRewards);
                    }
                    item.SetAsNew(rewardData.IsNew);
                    item.Toggle(rewardData.IsSelected);
                    if (rewardData.IsSelected) {
                        selectedRewardData = rewardData;
                    }
                } else {
                    item.Toggle(false);
                }
                item.Lock(rewardData == null);
            }
            return selectedRewardData;
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
            if (_bt == BtOpenModsPanel) {
                ToggleModsPanel();
            } else if (_bt == BTRemoveMods) {
                SelectReward(null);
            } else if (_bt == BtBonesShop)
            {
                ToggleShopPanel();
            }
        }

        void OnClickCategory(AnturaSpaceCategoryButton _bt)
        {
            if (showItemsTween.IsPlaying() || isTutorialMode && GetNewCategoryButton() != _bt) return;

            _bt.AnimateClick();
            _bt.PlayClickFx();
            SelectCategory(_bt.Category);
            if (onRewardCategorySelectedInCustomization != null) {
                onRewardCategorySelectedInCustomization(_bt.Category.ToString());
            }
        }

        void OnClickItem(AnturaSpaceItemButton _bt)
        {
            if (isTutorialMode && GetNewItemButton() != _bt) return;

            SelectReward(_bt.Data);
            Reward reward = RewardSystemManager.GetRewardById(_bt.Data.ID);
            if (reward != null && onRewardSelectedInCustomization != null) {
                onRewardSelectedInCustomization(reward.ID);
            }

            if (reward != null
                && (reward.Category == "EAR_R" || reward.Category == "EAR_L")
                && onRewardCategorySelectedInCustomization != null) {
                onRewardCategorySelectedInCustomization(reward.Category.ToString());
            }
        }

        void OnClickSwatch(AnturaSpaceSwatchButton _bt)
        {
            SelectSwatch(_bt.Data);

            // Is new check
            if (_bt.IcoNew.activeSelf) {
                _bt.SetAsNew(false);
                ReloadRewardsDatas();
                RefreshCategories();
                RefreshItems(true);
            }
        }

        #endregion

        public CanvasScaler canvasScaler;
        public Camera uiCamera;

        public Vector3 ScreenToUIPoint(Vector3 pos)
        {
            float resolutionRatio = Screen.height / canvasScaler.referenceResolution.y;
            return (pos - new Vector3(Screen.width / 2, Screen.height / 2)) / resolutionRatio;
        }

        public Vector3 WorldToUIPoint(Vector3 pos)
        {
            return ScreenToUIPoint(uiCamera.WorldToScreenPoint(pos));
        }

    }
}