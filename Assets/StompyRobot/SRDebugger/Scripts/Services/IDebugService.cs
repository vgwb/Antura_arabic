namespace SRDebugger
{
    public delegate void VisibilityChangedDelegate(bool isVisible);

    public delegate void ActionCompleteCallback(bool success);
}

namespace SRDebugger.Services
{
    public interface IDebugService
    {
        /// <summary>
        /// Current settings being used by the debugger
        /// </summary>
        Settings Settings { get; }

        /// <summary>
        /// True if the debug panel is currently being shown
        /// </summary>
        bool IsDebugPanelVisible { get; }

        /// <summary>
        /// True if the trigger is currently enabled
        /// </summary>
        bool IsTriggerEnabled { get; set; }

        IDockConsoleService DockConsole { get; }
        bool IsProfilerDocked { get; set; }

        /// <summary>
        /// Show the debug panel
        /// </summary>
        /// <param name="requireEntryCode">
        /// If true and entry code is enabled in settings, the user will be prompted for a passcode
        /// before opening the panel.
        /// </param>
        void ShowDebugPanel(bool requireEntryCode = true);

        /// <summary>
        /// Show the debug panel and open a certain tab
        /// </summary>
        /// <param name="tab">Tab that will appear when the debug panel is opened</param>
        /// <param name="requireEntryCode">
        /// If true and entry code is enabled in settings, the user will be prompted for a passcode
        /// before opening the panel.
        /// </param>
        void ShowDebugPanel(DefaultTabs tab, bool requireEntryCode = true);

        /// <summary>
        /// Hide the debug panel
        /// </summary>
        void HideDebugPanel();

        /// <summary>
        /// Hide the debug panel, then remove it from the scene to save memory.
        /// </summary>
        void DestroyDebugPanel();

        /// <summary>
        /// Open a bug report sheet.
        /// </summary>
        /// <param name="onComplete">Callback to invoke once the bug report is completed or cancelled. Null to ignore.</param>
        /// <param name="takeScreenshot">
        /// Take a screenshot before opening the report sheet (otherwise a screenshot will be taken as
        /// the report is sent)
        /// </param>
        /// <param name="descriptionContent">Initial content of the bug report description</param>
        void ShowBugReportSheet(ActionCompleteCallback onComplete = null, bool takeScreenshot = true,
            string descriptionContent = null);

        /// <summary>
        /// Event invoked whenever the debug panel opens or closes
        /// </summary>
        event VisibilityChangedDelegate PanelVisibilityChanged;
    }
}
