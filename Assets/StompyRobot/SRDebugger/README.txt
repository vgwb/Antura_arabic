=====================
SRDebugger - (C) Stompy Robot LTD 2015
=====================

Visit https://www.stompyrobot.uk/tools/srdebugger/documentation for more detailed documentation.

# Getting Started:

Open StompyRobot/SRDebugger/Scenes/Sample.unity for a simple example scene.

## Setup

### Unity 5

No setup is required. SRDebugger will automatically load at runtime unless "Auto Load" is disabled in settings. 
By default, the trigger to open the debug panel is attached to the top-left of the game view. Triple-tap there to open the panel. (This can be changed in the settings)
If Auto Load is disabled, follow the instructions for Unity 4.

### Unity 4

Drag the SRDebugger.Init prefab from the SRDebugger folder into your scene. Once loaded, the debug panel will be available in any scene during that play session.
By default, the trigger to open the debug panel is attached to the top-left of the game view. Triple-tap there to open the panel.
Recommended usage is to put the SRDebugger.Init prefab in the first scene that your game loads. 
This way, the debugger can pick up the maximum number of console messages from the initialisation stage of your game, and will be available to any subsequent scenes. 
Due to limitations in Unity, console messages from before SRDebugger loads are unable to be gathered.

## Configuration

On the menu bar, click "Window/SRDebugger Settings" to open the settings pane for SRDebugger. You can set up trigger behaviour, pin entry, and more here.

# Other

For documentation on other features, including the options tab, bug reporter, profiler, etc, visit the documentation online at https://www.stompyrobot.uk/tools/srdebugger/documentation

# Restrictions

 - Icons included in this pack must only be used in the SRDebugger panel. If you wish to use the icons outside of the debug panel, consider licensing from icons8.com/buy
 - Unauthorised distribution of this library is not permitted. See Unity Asset Store EULA for details.
 
# Credits

- Programming/Design by Simon Moles @ Stompy Robot (simon@stompyrobot.uk, www.stompyrobot.uk)
- Icons provided by Icons8 (www.icons8.com)
- Side-bar background pattern provided by Subtle Patterns (www.subtlepatterns.com)
- Orbitron font provided by the League of Moveable Type (theleagueofmoveabletype.com) (Open Font License 1.1)
- Source Code Pro font provided by Adobe (github.com/adobe-fonts/source-code-pro) (Open Font License 1.1)

# Change Log

1.4.9
----------

Changed: 
- Compatibility with Unity 5.4.
- SROptions: Read-only string options now expand to display entire string.

1.4.8
----------

New:
- Added "SROptions Window" for tweaking SROptions parameters while working in the Unity Editor. (Unity 5 only)

1.4.7
----------

New:
- Trigger can now be positioned in CenterLeft, CenterRight, BottomCenter, TopCenter positions.
- Options can now be positioned in TopCenter and BottomCenter positions.

Changes:
- Console now scrolls to the last log entry when first opened.
- Moved "using" statements inside namespace to prevent conflicts with user code.
- Renamed the hierarchy names of all prefabs to include an SR_ prefix to prevent conflicts with user code.

Fixes:
- Fixed input bug when using Unity 5.3.3p2.
- Allocation per frame when pin entry form is visible has been removed.
- Mono usage profiler correctly reports when not supported on 5.3+


1.4.6
----------

Fixes:
- Editor resources used by SRDebugger are no longer included in non-editor builds.

Known Issues:

- On Unity 5.3.0f4, errors are printed when resizing the docked console and profiler. This is a Unity bug and should be fixed in a future Unity update. See http://issuetracker.unity3d.com/issues/layoutrebuilder-errors-when-changing-rect-transform-width-in-layout-element-component for details.

1.4.5
----------

Changes:
- Added notice about known issue to Welcome window when running Unity 5.3
- Unity 4.7 is now minimum supported version.

Fixes:
- Bug reporter signup form continues to the next page correctly after submitting.

1.4.4
----------

Changes:
- Support for Bug Reporter on WebGL platform.
- Enabled HTTPS for bug reporter on iOS to comply with TLS restrictions.
- Documented pin entry API, and deprecated an obsolete parameter. (See documentation for example of how to use pin entry API)

Fixes:
- TouchInputModule is now added to default event system on Unity 4, allowing touch input to be recognised by SRDebugger.
- Welcome window no longer causes errors on Unity 4.

1.4.2 & 1.4.3
----------

Changes:
- Compatibility with Unity 5.3.0.
- Performance improvements when scrolling console log.

Fixes:
- Profiler no longer stops updating when a camera in the scene is disabled.
- (1.4.3) Fix build on Windows Store platform.

1.4.1
----------

Fixes:
- Bug reporter tab no longer requests pin entry after taking screenshot when "require pin every time" enabled.
- Compile fixes for Unity 5.2.2

1.4.0
----------

New:
- Brand new Settings window with more intuitive layout and tabbed interface.
- Added "Welcome" window that opens on first import to help first-time users.
- Can now customize the docked tools layout from the new settings window.
- Docked console alignment can be adjusted from the API (SRDebug.Instance.DockConsole.Alignment).
- Added new "Double Tap" mode for entry trigger.
- (EXPERIMENTAL) Added PlayMaker actions package (Open bug report sheet, Open/Close debug panel, Dock/Undock Console/Profiler, Enable/Disable trigger, etc).

