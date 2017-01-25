# Teacher System

The Teacher System represents the language teacher inside the application.
It is an Expert System that controls the learning progression of the player based on:
  * Player journey progression
  * Player learning performance
  * Expert configuration
  * Minigame support requirements
It is designed to be agnostic to the specific language and highly configurable in respect to mini-games.

The Teacher System can be found under the **EA4S.Teacher** namespace.

In this document, the **Teacher** is a shorthand for the Teacher System.
The person or group of persons that configure the Teacher is instead referred to
 singularly as the **expert**.


### Elements

The Teacher is composed of several elements:

**EA4S.TeacherAI** works is the entry point to the teacher functions. Helpers and Engines can be accessed from the **AppManager.Instance.Teacher** instance.

 * Engines are used to implement the expert system for a specific role:
	* **DifficultySelectionAI** is in charge of selecting what difficulty to use for a given minigame.
	* **MiniGameSelectionAI** is in charge of selecting what minigames to play during a given playsession
	* **WordSelectionAI** is in charge of selecting what dictionary data a minigame should use.  
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

##### Code Flow

The Difficulty Selection Engine is accessed through **TeacherAI.GetCurrentDifficulty(MiniGameCode miniGameCode)**.
 This is called by the **MiniGame Launcher** beore loading a specific minigame
  and assigned to the minigame's Game Configuration class.


#### MiniGame Selection Engine

This Engine is in charge of selecting what minigame to use for a given play session.

The selection of minigames depends on:
 * Whether there is a fixed sequence (*PlaySessionDataOrder.Sequence*) or a random one (*PlaySessionDataOrder.Random*). This is defined by the expert.
 * In case of a fixed sequence, in what order to select the minigames.
 * In case of a random sequence, the choice depends on:
	* What minigames are available at all in the application, as read from the database's **Db.MiniGameData**.
	* What minigames are supported / favoured by the current learning block, as read from the database's **Db.LearningBlockData**.
	* Whether the game was played recently or not (favour less played minigames).

The weights of the different variables can be configured in **ConfigAI**.


##### Code Flow

The MiniGame Selection Engine is accessed whenever a new play session start,
 through **TeacherAI.SelectMiniGamesForPlaySession()**.
 This is called by **TeacherAI.InitialiseCurrentPlaySession()**,
  triggered by the **MiniMap** script in the *Map scene* when a new
   play session is about to start.



#### Word Selection Engine

This Engine is in charge of selecting what dictionary data a minigame should use in a given play session.

 based on player progression, player performance, and the minigame's requirements

The selection of dictionary data is a two-stage process.

The first stage filters all learning data based on:
 * The selected minigame's learning rules and requirements (performed through a configured **QuestionBuilder**)
 * The current journey progression, as read from the journey data in the database (LearningBlocks and Stages)
 * Previously selected data for the same Question Builder.

The first stage is needed to make sure that all data to be selected matches
 the player's knowledge.

A second stage selects the learning data using weighted selection,
 with weights based on:
 * The learning score of the dictionary data. Lower scores will prompt a given data entry to appear more.
 * The time since the player last saw that dictionary data. Entries that have not appeared for a while are preferred.
 * The focus of the current learning block.

 * In case of a random sequence, the choice depends on:
	* What minigames are available at all in the application, as read from the database's **Db.MiniGameData**.
	* What minigames are supported / favoured by the current learning block, as read from the database's **Db.LearningBlockData**.
	* Whether the game was played recently or not (favour less played minigames).

The weights of the different variables can be configured in **ConfigAI**.



### Minigame dictionary data selection

The Teacher System is designed so that many minigames, with various requirements, can be supported through a single interface.
A procedure is needed to match what the teacher deems necessary for the current learning progression and what a given minigame can support.

*As an example, the teacher cannot select minigames that cannot show colors, if the learning block requires color words to be learned.*

This is accomplished through two methods:

 * A journey progression / minigame matching is provided to the teacher through the database's PlaySessionData ntries. This is used by the teacher to select minigames for a given playsession and make sure that a minigame can support at least some of the dictionary content for the learning objectives.

 *  Whenever a specificminigame is selected, the Teacher is configured to generate dictionary data that can be supported by the minigame. This is handled by the **Question Builders**

**Question Builders** define rules and requirements that the teacher should follow  when generating **Question Packs**.
A **Question Pack** contains the dictionary data that the minigame should use for its gameplay under the form of a question, a set of correct answers, and a set of wrong answers (refer to the [Data Flow documentation](DataFlow.md)).
Whenever required by the Teacher, a **IQuestionBuilder** must be returned by the minigame's configuration class. This is performed through  **IGameConfiguration.SetupBuilder()**.

A **Question Builder** thus defines:
	* the amount of Question Packs that a minigame instance can support
	* the type of dictionary data that the minigame wants to work with (Letters, Words, etc.)
	* The relationship among the different data objects (random words, all letters in a words, etc.)

The chosen **Question Builder** implementation defines what kind of dictionary data (letters, words, etc..) will be included inside the **Question Packs**. For example, a **RandomLettersQuestionBuilder** generates packs that contain letters as question and answers, while a **CommonLettersInWordBuilder** uses letters for its questions and words as the answers.

