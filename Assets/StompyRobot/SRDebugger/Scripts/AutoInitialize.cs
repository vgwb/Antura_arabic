#if UNITY_5

namespace SRDebugger
{
    using UnityEngine;

    public static class AutoInitialize
    {
        [RuntimeInitializeOnLoadMethod]
        public static void OnLoad()
        {
            if (Settings.Instance.IsEnabled && Settings.Instance.AutoLoad)
            {
                SRDebug.Init();
            }
        }
    }
}

#endif
