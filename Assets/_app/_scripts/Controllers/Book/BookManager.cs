using UnityEngine;
using EA4S;
using ModularFramework.Core;

namespace EA4S
{
    public class BookManager : MonoBehaviour
    {



        public void OpenMap()
        {
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Map");
        }
    }
}