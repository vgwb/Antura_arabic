using UnityEngine;
using EA4S;
using ModularFramework.Core;

namespace EA4S
{
    public class OpenPlayerBookScene : MonoBehaviour
    {
        public void OpenPlayerBook()
        {
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_PlayerBook");
        }
    }
}