Changes:
- Keyboard shortcuts can now have modifier keys set per-shortcut, instead of only for all shortcuts.
- Bug reporter signup form now provides more helpful error messages.

Fixes:
- Stack trace area no longer jumps to the bottom of the scroll area when selecting a log entry.
- DisplayName attribute now works correctly on methods in SROptions.
- Bug reporter progress bar no longer only fills half-way when submitting bug reports.
- Exception no longer occurs when opening debug panel if you have a custom tab.
- Fixed intertia in scroll views not being enabled when on mobile platforms.

1.3.0
----------

New:
- Profiler can now be docked. Enable by pressing the "pin" icon on profiler tab or via API (SRDebug.Instance.IsProfilerDocked), or via keyboard shortcuts
- Resize docked profiler by dragging edges
- Added IncrementAttribute for use with SROptions, used to specify how much a number will be incremented/decremented when buttons are pressed
- Can disable specific tabs in SRDebugger settings
- Added "Runtime" and "Display" categories to system tab (this information is also sent with bug reports)
- Support for Unity 5.2

Changes:
- Namespace remaining code in SRF library to avoid conflicts. (If you're using any of this code you may need to import SRF namespace in your files)

Fixes:
- Fixed opacity on docked console not resetting after failed resize drag
- Truncate long log messages to improve performance and prevent UGUI errors

1.2.1
----------

New:
- Added DisplayName attribute for use with SROptions.

Changes:
- Read-only properties are now added to options tab (but can't be modified).
- Sort attribute can now be applied to methods.

Fixes:
- Fixed compile errors when NGUI is imported in the same project.
- Removed excess logging when holding a number button in options tab.

1.2.0
----------

New:
- Dock console at the top of the screen. (open from the console tab, SRDebug API or keyboard shortcuts)
- Collapse duplicate log entries (enable in settings)
- Bug Report popover. Show bug reporter without granting access to the debug panel. Open via keyboard shortcut or the SRDebug API.
- Added Sort attribute to sort items in options tab. (See SROptions.Test.cs for examples)
- Added SROptions PropertyChanged support. Call OnPropertyChanged() in your setters and pinned options will update to reflect the new value.
- Entry code can now be entered with keyboard.

Changes:
- Sending screenshot with bug report now supported on web player.

Fixes:
- Fixed pin entry canvas not using correct UI camera.
- Modified namespaces and naming of internal classes to reduce conflicts with other assets.
- Fixed script updater having to run for Unity 5.1
- Misc bug fixes

1.1.2
----------

Changes:
- Bug reporter is now supported on Web Player builds (now uses Unity WWW instead of HttpWebRequest for API calls)
- System Information area now shows IL2CPP status on iOS builds
- Application.platform value is now included with bug reports
- Support for Unity 5.1

Fixes:
- Fixed issues with options panel and IL2CPP on iOS
- Unity Cloud Build information now formatted correctly
- Fixed Settings UI issue on Unity 5.1 beta
- Fixed Entry Code setting having no effect
- Fixed keyboard shortcuts bypassing entry code if enabled

1.1.1
----------

Changes:
- The version of SRF (https://github.com/StompyRobot/SRF) has been changed to the "Lite" version, containing only scripts relevant to SRDebugger. If you want the full SRF library it is available free on GitHub.

Fixes:
- SRDebugger no longer creates an event system in a scene if one already exists on Unity 5 using Auto-Init.
- Fixed CategoryAttribute being in the wrong namespace when when compiling for Windows 8 platforms.

1.1.0
----------

New:
- (Unity 5) Can enable "Auto-Init" in the Settings pane to automatically initialize SRDebugger without SRDebugger.Init prefab included in the scene.
- (BETA) Bug Reporter - Users can submit bug reports, with console log and system information included. These will be forwarded to you by email. (Enable in Settings)
- (BETA) Windows Store support
- Added support for Keyboard Shortcuts
- Added Trigger Behaviour option. Switch between "Triple-Tap" and "Tap-And-Hold" methods for opening debug panel
- Added Default Tab option in Settings pane
- Added Layer option to settings panel to choose which layer UI will be on
- Added Debug Camera mode (render debug panel UI to a camera instead of overlay)
- SRDebug.Init() method added for custom initialisation of SRDebugger without SRDebugger.Init prefab
- Event added to SRDebug on panel open/close

Changes:
- Scroll sensitivity has been improved for desktop platforms

1.0.2
----------

Fixed:
- Fixed console layout with Unity 4.6.3+
- Trigger Position setting now checked on init

1.0.1
----------

New:
- Unity 5.0 Support.
- Added option to Settings pane to require the entry code for every time the panel opens, instead of just the first time.

Fixed:
- Removed debug message when opening Options tab for first time.
- Fixed conflict with NGUI RealTime class.
- Fixed layout of pinned options when number of items exceeds screen width.

1.0.0
----------

Initial version.
