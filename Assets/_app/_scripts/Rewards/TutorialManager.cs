using Antura.Profile;
using UnityEngine;

namespace Antura.Rewards
{
    /// <summary>
    /// Base class for tutorial managers inside scenes.
    /// </summary>
    public abstract class TutorialManager : MonoBehaviour
    {
        public static bool TEST_DISABLE_TUTORIAL = false;

        public bool IsRunning { get; protected set; }

        public void HandleStart()
        {
            if (!FirstContactManager.I.IsInsideFirstContact())
            {
                gameObject.SetActive(false);
                IsRunning = false;
                Debug.Log("First contact is not on");
                return;
            }

            // DEBUG: removing the tutorial for now
            if (TEST_DISABLE_TUTORIAL)
            {
                gameObject.SetActive(false);
                IsRunning = false;
                Debug.Log("DEBUG - tutorial removed");
                return;
            }

            Debug.Log("Tutorial is starting (phase " + FirstContactManager.I.CurrentPhase + ")");
            IsRunning = true;

            InternalHandleStart();
        }

        protected abstract void InternalHandleStart();
    }
}