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

@todo: this should probably be the Antura Application (main)

### Core and Minigame code

The code of EA4S is separated in two main sections.

 * **core** code is related to the main application. This code should not be touched by mini-game developers.
 * **minigame** code is 
 
Note that part of the *core* code can be used by minigame code, such as *Tutorial* or *UI* code.

@todo: how to define minigame shared code instead?

### Application Flow

The entry point for the application is the (_app/_scenes/_Start) scene.
This scene initialises the AppManager, shows the ProfileSelectorUI 
 to allow the user to select a profile and uses the HomeManager to advance the application. 

Profile Selection:
 
@todo: add app flow diagram


The player profile is selected in the Home (_Start) scene.
The static learning database and the player's dynamic database are loaded now.

 
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

## Instantiate Managers


## Refactoring notes

 * AppManager and InstantiateManager spawn managers in several ways, standardize manager creation and access.
 * Many subsystems have their own singleton and should instead be represented in AppManager

 
 
### Navigation Manager

The Navigation Manager controls the transitions between different scenes in the application.
The logic for 

### Player Profile Manager

Controls



### Debug Manager

Controls

### Antura

@TODO: move to minigames description

Antura is... 
The Antura classes are used to control Antura's behaviours, its animations, and define the appearance of rewards.
AnturaAnimationController and AnturaWalkBehaviour control the animation state of Antura.
AnturaModelManager 

The contents of the AnturaSpace folder handle interactions with Antura in the AnturaSpace scene, used for reward and customization purposes.


### Games Selector


After a pin is accessed, the Games Selector is reached.
The GameSelector accesses the list of current mini games from the teacher data **TODO: REFACTOR THIS BEHAVIOUR** and shows it.
As the user pops all bubbles, the Games Selector starts the next scene through **GoToMinigame()**, by accessing 
 
The **EA4S.GamesSelector** namespace contains all code for the games selector.



#### Scenes

### Player Book

The Player Book scene contains information on 

### Home scene

The HomeManager controls the _Start scene, providing an entry point for all users prior to having selected a player profile. 
   
### End scene

The EndManager controls the app_End scene, providing an entry point for all users prior to having selected a player profile. 
   