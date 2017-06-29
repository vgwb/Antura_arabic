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
    [Category("Options")]
    [Sort(80)]
    public void ToggleQuality()
    {
        // refactor: move to DebugManager
        AppManager.I.ToggleQualitygfx();
        SRDebug.Instance.HideDebugPanel();
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

}
#endif