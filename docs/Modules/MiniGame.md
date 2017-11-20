# Antura MiniGames Interface

* TOC
{:toc}

In this document, we describe the programming interface that is used
by all the MiniGames in the Antura project.

The purpose of the interface is to expose to MiniGames a unified and simplified
way to access core functionalities, and to define how MiniGames are launched and configured,
including the dataflow from the content (e.g. question sets) database towards each MiniGame.

## Creating a new MiniGame project

All the MiniGames are in the **_games** directory of the Antura’s Unity project.

Instead of starting your own MiniGame from scratch, you can use the provided game template:

1. Make a copy of the **\_gametemplate** directory (which is in the **\_games** directory);

2. Rename it using the name of your game, e.g. *MyNewMasterpiece* and put it under the **_games** directory;

3. In the folder (*MyNewMasterpiece*) you will find a set of files and subfolders. You must find and rename "TemplateGame.cs" and “TemplateConfiguration.cs” into “MyNewMasterpieceGame.cs” and “MyNewMasterpieceConfiguration.cs”, according to the game name you chose;

4. Edit these source files, and change the class names in order to comply with this name change, for example:
    1. **Antura.Template** namespace should become *Antura.MyNewMasterpiece*
    2. **TemplateGame** class should become *MyNewMasterpieceGame*
    3. **TemplateConfiguration** class should become *MyNewMasterpieceConfiguration*

***note: MiniGame namespaces may change***


# Making a MiniGame accessible from the core application

A MiniGame does not exist in a vacuum. The core app needs to know of its existance and how to handle it.
For this purpose, the following should be performed:
- a new entry named *MyNewMasterpiece* should be added to the `MiniGameCode` enumerator
- the database should be updated to support the new MiniGame. (refer to the [Database](Database.md) doc). This requires:
  - adding a new **MiniGameData** entry
  - updating the table of **PlaySessionData** to support the new MiniGame at a given learning block
- `MiniGameAPI.ConfigureMiniGame()` should be updated to provide the correct configuration for the new MiniGame code.
- `LogAI.GetLearnRules(MiniGameCode code)` should be updated to provide the correct logging rules for the new MiniGame.

***note: the above requirements are bound to change as it couples MiniGames with the core codebase***

# Game Structure

Here we describe the software architecture that should be followed by your MiniGames.
If you copied the MiniGame template, the main classes are already partially implemented to be compliant with such architecture.

