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
        public Vector3 Position { get { return transform.position; } }
        public JourneyPosition JourneyPos { get { return new JourneyPosition(stage, learningBlock, playSession);} }

        [HideInInspector]
        public int playerPosIndex;

        [HideInInspector]
        public int stage;
        [HideInInspector]
        public int learningBlock;
        [HideInInspector]
        public int playSession;

        [Header("References")]
        public Material blackDot;
        public Material redDot;

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
            gameObject.SetActive(true);
        }

        public void SetLocked()
        {
            gameObject.SetActive(false);
        }

    }
}