# Teacher System

* TOC
{:toc}

The Teacher System represents the teacher of the language inside the application.
It is implemented as an Expert System that controls the learning progression of the player based on:
- Player journey progression
- Player learning performance
- Expert configuration
- Minigame support requirements
It is designed to be agnostic to the specific language and highly configurable in respect to mini-games.

In this document, the **Teacher** is a shorthand for the *Teacher System*.
The person or group of persons that configure the Teacher is instead referred to singularly as the **Expert**.

All code related to the Teacher can be found under the **Antura.Teacher** namespace.

## Classes

The Teacher is composed of several interrelated classes, joined by a composition relationship.

**Antura.TeacherAI** works as the entry point to the teacher functions.
All of the Teacher functionalities should be accessed through a singleton instance of TeacherAI: **AppManager.I.Teacher**.
Engines and Helpers are sub-elements of the Teacher and reside in their own classes.

- **Engine** classes are used to implement the expert system for a specific role:
    - **Difficulty Selection AI** is in charge of selecting what difficulty to use for a given minigame.
    - **MiniGame Selection AI** is in charge of selecting what minigames to play during a given playsession
    - **Vocabulary Selection AI** is in charge of selecting what vocabulary data a minigame should use.  
    - **Log AI ** handles the logic behind the logging of data at runtime.

- **Helper** classes expose interfaces for easier interaction with the Teacher and the app's Database:
    - **Score Helper** provides methods for storing, retrieving, and updating score values related to the learning progression.
    - **Journey Helper** provides methods for retrieving and comparing progression data from the database.
    - **Vocabulary Helper** provides method for retrieving and comparing vocabulary data.

## General Configuration

In general, the Teacher can be configured by the Expert through two main mechanisms:

- By editing the database contents through an Excel file, the Teacher can define the contents of *Play Sessions*, *Learning Blocks*, and the whole vocabulary.
Please refer to the following sections for specific details.
- By editing a set of weights, the Expert can further configure the logic inside the Teacher.
This is implemented as constants in the **ConfigAI** static class.



## Difficulty Selection Engine

Minigames can be configured to be more or less difficult (i.e. challanging) for the player.
This *Engine* is in charge of selecting what difficulty to use for a given minigame session.
Difficulty is defined as a float value in the range [0,1], with 0 being the easiest, and 1 being the hardest.

Note that the difficulty value is related only to the specific minigame (and thus not related to other minigames)
  and is implemented in the minigame's logic, following the above general rule.
 See the [Minigames](Minigames.md) documentation for further details.

The difficulty value logic depends on several variables:
- The current performance of the player for the given minigame. The game is more difficult the better the player gets. The current performance starts from zero and may rise up to 1.
 Failing a minigame (score 0) diminishes the performance. Completing  a minigame with a score of 2 or more increases it. Completing it with a score of 1 is ininfluent (steady performance).
- **REMOVED:** The age of the player. The game will be more difficult for older players.
- **REMOVED:** The current journey position of the player. The game is more difficult at advanced stages.

The weight contributions of the different variables can be statically configured in **ConfigAI**.

*Note that whenever a minigame is encountered for the first time its difficulty is forced to be zero (0), so to work as a tutorial session.*

The Difficulty Selection Engine is accessed through **TeacherAI.GetCurrentDifficulty(MiniGameCode miniGameCode)**.
This is called by the **MiniGame Launcher** before loading a specific minigame and assigned to the minigame's Game Configuration class.
The difficulty value in the Game Configuration is then read by the minigame and used for gameplay.

## MiniGame Selection Engine

This *Engine* is in charge of selecting what minigames to use for a given play session.
 The Teacher may call this Engine with the number of requested minigames as a parameter.

The logic for selection depends on two main mechanism:
filtering based on Play Sessions,
  and weighing based on past performance.

### Filtering step
As a first step, the **Play Session Data** table in the database defines what minigames can be selected for a given *Play Session*.
The Teacher makes sure to use this information when filtering minigames.
Minigames which have a zero value (or no value) in the Play Session table are filtered out.

