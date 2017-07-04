using EA4S.Core;
using EA4S.UI;
using UnityEngine;

namespace EA4S.GamesSelector
{

    /// <summary>
    /// Manages the games selector scene, which allows the player to see what minigames will be played next.
    /// </summary>
    public class GameSelectorScene : SceneBase
    {

        protected override void Start()
        {
            base.Start();
            GlobalUI.ShowPauseMenu(false);
            GlobalUI.ShowBackButton(true);
        }

    }
}