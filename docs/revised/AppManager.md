Managers
===============

*Edits:*

<table>
  <tr>
    <td>11-01-2017</td>
    <td>Michele Pirovano</td>
  </tr>
</table>

@todo: make sure to add a summary to all the main managers so to generate the docs.

### Application Flow

The entry point for the application is the (_app/_scenes/_Start) scene.
This scene initialises the AppManager, shows the ProfileSelectorUI 
 to allow the user to select a profile and uses the HomeManager to advance the application. 

Profile Selection:
 
@todo: add app flow diagram

 
### App Manager

The App Manager is the core of the application.
It functions as a general manager and entry point for all other systems and managers.

It is instantiated as a Singleton, accessible as AppManager.Instance.

The App Manager is used to start, reset, pause, and exit the game.
It also controls the general flow of the application.

@todo: The App Manager is configured through 

@todo: add Exit() to the AppManager? Where is it?

The (GameSetup) method functions as the entry point of the application.
All subsystems initialisation is carried out in this method.

## Refactoring notes

 * AppManager should be refactored to follow code guidelines
 * Many subsystems have their own singleton and should instead be represented in AppManager

### Home Manager

Controls the _Start scene, providing an entry point for all users prior to having selected a player profile. 
   
### End Manager

Controls the app_End scene, providing an entry point for all users prior to having selected a player profile. 
   
### Navigation Manager

The Navigation Manager controls the transitions between different scenes in the application.

### Player Profile Manager

Controls

