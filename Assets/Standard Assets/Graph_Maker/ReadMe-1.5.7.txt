----------------------------------------------
            Graph Maker
 Copyright © 2013-2016 Stuart S.
    http://forum.unity3d.com/threads/202437
----------------------------------------------

Thank you for buying Graph Maker!

Please read the FAQ section from the manual (GraphMaker.pdf)

For questions, suggestions, bugs: email rorakin3@gmail.com or post in the Unity thread!

-----------------
 Installation
-----------------

This pack is primarily developed / tested with the UGUI (unity GUI system).

- DOTween is the system used for Graph Maker animations.
If DOTween is already in your project, make sure to uncheck the DOTween folder during import.


-- To use Graph Maker with NGUI, copy and paste the Graph_Maker_NGUI package to your desktop, 
delete the Graph Maker folder in your project, and import the Graph_Maker_NGUI package.

- You cannot have both Graph Maker for UGUI and Graph Maker for NGUI installed in the same project.


-----------------
 Version History
-----------------
1.5.7
- NEW: Dual-y axis charts now supported.
- NEW: Added new "Upgrades Guide" section in manual.
- NEW: Font / font style options added for legend text.
- NEW: Hover / click events added for Pie legend entries.
- FIX: Hierarchical tree graph resizing issue.
- FIX: Tooltip flicker issue when tooltip would be off screen.
- CHANGE: Tooltip on hover exit called when gameobject being hovered disabled.
- CHANGE: X/Y-axis title text offset is now offset from an anchored edge of chart.

1.5.6
- NEW: A new alternate rendering mode for area shading, improving performance a great deal!
- FIX: Fix an issue with axis rescaling (auto grow / shrink functionality)

1.5.5
- NEW: Font, font color, and font style options added for x / y axis labels and series data labels
- NEW: Added hide arrow options
- NEW: New example scene for populating pie graph that starts out empty
- FIX: Fix issue with tooltips and worldspace / screenspace camera canvas types
- FIX: Fix issue with series data labels offsets and horizontal charts

1.5.4
- NEW: Click / Hover events added for Pie / Doughnut Graphs! New example scene added to demonstrate.
- FIX: Performance improvement for Pie Graphs not using "Doughnut" feature.
- CHANGE: Series parameter "AutoUpdateXDistBetween" is obsolete, refer to compiler warning if your code uses it.

1.5.3
- NEW: New example scene that animate plots data overtime, scene code also shows how to override all labeling functions.
- NEW: Options added for axes for label skipping (e.g. only label every other label on y-axis).
- CHANGE: Hierarchical tree width / height now based off Rect Transform.
- FIX: Improved shaders - The aliasing / artifacting for area shading is gone, even without pixel perfect set on canvas.
- FIX: Graph tooltip can no longer go beyond the screen edge.
- FIX: Ring graph labels intelligently stack (e.g. rings corresponding with the same value).

1.5.2
- NEW: FAQ section added to the manual, check it out!
- FIX: Area shading graphs / custom shaders work with Unity UI masking / scroll view components.
- FIX: The auto grow / shrink functionality updates labels / refreshes the graph.

1.5.1
- NEW: Text Mesh Pro support!
- NEW: There are now delegates for tooltip and datalabel text labeling functions.
- FIX: Fix issue with worldspace tooltips.

1.5
- NOTE: This version brings many changes. This version is --NOT-- backwards compatible, backup your project!
- NEW: Additional Graph type "Combo" allows doing both a bar chart and line chart in one graph. Specify series to be either bar or line.
- NEW: Additional Graph type "Line Stacked" allows for stacked line charts (similar to bar stacked)
- NEW: New Bezier Band Graph. This graph is a set of bands based on cubic bezier formula.
- NEW: Performance improvements with observable properties / lists. All graphs no longer check against cached values every frame.
- NEW: More custom Unity editors added (Series, Legends, Radar graphs, Ring graphs, Hierarchical Tree)
- NEW: Can specify list of colors instead of just single color for points / bars.
- NEW: Can specify to auto update bar width based on percentage of graph so updating bar width manually is not necessary.
- NEW: More examples including a real-time update example added to the X_Dynamic scene.
- NEW: Legend positioning is easier / more intuitive (anchored to chart)
- NEW: Legend can automatically detect its width / height exceeding the graph boundary, and then adjust its # rows / columns automatically.
- NEW: Auto center option added to pie chart, which automatically centers the pie chart (and legend) relative to its parent / background sprite.
- CHANGE: Axes have been refactored into a separate class (e.g. graph.yAxis.Labels instead of graph.yAxisLabels)
- CHANGE: Pie Chart size and Ring Graph outer radius removed (now more intuitive / based purely on Rect Transform).
- FIX: Fixed animation issue when changing values too quickly.

