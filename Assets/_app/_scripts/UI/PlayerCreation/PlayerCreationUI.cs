using System;
using DG.DeExtensions;
using DG.Tweening;
using UnityEngine;

namespace EA4S.UI
{
    /// <summary>
    /// Player creation UI
    /// </summary>
    public class PlayerCreationUI : MonoBehaviour
    {
        #region Serialized

        [Tooltip("Startup offset of categories")]
        public int StartupOffsetY = 180;
        public RectTransform CategoriesContainer;
        public PlayerCreationUICategory[] Categories; // 0: age // 1: gender // 2: avatar // 3: color

        #endregion

        bool allCategoriesSelected {
            get {
                foreach (PlayerCreationUICategory cat in Categories)
                {
                    if (cat.SelectedIndex < 0) return false;
                }
                return true;
            }
        }
        int selectionStep = 0; // 0: age // 1: gender // 2: avatar // 3: color
        float selectionStepOffset;
        Tween stepTween;

        #region Unity

        void Start()
        {
            selectionStepOffset = StartupOffsetY / 3f;
            CategoriesContainer.SetAnchoredPosY(StartupOffsetY);
            for (int i = 1; i < Categories.Length; ++i) Categories[i].gameObject.SetActive(false);

            // Listeners
            foreach (PlayerCreationUICategory cat in Categories)
            {
                cat.OnSelect += OnSelectCategory;
                cat.OnDeselectAll += OnDeselectAllInCategory;
            }
        }

        void OnDestroy()
        {
            foreach (PlayerCreationUICategory cat in Categories)
            {
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
            stepTween = CategoriesContainer.DOAnchorPosY(-selectionStepOffset, 0.4f).SetRelative();
        }

        void StepBackwards(int toStep)
        {
            int totSteps = selectionStep - toStep;
            if (stepTween != null) stepTween.Complete();
            for (int i = toStep + 1; i < selectionStep + 1; ++i)
            {
                PlayerCreationUICategory cat = Categories[i];
                if (i == 2) cat.ResetColor(); // Reset avatars colors
                cat.Select(-1);
                cat.gameObject.SetActive(false);
            }
            selectionStep = toStep;
            stepTween = CategoriesContainer.DOAnchorPosY(selectionStepOffset * totSteps, 0.4f).SetRelative();
        }

        #endregion

        #region Callbacks

        void OnSelectCategory(PlayerCreationUICategory category, UIButton uiButton)
        {
            int catIndex = Array.IndexOf(Categories, category);
            if (selectionStep < Categories.Length - 1 && catIndex == selectionStep) NextStep();
            else if (catIndex == 3)
            {
                // Color selection
                Categories[2].SetColor(uiButton.DefaultColor);
            }
        }

        void OnDeselectAllInCategory(PlayerCreationUICategory category)
        {
            int catIndex = Array.IndexOf(Categories, category);
            if (catIndex < selectionStep) StepBackwards(catIndex);
            else if (catIndex == 3) Categories[2].ResetColor();
        }

        #endregion
    }
}