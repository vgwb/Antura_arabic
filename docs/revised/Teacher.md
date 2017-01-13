EA4S Teacher System
===============

*Edits:*

<table>
  <tr>
    <td>13-01-2017</td>
    <td>Michele Pirovano</td>
  </tr>
</table>

The Teacher System represents the language teacher inside the application.
It is an Expert System that controls the learning progression of the player based on:
  * Player journey progression
  * Player learning performance
  * Expert configuration
  * Minigame support requirements
It is designed to be agnostic to the specific language and highly configurable in respect to mini-games.

The Teacher System can be found under the **EA4S.Teacher** namespace.

### Elements

The Teacher is composed of several elements:

**EA4S.TeacherAI** works is the entry point to the teacher functions. Helpers and Engines can be accessed from the **AppManager.Instance.Teacher** instance.
 
 * Engines are used to implement the expert system for a specific role:
	* **DifficultySelectionAI** is in charge of selecting what difficulty to use for a given minigame.
	* **MiniGameSelectionAI** is in charge of selecting what minigames to play during a given playsession, based on the player's position in the journey, the configured progression, and the current performance of the player.
	* **WordSelectionAI** is in charge of selecting what dictionary data a minigame should use based on player progression, player performance, and the minigame's requirements.  
	* **LogAI** handles the logging of play data at runtime.
	
 * Helper classes make interaction with the underlying Database straightforward:
    * **ScoreHelper** provides methods for storing, retrieving, and updating score values related to learning data.
	* **JourneyHelper** provides methods for retrieving and comparing data progression data from the database.
	* **WordHelper** provides method for retrieving and comparing dictionary data.

### Engines

#### Difficulty Selection Engine

This Engine is in charge of selecting what difficulty to use for a given minigame.

Mini games can be configured to be more or less difficult for the player.
 The difficulty value is related only to.
  See #TODO LINK MINIGAME CREATION DOCS#
 
The difficulty value depends on:
 * The age of the player. The game will be more difficult for older players.
 * The current performance of the player for the given minigame. The game is more difficult the better the player gets.
 * The current journey position of the player. The game is more difficult at advanced stages.
 
The weights of the different variables can be configured in **ConfigAI**.


The Difficulty Selection Engine is accessed through **TeacherAI.GetCurrentDifficulty(MiniGameCode miniGameCode)**.
 This is called by the **MiniGame Launcher** beore loading a specific minigame
  and assigned to the minigame's Game Configuration class.
 
##### Code Flow
 
 * **MiniGameSelectionAI** is in charge of selecting what minigames to play during a given playsession, based on the player's position in the journey, the configured progression, and the current performance of the player.
	* **WordSelectionAI** is in charge of selecting what dictionary data a minigame should use based on player progression, player performance, and the minigame's requirements.  
	* **LogAI** handles the logging of play data at runtime.
	
 
#### MiniGame Selection Engine

This Engine is in charge of selecting what minigame to use for a given play session.

Mini games can be configured to be more or less difficult for the player.
 The difficulty value is related only to 
 
The difficulty value depends on:
 * The age of the player. The game will be more difficult for older players.
 * The current performance of the player for the given minigame. The game is more difficult the better the player gets.
 * The current journey position of the player. The game is more difficult at advanced stages.
 
The weights of the different variables can be configured in **ConfigAI**.


##### Code Flow
 
### Minigame selection support

The Teacher System is designed so that many minigames can be supported with various requirements.
A procedure is needed to match what the teacher deems necessary for the current learning progression and what a given minigame can support.

As a simple example, the teacher cannot select minigames  


 *) A journey progression / minigame matching is provided to the teacher. This is used by the teacher to select minigames for a given playsession and make sure that a minigame can support at least some of the dictionary content for the learning objectives.
 
 *) The Teacher is first configured.
 

For this purpose, the 

When a new mingame is created, a **QuestionBuilder** must be assigned and configured in its Configuration class.
This is performed through the **IGameConfiguration.SetupBuilder()** method. The method must return an *IQuestionBuilder** that defines
 how Qoestion Packs are generated for the minigame.
 
A *QuestionBuilder* defines rules and requirements that the teacher should follow based on the minigame capabilities.
This includes:
	* the amount of Question Packs that a minigame instance can support
	* the type of data that the minigame wants to work with (Letters, Words, etc.)
	* The relationship among the different data objects (random words, all letters in a words, etc.)
	
A **QuestionBuilder** generates lists of *QuestionPacks* through the *CreateAllQuestionPacks** method.


Each *QuestionPack* contains ....

Several QuestionBuilders have been created to support common rules:
 * **AlphabetQuestionBuilder** provides all (or part of ) the letter of the alphabet.
 * **CommonLettersInWordBuilder** provides a 
 * refer to the API documentation for a complete list of question builders.

Note however that a new QuestionBuilder can be created for specific minigames, if the need arises.
 
the Teacher System can be configured by specifying rules




#### Configuration

The teacher can be configured by editing constants in the **ConfigAI** static class.

### Refactoring Notes

 * Helpers should probably belong to the DB, and not to the teacher.
 * LogAI should be a Helper
