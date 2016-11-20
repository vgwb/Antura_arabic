using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using EA4S;
using ModularFramework.Core;

public partial class SROptions
{
    public void LaunchMinigame(MiniGameCode minigameCode)
    {
        WidgetPopupWindow.I.Close();
        DebugManager.I.LaunchMinigGame(minigameCode);
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Options")]
    [Sort(1)]
    public bool Cheat { get { return DebugManager.I.CheatMode; } set { DebugManager.I.CheatMode = value; } }

    [Category("Options")]
    [NumberRange(1, 6)]
    [Sort(1)]
    public int Stage { get { return DebugManager.I.Stage; } set { DebugManager.I.Stage = value; } }

    [Category("Options")]
    [NumberRange(1, 6)]
    [Sort(2)]
    public int LearningBlock { get { return DebugManager.I.LearningBlock; } set { DebugManager.I.LearningBlock = value; } }

    [Category("Options")]
    [NumberRange(1, 6)]
    [Sort(3)]
    public int PlaySession { get { return DebugManager.I.PlaySession; } set { DebugManager.I.PlaySession = value; } }

    [Category("Options")]
    [Sort(4)]
    public DifficulyLevels DifficultyLevel { get { return DebugManager.I.DifficultyLevel; } set { DebugManager.I.DifficultyLevel = value; } }

    [Category("Options")]
    [Sort(60)]
    public void Home()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("_Start");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Options")]
    [Sort(80)]
    public void ToggleQuality()
    {
        AppManager.Instance.ToggleQualitygfx();
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Minigames")]
    [Sort(10)]
    public void AlphabetSong()
    {
        LaunchMinigame(MiniGameCode.AlphabetSong);
    }

    [Category("Minigames")]
    [Sort(11)]
    public void BalloonsLetter()
    {
        LaunchMinigame(MiniGameCode.Balloons_letter);
    }

    [Category("Minigames")]
    [Sort(11)]
    public void BalloonsWords()
    {
        LaunchMinigame(MiniGameCode.Balloons_words);
    }

    [Category("Minigames")]
    [Sort(11)]
    public void BalloonsCounting()
    {
        LaunchMinigame(MiniGameCode.Balloons_counting);
    }

    [Category("Minigames")]
    [Sort(11)]
    public void BalloonsSpelling()
    {
        LaunchMinigame(MiniGameCode.Balloons_spelling);
    }

    [Category("Minigames")]
    [Sort(12)]
    public void ColorTickle()
    {
        LaunchMinigame(MiniGameCode.ColorTickle);
    }

    [Category("Minigames")]
    [Sort(13)]
    public void DancingDots()
    {
        LaunchMinigame(MiniGameCode.DancingDots);
    }

    [Category("Minigames")]
    [Sort(14)]
    public void Egg()
    {
        LaunchMinigame(MiniGameCode.Egg);
    }

    [Category("Minigames")]
    [Sort(15)]
    public void FastCrowdWords()
    {
        LaunchMinigame(MiniGameCode.FastCrowd_words);
    }

    [Category("Minigames")]
    [Sort(15)]
    public void FastCrowdLetter()
    {
        LaunchMinigame(MiniGameCode.FastCrowd_letter);
    }

    [Category("Minigames")]
    [Sort(15)]
    public void FastCrowdAlphabet()
    {
        LaunchMinigame(MiniGameCode.FastCrowd_alphabet);
    }

    [Category("Minigames")]
    [Sort(15)]
    public void FastCrowdCounting()
    {
        LaunchMinigame(MiniGameCode.FastCrowd_counting);
    }

    [Category("Minigames")]
    [Sort(15)]
    public void FastCrowdSPelling()
    {
        LaunchMinigame(MiniGameCode.FastCrowd_spelling);
    }

    [Category("Minigames")]
    [Sort(16)]
    public void HideAndSeek()
    {
        LaunchMinigame(MiniGameCode.HideSeek);
    }


    [Category("Minigames")]
    [Sort(17)]
    public void MakeFriends()
    {
        LaunchMinigame(MiniGameCode.MakeFriends);
    }

    [Category("Minigames")]
    [Sort(18)]
    public void Maze()
    {
        LaunchMinigame(MiniGameCode.Maze);
    }

    [Category("Minigames")]
    [Sort(19)]
    public void MissingLetter()
    {
        LaunchMinigame(MiniGameCode.MissingLetter);
    }

    [Category("Minigames")]
    [Sort(19)]
    public void MissingLetterPhrases()
    {
        LaunchMinigame(MiniGameCode.MissingLetter_phrases);
    }

    [Category("Minigames")]
    [Sort(20)]
    public void MixedLettersSpelling()
    {
        LaunchMinigame(MiniGameCode.MixedLetters_spelling);
    }

    [Category("Minigames")]
    [Sort(20)]
    public void MixedLettersAlphabet()
    {
        LaunchMinigame(MiniGameCode.MixedLetters_alphabet);
    }

    [Category("Minigames")]
    [Sort(21)]
    public void ReadingGame()
    {
        LaunchMinigame(MiniGameCode.ReadingGame);
    }

    [Category("Minigames")]
    [Sort(21)]
    public void Scanner()
    {
        LaunchMinigame(MiniGameCode.Scanner);
    }

    [Category("Minigames")]
    [Sort(21)]
    public void ScannerPhrase()
    {
        LaunchMinigame(MiniGameCode.Scanner_phrase);
    }

    [Category("Minigames")]
    [Sort(21)]
    public void SickLetters()
    {
        LaunchMinigame(MiniGameCode.SickLetters);
    }

    [Category("Minigames")]
    [Sort(22)]
    public void TakeMeHome()
    {
        LaunchMinigame(MiniGameCode.TakeMeHome);
    }

    [Category("Minigames")]
    [Sort(23)]
    public void ThrowBallsWOrds()
    {
        LaunchMinigame(MiniGameCode.ThrowBalls_words);
    }

    [Category("Minigames")]
    [Sort(23)]
    public void ThrowBallsLetters()
    {
        LaunchMinigame(MiniGameCode.ThrowBalls_letters);
    }

    [Category("Minigames")]
    [Sort(23)]
    public void ThrowBallsLetterInWord()
    {
        LaunchMinigame(MiniGameCode.ThrowBalls_letterinword);
    }

    [Category("Minigames")]
    [Sort(24)]
    public void ToboganWords()
    {
        LaunchMinigame(MiniGameCode.Tobogan_words);
    }

    [Category("Minigames")]
    [Sort(24)]
    public void ToboganLetters()
    {
        LaunchMinigame(MiniGameCode.Tobogan_letters);
    }


    [Category("Assessments")]
    public void LetterShape()
    {
        LaunchMinigame(MiniGameCode.Assessment_LetterShape);
    }

    [Category("Assessments")]
    public void WordsWithLetter()
    {
        LaunchMinigame(MiniGameCode.Assessment_WordsWithLetter);
    }
    [Category("Assessments")]
    public void MatchLettersToWord()
    {
        LaunchMinigame(MiniGameCode.Assessment_MatchLettersToWord);
    }
    [Category("Assessments")]
    public void CompleteWord()
    {
        LaunchMinigame(MiniGameCode.Assessment_CompleteWord);
    }
    [Category("Assessments")]
    public void OrderLettersOfWord()
    {
        LaunchMinigame(MiniGameCode.Assessment_OrderLettersOfWord);
    }
    [Category("Assessments")]
    public void VowelOrConsonant()
    {
        LaunchMinigame(MiniGameCode.Assessment_VowelOrConsonant);
    }
    [Category("Assessments")]
    public void SelectPronouncedWord()
    {
        LaunchMinigame(MiniGameCode.Assessment_SelectPronouncedWord);
    }
    [Category("Assessments")]
    public void MatchWordToImage()
    {
        LaunchMinigame(MiniGameCode.Assessment_MatchWordToImage);
    }
    [Category("Assessments")]
    public void WordArticle()
    {
        LaunchMinigame(MiniGameCode.Assessment_WordArticle);
    }
    [Category("Assessments")]
    public void SingularDualPlural()
    {
        LaunchMinigame(MiniGameCode.Assessment_SingularDualPlural);
    }
    [Category("Assessments")]
    public void SunMoonWord()
    {
        LaunchMinigame(MiniGameCode.Assessment_SunMoonWord);
    }
    [Category("Assessments")]
    public void SunMoonLetter()
    {
        LaunchMinigame(MiniGameCode.Assessment_SunMoonLetter);
    }
    [Category("Assessments")]
    public void QuestionAndReply()
    {
        LaunchMinigame(MiniGameCode.Assessment_QuestionAndReply);
    }

    //[Category("Manage")]
    //public void Database()
    //{
    //    WidgetPopupWindow.I.Close();
    //    GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("manage_Database");
    //    SRDebug.Instance.HideDebugPanel();
    //}

    ////[Category("Minigames")]
    ////public void DontWakeUp()
    ////{
    ////    WidgetPopupWindow.I.Close();
    ////    GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_DontWakeUp");
    ////    SRDebug.Instance.HideDebugPanel();
    ////}

    //[Category("Shortcuts")]
    //public void EndFastCrowdGame()
    //{
    //    //      EA4S.FastCrowd.FastCrowd.Instance.DebugForceEndGame();
    //    SRDebug.Instance.HideDebugPanel();
    //}



    /// MakeFriends
    [Category("MakeFriends")]
    public bool MakeFriendsUseDifficulty { get; set; }

    [Category("MakeFriends")]
    public EA4S.MakeFriends.MakeFriendsVariation MakeFriendsDifficulty { get; set; }

    /// ThrowBalls
    private bool ThrowBallsShowProjection = true;
    [Category("ThrowBalls")]
    public bool ShowProjection {
        get { return ThrowBallsShowProjection; }
        set { ThrowBallsShowProjection = value; }
    }

    private float ThrowBallselasticity = 19f;
    [Category("ThrowBalls")]
    public float Elasticity {
        get { return ThrowBallselasticity; }
        set { ThrowBallselasticity = value; }
    }
}
