using UnityEngine;
using UnityEngine.UI;
using ModularFramework.Core;
using ModularFramework.Components;

namespace EA4S
{
    // refactor: was this used as a old way to call minigames? All access should be through the MiniGamesLauncher now.
    public class OnClickStartGame : MonoBehaviour
    {
        /// <summary>
        /// Gameplay info.
        /// </summary>
        public AnturaGameplayInfo GameplayInfo;

        void Start()
        {
            if (gameObject.GetComponent<Button>())
                gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        public void OnMouseDown()
        {
            OnClick();
        }

        public void OnClick()
        {
            GameManager.Instance.Modules.GameplayModule.GameplayStart(GameplayInfo);
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_" + GameplayInfo.GameId);
        }
    }
}