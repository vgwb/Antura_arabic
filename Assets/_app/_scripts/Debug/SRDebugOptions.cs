#if SRDebuggerEnabled
using System.ComponentModel;
using UnityEngine;
using EA4S;
using EA4S.Core;
using EA4S.Database;
using EA4S.Debugging;
using EA4S.Rewards;
using EA4S.UI;

// refactoring: this is tied to SRDebugger, but we have a DebugManager. Move all debug logic there and make this behave only as a wrapping interface.
public partial class SROptions
{
    public void LaunchMinigame(MiniGameCode minigameCode)
    {
        if (!AppConstants.DebugStopPlayAtWrongPlaySessions || AppManager.I.Teacher.CanMiniGameBePlayedAfterMinPlaySession(new JourneyPosition(Stage, LearningBlock, PlaySession), minigameCode)) {
            WidgetPopupWindow.I.Close();
            DebugManager.I.LaunchMiniGame(minigameCode);
            SRDebug.Instance.HideDebugPanel();
        } else {
            if (AppConstants.DebugStopPlayAtWrongPlaySessions) {
                JourneyPosition minJ = AppManager.I.JourneyHelper.GetMinimumJourneyPositionForMiniGame(minigameCode);
                if (minJ == null) {
                    Debug.LogWarningFormat("Minigame {0} could not be selected for any PlaySession. Please check the PlaySession data table.", minigameCode);
                } else {
                    Debug.LogErrorFormat("Minigame {0} cannot be selected this PlaySession. Min: {1}", minigameCode, minJ.ToString());
                }
            }
        }
    }

    [Category("Report Bug")]
    [Sort(1)]
    public void ReportBug()
    {
        AppManager.I.OpenSupportForm();
    }

    [Category("Options")]
    [Sort(1)]
    public bool Cheat { get { return DebugManager.I.CheatMode; } set { DebugManager.I.CheatMode = value; } }

    [Category("Options")]
    [Sort(1)]
    public void ResetAll()
    {
        // refactor: move to DebugManager
        AppManager.I.PlayerProfileManager.ResetEverything();
        SRDebug.Instance.HideDebugPanel();
        AppManager.I.NavigationManager.GoToHome(debugMode: true);
        Debug.Log("Reset ALL players and DB.");
    }

    [Category("Options")]
    [Sort(1)]
    public bool IgnoreJourneyData { get { return DebugManager.I.IgnoreJourneyData; } set { DebugManager.I.IgnoreJourneyData = value; } }

    [Category("Options")]
    [Sort(1)]
    public bool VerboseTeacher { get { return EA4S.Teacher.ConfigAI.verboseTeacher; } set { EA4S.Teacher.ConfigAI.verboseTeacher = value; } }

    [Category("Options")]
    [Sort(1)]
    public bool SafeLaunch { get { return AppConstants.DebugStopPlayAtWrongPlaySessions; } set { AppConstants.DebugStopPlayAtWrongPlaySessions = value; } }

    [Category("Options")]
    [NumberRange(1, 6)]
    [Sort(10)]
    public int Stage { get { return DebugManager.I.Stage; } set { DebugManager.I.Stage = value; } }

    [Category("Options")]
    [NumberRange(1, 15)]
    [Sort(20)]
    public int LearningBlock { get { return DebugManager.I.LearningBlock; } set { DebugManager.I.LearningBlock = value; } }

    [Category("Options")]
    [NumberRange(1, 100)]
    [Sort(30)]
    public int PlaySession { get { return DebugManager.I.PlaySession; } set { DebugManager.I.PlaySession = value; } }

    [Category("Options")]
    [Sort(40)]
    public DifficultyLevel DifficultyLevel { get { return DebugManager.I.DifficultyLevel; } set { DebugManager.I.DifficultyLevel = value; } }

    [Category("Options")]
    [NumberRange(1, 6)]
    [Sort(50)]
    public int NumberOfRounds { get { return DebugManager.I.NumberOfRounds; } set { DebugManager.I.NumberOfRounds = value; } }

