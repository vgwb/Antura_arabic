using Antura.Audio;
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
        //public int SequenceIndex { get { return playerPosIndex; } }
        public Vector3 Position { get { return transform.position; } }
        public JourneyPosition JourneyPos { get { return new JourneyPosition(stage, learningBlock, playSession);} }

        //[HideInInspector]
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
                if (other.gameObject.GetComponent<PlayerPin>().playerOverDotPin)
                {
                    AudioManager.I.PlaySound(Sfx.UIButtonClick);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ChangeMaterialDotToBlack();
            }
        }

        private void ChangeMaterialDotToBlack()
        {
            GetComponent<Renderer>().material = blackDot;
        }

        private void ChangeMaterialDotToRed()
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