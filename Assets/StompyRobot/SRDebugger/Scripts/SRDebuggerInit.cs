namespace SRDebugger
{
    using SRF;
    using UnityEngine;

    /// <summary>
    /// Add this component somewhere in your scene to automatically load SRDebugger when the scene is loaded.
    /// By default, SRDebugger will defer loading any UI except the corner-trigger until the user requests it.
    /// It is recommended to add this to the very first scene in your game. This will ensure the console log
    /// will hold useful information about your game initialization.
    /// </summary>
    [AddComponentMenu("SRDebugger Init")]
    public class SRDebuggerInit : SRMonoBehaviourEx
    {
        protected override void Awake()
        {
            base.Awake();

            if (!Settings.Instance.IsEnabled)
            {
                return;
            }

            SRDebug.Init();
        }

        protected override void Start()
        {
            base.Start();

            Destroy(CachedGameObject);
        }
    }
}
