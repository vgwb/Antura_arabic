using UnityEngine;

// refactor: this class should be part of the DontWakeUp minigame code
namespace EA4S.DontWakeUp
{
    public class DangerMeter : MonoBehaviour
    {

        float intensity;
        public CanvasGroup dangerCanvas;
        public GameObject dangerDog;

        void Start()
        {
            intensity = 0f;
        }

        public void SetIntensity(float newIntensity)
        {
            intensity = Mathf.Clamp(newIntensity, 0f, 1f);
            dangerCanvas.alpha = intensity;
        }

    }
}
