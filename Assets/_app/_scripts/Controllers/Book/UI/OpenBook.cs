using UnityEngine;
using EA4S;
using ModularFramework.Core;

namespace EA4S
{
    public class OpenBook : MonoBehaviour
    {
        public void OpenPlayerBook()
        {
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Book");
        }
    }
}