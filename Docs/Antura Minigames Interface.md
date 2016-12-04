**Antura Minigames Interface**

*Edits:*

<table>
  <tr>
    <td>04-10-2016</td>
    <td>Davide Barbieri</td>
  </tr>
  <tr>
    <td>28-10-2016</td>
    <td>Davide Barbieri</td>
  </tr>
  <tr>
    <td>31-10-2016</td>
    <td>Davide Barbieri</td>
  </tr>
  <tr>
    <td>01-11-2016</td>
    <td>Davide Barbieri</td>
  </tr>
  <tr>
    <td>16-11-2016</td>
    <td>Davide Barbieri</td>
  </tr>
</table>




In this document, we describe the programming interface that should be used
by all the minigames in the Antura project.

The purpose of the interface is to expose to mini-games a unified and simplified
way to access core functionalities, and to define how minigames are launched and configured,
including the dataflow from the content (e.g. question sets) database towards each minigame.

In this document, we define the word *core* as everything that is not programmed from mini-games’ developers.

# Creating a new mini-game project

If you already have a minigame and don’t want to change its architecture to the proposed one;
then, just skip to "**_Adapting an already made mini-game when you have absolutely no time to refactor your code using game states_**".

All the mini-games must be in the **__games_** directory of the Antura’s Unity3D project.

Instead of starting your own mini-game from scratch, you could use our game template:

1. Make a copy of the **__gametemplate _**directory (which is in the **__games _**directory);

2. Rename it using the name of your game, e.g. *MyNewMasterpiece* and put it under the **_games** directory;

3. In such folder (*MyNewMasterPiece*), you will find a set of files and subfolders. You must find and rename "TemplateGame.cs" and “TemplateConfiguration.cs” into “MyNewMasterpieceGame.cs” and “MyNewMasterpieceConfiguration.cs”, according to the game’s name you chose;

4. Edit these source files, and change the class names in order to comply with this name change, for example:

    1. **EA4S.Template** namespace should become *EA4S.MyNewMasterpiece*

    2. **TemplateGame** class should become *MyNewMasterpieceGame*

    3. **TemplateConfiguration** class should become *MyNewMasterpieceConfiguration*

# Game Structure

Here is described the software architecture that should be followed by your mini-games.
If you copied the Mini-game template, the main classes are already partially implemented to be compliant with such architecture.

