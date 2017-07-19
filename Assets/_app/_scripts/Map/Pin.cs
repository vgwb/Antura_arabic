using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    /// A pin on the map. 
    /// Defines an assessment play session.
    /// </summary>
    public class Pin : MonoBehaviour
    {
        [HideInInspector]
        public int learningBlock;

        [Header("References")]
        public Dot dot;

        [HideInInspector]
        public Rope rope;   // rope assigned to the LB

        public GameObject pinV1;
        public GameObject pinV2;

        [HideInInspector]
        public GameObject currentPinMesh;

        public void Initialise(int _stage, int _learningBlock)
        {
            learningBlock = _learningBlock;
            if (_learningBlock % 2 == 0)
            {
                pinV2.gameObject.SetActive(false);
                currentPinMesh = pinV1;
            }
            else
            {
                pinV1.gameObject.SetActive(false);
                currentPinMesh = pinV2;
            }

            // The dot is set at the assessment
            dot.Initialise(_stage, _learningBlock, AppManager.I.JourneyHelper.AssessmentPlaySessionIndex);
        }

        public void SetUnlocked()
        {
            dot.gameObject.SetActive(true);
        }
        public void SetLocked()
        {
            dot.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                currentPinMesh.SetActive(false);
                dot.ChangeMaterialDotToRed();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                currentPinMesh.SetActive(true);
                dot.ChangeMaterialDotToBlack();
            }
        }

/*
        private void ChangeMaterialPinToBlack()
        {
            Dot.GetComponent<Renderer>().material = blackPin;
        }

        private void ChangeMaterialPinToRed()
        {
            Dot.GetComponent<Renderer>().material = redPin;
        }
        */
    }
}