> In case of fixed sequence (*PlaySessionDataOrder.Sequence* set in the **Play Session Data** table), the numbers placed in the Play Session for each minigame are used to define the sequence. This is used for the initial Play Sessions to force an order on minigames.

### Weighted selection step
The second step performs a weighing over the available minigames, based on several variables:

- **Manual weights**: in case of random sequence (*PlaySessionDataOrder.Random* set in the **Play Session Data** table), the numbers placed in the Play Session for each minigame are used as weights. This produces a [0,1] value for each available minigame.
- **Recent play weight**: the number of days since the last time the minigame was played is taken into account to produce a value in the [0,1] range for each available minigame, favouring minigames that have not been played for a while.

The resulting weights are added (each with a configurable contribution weight) and used in a round of roulette selection.
The contribution weights, as well as the number of days for the *recent play* logic, can be configured in **ConfigAI**.

### Code
The MiniGame Selection Engine is accessed through the method **PerformSelection(string playSessionId, int numberToSelect)** that returns a  **List<MiniGameData>** containing the selected minigames.

Whenever a new play session start, this is called by the Teacher through **TeacherAI.InitialiseCurrentPlaySession()**. triggered by the **MiniMap** script in the *Map scene* when a new  play session is about to start.

## Vocabulary Selection Engine

This *Engine* is in charge of selecting what vocabulary data a minigame should use in a given play session, based on player progression, player performance, and the minigame's requirements.
The selected vocabulary data will be used as the *focus* for each minigame session (i.e. as questions to be posed, or as correct answers to find).

The logic for selection depends on three main mechanism: two fltering steps and a weighted selection step.

### Filtering step
The first step is needed to make sure that all data to be selected matches the player's knowledge.
The step filters all vocabulary data in the database based on:

- **Minigame requirements**:
 The selected minigame  usually works with a subset of the whole data.
 For example, many minigames work with letters, but not with words. rules and requirements may.
 This is performed through a configured **QuestionBuilder** (refer to the section below).
- **Data Variability**: Previously selected data for the same minigame instance may be filtered out to increase variability.
The focus of this filter is to make sure that data is not repeated too much, avoiding repeating the same data in succession in the same play sessions.
 This is performed through two phases. An *intra-pack* phase avoids repeating the same data in the same question pack, if possible.
  This can be configured per minigame with the **SelectionSeverity** parameters.  
  An *inter-pack* phase avoids repeating the same data in the same list of question packs, if possible.
  This can be configured per minigame with the **PackListHistory** parameters.  
- **Journey progression**:
The current journey progression (i.e. the position in the Stage/LearningBlock/PlaySession map) is taken into account, filtering out data that is too difficult for the current learning progression (i.e. it is not available up to the current Learning Block).

### Journey priority step
After the first step, we can be sure that *all* selected data can be played by the selected minigame and is suitable to the player's knowledge.
The Teacher performs a second step by weighing vocabulary entries based on the learning block focus.
This is used, if possible, as a strict filter.
Data that belongs to the *current* learning block is given large priority, making sure that all data of the learning block is used if available.
If not enough data is available for the current learning block, data from the previous play sessions (going back in the progression) is selected and given a weight based on linear distance from the current learning block.

### Weighted selection step
As a final step, the Teacher selects the actual vocabulary entries using weighted selection, with weights based on:

- **Learning Score**: Lower scores for a vocabulary entry (as retrieved from **VocabularyScoreData** instances) will prompt a given data entry to appear more.
- **Recent Play**: The time since the player last saw a specific vocabulary entry. Entries that have not appeared for a while are preferred.
- **Learning Block Focus**: Learning blocks closer to the current one receive a higher weight. (see the previous step)

The final weight for each vocabulary entry is computed as a weighted sum of these variable.
Each variable is assigned a *contribution weight*, which can be manually configured in **ConfigAI**.
If a vocabulary entry has a too lower weight, a minimum weight can however be assigned, defined as **ConfigAI.data_minimumTotalWeight**.

### Inter-pack ordering

At last, after data has been selected, an optional ordering may be enforced. This can be useful to make sure that the vocabulary data entires of a given minigame play session appear during play in order of difficulty
The ordering is performed on the **intrinsic difficulty** of each vocabulary data entry.

