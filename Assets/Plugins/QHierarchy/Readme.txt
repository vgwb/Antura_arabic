QHierarchy
==========
Thank you for downloading QHierarchy!
Please rate it if you liked it!

Using Of The Package
=================
QHierarchy is very easy to use. As soon as the package is 
imported into project, the Hierarchy window will change its view.

It will get:

- Button to show / hide a game object
Allows changing the visibility of a game object right from the Hierarchy window. 
Use Shift+Click to show / hide a game object and its children
Use Alt+Click to show / hide a game object and its siblings
Use Ctrl/Cmd + Click to show / hide a game object only during editing.

- Button to lock / unlock a game object 
Allows locking / unlocking a game object right from the Hierarchy window.
Use Shift+Click to lock / unlock a game object and its children
Use Alt+Click to lock / unlock a game object and its siblings

- Button to change Static property.
Allows to change Static checkbox of a game object right from the Hierarchy window.
Use Shift+Click to change Static checkbox of a game object and its children
Use Alt+Click to change Static checkbox of a game object and its siblings

- Button to enable / disable renderer component of a game object
Allows to enable/disable renderer component of a game object right from the Hierarchy window.
Use Ctrl/Cmd + Click to hide only wireframe of game object

- Icons of game objects 
The icon appears if it is set to a game object in Inspector window 
 
- Icon of MonoBehaviour script attached
The icon appears if MonoBehaviour script has been attached to a game object
 
- Error icon
The icon appears if:
* MonoBehaviour script of GameObject is missing
* Reference property of script is null
* String property is empty

- Displaying tag and layer of a game object
Tag and layer appear if they are not set to default values

- Displaying the list of game objects in the form of a tree for an easy visual navigation

Use the QHierarchy setting window (Window -> QTools -> QHierarchy -> Settings) to enable or disable any of the functions. 

Support
=======
If there are any difficulties 
or you have questions or suggestions, 
please, contact me: qtools.develop@gmail.com

Version History
===============
2.19
- After the update 2.17 were discovered a few serious bugs with static batching and the long duration of project buildings.
In order to maintain the added functionality in the update 2.16, I had to go to the following changes: 
the QHierarchyObjectList GameObject will now be stored in the scenes when you build the project. 
This script will restore the visibility of the GameObjects if it was changed in the editor for edit-time.

2.18
- Fixed: show / hide / static / lock / unlock siblings gameobject now works (for root objects - only for Unity 5.3.3 and above)
- Changed: show / hide / static / lock / unlock now depends on the selected objects

2.17
- Fixed: scenes are wrong order after build.

2.16
- Added: Now the plugin supports multi scene.
- Fixed: Edit-time visible / invisible objects don't restore the state in the build.
- Removed: alphabetical sort (obsolete)

2.15
- Feature: Added the ability to hide the wireframe of object  (Ctrl/Cmd + Click on the Renderer Button)

2.14
- Feature: Added Enable / Disable Renderer Button (Renderer Button)

2.13
- Added: Now you can Click+Drag over Lock / Visible / Static buttons to change state of many objects in one movement.

2.12 
- Fixed: Icon is not shown when "Tag And Layer" have "Fixed" layout option.
- Added option to set percent width of "Tag And Layer" label.
- Error icon is shown when list has missing element

2.11
- Added option to ignore standard scripts for MonoBehaviour Icon

2.10
- Fixed: Error icon is not displayed for objects that have: [HideInHierarchy] attribute, public static properties or public delegates.
- Fixed: can't rename GameObject when hidden QHierarchy GameObject is showed

2.9
- Fixed bug: no error icon for game object with a null reference in the properties.
- Added option to hide error icon for disabled components.

2.8
- Added option to display a warning message when use Modifier + Click
- Fixed bug when QHierarchyObjectList is continuous lock/unlock
- Now you can remove QHierarchyObjectList, but "lock, unlock, edit visible" states will be reset. It will not be created again unless you use these functions.

2.7
- Added Shift and Alt modifiers for Static button
- Fixed bug when QHierarchy not work after import
- Fixed bug that showed message "A script behaviour has a different serialization layout when loading" after build
- Added option in Settings window that shows hidden QHierarchy GameObject

2.6
- Feature: Static Button Added
- Tested with Unity 5.1

2.5
- Fix some bugs

2.4
- Feature: Prevent selection of locked objects
- Fix some bugs

2.3
- Fix bug when settings window appears when switch Play / Stop

2.2
- Feature: Option to set custom icon for any tag
- Feature: Option to add indentation after icons (useful if you use other plug-ins that add another icons in the window hierarchy)
- Settings inspector improved
- Performance optimization

2.1
- Fix Scroll of Settings Window
- Fix Lock / Unlock

2.0
- Feature: Displaying tag and layer of a game object added
- Feature: Option to hide / show a game object during edit-time added
- Feature: Option to display error icon in case of an error in the children added
- Feature: The order of icons can be changed
- Code improvements
- Settings inspector improved
- Bugs fixed

1.4
- Feature: Error Icon Added

1.3
- Minor graphic bugs fixed
- Tested with Unity 5.0

1.2
- Feature: MonoBehaviour Icon Added

1.1
- Minor bugs fixed

1.0
- Initial Release