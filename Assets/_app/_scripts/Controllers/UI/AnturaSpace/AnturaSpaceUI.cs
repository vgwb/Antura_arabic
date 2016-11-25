// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/24

using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    public class AnturaSpaceUI : MonoBehaviour
    {
        public int MaxItems = 9;
        [Header("References")]
        public UIButton BtOpenModsPanel;
        public RectTransform CategoriesContainer, ItemsContainer, SwatchesContainer;
        public AnturaSpaceItemButton BtItemMain;

        bool isModsPanelOpen;
        AnturaSpaceCategoryButton[] btsCategories;
        AnturaSpaceItemButton[] btsItems;
        AnturaSpaceSwatchButton[] btsSwatches;
        Tween showCategoriesTween, showItemsTween, showSwatchesTween;

        #region Unity

        void Start()
        {
            btsCategories = CategoriesContainer.GetComponentsInChildren<AnturaSpaceCategoryButton>(true);
            btsSwatches = ItemsContainer.GetComponentsInChildren<AnturaSpaceSwatchButton>(true);
            SelectCategoryButton(null);
            // Create items
            btsItems = new AnturaSpaceItemButton[MaxItems];
            btsItems[0] = BtItemMain;
            for (int i = 1; i < MaxItems; ++i) {
                AnturaSpaceItemButton item = Instantiate(BtItemMain);
                item.transform.SetParent(BtItemMain.transform.parent, false);
                item.Setup();
                btsItems[i] = item;
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
                SelectCategoryButton(null);
                showCategoriesTween.PlayBackwards();
                showItemsTween.PlayBackwards();
                showSwatchesTween.PlayBackwards();
            }
        }

        void SelectCategoryButton(AnturaSpaceCategoryButton _bt)
        {
            foreach (AnturaSpaceCategoryButton bt in btsCategories) {
                if (_bt == bt) _bt.Toggle(true, true);
                else bt.Toggle(false);
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

            SelectCategoryButton(_bt);
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