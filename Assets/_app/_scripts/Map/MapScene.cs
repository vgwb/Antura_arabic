using Antura.Core;

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
            AppManager.I.NavigationManager.GoToNextScene();
        }
    }
}