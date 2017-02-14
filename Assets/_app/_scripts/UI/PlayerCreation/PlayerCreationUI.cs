using System;
using DG.DeExtensions;
using DG.Tweening;
using EA4S.Core;
using EA4S.Scenes;
using UnityEngine;

namespace EA4S.UI
{
    /// <summary>
    /// Player creation UI
    /// </summary>
    public class PlayerCreationUI : MonoBehaviour
    {
        public enum CategoryType
        {
            Age,
            Gender,
            Avatar,
            Color
        }

        static class CategoryIndex
        {
            public const int Age = 0;
            public const int Gender = 1;
            public const int Avatar = 2;
            public const int Color = 3;
        }

        #region Serialized

        [Tooltip("Startup offset of categories")]
        public int StartupOffsetY = 180;
        public UIButton BtCreate;
        public RectTransform CategoriesContainer;
        public PlayerCreationUICategory[] Categories; // 0: age // 1: gender // 2: avatar // 3: color

        #endregion

        bool allCategoriesSelected {
            get {
                foreach (PlayerCreationUICategory cat in Categories) {
                    if (cat.SelectedIndex < 0) return false;
                }
                return true;
            }
        }
        int selectionStep = 0; // 0: age // 1: gender // 2: avatar // 3: color
        float selectionStepOffsetY;
        Tween stepTween;

        #region Unity

        void Start()
        {
            selectionStepOffsetY = StartupOffsetY / 3f;
            CategoriesContainer.SetAnchoredPosY(StartupOffsetY);
            for (int i = 1; i < Categories.Length; ++i) Categories[i].gameObject.SetActive(false);
            BtCreate.gameObject.SetActive(false);

            // Listeners
            BtCreate.Bt.onClick.AddListener(CreateProfile);
            foreach (PlayerCreationUICategory cat in Categories) {
                cat.OnSelect += OnSelectCategory;
                cat.OnDeselectAll += OnDeselectAllInCategory;
            }
        }

        void OnDestroy()
        {
            BtCreate.Bt.onClick.RemoveAllListeners();
            foreach (PlayerCreationUICategory cat in Categories) {
                cat.OnSelect -= OnSelectCategory;
                cat.OnDeselectAll -= OnDeselectAllInCategory;
            }
            stepTween.Kill();
        }

        #endregion

        #region Methods

        void NextStep()
        {
            selectionStep++;
            if (stepTween != null) stepTween.Complete();
            Categories[selectionStep].gameObject.SetActive(true);
            stepTween = CategoriesContainer.DOAnchorPosY(StartupOffsetY - selectionStepOffsetY * selectionStep, 0.4f);
        }

        void StepBackwards(int toStep)
        {
            int totSteps = selectionStep - toStep;
            if (stepTween != null) stepTween.Complete();
            for (int i = toStep + 1; i < selectionStep + 1; ++i) {
                PlayerCreationUICategory cat = Categories[i];
                if (i == CategoryIndex.Color) Categories[CategoryIndex.Avatar].ResetColor(); // Reset avatars colors
                cat.Select(-1);
                cat.gameObject.SetActive(false);
            }
            selectionStep = toStep;
            stepTween = CategoriesContainer.DOAnchorPosY(StartupOffsetY - selectionStepOffsetY * selectionStep, 0.4f);
        }

        void SetGender()
        {
            Categories[CategoryIndex.Avatar].AvatarSetIcon(Categories[CategoryIndex.Gender].SelectedIndex == 1);
        }

        void CreateProfile()
        {
            PlayerCreationScene.CreatePlayer(
                Categories[CategoryIndex.Age].SelectedIndex + 4,
                Categories[CategoryIndex.Gender].SelectedIndex == 0 ? PlayerGender.M : PlayerGender.F,
                Categories[CategoryIndex.Avatar].SelectedIndex + 1,
                (PlayerTint)(Categories[CategoryIndex.Color].SelectedIndex + 1)
            );
        }

        #endregion

        #region Callbacks

        void OnSelectCategory(PlayerCreationUICategory category, UIButton uiButton)
        {
            int catIndex = Array.IndexOf(Categories, category);
            if (selectionStep < Categories.Length - 1 && catIndex == selectionStep) NextStep();
            switch (catIndex) {
                case CategoryIndex.Gender:
                    SetGender();
                    break;
                case CategoryIndex.Color:
                    Categories[CategoryIndex.Avatar].SetColor(uiButton.DefaultColor);
                    BtCreate.gameObject.SetActive(true);
                    BtCreate.Pulse();
                    break;
            }
        }

        void OnDeselectAllInCategory(PlayerCreationUICategory category)
        {
            BtCreate.StopPulsing();
            BtCreate.gameObject.SetActive(false);
            int catIndex = Array.IndexOf(Categories, category);
            if (catIndex < selectionStep) StepBackwards(catIndex);
            else if (catIndex == CategoryIndex.Color) Categories[CategoryIndex.Avatar].ResetColor();
        }

        #endregion
    }
}