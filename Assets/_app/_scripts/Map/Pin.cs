using Antura.Core;
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
        public Rope rope;

        public GameObject pinV1;
        public GameObject pinV2;

        [HideInInspector]
        public GameObject currentPinMesh;

        [HideInInspector]
        public bool isLocked;

        public void Initialise(int _stage, int _learningBlock)
        {
            learningBlock = _learningBlock;
            if (_learningBlock % 2 == 0) {
                pinV2.gameObject.SetActive(false);
                currentPinMesh = pinV1;
            } else {
                pinV1.gameObject.SetActive(false);
                currentPinMesh = pinV2;
            }

            // The dot is set at the assessment
            dot.Initialise(_stage, _learningBlock, AppManager.I.JourneyHelper.AssessmentPlaySessionIndex);
        }

        public void SetUnlocked()
        {
            isLocked = false;
            dot.gameObject.SetActive(true);
        }
        public void SetLocked()
        {
            isLocked = true;
            dot.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player")) {
                currentPinMesh.SetActive(false);
                dot.ChangeMaterialDotToRed();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player")) {
                currentPinMesh.SetActive(true);
                dot.ChangeMaterialDotToBlack();
            }
        }

        public void SetPlaySessionState(PlaySessionState playSessionState)
        {
            // TODO: do something with the score

            //int score = 0;
            // if (playSessionState != null) score = playSessionState.score;

            /*
            var mat = currentPinMesh.GetComponentInChildren<MeshRenderer>().material;
            switch (score)
            {
                case 0:
                    mat.color = Color.black;
                    break;
                case 1:
                    mat.color = Color.red;
                    break;
                case 2:
                    mat.color = Color.blue;
                    break;
                case 3:
                    mat.color = Color.yellow;
                    break;
            }
            */
        }
    }
}