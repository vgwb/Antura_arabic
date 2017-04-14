# Antura Database management

Here is a description of the Antura Database and proposal for the web application to deal with the upload of the db files from the Antura app to the servers.

## Overview

Antura app stores all the data in two datasets:
A static set of tables with all the vocabularies and game settings (like the journey data).. These data should not change and is fixed for all players
A dynamic set of tables, storing all the player performances and informations. These tables are stored in a SQLite database.
When a player starts player, a UUID (Unique Identifier) and a new SQLite database are created. This UUID is the key to identify the player. The SQLite file itself is named Antura_Player_UUID value (like ```Antura_Player_e7e8d59a-b148-48b0-a848-4fa6fb4116cb.sqlite3``` )

The app is going to allow the export of the sqlite db, and we were thinking that the best way was to upload the file directly into a web server, injecting the SQLite data into a bigger “master collector” mysql db, to allow comparative analysis and queries.

This document describes all the Database related info needed for the app developers, the web developers and the data analysts.
Web upload DB project
The web app receiving the db files should be an independent app, maybe accessible through a static domain like: <https://db.antura.org> (if possible it’s much better a third level domain to separate the app from the wordpress website living at antura.org )

We think that the Antura App should just access that page, handshake a security token, and then upload its SQLite file.
The application can be developed with any web technology, but we assume to use LAMP (Linux / Apache / Mysql / PHP) since it’s a very common stack.

As first step, we imagine a script like <https://db.antura.org/upload.php> where we can upload our file as a multi-part form.
The file won’t be bigger than 1Mb.
The script should save this file into a named folder, like /uploads/2017/04/filename
(since we expect several thousands of files, it’s better to organize them by month)

When the upload finishes with success, an log entry should be added to the databases, like “file x has been uploaded at this time from , it’s location is y”.
Just after the uploading a new script should be called that opens the sqlite file, processes it and injects its data into the main mysql database (we’ll describe the exact procedure later)
An email should be sent to a list of addresses to notify the new file entry.

## Db filename

Filename: Antura_Player_e7e8d59a-b148-48b0-a848-4fa6fb4116cb.sqlite3
The “e7e8d59a-b148-48b0-a848-4fa6fb4116cb” is the uuid (universal identifier) that defines the player

