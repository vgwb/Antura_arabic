# Data Flow

This document describes how dictionary data flows from the database, through the Teacher System, to the specific minigames.

@todo: add a diagram for the data flow

### Database

All learning data of a given language (letters, words, and phrases),
 as well as their relationships, are contained in a database.

The Teacher can directly access the database.
The database must instead not be accessed directly by the minigames,
  as the Teacher must filter the data based on learning requirements.

Refer to the **Database** documentation for further details.
@todo: LINK

### Teacher filtering

The minigame data flow is started whenever a minigame is
  launched through **MiniGameAPI.StartGame()**.

Prior to loading the minigame, the Teacher retrieves the
 static minigame configuration (*IGameConfiguration*) and
  obtains from it the configured **IQuestionBuilder**  through a call to **IGameConfiguration.SetupBuilder()**.
  The **IQuestionBuilder** defines the learning rules and requirements for the current minigame variation.

 Based on the minigame rules and requirements,
  the Teacher then selects the learning data suited to the current context
   and generates a list of **IQuestionPack** through
   **QuestionPacksGenerator.GenerateQuestionPacks(IQuestionBuilder  builder)**.

 Each **IQuestionPack**  thus generated contains a list of questions, correct answers, and wrong answers suitable
 for the current learning progression and supported by the minigame.

 The list is then injected into the game configuration
  by creating a **FindRightLetterQuestionProvider**
   and storing it inside **IGameConfiguration.Questions**,
    then the minigame is loaded.
 The **FindRightLetterQuestionProvider** is here used to provide sequentially the
  list of question packs that the Teacher selected.

Refer to the **Teacher** documentation for further details on **QuestionBuilders**
 and on how the Teacher selects the learning data.
@todo: LINK


### Question Provider & Minigame Data Access

 At any point during play (but, usually, during the minigame's initialisation),
  the minigame code can access the next question pack
   for play by calling its static configuration's instance
    **IGameConfiguration.Questions.GetNextQuestion()** and thus retrieving a **IQuestionPack**.

 The minigame is then free to display the learning data
 to the player according to its inner workings.
 For this purpose, Living Letters can be used as a convenient
  and shared way to display the learning data.


### Living Letters

Minigames can display the learning data to the players through Living Letters.
Living Letters are animated characters that can display a piece of learning data.

The Living Letters can display learning data in several ways, depending on the type of data to display.
By assigning a **LL_XXXData** to a **LetterObjectView**, the corresponding data is displayed on the Living Letter.
The display method is set by converting a given learning data instance into a correpsonding **LL_XXXData** instance.

 * **LL_LetterData** displays a Db.LetterData in text form (a single letter)
 * **LL_WordData** displays a Db.WordData in text form
 * **LL_ImageData** displays a Db.WordData in image form (a drawing or picture)
 * **LL_PhraseData** displays a Db.PhraseData in text form




### Test data

 @TODO: connect to the Antura Minigame Interface.md
