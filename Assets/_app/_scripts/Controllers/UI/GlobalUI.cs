// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/07/28 15:04
// License Copyright (c) Daniele Giardini

using DG.DemiLib.Attributes;
using UnityEngine;

namespace EA4S
{
    /// <summary>
    /// Global UI created dynamically at runtime,
    /// contains all global UI elements > Pause, SceneTransitioner, ContinueScreen, PopupScreen
    /// </summary>
    [ScriptExecutionOrder(-100)]
    public class GlobalUI : MonoBehaviour
    {
        public static GlobalUI I { get; private set; }

        public static SceneTransitioner SceneTransitioner { get; private set; }

        public static ContinueScreen ContinueScreen { get; private set; }

        public static WidgetPopupWindow WidgetPopupWindow { get; private set; }

        public static WidgetSubtitles WidgetSubtitles { get; private set; }

        const string ResourceId = "Prefabs/UI/GlobalUI";

        public static void Init()
        {
            if (I != null)
                return;

            GameObject go = Instantiate(Resources.Load<GameObject>(ResourceId));
            go.name = "[GlobalUI]";
            DontDestroyOnLoad(go);
        }

        void Awake()
        {
            I = this;

            // Awake all global UI elements
            SceneTransitioner = StoreAndAwake<SceneTransitioner>();
            ContinueScreen = StoreAndAwake<ContinueScreen>();
            WidgetPopupWindow = StoreAndAwake<WidgetPopupWindow>();
            WidgetSubtitles = StoreAndAwake<WidgetSubtitles>();
        }

        void OnDestroy()
        {
            I = null;
        }

        T StoreAndAwake<T>() where T : Component
        {
            T obj = this.GetComponentInChildren<T>(true);
            obj.gameObject.SetActive(true);
            return obj;
        }
    }
}