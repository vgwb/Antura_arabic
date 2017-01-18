2017 Refactoring Guidelines - EA4S - Antura and the Letters
=================

*Edits:*

<table>
  <tr>
    <td>11-01-2017</td>
    <td>Michele Pirovano</td>
  </tr>
</table>


These notes attempt to provide notes and guidelines for refactoring the Antura code.

### Code annotations

The code is annotated to highlight pieces of code that should be refactored.
The following tags may be found throughout the code:

  * (refactor): the code should be logically refactored 
  * (obsolete): the code should be removed
  * (convention): the code should be refactored to match our conventions

### Documentation notes

The specific systems documentation files contain more general notes on refactoring partaining to that subsystem.
See the .md files for more information.

### General refactoring notes

	* Codebase
		* Game and Core scripts should be correctly separated. The Intro scene is using code from the _games/_common folders (IGameState for example). The contents of (_games/_common) could be placed in (_app/GamesCommonCode), so that the (_games) folder could be removed completely with no consequence.
		* There are different implementations of state machines in the codebase (see IGameState versus StateMachineBehaviour)
		* Database and PlayerProfile use should be linked (no ease of access to either)
		* The core code and the Teacher work only with QuestionProviders, so it would be better to just assume that all games will use them (and no other provider)
		* A lot of the code is tied to the Arabic language. See Localization, the QuestionBuilders, and many minigames.
		* Part of the core depends on what minigames are available. The core should work without minigames. See SRDebug, MiniGameAPI, and others.
		* ModularFramework is included for some parts, but not for others. We should decide whether to keep it or not. It may be better to remove it and merge what we need inside the core app instead.
		* First Contact code is scattered throughout the codebase
		
	* Nomenclature
		* The word 'Data' is used interchangeably for 'LivingLetterData' and the database 'Data' (see LL_WordData versus Db.WordData). This creates confusion especially when handling learning data. This can be solved by making sure that LivingLetters data is converted into Views, and that the database's learning data is instead defined as LearningData (example: WordLearningData). Note also that the core should not use LL***Data, but ***Data. The view choice should be made by the minigame.
		* To avoid clashes, we should have a **EA4S.Minigames.XXX* namespace for each minigame.
		* We need to standardize some nomenclature: "core" vs "minigames" / "minigame names" vs "minigame IDs" / "vote" vs "outcome" vs "grade" vs "score"

	* Managers
		* Managers are scattered among many different folders and use widely different conventions. They should be refactored to show a common intention.
		* Randomization methods and classes use must be standardized throught the codebase. 
		* In general, static Helpers and Utilities should be better organized and standardized.
		* Scene managers do not follow a single convention (IntroManager, HomeManager, etc). An abstract SceneManager can be added.
		* Many managers are static makeshift singleton-like classes. Better access should be provided.

	* App flow
		* The application flow is confusing and there is no single logic throught the codebase for it. It should be standardized and placed in a NavigationManager (or something similar, which can also hold the state of the current play session and minigames selection)
		* The NavigationManager should be the sole responsible for navigation (and it is not).
		* We need a PlaySessionManager that holds the data returned by the TeacherAI whenever a new play session is started (CurrentMiniGames, Play Session State, minigame results, etc.)
		* MiniGameAPI, MiniGameLauncher and Debug minigame launch code need to be merged.

   
### Specific refactoring notes

   * Rewards are not in the PlayerProfile but will be merged with the Database code.
