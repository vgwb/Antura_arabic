using UnityEngine;
using ModularFramework.Core;
using ModularFramework.Components;

namespace CGL.Antura.Components {

    public class OnClickStartGame : OnClickButtonChangeScene {
        public override void OnClick() {
            string sceneTypeName = getSelectedSceneName();
            string gameId = getSelectedGameID();

            if (!CheckIfSceneExist())
                Debug.LogError("Scene attempt to load not found. Check Build settings.");

            string sceneName = getFullSelectedSceneName();
            GameManager.Instance.Modules.GameplayModule.ActualGameplayInfo = new AnturaGameplayInfo() { GameId = gameId };
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition(sceneName);
        }
    }
}
