using EA4S.Audio;
using EA4S.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EA4S.Core
{
    /// <summary>
    /// Takes care of generating managers when needed.
    /// Tied to the AppManager.
    /// </summary>
    // refactor: standardize Manager access between this and AppManager
    public class InstantiateManagers : MonoBehaviour
    {
        public GameObject AudioManager;
        public GameObject EventsManager;

        void Awake()
        {
            if (FindObjectOfType(typeof(AudioManager)) == null) {
                Instantiate(AudioManager);
            }

            if (FindObjectOfType(typeof(EventSystem)) == null) {
                Instantiate(EventsManager);
            }

            // init the mighty GlobalUI
            GlobalUI.Init();
        }
    }
}