1.4
- NOTE: This version brings many many changes, and is thus incompatible with previous Graph Maker versions.
- NEW: A new type of graph has been added called Ring Graph.
- NEW: Custom Editors! Now it is much easier to interact with publicly exposed Graph Maker variables via the Unity Editor.
- NEW: Pie graph updates! Now possible to make doughnut graphs from pie graphs. Also possible to explode slices such that they evenly align at the outside edge.
- NEW: X/Y axis Labeling system rewritten / greatly improved for axis graphs. Use label types to easily define label behavior. Can now also be independent from grid ticks.
- NEW: Legend system has been rewritten. Pie graphs and Axis Graphs now use identical legend code, and legend parameters are specified on a legend specific object.
- NEW: Now possible to graph null values (e.g. broken line segments). Use the newly added groups and enable grouping variables to do this.
- NEW: The animation library HOTween has been replaced with the newer / faster animation library DOTween.
- NEW: WMG_Data_Source has been added to make it easy to auto pull data from a generic object data source using reflection. Works for PlayMaker variables as well.
- CHANGE: All graphs now expose a public Refresh() function, and an Auto Refresh boolean. Disable auto refresh and manually call Refresh() for slightly improved performance.
- CHANGE: X/Y axis lengths have been removed. This is now controlled entirely by the width / height of the root rect transform / NGUI widget.
- CHANGE: Daikon Forge is no longer supported / updated since this GUI system is no longer supported / updated.

1.3.9
- NEW: Radar graphs! Now you can create pentagonal or other shape based graphs!
- NEW: Graphs can now dynamically resize! There are several options to select what gets resized. Because of this change, you may get runtime errors upgrading Graph Maker from a previous version. To solve this ensure your existing graphs have a RectTransform or a NGUI widget component at the graph root.
- FIX: Building for Windows Phone 8 reflection issues addressed with separate reflection static class with assembly directives. Note that HOTween dll also needs to be changed for WP8 build (refer to HOTween website).
- FIX: All animations are now independent of time scale (can work during pause screen).
- FIX: The auto-update space between functionality behavior has been improved.

1.3.8
- NEW: UGUI Support!

1.3.7
- NEW: Data labels! Add and customize data labels for each series.
- NEW: Titles! Optionally include x / y axis titles, and a graph title.
- NEW: Bar charts don't have to start from the bottom! Freely move around the base for bar charts to create upside down bar charts, or a combination.
- NEW: Additional legend options! Legends can now have multiple rows (horizontal legends), or columns (vertical legends). 

1.3.6.1 
- FIX: Fixed error during build for NGUI.

1.3.6
- NEW: Stacked line graphs and ability to add custom area shading for any line series added using advanced custom shaders created using Shader Forge!

1.3.5
- NEW: Hierarchical skill trees! Example scene has been added to demonstrate skill trees. Create your own skill trees by specifying nodes and links.

1.3.4
- NEW: Out of the box tooltips! Doing tooltips no longer requires you to write any code, simply set some graph level parameters!
- NEW: Automatic animations! Several animations are now possible without requiring you to write any code, simply set some graph level parameters!
- NEW: Series level setting to add more spacing between series. This is useful to have bar graphs where the bars are not right next to eachother.
- NEW: More dynamic axis type settings. New auto origin - x/y options allow the axes to position closest to the origin. The origin is also configurable.
- NEW: For the auto axes type options, you can now also specify whether the axes lock to the nearest gridline or move around freely.

