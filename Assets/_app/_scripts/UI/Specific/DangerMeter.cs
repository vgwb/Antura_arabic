using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
