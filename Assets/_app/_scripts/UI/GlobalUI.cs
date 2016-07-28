// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/07/28 15:04
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace EA4S
{
    /// <summary>
    /// Global UI created dynamically at runtime,
    /// contains all global UI elements > Pause, SceneTransitioner, ContinueScreen, PopupScreen
    /// </summary>
    public class GlobalUI : MonoBehaviour
    {
        public static GlobalUI I { get; private set; }
        public static SceneTransitioner SceneTransitioner { get; private set; }
        public static ContinueScreen ContinueScreen { get; private set; }

        const string ResourceId = "Prefabs/UI/GlobalUI";

        public static void Init()
        {
            if (I != null) return;

            GameObject go = Instantiate(Resources.Load<GameObject>(ResourceId));
            go.name = "[GlobalUI]";
            DontDestroyOnLoad(go);
        }

        void Awake()
        {
            I = this;

            // Awake all global UI elements
            SceneTransitioner = this.GetComponentInChildren<SceneTransitioner>(true);
            SceneTransitioner.gameObject.SetActive(true);
            ContinueScreen = this.GetComponentInChildren<ContinueScreen>(true);
            ContinueScreen.gameObject.SetActive(true);
        }

        void OnDestroy()
        {
            I = null;
        }
    }
}