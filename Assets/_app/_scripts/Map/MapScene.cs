using Antura.Core;
using Antura.Debugging;

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

            DebugManager.OnSkipCurrentScene += HandleSkipScene;
        }

        void OnDestroy()
        {
            DebugManager.OnSkipCurrentScene -= HandleSkipScene;
        }

        private void HandleSkipScene()
        {
            Play();
        }

        public void GoToAnturaSpace()
        {
            AppManager.I.NavigationManager.GoToAnturaSpace();
        }

        public void Play()
        {
            AppManager.I.NavigationManager.GoToNextScene();
        }
    }
}