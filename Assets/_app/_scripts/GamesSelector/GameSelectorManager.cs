using UnityEngine;

namespace EA4S.GamesSelector
{

    /// <summary>
    /// Manages the games selector scene, which allows the player to see what minigames will be played next.
    /// </summary>
    public class GameSelectorManager : MonoBehaviour
    {

        void Start()
        {
            AppManager.I.NavigationManager.CurrentScene = AppScene.GameSelector;
            GlobalUI.ShowPauseMenu(false);
            GlobalUI.ShowBackButton(true, ExitThisScene);
        }

        public void ExitThisScene()
        {
            // refactor: the NavigationManager should handle the back-target scene
            AppManager.I.NavigationManager.GoToScene(AppScene.Map);
        }
    }
}