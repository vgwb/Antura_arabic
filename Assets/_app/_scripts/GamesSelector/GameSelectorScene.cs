using Antura.UI;
using Antura.Core;
using UnityEngine;

namespace Antura.GamesSelector
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