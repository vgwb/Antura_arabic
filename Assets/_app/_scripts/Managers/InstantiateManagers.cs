﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace EA4S
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
        public GameObject TouchManager;

        void Awake()
        {
            if (FindObjectOfType(typeof(AudioManager)) == null) {
                Instantiate(AudioManager);
            }

            if (FindObjectOfType(typeof(EventSystem)) == null) {
                Instantiate(EventsManager);
            }

            if (FindObjectOfType(typeof(Lean.Touch.LeanTouch)) == null) {
                Instantiate(TouchManager);
            }

            // init the mighty GlobalUI
            GlobalUI.Init();
        }
    }
}