The minigame main class should extend **MiniGame** class, inside the EA4S namespace.
Such class is realized using the *State Pattern*. ([https://en.wikipedia.org/wiki/State_pattern](https://en.wikipedia.org/wiki/State_pattern))


The game is divided in an arbitrary number of states, for example:

* *IntroductionState*, a state in which you present the game e.g. through an animation

* *ProblemState*, a state in which you describe which kind of problem the player should solve;

* *PlayState*, a state in which you process player’s input and implement the actual game;

* *ResultState*, a state in which you show the result (e.g. the score) of the player.

Such list is just an example of what states a game could have, it’s up to the mini-game developer to understand how much and what kind of state he should implement.

At each time, only one state is active and running. When it’s running, Update() and UpdatePhysics() are called on the state in each frame. UpdatePhysics is the equivalent of Unity3D’s  FixedUpdate.


All state objects are instanced in the game class, which exposes them as public fields.
Each state must have a reference to the minigame main class, that you could pass through constructor.

In this way, when you want to switch game state, you could call:

**_game.SetCurrentState(game.NewStateName);_**

each time a state transition is done, the **_ExitState()_** method is called on the previous state, and **_EnterState()_** is called on the next state.

The purpose of these methods is to process things like setting up the scene graphics, resetting timers, showing/hiding panels, etc.

# Ending the Game

When the game is over, call the method EndGame of the {GameName}Game class:

**_game.EndGame(howMuchStars, gameScore);_**

**_howMuchStars_** should be in the range (0 to 3, included);

**_gameScore _**is game-specific (could be 0, if not defined for that minigame)

In this way, the game will automatically switch to a special OutcomeState, and show how much stars were obtained, then quit the game.

# Adapting an already made mini-game when you have absolutely no time to refactor your code using states

With this and all the following sections, I’ll describe the mandatory requirements that each mini-game should fulfill in order to be compliant with the rest of the Antura project.
If you created a game using the template described in the previous sections, skip to "**Game Configuration**"

The first requirement of your minigame is to have the game configuration script described in "Game Configuration". If you want to see how a configuration class is made, you could just copy it from the template directory or from Tobogan/Fastcrowd minigames.

From that configuration class (which is a singleton per game) you will access both core functionalities (e.g. Audio/Input/HUD) and receive data from the app.


In the only case you are still extending the old MiniGameBase class, you must add this inside your game manager Update():

**var inputManager = Context.GetInputManager();**

**inputManager.Enabled = !(GlobalUI.PauseMenu.IsMenuOpen);**

**inputManager.Update(Time.deltaTime);**

# Game Configuration

Each game folder should have two main folders for scripts:
*_scripts*

*_configurationscripts*

all the game-related scripts, should be placed inside the **_scripts **folder;
**_configurationscripts **is a service folder used by core programmers to define game specific 
classes and **should not be modified by minigame creators**.

The {GameName}Configuration.cs defines how a minigame is configured by the app,
and provides the minigame some useful interfaces.

## Accessing core functionalities

When you need to access a core feature in any part of your game {GameName},
you do it through the **game context**:

**_var context = {GameName}Configuration.Instance.Context;_**

such object implements the **IGameContext** interface, which is defined in _common/_scripts/Context/IGameContext.cs.
When you need a core functionality, take a look at that file.

For example, to show the popup widget (that is, a large dialog with some text inside it),

you call:

**_context.GetPopupWidget().Show(callback, text, isArabic);_**

or, to play the game music:

**_context.GetAudioManager().PlayMusic(Music.MainTheme);_**

To have a list of all the possible functionalities that you could use, 
take a look into the **_IGameContext_** source.
If you need some core functionality that is missing in the current interface,

please ask the creator of this document.

## Audio Manager

The Audio Manager provides some simple methods to play in-game audio, for example:

**_IAudioSource PlaySound(Sfx sfx);_**

**_IAudioSource PlayLetterData(ILivingLetterData id);_**

such methods returns an *IAudioSource.*

It behaves in a similar way to Unity’s AudioSource.
It exposes some properties and methods like:

* Pitch

* Volume

* IsPlaying()

* Pause()/Stop()/Play()

## Working with the UI

When you are working on your mini-game, you do not need to know what prefab are used for the UI or how it is implemented.
The game context, **_{GameName}Configuration.Instance.Context_**, will provide you a set of interfaces to widgets that you can call from your game code.

For example:
**_ISubtitlesWidget GetSubtitleWidget();_**

**_IStarsWidget GetStarsWidget();_**

**_IPopupWidget GetPopupWidget();_**

**_ICheckmarkWidget GetCheckmarkWidget();_**

More widgets’ interfaces will be added to the context as soon the graphics will be produced.

## Game Difficulty

The game configuration class will also provide a difficulty level, set by the game.

**_float difficulty = {GameName}Configuration.Instance.Difficulty;_**

The game difficulty is expressed as a float in the range [0, 1],
meaning 0 : easy, 1 : hard.
How such difficulty level is implemented by the game is not defined
and could be chosen by the minigame developer.

For example, it could have a game speed which is dependent on the difficulty:
e.g. **_speed = normalSpeed * difficulty;_**

or, it could have a finite set of parameters configurations, based on difficulty interval:

```
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

In this case, please configure a set of at least 5 different configurations
(very easy, easy, medium, hard, very hard).

# Retrieving content from core

Often, a mini-game needs some content to be passed directly from the core code.
For instance, some minigames need a set of arabic words, that are chosen by the
core based on the current game world, or depending on the past playing history.

Such content will be passed to the game using the {GameName}Configuration.cs class
by core programmers to mini-games programmers, through one or more **_Provider_** interfaces.

Such providers are added to the Configuration class only if the game needs them.
For example, if a game needs a set of arabic words to develop its gameplay;
the Configuration class will provide a member:

**_ILivingLetterDataProvider wordsProvider;_**

therefore, the game will ask for a word data using that interface, each time it needs another one:

**_ILivingLetterData data = {GameName}Configuration.Instance.wordsProvider.GetNextData();_**

**EDIT: WE ARE CURRENTLY USING ONLY QUESTION PROVIDERS FOR ALL MINIGAMES (it is described in the following section)**

## QuestionProvider

There are also special Providers for structured data, that will be defined together with the core programmers.

After reading this section, write inside this table
[[Data provider usage table]](https://docs.google.com/spreadsheets/d/1XisADjQ97yEEN2ZSeLga1txPQy6H2v77FdPkNOcHro0)
how they should fill the IQuestionPack (described in this section) that they must pass to your minigame.

For example, there are providers which implements the interface **_IQuestionProvider_**, which exposes the following methods:

* **_IQuestionPack GetNextQuestion();_**
* **_string GetDescription();_**

Its purpose is to provide a stream of objects that implements the interface **_IQuestionPack_**, a very general abstraction for a choosing game which includes letters, words and images as fundamental parts.

What is returned as **_IQuestionPack_**, will define a package formed by:

* a **question** (in form of a Letter, a Word or an Image);

* a set of **wrong answers**, (in form of Letters, Words or Images);

* a set of **correct answers**, (in form of Letters, Words or Images);

It follows a list of possible examples:

* The game shows a word, you must select only the letters which are part of that word

    * The question is the word;
    * The correct answers are the set of letters which constitutes the word;
    * The wrong answers are a set of random letters not included in the correct set;

* The game shows a image, you must select the word W representing that image

    * The question is the image
    * The correct answers is a set with only one element, that’s the word W
    * The wrong answers is a set of random words, different from W

* The game shows a letter with its dots/signs hidden, the player hear its sound and should understand which is the correct sign that should be placed on the letter.

    * The question is the letter (the game should understand how to hide its signs/dots)
    * The correct answers is the set made just by the correct sign/dot
    * The wrong answers are all the other possible signs/dots

# Generating content for test purposes

When you are developing your mini-game, there is the need to generate game data, like letters or words, to be used as test content.
To do so, just define the default **Question Provider **in your **Game Configuratio****n** class as your custom provider, in the Game Configuration constructor (so it will be used when you will launch your scene directly).

For example:

```
private {GameName}Configuration()
{
            Questions = new MyQuestionProvider();
            Context = new SampleGameContext();
            Difficulty = 0.0f;
}
```

Then, implement your question provider by generating data (*ILivingLetterData*) using:

**_var newWord = AppManager.Instance.Teacher.GetRandomTestWordDataLL();_**

**_var newLetter = AppManager.Instance.Teacher.GetRandomTestLetterLL();_**

You can also decompose a word in letters:
**_var letters = ArabicAlphabetHelper.LetterDataListFromWord(newWord.Data.Arabic, AppManager.Instance.Letters)_**

Get all Letters/Words/Phrases:**_
var letters = AppManager.Instance.DB.GetAllLetterData();
var words = AppManager.Instance.DB.GetAllWordData();
var phrases = AppManager.Instance.DB.GetAllPhraseData();_**

By doing so, when the *real *data will be passed to your minigame from the application, you will not have to change anything, since the **_Questions_** field will be overwritten by the application.

# Using the Living Letter prefab

In the **_app/Resources/Prefabs/Actors/** directory you will find a prefab named **LivingLetter**.

That is the prefab for the animated living letter that should be used by all the minigames.

For the LLs, you must use the LL prefab in _app/Resources/Prefabs/Actors/LLPrefab **without breaking the prefab reference**.
If you need a custom prefab, instantiate it in the scene, add your components on it (this will not break the reference to the original prefab), disable it in the scene, and use that as prefab (e.g. dragging it in the inspector of the scene’s components). Remember to reactivate it upon instantiation.

The prefab has a **LetterObjectView** component that let you change animation and set the arabic word/letter on it.

To set the current letter/word, just call
**void Init(ILivingLetterData _data)**

by passing the data that you want to see displayed on the LL. 

Use **letterObjectView.Data** to get the current data.

Then, you can drive the animations using the following interface.

**_bool Crouching;_** // the LL is crouching

**_bool Falling;_** // the LL is falling*

**_bool Horraying;_** // continous horray

You can switch state by using the following method:
**_void SetState(LLAnimationStates newState)_**

The supported states are:
**_        LL_idle,_** // when the LL is standing

**_        LL_walking,_** // when the LL is walking or running

**_        LL_dragging,_** // when the player is dragging the LL

**_        LL_hanging,_** // special state for Baloons game (still waiting for animation in the fbx)

**_        LL_dancing,_** // Dance!

**_        LL_rocketing,_** // LL on the Rocket (use DoHorray/{set horraying} for rocket hooray)

**_        LL_tickling,_** // LL is tickling

**_        LL_limbless_** // when the LL has no arms and legs

To switch between Walking and running use:

**_void SetWalkingSpeed(speed);_**

*the animation will blend between walk (speed = 0) and run (speed = 1).*

Special animation triggers (it will perform an animation and go back to idle).

**_void DoHorray();_** // triggers a single horray

**_void DoAngry();_**

**_void DoHighFive();_**

**_void DoDancingWin();_**

**_void DoDancingLose();_**

**_void ToggleDance();_** // Switch dance between dancing1 and dancing2

**_void DoTwirl(System.Action onLetterShowingBack);_**

The DoTwirl animation will trigger your callback when the letter is showing its back to the camera (so you can change letter in that moment).

The following methods can be used to perform a jump. Animations are in place, so you have to move transform when performing jump and just notify the animator with the following events.

**_void OnJumpStart();_**

**_void OnJumpMaximumHeightReached();_**

**_void OnJumpEnded();_**

The Living Letter View has a **Poof()** method that let you create a "poof" particle animation in the current position of the letter. You can use it when you want to make the LL disappear and re-appear on another position, or simply destroy it;

# Using the Antura prefab

In the **_app/Resources/Prefabs/Actors/** directory you will find a prefab named **Antura**.

That is the prefab for the animated living letter that should be used by all the minigames.

For Antura, you must use the original prefab **without breaking the prefab reference**.
If you need a custom prefab, instantiate it in the scene, add your components on it (this will not break the reference to the original prefab), disable it in the scene, and use that as prefab (e.g. dragging it in the inspector of the scene’s components). Remember to reactivate it upon instantiation.

The prefab has a **AnturaAnimationController** component that let you change animation and set the arabic word/letter on it. It is pretty similar to the LL view.


You can switch state by using the following property:
**_AnturaAnimationStates State_**

The supported states are:
**_    idle,_**  // Antura is standing

**_    walking,_** // Antura walking/running,

**_    sitting,_** // Antura is sitting

**_    sleeping,_** // Antura is sleeping

**_    sheeping,_** // Antura is jumping in place

**_    sucking_** // Antura is inhaling

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

**_void DoBark()_**

**_void DoSniff()_**

**_void DoShout()_**

**_void DoBurp()_**

**_DoSpit(bool openMouth)_**
    
The following methods can be used to perform a jump. Animations are in place, so you have to move transform when performing jump and just notify the animator with the following events.
Such events must be called in this order:

**_void OnJumpStart();_**

**_void OnJumpMaximumHeightReached();_**

**_void OnJumpGrab()_**

**_void OnJumpEnded();_**

This method:

**_void DoCharge(System.Action onChargeEnded)_**
makes Antura do an angry charge. 
The Dog makes an angry charging animation (it must stay in the same position during this animation);
IsAngry is set to true automatically (needed to use the angry run).

After such animation ends, **_onChargeEnded_** will be called to inform you, and passes automatically into running state.
You should use **_onChargeEnded_** to understand when you should begin to move the antura's transform.

# Adding Environment Assets

Note: Ask Gaetano Leonardi about the new graphics, since he was already preparing it for the current mini-games (he made a copy of the minigames’ scenes to update them).

Environment graphics, like trees, are selected in the scene in order to match the current world of the minigame (there are 6 worlds).
To do so, you must use the following auto-switching component: **AutoWorldPrefab**.

* Create an empy game object

* Add the "**AutoWorldPrefab**" component on it

* Select the **Prefab Set**, using the inspector

* In the **assets** tab you will find a list of possible assets, e.g.

    * Tree1

    * Tree2

    * Foreground1

* From the "Test World" drop-down in the inspector you can preview how the piece will look like when instanced in each world

* You can scale the gameobject you created; the scale will be applied to any world prefab is instanced;

* **WARNING:** the AutoWorldPrefab component will delete any gameobject that is child of the gameobject it is attached; so, be careful when you add the component to an existing gameobject (you cannot undo).

Another requisite of each minigame scene is that the camera that will render your environment has the following scripts:

* **CameraFog** (if the scene must have fog)

* **AutoWorldCameraColor**, that will change the camera background color and the fog color in CameraFog, according to the current world

The AutoWorldCameraColor, as in AutoWorldPrefab, needs that a field is configured by inspector. The name of the field is **Backgroung Color Set**, and currently you will find just an asset called "**CameraColors**" to be selected.

