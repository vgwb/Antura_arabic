# Developer Guidelines and Tips
* TOC
{:toc}

Developers should follow these guidelines for contributing to the project.

## Coding conventions

  * Indent using four spaces (no tabs)
  * Use Unix newline
  * Use [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style) braces, but for `if` and `for`
  * Use **camelCase** for internal and private fields
  * Use **CamelCase** for public fields
  * Use **CamelCase** for all methods, public and private, for classes, enum types and enum values.
  * Use **THIS_STYLE** for constants.
  * Regions can be used to group code logically. No nested regiones. Use **CamelCase** for region names.
  * No copyright notice nor author metadata should be present at the start of the file, unless it is of a third party
  * **Never** commit if you encounter compilation errors or warnings.

## Naming conventions

- Use *MiniGame*, not *Minigame* (but *minigame* when lowercase)
- All data related to the learning content should be referred to as *Vocabulary data* (instead of the triad Letter/Word/Phrase)
- All data related to the journey progression should be referred to as *Journey data*

## Namespaces

The whole codebase is under the **Antura** namespace.
The main systems can be accessed through the Antura namespace and thus fall under it.

All minigames are under the **Antura.MiniGames** namespace.
Each minigame needs its own namespace in the form of **Antura.MiniGames.GAME_ID** with GAME_ID being the name of the minigame.

Most core code will be in a subsystem.
Specific subsystem code is inside a **Antura.SUBSYSTEM** namespace, where SUBSYSTEM is the subsystem's name.  
What follows is a list of subsystems with their namespaces:

 * **Antura.Core** for the core managers and data of the appllication.
 * **Antura.AnturaSpace** for code related to the Antura Space scene.
 * **Antura.PlayerBook** for code related to the Player Book scene.
 * **Antura.GamesSelector** handles the Games Selector scene.
 * **Antura.Animations** for general animation utilities.
 * **Antura.Database** for database access and organization.
 * **Antura.LivingLetters** for scripts related to the Living Letter characters.
 * et cetera...

**Never commit anything without a namespace, nor anything under the root Antura namespace**

## Project Structure
### Git Ignore

there are several fiels and directories put under Git Ignore.. the useful from Dev POV are:

```
Local/
Local.meta
```

if you create a Assets/Local directory, you can put inside whatever personal you want, and won't be versioned.