The MiniGame main class should extend **MiniGame** class, inside the Antura namespace.
Such class is realized using the [*State Pattern*](https://en.wikipedia.org/wiki/State_pattern)
The game is divided in an arbitrary number of states, for example:

- *IntroductionState*, a state in which you present the game e.g. through an animation

- *ProblemState*, a state in which you describe which kind of problem the player should solve;

- *PlayState*, a state in which you process player’s input and implement the actual game;

- *ResultState*, a state in which you show the result (e.g. the score) of the player.

Such list is just an example of what states a game could have, it’s up to the MiniGame developer to understand how many and what kind of states he should implement.

At a given time, only one state is active and running. When it’s running, `Update()` and `UpdatePhysics()` are called on the state in each frame. UpdatePhysics is the equivalent of Unity3D’s FixedUpdate.


All state objects are instanced in the game class, which exposes them as public fields.
Each state must have a reference to the MiniGame main class, that you could pass through the constructor.

In this way, when you want to switch game state, you can call:

```C#
game.SetCurrentState(game.NewStateName);
```

each time a state transition is completed, the `ExitState()` method is called on the previous state, and `EnterState()` is called on the next state.

The purpose of these methods is to process things like setting up the scene graphics, resetting timers, showing/hiding panels, etc.

# Ending the Game

When the game is over, call the method EndGame of the {GameName}Game class:

```C#
game.EndGame(howMuchStars, gameScore);
```

`howMuchStars` should be in the range (0 to 3, included);

`gameScore` is game-specific (could be 0, if not defined for that MiniGame)

In this way, the game will automatically switch to a special OutcomeState, and show how many stars were obtained, then quit the game.



# Game Configuration

Each game folder should have two main folders for scripts:

*_scripts*

*_configurationscripts*

all the game-related scripts, should be placed inside the **_scripts** folder;
**_configurationscripts** is a service folder used to define game specific classes that help the in communicating with the core codebase.

The first requirement of your MiniGame is to have a game configuration script.
If you want to see how a configuration class is made, you could just copy it from the template directory.

The {GameName}Configuration.cs defines how a MiniGame is configured by the app, and provides the MiniGame some useful interfaces.



# Minigame Variations

Each MiniGame is created inside its own scene and namespace.
Usually, the core application refers to the MiniGame using a 1-to-1 relationship, detailed by the **MiniGameCode** that represents the MiniGame in the core application.

However, sometimes it is useful to have a single scene support multiple instances of MiniGames with slight variations.
These MiniGames are called *variations*.
Variations are transparent to the core application (they are considered different MiniGames), but multiple variations point to the same MiniGame scene.

Variations are specified in the specific MiniGame's configuration code, if needed.

## Game Difficulty

The game configuration provides a difficulty level.
This difficulty value is provided by the Teacher and can be accessed as:

```C#
float difficulty = {GameName}Configuration.Instance.Difficulty;
```

The game difficulty is expressed as a float in the range [0, 1], meaning 0 : easiest, 1 : hardest.

How such difficulty level is implemented must be defined by the MiniGame developer.

Possible choices for difficulty can be:
- Play speed
- Aiming precision
- Rhythm
- Short-term memory

For example, the MiniGame can linearly control the game speed based on difficulty:

```C#
_speed = normalSpeed - difficulty;
```

or, it could have a finite set of parameters configurations, based on difficulty interval:

```C#
if (difficulty < 0.333f)
{
  // configure game for "easy"
}

else if (difficulty < 0.666f)
{
  // configure game for "medium"
}
else
{
  // configure game for "hard"
}
```

In this case, please configure a set of at least 5 different configurations (**very easy, easy, medium, hard, very hard**).

**Note that this difficulty must however be related to playskill, not to the learning content.**
This is because learning difficulty is already taken care of by the Teacher generating suitable Question Packs.

## Accessing core functionalities

When you need to access a core feature in any part of your game {GameName}, you do it through the **game context**:

```C#
var context = {GameName}Configuration.Instance.Context;
```

Such object implements the `IGameContext` interface.

For example, to show the popup widget (that is, a large dialog with some text inside it),

you call:

```C#
context.GetPopupWidget().Show(callback, text, isArabic);
```

or, to play the game music:

```C#
context.GetAudioManager().PlayMusic(Music.MainTheme);
```

To have a list of all the possible functionalities that you could use, take a look into the `IGameContext` source.

## Audio Manager

The Audio Manager provides some simple methods to play in-game audio, for example:

```C#
IAudioSource PlaySound(Sfx sfx);
IAudioSource PlayLetterData(ILivingLetterData id);
```

It behaves in a similar way to Unity’s AudioSource.
It exposes some properties and methods like:

* Pitch
* Volume
* IsPlaying()
* Pause() / Stop() / Play()

## Working with the UI

When you are working on your MiniGame, you do not need to know what prefab are used for the UI or how it is implemented.
The game context `IGameContext` will provide you a set of interfaces to widgets that you can call from your game code.

For example:

```C#
ISubtitlesWidget GetSubtitleWidget();
IStarsWidget GetStarsWidget();
IPopupWidget GetPopupWidget();
ICheckmarkWidget GetCheckmarkWidget();
```

## Retrieving dictionary content from Core

Usually, a MiniGame needs dictionary content to be passed directly from the core code.
For instance, some MiniGames need a set of words, that are chosen by the core based on the current game world, or depending on past play history.

Such content is passed to the game using the {GameName}Configuration.cs class by core programmers to MiniGames programmers, through a `QuestionProvider` interface.

The `IQuestionProvider` interface exposes the following methods:

* `IQuestionPack GetNextQuestion();`
* `string GetDescription();`

Its purpose is to provide a stream of objects that implements the interface `IQuestionPack`, a very general abstraction for a learning question which includes letters, words and images as fundamental parts.
gLetterData* questions and answers.

What is returned as `IQuestionPack`, will define a package containing `ILivingLetterData` instances structured as:

- a set of **questions**;
- a set of **wrong answers**;
- a set of **correct answers**;

All data implements the `ILivingLetterData` interface and can thus be displayed throguh a Living Letter.
The data can be one of the following:

- **LL_LetterData** contains a Db.LetterData in text form (a single letter)
- **LL_WordData** contains a Db.WordData in text form
- **LL_ImageData** contains a Db.WordData in image form (a drawing or picture)
- **LL_PhraseData** contains a Db.PhraseData in text form


What follows is a list of possible examples:

- The game shows a word, you must select only the letters which are part of that word

   - The question is the word;
   - The correct answers are the set of letters which constitutes the word;
   - The wrong answers are a set of random letters not included in the correct set;

- The game shows a image, you must select the word W representing that image

   - The question is the image
   - The correct answers is a set with only one element, that’s the word W
   - The wrong answers is a set of random words, different from W

- The game shows a letter with its dots/signs hidden, the player hear its sound and should understand which is the correct sign that should be placed on the letter.

   - The question is the letter (the game should understand how to hide its signs/dots)
   - The correct answers is the set made just by the correct sign/dot
   - The wrong answers are all the other possible signs/dots

# Question Builder

Each MiniGame (or MiniGame variation) requires question packs in different forms and this is defined by implementing the method `SetupBuilder()` inside the Game Configuration, which returns an object implementing the *IQuestionBuilder* interface.

The **IQuestionBuilder** defines the learning rules and requirements for the current MiniGame variation and must be correctly setup and configured.
The Question Builder will generate the correct **Question Packs** for a given MiniGame instance.

The MiniGame developer can choose from a set of question builders that the Teacher can support. Refer to the Teacher documentation for further details.

# Generating content for test purposes

When developing a MiniGame, test dictionary data, like letters or words, is needed to test the gameplay.
To do so, just define a default **Question Provider** in your **Game Configuration** class as a custom provider, in the Game Configuration constructor.

For example:

```C#
private {GameName}Configuration()
{
  Questions = new MyQuestionProvider();
  Context = new SampleGameContext();
  Difficulty = 0.0f;
}
```

Then, implement your question provider by generating (*ILivingLetterData*) using:

```C#
var newWord = AppManager.I.Teacher.GetRandomTestWordDataLL()
var newLetter = AppManager.I.Teacher.GetRandomTestLetterLL();
```
@todo: remove references to the teacher.

You can also decompose a word in letters:
```C#
var letters = ArabicAlphabetHelper.LetterDataListFromWord(newWord.Data.Arabic, AppManager.I.Letters)
```
@todo: remove references to the arabic language.

Or instead get all Letters/Words/Phrases:

```C#
var letters = AppManager.I.DB.GetAllLetterData();
var words = AppManager.I.DB.GetAllWordData();
var phrases = AppManager.I.DB.GetAllPhraseData();
```


The default Question Provider is used when you launch the game's scene directly.
Note that when playing the game through the core application, the Question Provider will be the one defined by the core system to provide meaningful learning data as selected by the Teacher.
Make sure that the test Question Provider content matches real content, otherwise your game may not work when launched through the core application.

# Using the Living Letter prefab

In the **_app/Resources/Prefabs/Actors/** directory you will find a prefab named **LivingLetter**.

That is the prefab for the animated living letter that should be used by all the MiniGames.

For the LLs, you must use the LL prefab in _app/Resources/Prefabs/Actors/LLPrefab **without breaking the prefab reference**.
If you need a custom prefab, instantiate it in the scene, add your components on it (this will not break the reference to the original prefab), disable it in the scene, and use that as prefab (e.g. dragging it in the inspector of the scene’s components). Remember to reactivate it upon instantiation.

The prefab has a **LetterObjectView** component that let you change animation and set the arabic word/letter on it.

To set the current vocabulary data, use one of the following methods:

```C#
void Initialize(ILivingLetterData _data);
void Initialize(ILivingLetterData data, string customText, float scale);
```

by passing the data that you want to see displayed on the LL.

Use `letterObjectView.Data` to get the current data.

Then, you can drive the animations using the following interface.

```C#
bool Crouching; // the LL is crouching
bool Falling; // the LL is falling*
bool Horraying; // continous horray
```

You can switch state by using the following method:
`void SetState(LLAnimationStates newState);`

The supported states are:
```C#
LL_idle, // when the LL is standing
LL_walking, // when the LL is walking or running
LL_dragging, // when the player is dragging the LL
LL_hanging, // special state for Baloons game (still waiting for animation in the fbx)
LL_dancing, // Dance!
LL_rocketing, // LL on the Rocket (use DoHorray/{set horraying} for rocket hooray)
LL_tickling, // LL is tickling
LL_limbless // when the LL has no arms and legs
```

To switch between Walking and running use:

`void SetWalkingSpeed(speed);`

*the animation will blend between walk (speed = 0) and run (speed = 1).*

Special animation triggers (it will perform an animation and go back to idle).

```C#
void DoHorray(); // triggers a single horray
void DoAngry();
void DoHighFive();
void DoDancingWin();
void DoDancingLose();
void ToggleDance(); // Switch dance between dancing1 and dancing2
void DoTwirl(System.Action onLetterShowingBack);
```

The DoTwirl animation will trigger your callback when the letter is showing its back to the camera (so you can change letter in that moment).

The following methods can be used to perform a jump. Animations are in place, so you have to move transform when performing jump and just notify the animator with the following events.

```C#
void OnJumpStart();
void OnJumpMaximumHeightReached();
void OnJumpEnded();
```

The Living Letter View has a **Poof()** method that let you create a "poof" particle animation in the current position of the letter. You can use it when you want to make the LL disappear and re-appear on another position, or simply destroy it;



# Using the Antura prefab

The contents of the AnturaSpace folder handle interactions with Antura in the AnturaSpace scene, used for reward and customization purposes.

The Antura classes are used to control Antura's behaviours, its animations, and define the appearance of rewards.
**AnturaAnimationController** and **AnturaWalkBehaviour** control the animation state of Antura.

In the **_app/Resources/Prefabs/Actors/** directory you will find a prefab named **Antura**.

That is the prefab for the animated living letter that should be used by all the MiniGames.

For Antura, you must use the original prefab **without breaking the prefab reference**.
If you need a custom prefab, instantiate it in the scene, add your components on it (this will not break the reference to the original prefab), disable it in the scene, and use that as prefab (e.g. dragging it in the inspector of the scene’s components). Remember to reactivate it upon instantiation.

The prefab has a **AnturaAnimationController** component that let you change animation and set the arabic word/letter on it. It is pretty similar to the LL view.



You can switch state by using the following property:
`AnturaAnimationStates State`

The supported states are:

```C#
idle,  // Antura is standing
walking, // Antura walking/running,
sitting, // Antura is sitting
sleeping, // Antura is sleeping
sheeping, // Antura is jumping in place
sucking // Antura is inhaling
```

Properties:

Such property:
**_bool IsAngry_**
is used when Antura is sitting, or running to select a special sitting/running animation.

Such properties are used when Antura is idle to select a special idle animation.

**_bool isExcited;_**

**_bool isSad;_**

To switch between Walking and running use:

**_void SetWalkingSpeed(speed);_**

*the animation will blend between walk (speed = 0) and run (speed = 1).*

```C#
void DoBark();

void DoSniff();

void DoShout();

void DoBurp();

DoSpit(bool openMouth);
```

The following methods can be used to perform a jump. Animations are in place, so you have to move transform when performing jump and just notify the animator with the following events.
Such events must be called in this order:

```C#
void OnJumpStart();

void OnJumpMaximumHeightReached();

void OnJumpGrab();

void OnJumpEnded();
```

This method:

`void DoCharge(System.Action onChargeEnded);`
makes Antura do an angry charge.
The Dog makes an angry charging animation (it must stay in the same position during this animation);
IsAngry is set to true automatically (needed to use the angry run).

After such animation ends, **_onChargeEnded_** will be called to inform you, and passes automatically into running state.
You should use **_onChargeEnded_** to understand when you should begin to move the antura's transform.

# Adding Environment Assets

Environment graphics, like trees, are selected in the scene in order to match the current world of the MiniGame (there are 6 worlds).
To do so, you must use the following auto-switching component: **AutoWorldPrefab**.

* Create an empy game object
* Add the "**AutoWorldPrefab**" component on it
* Select the **Prefab Set**, using the inspector
* In the **assets** tab you will find a list of possible assets, e.g.
   - Tree1
   - Tree2
   - Foreground1

* From the "Test World" drop-down in the inspector you can preview how the piece will look like when instanced in each world

* You can scale the gameobject you created; the scale will be applied to any world prefab is instanced;

* **WARNING:** the AutoWorldPrefab component will delete any gameobject that is child of the gameobject it is attached; so, be careful when you add the component to an existing gameobject (you cannot undo).

Another requisite of each MiniGame scene is that the camera that will render your environment has the following scripts:

* **CameraFog** (if the scene must have fog)

* **AutoWorldCameraColor**, that will change the camera background color and the fog color in CameraFog, according to the current world

The AutoWorldCameraColor, as in AutoWorldPrefab, needs that a field is configured by inspector. The name of the field is **Backgroung Color Set**, and currently you will find just an asset called "**CameraColors**" to be selected.

#### Refactoring Notes
- variations are not actually enforced by the codebase, but it would be a good idea to make all games use them, as currently the core app reasons in terms of 'MiniGameCode', but a MiniGame is actually identified by the 'game scene' and the 'variation'