### Wrong answers

Many minigames require vocabulary entries that function as wrong answers, which need not be selected.
These entries need not be as strict as the *in-focus* (i.e. correct) answers, as they are not the primary focus of the minigame.
However, the logic for selection is similar to the selection of correct data, with the following differences:
- **Loose Journey Progression**: Wrong data may be selected also among all data of the current stage, regardless of the reached learning block.
This changes the filtering steps so to avoid filtering out all data of the current stage and so to avoid prioritizing the current learning block data.

Note that minigames may have specific requirements also for wrong answers (see the next section).


## Minigame requirements

The Teacher System is designed so that many minigames, with various requirements, can be supported through a single interface.
A procedure is needed to match what the teacher deems necessary for the current learning progression and what a given minigame can support.

*As an example, the teacher cannot select minigames that cannot show colors, if the learning block requires color words to be learned.*

This is accomplished through two methods:

- A journey progression / minigame matching is provided to the teacher through the database's PlaySessionData ntries. This is used by the teacher to select minigames for a given playsession and make sure that a minigame can support at least some of the dictionary content for the learning objectives.
- Whenever a specific minigame is selected, the Teacher is configured to generate vocabulary data that can be supported by the minigame. This is handled by the **Question Builders**

**Question Builders** define rules and requirements that the teacher should follow  when generating **Question Packs**.
A **Question Pack** contains the dictionary data that the minigame should use for its gameplay under the form of a question, a set of correct answers, and a set of wrong answers (refer to the [Data Flow documentation](DataFlow.md)).
Whenever required by the Teacher, a **IQuestionBuilder** must be returned by the minigame's configuration class. This is performed through  **IGameConfiguration.SetupBuilder()**.

A **Question Builder** thus defines:
- the amount of Question Packs that a minigame instance can support
- the type of dictionary data that the minigame wants to work with (Letters, Words, etc.)
- The relationship among the different data objects (random words, all letters in a words, etc.)

The chosen **Question Builder** implementation defines what kind of dictionary data (letters, words, etc..) will be included inside the **Question Packs**. For example, a **RandomLettersQuestionBuilder** generates packs that contain letters as question and answers, while a **CommonLettersInWordBuilder** uses letters for its questions and words as the answers.


Several pre-created **Question Builders** have been created to support common rules:
- **AlphabetQuestionBuilder** creates a pack made of part of the alphabet.
- **EmptyQuestionBuilder** creates fake packs, it can be used during development when the game is not yet ready to accept questions packs.
- **LettersByXXXBuilder** creates packs that containletters that should be recognized based on some of their properties (this can, for example, be the number, or the type).
- **WordsByXXXBuilder** creates packs that contain words that should be recognized based on some of their properties (this can, for example, be the number, or the type).
- **RandomLettersQuestionBuilder** creates packs that contain random letters.
- **RandomWordsQuestionBuilder** creates packs that contain random words.
- **LettersInWordQuestionBuilder** creates packs with a word and all letters contained in that word.
- **CommonLettersInWordBuilder** creates packs with some letters and words that share those letters.
- **WordsInPhraseQuestionBuilder** creates packs with a phrase and all words contained in that phrase.

Refer to the API documentation for a complete list of question builders.

Note however that a new QuestionBuilder can be created for specific minigame, if the need arises.


## Question Builder configuration

**Question Builders** are designed to be flexible, so that minigames can safely configure them with their specific requirements.
In its **SetupBuilder()** call, a minigame's configuration class may configure the **Question Builder** by specifying a set of parameters.

The parameters are different for each Question Builder, but some common parameters include:
- **nPacks**: the number of packs that should be generated using this builder for the minigame play session.
- **nCorrect**: the number of correct answers to generate.
- **nWrong**: the number of wrong answers to generate.
- **parameters**: an instance of **QuestionBuilderParameters** that defines additional common parameters.