Several pre-created **Question Builders** have been created to support common rules:
 * **AlphabetQuestionBuilder** creates a pack made of part of the alphabet.
 * **EmptyQuestionBuilder** creates fake packs, it can be used during development when the game is not yet ready to accept questions packs.
 * **LettersByXXXBuilder** creates packs that containletters that should be recognized based on some of their properties (this can, for example, be the number, or the type).
 * **WordsByXXXBuilder** creates packs that contain words that should be recognized based on some of their properties (this can, for example, be the number, or the type).
 * **RandomLettersQuestionBuilder** creates packs that contain random letters.
 * **RandomWordsQuestionBuilder** creates packs that contain random words.
 * **LettersInWordQuestionBuilder** creates packs with a word and all letters contained in that word.
 * **CommonLettersInWordBuilder** creates packs with some letters and words that share those letters.
 * **WordsInPhraseQuestionBuilder** creates packs with a phrase and all words contained in that phrase.
Refer to the API documentation for a complete list of question builders.

Note however that a new QuestionBuilder can be created for specific minigame, if the need arises.


#### Question Builder configuration

**Question Builders** are designed to be flexible, so that minigames can safely configure them with their specific requirements.
In its **SetupBuilder()** call, a minigame's configuration class may configure the **Question Builder** by specifying a set of parameters.

The parameters are different for each Question Builder, but some common parameters include:
 * **nPacks**: the number of packs that should be generated using this builder for the minigame play session.
 * **nCorrect**: the number of correct answers to generate.
 * **nWrong**: the number of wrong answers to generate.
 * **parameters**: an instance of **QuestionBuilderParameters** that defines additional common parameters.

A **QuestionBuilderParameters** defines additional common parameters and includes:
 * **PackListHistory correctChoicesHistory**: the logic to follow for correct answers when generating multiple packs (see below). Defaults to *RepeatWhenFull*.
 * **PackListHistory wrongChoicesHistory**: the logic to follow for wrong answers when generating multiple packs (see below). Defaults to *RepeatWhenFull*.
 * **bool useJourneyForCorrect**: whether the dictionary data available for use as correct answers should be limited to what the player has already unlocked. Defaults to *true*.
 * **bool useJourneyForWrong**: whether the dictionary data available for use as wrong answers should be limited to what the player has already unlocked. Defaults to *false*.
 * **SelectionSeverity correctSeverity**: the logic to use when selecting correct answers when data is not enough (see below). Defaults to *MayRepeatIfNotEnough*.
 * **SelectionSeverity wrongSeverity**: the logic to use when selecting wrong answers when data is not enough (see below).  Defaults to *MayRepeatIfNotEnough*.
 * **LetterFilters letterFilters**: additional language-specific rules that define if some letters should be left out when performing the selection. Refer to the API for further details.
 * **WordFilters wordFilters**: additional language-specific rules that define if some words should be left out when performing the selection. Refer to the API for further details.
 * **PhraseFilters phraseFilters**: additional language-specific rules that define if some phrases should be left out when performing the selection. Refer to the API for further details.

**PackListHistory** may take one of the following values:
 * **NoFilter**:  Multiple packs in the game have no influence one over the other
 * **ForceAllDifferent**: Makes sure that multiple packs in a list do not contain the same values
 * **RepeatWhenFull**:  Try to make sure that multiple packs have not the same values, fallback to NoFilter if we cannot get enough data
 * **SkipPacks**:  If we cannot find enough data, reduce the number of packs to be generated

**SelectionSeverity** may take one of the following values:
 * **AsManyAsPossible**: If possible, the given number of data values is asked for, or less if there are not enough.
 * **AllRequired**: The given number of data values is required. Error if it is not reached.
 * **MayRepeatIfNotEnough**: May repeat the same values if not enough values are found



#### Question Builder implementation

Whenever a new minigame instance is started, the Teacher retrieves the minigame's **IQuestionBuilder** through **SetupBuilder()**, then  generates a **List\<QuestionPacks\>** through the builder's **CreateAllQuestionPacks()** method.

**CreateAllQuestionPacks()** is implemented differently for each Question Builder, but in general it just generatas a sequential list of packs through multiple casll to **CreateSingleQuestionPackData()**.

**CreateSingleQuestionPackData()** defines the actual logic for generating the Question Pack and is thus the core of the Question Builder. This method ends with a call to **QuestionPackData.Create(question, correctAnswers, wrongAnswers)** which creates the final pack from different sets of **IConvertibleToLivingLetterData** instances, which define the dictionary data that should be used and for what roles. For the current system, this can be either a **LetterData**, a **WordData**, or a **PhraseData**.

The **Question Builder** will thus generate the question, the correct answers, and the wrong answers using calls to **WordSelectionAI.SelectData()** and by specifying what data it wants to use through the method's arguments:
 * **System.Func\<List\<T\>\> builderSelectionFunction** is a delegate that defines what data to work from based on the question builder logic. For example, for the **RandomLettersQuestionBuilder** this is all the available letters given the current letter filters. This is usually performed through the **WordHelper** class methods, which help in retrieving dictionary data from the database based on specific rules.
 * **SelectionParameters selectionParams** is an internal structure that defines parameters for filtering and selecting learning data based on the minigame requirements. This is built from the **QuestionBuilderPamaters** instance defined above.


#### Configuration

The teacher can be configured by editing constants in the **ConfigAI** static class.

### Refactoring Notes

 * Helpers should probably belong to the DB, and not to the teacher.
 * LogAI should be a Helper
