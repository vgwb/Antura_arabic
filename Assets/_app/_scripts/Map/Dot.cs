using Antura.Core;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    /// A dot on the map. 
    /// Defines a non-assessment play session.
    /// </summary>
    public class Dot : MonoBehaviour, IMapLocation
    {
        public Vector3 Position
        {
            get { return transform.position; }
        }

        public JourneyPosition JourneyPos
        {
            get { return new JourneyPosition(stage, learningBlock, playSession); }
        }

        [HideInInspector]
        public int playerPosIndex;

        [HideInInspector]
        public int stage;
        [HideInInspector]
        public int learningBlock;
        [HideInInspector]
        public int playSession;

        [HideInInspector]
        public bool isLocked;

        [Header("References")]
        public Material blackDot;
        public Material redDot;

        public MeshRenderer scoreFeedbackRenderer;

        public void Initialise(int _stage, int _learningBlock, int _playSession)
        {
            stage = _stage;
            learningBlock = _learningBlock;
            playSession = _playSession;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ChangeMaterialDotToRed();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ChangeMaterialDotToBlack();
            }
        }

        public void ChangeMaterialDotToBlack()
        {
            GetComponent<Renderer>().material = blackDot;
        }

        public void ChangeMaterialDotToRed()
        {
            GetComponent<Renderer>().material = redDot;
        }

        public void SetUnlocked()
        {
            isLocked = false;
            gameObject.SetActive(true);
        }

        public void SetLocked()
        {
            isLocked = true;
            gameObject.SetActive(false);
        }

        public void SetPlaySessionState(PlaySessionState playSessionState)
        {
            // TODO: do something with the score

            int score = 0;
            if (playSessionState != null) score = playSessionState.score;

            var mat = scoreFeedbackRenderer.GetComponentInChildren<MeshRenderer>().material;
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
        }
    }
}