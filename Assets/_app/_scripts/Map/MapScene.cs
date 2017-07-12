using Antura.Core;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    /// Manages the Map scene, from which the next Play Session can be started.
    /// </summary>
    public class MapScene : SceneBase
    {
        protected override void Start()
        {
            base.Start();
            //KeeperManager.I.PlayDialog(Db.LocalizationDataId.Map_Intro);
        }

        public void GoToAnturaSpace()
        {
            AppManager.I.NavigationManager.GoToAnturaSpace();
        }

        public void Play()
        {
            // TODO refactor: move this initalisation to a better place, maybe inside the MiniGameLauncher.
            AppManager.I.NavigationManager.GoToNextScene();
        }
    }
}