A **QuestionBuilderParameters** defines additional common parameters and includes:
- **PackListHistory correctChoicesHistory**: the logic to follow for correct answers when generating multiple packs (see below). Defaults to *RepeatWhenFull*.
- **PackListHistory wrongChoicesHistory**: the logic to follow for wrong answers when generating multiple packs (see below). Defaults to *RepeatWhenFull*.
- **bool useJourneyForCorrect**: whether the dictionary data available for use as correct answers should be limited to what the player has already unlocked. Defaults to *true*.
- **bool useJourneyForWrong**: whether the dictionary data available for use as wrong answers should be limited to what the player has already unlocked. Defaults to *false*.
- **SelectionSeverity correctSeverity**: the logic to use when selecting correct answers when data is not enough (see below). Defaults to *MayRepeatIfNotEnough*.
- **SelectionSeverity wrongSeverity**: the logic to use when selecting wrong answers when data is not enough (see below).  Defaults to *MayRepeatIfNotEnough*.
- **LetterFilters letterFilters**: additional language-specific rules that define if some letters should be left out when performing the selection. Refer to the API for further details.
- **WordFilters wordFilters**: additional language-specific rules that define if some words should be left out when performing the selection. Refer to the API for further details.
- **PhraseFilters phraseFilters**: additional language-specific rules that define if some phrases should be left out when performing the selection. Refer to the API for further details.

**PackListHistory** may take one of the following values:
- **NoFilter**:  Multiple packs in the game have no influence one over the other
- **ForceAllDifferent**: Makes sure that multiple packs in a list do not contain the same values
- **RepeatWhenFull**:  Try to make sure that multiple packs have not the same values, fallback to NoFilter if we cannot get enough data
- **SkipPacks**:  If we cannot find enough data, reduce the number of packs to be generated

**SelectionSeverity** may take one of the following values:
- **AsManyAsPossible**: If possible, the given number of data values is asked for, or less if there are not enough.
- **AllRequired**: The given number of data values is required. Error if it is not reached.
- **MayRepeatIfNotEnough**: May repeat the same values if not enough values are found



## Question Builder implementation

Whenever a new minigame instance is started, the Teacher retrieves the minigame's **IQuestionBuilder** through **SetupBuilder()**, then  generates a **List\<QuestionPacks\>** through the builder's **CreateAllQuestionPacks()** method.

**CreateAllQuestionPacks()** is implemented differently for each Question Builder, but in general it just generatas a sequential list of packs through multiple casll to **CreateSingleQuestionPackData()**.

**CreateSingleQuestionPackData()** defines the actual logic for generating the Question Pack and is thus the core of the Question Builder. This method ends with a call to **QuestionPackData.Create(question, correctAnswers, wrongAnswers)** which creates the final pack from different sets of **IConvertibleToLivingLetterData** instances, which define the dictionary data that should be used and for what roles. For the current system, this can be either a **LetterData**, a **WordData**, or a **PhraseData**.

The **Question Builder** will thus generate the question, the correct answers, and the wrong answers using calls to **WordSelectionAI.SelectData()** and by specifying what data it wants to use through the method's arguments:
- **System.Func\<List\<T\>\> builderSelectionFunction** is a delegate that defines what data to work from based on the question builder logic. For example, for the **RandomLettersQuestionBuilder** this is all the available letters given the current letter filters. This is usually performed through the **WordHelper** class methods, which help in retrieving dictionary data from the database based on specific rules.
- **SelectionParameters selectionParams** is an internal structure that defines parameters for filtering and selecting learning data based on the minigame requirements. This is built from the **QuestionBuilderPamaters** instance defined above.


## Configuration

The learning progression can be configured through two main sources:

- Editing the PlaySessionData and LearningBlockData tables in the static database. These define the progression of the learning content from lower to higher stages.
 The learning content should be distributed so that harder content appears at higher stages.
 The system will make sure to use this information when **filtering** dictionary content.
*Example: content at higher learning blocks cannot appear at lower play sessions*

- Editing the ConfigAI weight constants. These values define how much weight to give to each rule when selecting dictionary content.
The system will make sure to use this information when **weighing** dictionary content.
*Example: content that has been seen recently may appear less often*


## Refactoring Notes

- Helpers should probably belong to the DB, and not to the teacher.
- LogAI should be a Helper
