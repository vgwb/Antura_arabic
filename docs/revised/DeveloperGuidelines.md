Contributing to EA4S - Antura and the Letters
=================

*Edits:*

<table>
  <tr>
    <td>11-01-2017</td>
    <td>Michele Pirovano</td>
  </tr>
</table>

Developers should follow these guidelines for contributing to the project.

### Coding conventions

  * Indent using four spaces (no tabs)
  * Use Unix newline
  * Use [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style) braces
  * Regions can be used to group code logically. Use (CamelCase) for region names.
  * Use (camelCase) for internal and private fields
  * Use (CamelCase) for public fields
  * Use (CamelCase) for all methods, public and private
  
### Namespaces

The whole codebase is under the (EA4S) namespace.
The main systems can be accessed through the EA4S namespace and thus fall under it.

All minigames are under the (EA4S.MiniGames) namespace.

Specific subsystem code is inside a (EA4S.***) namespace, where (***) is the subsystem's name.

  EA4S
  
What follows is a list of all subsystems with their namespaces:

 * **EA4S.Animations** for general animation utilities.
 * **EA4S.AnturaSpace** for code related to the Antura Space scene.
 * **EA4S.PlayerBook** for code related to the Player Book scene.
  

### Git Commit Messages

  * to be expanded

### Documentation Styleguide

  * to be expanded

 
### Current issues
 
  * What about copyright notices? there are several around the codebase
  * We need to define code conventions. Issues with AppConstants, 
  