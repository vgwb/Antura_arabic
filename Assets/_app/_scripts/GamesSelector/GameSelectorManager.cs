using UnityEngine;

namespace EA4S
{
    public class GameSelectorManager : MonoBehaviour
    {

        void Start()
        {
            NavigationManager.I.CurrentScene = AppScene.GameSelector;
            GlobalUI.ShowPauseMenu(false);
            GlobalUI.ShowBackButton(true, ExitThisScene);
        }

        public void ExitThisScene()
        {
            NavigationManager.I.GoToScene(AppScene.Map);
        }
    }
}