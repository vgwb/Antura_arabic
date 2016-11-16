namespace SRDebugger.Services.Implementation
{
    using System;
    using Internal;
    using SRF;
    using SRF.Service;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Service(typeof (IDebugService))]
    public class SRDebugService : IDebugService
    {
        private readonly IDebugPanelService _debugPanelService;
        private readonly IDebugTriggerService _debugTrigger;
        private readonly ISystemInformationService _informationService;
        private readonly IOptionsService _optionsService;
        private readonly IPinnedUIService _pinnedUiService;

        private bool _entryCodeEnabled;
        private bool _hasAuthorised;
        private DefaultTabs? _queuedTab;

        public SRDebugService()
        {
            SRServiceManager.RegisterService<IDebugService>(this);

            // Load profiler
            SRServiceManager.GetService<IProfilerService>();

            // Setup trigger service
            _debugTrigger = SRServiceManager.GetService<IDebugTriggerService>();

            _informationService = SRServiceManager.GetService<ISystemInformationService>();
            _pinnedUiService = SRServiceManager.GetService<IPinnedUIService>();
            _optionsService = SRServiceManager.GetService<IOptionsService>();

            // Create debug panel service (this does not actually load any UI resources until opened)
            _debugPanelService = SRServiceManager.GetService<IDebugPanelService>();

            // Subscribe to visibility changes to provide API-facing event for panel open/close
            _debugPanelService.VisibilityChanged += DebugPanelServiceOnVisibilityChanged;

            _debugTrigger.IsEnabled = Settings.EnableTrigger == Settings.TriggerEnableModes.Enabled ||
                                      Settings.EnableTrigger == Settings.TriggerEnableModes.MobileOnly &&
                                      Application.isMobilePlatform;

            _debugTrigger.Position = Settings.TriggerPosition;

            if (Settings.EnableKeyboardShortcuts)
            {
                SRServiceManager.GetService<KeyboardShortcutListenerService>();
            }

            _entryCodeEnabled = Settings.Instance.RequireCode && Settings.Instance.EntryCode.Count == 4;

            if (Settings.Instance.RequireCode && !_entryCodeEnabled)
            {
                Debug.LogError("[SRDebugger] RequireCode is enabled, but pin is not 4 digits");
            }

            // Ensure that root object cannot be destroyed on scene loads
            var srDebuggerParent = Hierarchy.Get("SRDebugger");
            Object.DontDestroyOnLoad(srDebuggerParent.gameObject);
        }

        public Settings Settings
        {
            get { return Settings.Instance; }
        }

        public bool IsDebugPanelVisible
        {
            get { return _debugPanelService.IsVisible; }
        }

        public bool IsTriggerEnabled
        {
            get { return _debugTrigger.IsEnabled; }
            set { _debugTrigger.IsEnabled = value; }
        }

        public bool IsProfilerDocked
        {
            get { return Service.PinnedUI.IsProfilerPinned; }
            set { Service.PinnedUI.IsProfilerPinned = value; }
        }

        public void AddSystemInfo(InfoEntry entry, string category = "Default")
        {
            _informationService.Add(entry, category);
        }

        public void ShowDebugPanel(bool requireEntryCode = true)
        {
            if (requireEntryCode && _entryCodeEnabled && !_hasAuthorised)
            {
                PromptEntryCode();
                return;
            }

            _debugPanelService.IsVisible = true;
        }

        public void ShowDebugPanel(DefaultTabs tab, bool requireEntryCode = true)
        {
            if (requireEntryCode && _entryCodeEnabled && !_hasAuthorised)
            {
                _queuedTab = tab;
                PromptEntryCode();
                return;
            }

            _debugPanelService.IsVisible = true;
            _debugPanelService.OpenTab(tab);
        }

        public void HideDebugPanel()
        {
            _debugPanelService.IsVisible = false;
        }

        public void DestroyDebugPanel()
        {
            _debugPanelService.IsVisible = false;
            _debugPanelService.Unload();
        }

        #region Options

        public void AddOptionContainer(object container)
        {
            _optionsService.AddContainer(container);
        }

        public void RemoveOptionContainer(object container)
        {
            _optionsService.RemoveContainer(container);
        }

        public void PinAllOptions(string category)
        {
            foreach (var op in _optionsService.Options)
            {
                if (op.Category == category)
                {
                    _pinnedUiService.Pin(op);
                }
            }
        }

        public void UnpinAllOptions(string category)
        {
            foreach (var op in _optionsService.Options)
            {
                if (op.Category == category)
                {
                    _pinnedUiService.Unpin(op);
                }
            }
        }

        public void PinOption(string name)
        {
            foreach (var op in _optionsService.Options)
            {
                if (op.Name == name)
                {
                    _pinnedUiService.Pin(op);
                }
            }
        }

        public void UnpinOption(string name)
        {
            foreach (var op in _optionsService.Options)
            {
                if (op.Name == name)
                {
                    _pinnedUiService.Unpin(op);
                }
            }
        }

        public void ClearPinnedOptions()
        {
            _pinnedUiService.UnpinAll();
        }

        #endregion

        #region Bug Reporter
        public void ShowBugReportSheet(ActionCompleteCallback onComplete = null, bool takeScreenshot = true,
            string descriptionContent = null)
        {
            var popoverService = SRServiceManager.GetService<BugReportPopoverService>();

            if (popoverService.IsShowingPopover)
            {
                return;
            }

            popoverService.ShowBugReporter((succeed, message) =>
            {
                if (onComplete != null)
                {
                    onComplete(succeed);
                }
            }, takeScreenshot, descriptionContent);
        }

        #endregion

        public IDockConsoleService DockConsole
        {
            get { return Service.DockConsole; }
        }

        public event VisibilityChangedDelegate PanelVisibilityChanged;

        private void DebugPanelServiceOnVisibilityChanged(IDebugPanelService debugPanelService, bool b)
        {
            if (PanelVisibilityChanged == null)
            {
                return;
            }

            try
            {
                PanelVisibilityChanged(b);
            }
            catch (Exception e)
            {
                Debug.LogError("[SRDebugger] Event target threw exception (IDebugService.PanelVisiblityChanged)");
                Debug.LogException(e);
            }
        }

        private void PromptEntryCode()
        {
            SRServiceManager.GetService<IPinEntryService>()
                .ShowPinEntry(Settings.Instance.EntryCode, SRDebugStrings.Current.PinEntryPrompt,
                    entered =>
                    {
                        if (entered)
                        {
                            if (!Settings.Instance.RequireEntryCodeEveryTime)
                            {
                                _hasAuthorised = true;
                            }

                            if (_queuedTab.HasValue)
                            {
                                var t = _queuedTab.Value;

                                _queuedTab = null;
                                ShowDebugPanel(t, false);
                            }
                            else
                            {
                                ShowDebugPanel(false);
                            }
                        }

                        _queuedTab = null;
                    });
        }
    }
}
