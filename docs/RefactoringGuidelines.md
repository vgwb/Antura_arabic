2017 Refactoring Guidelines - EA4S - Antura and the Letters
=================

*Edits:*

<table>
  <tr>
    <td>11-01-2017</td>
    <td>Michele Pirovano</td>
  </tr>
  <tr>
    <td>24-01-2017</td>
    <td>Michele Pirovano</td>
  </tr>
</table>


These notes represent guidelines for refactoring the Antura code.

### Code annotations

The code is annotated to highlight pieces of code that should be refactored.
The following tags may be found throughout the code:

  * (refactor): the code should be logically refactored 
  * (obsolete): the code should be removed
  * (convention): the code should be refactored to match our conventions

### Documentation notes

The specific systems documentation files contain more general notes on refactoring partaining to that subsystem.
See the other .md files for more information.


### Refactoring notes: Important

	* Database and PlayerProfile use should be linked (no ease of access to either)
	* MiniJSON and MySQLLite should be moved to the Plugins (and check whether they can be used at all!) 
	* ModularFramework is included for some parts, but not for others. It may be better to remove it and merge what we need inside the core app instead.
	* Managers are scattered among many different folders and use widely different conventions. They should be refactored to show a common intention.
	* Many managers are static makeshift singleton-like classes. Better access should be provided.
	* Scene managers do not follow a single convention (IntroManager, HomeManager, etc). An abstract SceneManager can be added.
	* Helper / utility methods and classes use must be standardized throught the codebase. 
	* The application flow is confusing and there is no single logic throught the codebase for it. It should be standardized and placed in a NavigationManager (or something similar, which can also hold the state of the current play session and minigames selection)
	* MiniGameAPI, MiniGameLauncher and Debug minigame launch code need to be merged.

	
### Refactoring notes: New Language

These refactoring notes should be followed to prepare for supporting a new language.

	* Game and Core scripts should be correctly separated. For example, the Intro scene is using code from the _games/_common folders (IGameState for example). The contents of (_games/_common) could be placed in (_app/GamesCommonCode), so that the (_games) folder could be removed completely with no consequence.
	* A lot of the code is tied to the Arabic language. See Localization, the QuestionBuilders, and many minigames.

 
### Refactoring notes: Wishlist

	* The core code and the Teacher work only with QuestionProviders, so it would be better to just assume that all games will use them (and no other provider)
	* The word 'Data' is used interchangeably for 'LivingLetterData' and the database 'Data' (see LL_WordData versus Db.WordData). This creates confusion especially when handling learning data. This can be solved by making sure that LivingLetters data is converted into Views, and that the database's learning data is instead defined as LearningData (example: WordLearningData). Note also that the core should not use LL***Data, but ***Data. The view choice should be made by the minigame.
	* We need to standardize some nomenclature: "core" vs "minigames" / "minigame names" vs "minigame IDs" / "vote" vs "outcome" vs "grade" vs "score"

   