1.3.3
- NEW: Click and hover events have been added to make adding interactivity to graphs very easy.
- NEW: Line padding variable added to series script to allow creating lines that don't exactly touch at the point. Useful for creating hollow points.
- NEW: Hide x / y axis tick boolean variables added to the graph, can be used to show / hide axis ticks independently of labels and vice versa.
- NEW: Hide legend labels boolan variable added, useful now that legend events can be added, since this can be shown in a tooltip.
- NEW: API for dynamically instantiating and deleting series, useful if you don't know how many series you will have for a given graph.
- NEW: NGUI 2.7 is now supported for both Unity 4 and Unity 3.5.
- CHANGE: Data generation example scene code is now mostly GUI system independent.
- CHANGE: Functionality in the manager script has been split up: caching, data generators, events, and path finding are now smaller separate scripts.

1.3.2
- NEW: Animations! Example scene has been updated to demonstrate the use of the animation functions. All animations use HOTween.
- FIX: Fixed issues for Daikon version upgrade 1.0.13 -> 1.0.14
- FIX: Different default link prefab is now used for all lines in all graphs, which improves overall line quality.
- FIX: Axis Graph script is now fully cached, performance should be the best it can possibly be. This removed the refresh every frame variable.
- NOTE: These changes break backwards compatibility, but can be easily addressed
- FIX: Prefab reference variables moved from series to graph script, so they don't need to be set for each series.
- FIX: Line Width variable renamed to Line Scale

1.3.1
- FIX: Fixed issue discovered when upgrading DFGUI version where first label not positioned correctly
- FIX: WMG_Grid now implements caching, increasing general performance for all graphs (WMG_Grid is used for grid lines and axis labels)

1.3
- NOTE: This version brings many changes that break backwards compatibility, highly recommend remaking your existing graphs from the new examples.
- NEW: New interactive example scene added for both NGUI and Daikon that showcases many Graph Maker features.
- NEW: Ability to do real-time update for an arbitrary float variable (uses reflection similar to property binding in Daikon).
- NEW: Codebase refactored to use nearly all GUI independent code. All NGUI and Daikon specific code in a manager script.
- NEW: Ability to automatically set x / y axis min / max values based on series point data added.
- NEW: Ability to specify an axes type. This sets the axes positions based on a quadrant type.
- NEW: Added an axes width variable to more easily change the width of the axes.
- NEW: Legend entry font size variable added.
- NEW: Connect first to last variable added, which links the first and last points. Useful for creating a circle.
- NEW: Added hide x / y labels variables.
- FIX: Huge performance improvement for the update every frame functionality with caching, this removed the series update boolean.
- FIX: Resolved offset issues in Daikon due to differences in pivot / position behavior in NGUI vs Daikon.
- FIX: Auto space based on number of points variables moved from graph script to series script.
- FIX: Replaced point / line prefab variables with a list of prefabs to easily switch prefabs at runtime.
- FIX: "Don't draw ... by default" and list of booleans "Hide Lines / Points" replaced with single hide points / lines boolean.
- FIX: Changed the axis lines to always be center pivoted to resolve some axis positioning issues.
- FIX: Fixed some vertical vs horizontal issues. Behavior is to swap many x / y specific values instead of rotate everything.
- FIX: Tick offset float variables replaced with above vs below and right vs left booleans. The axes type automatically sets these.

1.2.1:
- NEW: Added support for Daikon Forge
- NEW: Added support for NGUI + Unity 3.5

1.2:
- NEW: Upgraded from NGUI 2.7 to 3.0
- NEW: Graph type parameter to switch between line, side-by-side bar, stacked bar, and percentage stacked bar.
- NEW: Orientation type parameter to switch between vertical and horizontal graphs. Useful for horizontal bar charts.
- NEW: Added parameters to control placement of axes and what axis arrows display. Can now make 4-quadrant graphs.
- NEW: Scatter plot prefab added to showcase changes made to better support scatter plots.
- FIX: Series point value data changed from Float to Vector2 to more easily support scatter plots and arbitrary data plotting.
- FIX: Negative values did not update all labels properly, and data was also not positioned correctly for negative values.

1.1:
- NEW: First Unity Asset Store published version

1.0:
- NEW: Created
