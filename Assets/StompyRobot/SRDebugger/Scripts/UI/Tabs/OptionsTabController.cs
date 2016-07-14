namespace SRDebugger.UI.Tabs
{
    using System.Collections.Generic;
    using Controls;
    using Controls.Data;
    using Internal;
    using Other;
    using Services;
    using SRF;
    using UnityEngine;
    using UnityEngine.UI;

    public class OptionsTabController : SRMonoBehaviourEx
    {
        private readonly List<OptionsControlBase> _controls = new List<OptionsControlBase>();

        private readonly Dictionary<OptionDefinition, OptionsControlBase> _options =
            new Dictionary<OptionDefinition, OptionsControlBase>();

        private bool _queueRefresh;
        private bool _selectionModeEnabled;

        [RequiredField] public ActionControl ActionControlPrefab;

        [RequiredField] public CategoryGroup CategoryGroupPrefab;

        [RequiredField] public RectTransform ContentContainer;

        [RequiredField] public GameObject NoOptionsNotice;

        [RequiredField] public Toggle PinButton;

        [RequiredField] public GameObject PinPromptSpacer;

        [RequiredField] public GameObject PinPromptText;

        protected override void Start()
        {
            base.Start();

            PinButton.onValueChanged.AddListener(SetSelectionModeEnabled);

            PinPromptText.SetActive(false);
            PinPromptSpacer.SetActive(false);

            Populate();

            SROptions.Current.PropertyChanged += SROptionPropertyChanged;
        }

        private void SROptionPropertyChanged(object sender, string propertyName)
        {
            _queueRefresh = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Service.Panel.VisibilityChanged += PanelOnVisibilityChanged;
        }

        protected override void OnDisable()
        {
            // Always end pinning mode when tabbing away
            SetSelectionModeEnabled(false);

            if (Service.Panel != null)
            {
                Service.Panel.VisibilityChanged -= PanelOnVisibilityChanged;
            }

            base.OnDisable();
        }

        protected override void Update()
        {
            base.Update();

            if (_queueRefresh)
            {
                _queueRefresh = false;
                Refresh();
            }
        }

        private void PanelOnVisibilityChanged(IDebugPanelService debugPanelService, bool b)
        {
            // Always end pinning mode when panel is closed
            if (!b)
            {
                SetSelectionModeEnabled(false);

                // Refresh bindings for all pinned controls
                Refresh();
            }
            else if (b && CachedGameObject.activeInHierarchy)
            {
                // If the panel is visible, and this tab is active (selected), refresh all the data bindings
                Refresh();
            }
        }

        public void SetSelectionModeEnabled(bool isEnabled)
        {
            if (_selectionModeEnabled == isEnabled)
            {
                return;
            }

            _selectionModeEnabled = isEnabled;

            PinButton.isOn = isEnabled;
            PinPromptText.SetActive(isEnabled);
            PinPromptSpacer.SetActive(isEnabled);

            foreach (var kv in _options)
            {
                kv.Value.SelectionModeEnabled = isEnabled;
            }

            // Return if entering selection mode
            if (isEnabled)
            {
                return;
            }

            foreach (var kv in _options)
            {
                var control = kv.Value;

                if (control.IsSelected && !Service.PinnedUI.HasPinned(kv.Key))
                {
                    Service.PinnedUI.Pin(kv.Key);
                }
                else if (!control.IsSelected && Service.PinnedUI.HasPinned(kv.Key))
                {
                    Service.PinnedUI.Unpin(kv.Key);
                }
            }
        }

        private void Refresh()
        {
            for (var i = 0; i < _options.Count; i++)
            {
                _controls[i].Refresh();
            }
        }

        #region Initialisation

        protected void Populate()
        {
            var sortedOptions = new Dictionary<string, List<OptionDefinition>>();

            foreach (var option in Service.Options.Options)
            {
                // Find a properly list for that category, or create a new one
                List<OptionDefinition> memberList;

                if (!sortedOptions.TryGetValue(option.Category, out memberList))
                {
                    memberList = new List<OptionDefinition>();
                    sortedOptions.Add(option.Category, memberList);
                }

                memberList.Add(option);
            }

            var hasCreated = false;

            foreach (var kv in sortedOptions)
            {
                if (kv.Value.Count == 0)
                {
                    continue;
                }

                hasCreated = true;
                CreateCategory(kv.Key, kv.Value);
            }

            if (hasCreated)
            {
                NoOptionsNotice.SetActive(false);
            }
        }

        protected void CreateCategory(string title, List<OptionDefinition> options)
        {
            options.Sort((d1, d2) => d1.SortPriority.CompareTo(d2.SortPriority));

            var instance = SRInstantiate.Instantiate(CategoryGroupPrefab);

            instance.CachedTransform.SetParent(ContentContainer, false);
            instance.Header.text = title;

            foreach (var option in options)
            {
                var control = OptionControlFactory.CreateControl(option, title);

                if (control == null)
                {
                    Debug.LogError("[SRDebugger.OptionsTab] Failed to create option control for {0}".Fmt(option.Name));
                    continue;
                }

                control.CachedTransform.SetParent(instance.Container, false);
                control.IsSelected = Service.PinnedUI.HasPinned(option);
                control.SelectionModeEnabled = false;

                _options.Add(option, control);
                _controls.Add(control);
            }
        }

        #endregion
    }
}
