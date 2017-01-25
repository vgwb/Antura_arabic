# Project Structure

* TOC
{:toc}

This document describes the organization of the project folders.

## Folders

The project is separated into 4 main folders:

- **_app** contains all assets and scripts related to the general application. Under **_scripts**, the code for each of the app's subsystems is in its own folder.
- **_games** contains all assets and scripts related to the mini-games.
  - Each mini-game has its own sub-folder, with the game name as the folder name.
  - **_common** contains assets that are shared among games
  - **_gametemplate** contains a template for creating a new minigame from scratch.
- **_manage** contains scenes useful for previewing and/or manage data.

Other folders have special purposes:

- **Plugins** and (StreamingAssets) contain build-related files.
- **Resources** contains settings and data that is loaded at runtime.
- **Standard Assets** contains third party plugins and code.
- **StompyRobot** is third party plugins that must be placed in the root folder to function.

## Refactoring notes

- Move the contents of **_app/Elements** to the correct Models/Images folders.
- Delete all contents of **_tests** and **Trash**
- Move the contents of **Resources** to **_app/Resources**
- Parts of **_manage** should be moved to **_testing** and be removed from the main public repository (as used only for test purposes)
- **Standard Assets** could be renamed to (Third Party) and (Standard Assets/Effects) and (Standard Assets/Editor) should be placed under a (Unity Standard Assets) folder.