## Static Tables schemes
```
    CREATE TABLE `_LetterData` (
      `Id` varchar(255),
      `Number` int(11) DEFAULT NULL,
      `Added` varchar(255),
      `Active` tinyint(4) DEFAULT NULL,
      `Complexity` varchar(255),
      `Title` varchar(255),
      `Kind` varchar(255),
      `BaseLetter` varchar(255),
      `Symbol` varchar(255),
      `SoundZone` varchar(255),
      `Type` varchar(255),
      `Tag` varchar(255),
      `Notes` varchar(255),
      `SunMoon` varchar(255),
      `Sound` varchar(255),
      `Isolated` varchar(255),
      `Final` varchar(255),
      `Medial` varchar(255),
      `Initial` varchar(255),
      `Isolated_Unicode` varchar(255),
      `Final_Unicode` varchar(255),
      `Medial_Unicode` varchar(255),
      `Initial_Unicode` varchar(255),
      `Symbol_Unicode` varchar(255),
      `FinalFix` varchar(255),
      `MedialFix` varchar(255),
      `InitialFix` varchar(255),
      `Old_Isolated` varchar(255),
      `Old_Final` varchar(255),
      `Old_Medial` varchar(255),
      `Old_Initial` varchar(255),
    )

    CREATE TABLE `_MiniGameData` (
      `Id` varchar(255),
      `Type` varchar(255),
      `Status` varchar(255),
      `Main` varchar(255),
      `Variation` varchar(255),
      `Badge` varchar(255),
      `Description` varchar(255),
      `Team` varchar(255),
      `Title_En` varchar(255),
      `Title_Ar` varchar(255),
      `Intro_En` varchar(255),
      `Intro_Ar` varchar(255),
      `Tutorial_En` varchar(255),
      `Tutorial_Ar` varchar(255),
      `Scene` varchar(255),
      `Notes` varchar(255),
      `SkillTiming` varchar(255),
      `SkillPrecision` varchar(255),
      `SkillObservation` varchar(255),
      `SkillListening` varchar(255),
      `SkillLogic` varchar(255),
      `SkillMemory` varchar(255),
      `TOTAL` varchar(255),
      `Comprehension` varchar(255),
    )


    CREATE TABLE `_PhraseData` (
      `Id` varchar(255),
      `Active` tinyint(4) DEFAULT NULL,
      `English` varchar(255),
      `Arabic` varchar(255),
      `Linked` varchar(255),
      `Category` varchar(255),
      `Words` varchar(255),
      `Answers` varchar(255),
      `Complexity` varchar(255),
      `Notes` varchar(255),
    )

    CREATE TABLE `_PlaySessionData` (
      `Stage` int(11) DEFAULT NULL,
      `LearningBlock` int(11) DEFAULT NULL,
      `PlaySession` int(11) DEFAULT NULL,
      `Title_En` varchar(255),
      `Title_Ar` varchar(255),
      `Description_En` varchar(255),
      `Description_Ar` varchar(255),
      `Notes` varchar(255),
      `Focus` varchar(255),
      `Letters` varchar(255),
      `Type` varchar(255),
      `Words` varchar(255),
      `Words_previous` varchar(255),
      `Phrases` varchar(255),
      `Phrases_previous` varchar(255),
      `Order` varchar(255),
      `NumberOfMinigames` varchar(255),
      `NumberOfRounds` varchar(255),
      `AssessmentType` varchar(255),
      `Maze` varchar(255),
      `FastCrowd_letter` varchar(255),
      `ThrowBalls_letters` varchar(255),
      `Tobogan_letters` varchar(255),
      `Balloons_letter` varchar(255),
      `TakeMeHome` varchar(255),
      `Egg_letters` varchar(255),
      `Egg_sequence` varchar(255),
      `ColorTickle` varchar(255),
      `HideSeek` varchar(255),
      `AlphabetSong_alphabet` varchar(255),
      `AlphabetSong_letters` varchar(255),
      `MixedLetters_alphabet` varchar(255),
      `FastCrowd_alphabet` varchar(255),
      `MakeFriends` varchar(255),
      `DancingDots` varchar(255),
      `SickLetters` varchar(255),
      `ThrowBalls_letterinword` varchar(255),
      `MissingLetter` varchar(255),
      `Balloons_spelling` varchar(255),
      `FastCrowd_spelling` varchar(255),
      `MixedLetters_spelling` varchar(255),
      `Balloons_words` varchar(255),
      `FastCrowd_words` varchar(255),
      `ThrowBalls_words` varchar(255),
      `Tobogan_words` varchar(255),
      `Scanner` varchar(255),
      `Balloons_counting` varchar(255),
      `FastCrowd_counting` varchar(255),
      `MissingLetter_phrases` varchar(255),
      `MissingLetter_forms` varchar(255),
      `Scanner_phrase` varchar(255),
      `ReadingGame` varchar(255),
    )

    CREATE TABLE `_StageData` (
      `Id` varchar(255),
      `Title_En` varchar(255),
      `Title_Ar` varchar(255),
      `Description` varchar(255),
    )

    CREATE TABLE `_WordData` (
      `Id` varchar(255),
      `added` varchar(255),
      `Active` varchar(255),
      `Kind` varchar(255),
      `Category` varchar(255),
      `Complexity` varchar(255),
      `Gender` varchar(255),
      `Form` varchar(255),
      `Article` varchar(255),
      `LinkedWord` varchar(255),
      `Value` varchar(255),
      `Arabic` varchar(255),
      `Symbols` varchar(255),
      `Drawing` varchar(255),
      `Letters` varchar(255),
      `Transliteration` varchar(255),
      `SunMoon` varchar(255),
      `Notes` varchar(255),
    )
```
## Dynamic Tables  schemes
```
    CREATE TABLE `DatabaseInfoData` (
      `Uuid` longtext,
      `Timestamp` int(11) DEFAULT NULL,
      `DynamicDbVersion` longtext,
      `StaticDbVersion` longtext COLLATE utf8_bin
    )

    CREATE TABLE `JourneyScoreData` (
      `Uuid` longtext,
      `JourneyDataType` int(11) DEFAULT NULL,
      `ElementId` longtext,
      `Stage` int(11) DEFAULT NULL,
      `LearningBlock` int(11) DEFAULT NULL,
      `PlaySession` int(11) DEFAULT NULL,
      `Stars` int(11) DEFAULT NULL,
      `UpdateTimestamp` int(11) DEFAULT NULL
    )

    CREATE TABLE `LogInfoData` (
      `Id` int(11) NOT NULL,
      `Uuid` longtext,
      `AppSession` int(11) DEFAULT NULL,
      `Timestamp` int(11) DEFAULT NULL,
      `Event` int(11) DEFAULT NULL,
      `Scene` int(11) DEFAULT NULL,
      `AdditionalData` longtext,
      )

    CREATE TABLE `LogMiniGameScoreData` (
      `Id` int(11) NOT NULL,
      `Uuid` longtext,
      `AppSession` int(11) DEFAULT NULL,
      `Timestamp` int(11) DEFAULT NULL,
      `Stage` int(11) DEFAULT NULL,
      `LearningBlock` int(11) DEFAULT NULL,
      `PlaySession` int(11) DEFAULT NULL,
      `MiniGameCode` int(11) DEFAULT NULL,
      `Stars` int(11) DEFAULT NULL,
      `PlayTime` double DEFAULT NULL,
      )

    CREATE TABLE `LogMoodData` (
      `Id` int(11) NOT NULL,
      `Uuid` longtext,
      `AppSession` int(11) DEFAULT NULL,
      `Timestamp` int(11) DEFAULT NULL,
      `MoodValue` double DEFAULT NULL,
      )

    CREATE TABLE `LogPlayData` (
      `Id` int(11) NOT NULL,
      `Uuid` longtext,
      `AppSession` int(11) DEFAULT NULL,
      `Timestamp` int(11) DEFAULT NULL,
      `Stage` int(11) DEFAULT NULL,
      `LearningBlock` int(11) DEFAULT NULL,
      `PlaySession` int(11) DEFAULT NULL,
      `MiniGameCode` int(11) DEFAULT NULL,
      `PlayEvent` int(11) DEFAULT NULL,
      `PlaySkill` int(11) DEFAULT NULL,
      `Score` double DEFAULT NULL,
      `AdditionalData` longtext,
      )

    CREATE TABLE `LogPlaySessionScoreData` (
      `Id` int(11) NOT NULL,
      `Uuid` longtext,
      `AppSession` int(11) DEFAULT NULL,
      `Timestamp` int(11) DEFAULT NULL,
      `Stage` int(11) DEFAULT NULL,
      `LearningBlock` int(11) DEFAULT NULL,
      `PlaySession` int(11) DEFAULT NULL,
      `Stars` int(11) DEFAULT NULL,
      `PlayTime` double DEFAULT NULL,
      )

    CREATE TABLE `LogVocabularyScoreData` (
      `Id` int(11) NOT NULL,
      `Uuid` longtext,
      `AppSession` int(11) DEFAULT NULL,
      `Timestamp` int(11) DEFAULT NULL,
      `Stage` int(11) DEFAULT NULL,
      `LearningBlock` int(11) DEFAULT NULL,
      `PlaySession` int(11) DEFAULT NULL,
      `MiniGameCode` int(11) DEFAULT NULL,
      `VocabularyDataType` int(11) DEFAULT NULL,
      `ElementId` longtext,
      `Score` double DEFAULT NULL,
      )

    CREATE TABLE `PlayerProfileData` (
      `Timestamp` int(11) DEFAULT NULL,
      `Uuid` longtext,
      `AvatarId` int(11) DEFAULT NULL,
      `Gender` int(11) DEFAULT NULL,
      `Tint` int(11) DEFAULT NULL,
      `IsDemoUser` int(11) DEFAULT NULL,
      `JourneyCompleted` int(11) DEFAULT NULL,
      `TotalScore` double DEFAULT NULL,
      `Age` int(11) DEFAULT NULL,
      `ProfileCompletion` int(11) DEFAULT NULL,
      `MaxStage` int(11) DEFAULT NULL,
      `MaxLearningBlock` int(11) DEFAULT NULL,
      `MaxPlaySession` int(11) DEFAULT NULL,
      `CurrentStage` int(11) DEFAULT NULL,
      `CurrentLearningBlock` int(11) DEFAULT NULL,
      `CurrentPlaySession` int(11) DEFAULT NULL,
      `TotalBones` int(11) DEFAULT NULL,
      `CurrentAnturaCustomization` longtext,
      `AdditionalData` longtext COLLATE utf8_bin
    )

    CREATE TABLE `VocabularyScoreData` (
      `Uuid` longtext,
      `VocabularyDataType` int(11) DEFAULT NULL,
      `ElementId` longtext,
      `Score` double DEFAULT NULL,
      `Unlocked` int(11) DEFAULT NULL,
      `UpdateTimestamp` int(11) DEFAULT NULL
    )
```

## Procedures
```
    update DatabaseInfoData set Uuid = "e7e8d59a-b148-48b0-a848-4fa6fb4116cb";
    update JourneyScoreData set Uuid = "e7e8d59a-b148-48b0-a848-4fa6fb4116cb";
    update LogInfoData set Uuid = "e7e8d59a-b148-48b0-a848-4fa6fb4116cb";
    update LogMiniGameScoreData set Uuid = "e7e8d59a-b148-48b0-a848-4fa6fb4116cb";
    update LogMoodData set Uuid = "e7e8d59a-b148-48b0-a848-4fa6fb4116cb";
    update LogPlayData set Uuid = "e7e8d59a-b148-48b0-a848-4fa6fb4116cb";
    update LogPlaySessionScoreData set Uuid = "e7e8d59a-b148-48b0-a848-4fa6fb4116cb";
    update LogVocabularyScoreData set Uuid = "e7e8d59a-b148-48b0-a848-4fa6fb4116cb";
    update RewardPackUnlockData set Uuid = "e7e8d59a-b148-48b0-a848-4fa6fb4116cb";
    update VocabularyScoreData set Uuid = "e7e8d59a-b148-48b0-a848-4fa6fb4116cb";
```