    [Category("Options")]
    [Sort(80)]
    public void ToggleQuality()
    {
        // refactor: move to DebugManager
        AppManager.I.ToggleQualitygfx();
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Navigation")]
    public void GoHome()
    {
        // refactor: move to DebugManager
        WidgetPopupWindow.I.Close();
        AppManager.I.NavigationManager.GoToHome(debugMode: true);
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Navigation")]
    public void GoMap()
    {
        WidgetPopupWindow.I.Close();
        AppManager.I.NavigationManager.GoToMap(debugMode: true);
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Navigation")]
    public void GoNext()
    {
        WidgetPopupWindow.I.Close();
        AppManager.I.NavigationManager.GoToNextScene();
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Navigation")]
    public void GoEnd()
    {
        WidgetPopupWindow.I.Close();
        AppManager.I.NavigationManager.GoToEnding(debugMode: true);
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Navigation")]
    public void GoToReservedArea()
    {
        WidgetPopupWindow.I.Close();
        AppManager.I.NavigationManager.GoToReservedArea(debugMode: true);
        SRDebug.Instance.HideDebugPanel();
    }

    // refactor: we should use a parameterized version for calling minigames so we are not dependant on the existing minigames
    [Category("Minigames")]
    [Sort(10)]
    public void AlphabetSong()
    {
        LaunchMinigame(MiniGameCode.AlphabetSong_alphabet);
    }

    [Category("Minigames")]
    [Sort(10)]
    public void DiacriticSong()
    {
        LaunchMinigame(MiniGameCode.AlphabetSong_letters);
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
    public void EggLetters()
    {
        LaunchMinigame(MiniGameCode.Egg_letters);
    }

    [Category("Minigames")]
    [Sort(14)]
    public void EggSequence()
    {
        LaunchMinigame(MiniGameCode.Egg_sequence);
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
    public void MissingLetterForms()
    {
        LaunchMinigame(MiniGameCode.MissingLetter_forms);
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
    public void LetterForm()
    {
        LaunchMinigame(MiniGameCode.Assessment_LetterForm);
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
    public void MatchLettersToWordForms()
    {
        LaunchMinigame(MiniGameCode.Assessment_MatchLettersToWord_Form);
    }
    [Category("Assessments")]
    public void CompleteWord()
    {
        LaunchMinigame(MiniGameCode.Assessment_CompleteWord);
    }
    [Category("Assessments")]
    public void CompleteWordForms()
    {
        LaunchMinigame(MiniGameCode.Assessment_CompleteWord_Form);
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

    // refactor: minigame-specific debug options should not be here, place them in other partial classes if truly needed

    /// MakeFriends
    [Category("MakeFriends")]
    public bool MakeFriendsUseDifficulty { get; set; }

    [Category("MakeFriends")]
    public EA4S.Minigames.MakeFriends.MakeFriendsVariation MakeFriendsDifficulty { get; set; }

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

    [Category("Player Profile")]
    [Sort(1)]
    public bool FirstContactPassed { get { return DebugManager.I.FirstContactPassed; } set { DebugManager.I.FirstContactPassed = value; } }

    [Category("Player Profile")]
    [Sort(2)]
    public void DeleteAllProfiles()
    {
        ResetAll();
    }

    [Category("Player Profile")]
    [Sort(2)]
    public void CreateTestProfile()
    {
        AppManager.I.PlayerProfileManager.CreatePlayerProfile(4, PlayerGender.F, 1, PlayerTint.Blue);
        AppManager.I.NavigationManager.GoToHome(debugMode: true);
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Player Profile")]
    [Sort(2)]
    public void GiveBones()
    {
        AppManager.I.Player.AddBones(10);
    }

    [Category("Max Journey Position")]
    [Sort(5)]
    public string CurrentMaxJouneryPosition {
        get { return AppManager.I.Player.MaxJourneyPosition.ToString(); }
    }

    [Category("Max Journey Position")]
    [Sort(6)]
    public void ForwardMaxPosition()
    {
        // refactor: move to DebugManager
        JourneyPosition newPos = AppManager.I.JourneyHelper.FindNextJourneyPosition(AppManager.I.Player.MaxJourneyPosition);
        if (newPos != null) {
            AppManager.I.Player.SetMaxJourneyPosition(newPos, true);
        }
        SRDebug.Instance.HideDebugPanel();
        SRDebug.Instance.ShowDebugPanel();
    }

    [Category("Max Journey Position")]
    [Sort(6)]
    public void SecondToLastJourneyPos()
    {
        JourneyPosition newPos = AppManager.I.JourneyHelper.GetFinalJourneyPosition();
        newPos.PlaySession = 2;
        if (newPos != null) {
            AppManager.I.Player.SetMaxJourneyPosition(newPos, true);
            FirstContactPassed = true;
        }
        SRDebug.Instance.HideDebugPanel();
        SRDebug.Instance.ShowDebugPanel();
        GoMap();
    }

    [Category("Max Journey Position")]
    [Sort(7)]
    public void UnlockAll()
    {
        // refactor: move to DebugManager
        AppManager.I.Player.SetMaxJourneyPosition(new JourneyPosition(6, 15, 100), true);
        SRDebug.Instance.HideDebugPanel();
        SRDebug.Instance.ShowDebugPanel();
    }

    [Category("Max Journey Position")]
    [Sort(8)]
    public void ResetMaxPosition()
    {
        // refactor: move to DebugManager
        AppManager.I.Player.ResetMaxJourneyPosition();
        SRDebug.Instance.HideDebugPanel();
        SRDebug.Instance.ShowDebugPanel();
    }

    [Category("Rewards")]
    [Sort(1)]
    public void UnlockFirstReward()
    {
        RewardSystemManager.UnlockFirstSetOfRewards();
    }

    [Category("Rewards")]
    [Sort(2)]
    public void UnlockNextPlaysessionRewards()
    {
        // refactor: move to DebugManager

        //JourneyPosition CurrentJourney = AppManager.I.Player.CurrentJourneyPosition;
        foreach (RewardPackUnlockData pack in RewardSystemManager.GetNextRewardPack()) {
            AppManager.I.Player.AddRewardUnlocked(pack);
            Debug.LogFormat("Pack added: {0}", pack.ToString());
        }
        JourneyPosition next = AppManager.I.JourneyHelper.FindNextJourneyPosition(AppManager.I.Player.CurrentJourneyPosition);
        if (next != null) {
            AppManager.I.Player.SetMaxJourneyPosition(new JourneyPosition(next.Stage, next.LearningBlock, next.PlaySession));
            AppManager.I.Player.SetCurrentJourneyPosition(new JourneyPosition(next.Stage, next.LearningBlock, next.PlaySession));
        }
    }

    [Category("Rewards")]
    [Sort(3)]
    public void UnlockAllRewards()
    {
        RewardSystemManager.UnlockAllRewards();
    }
}
#endif