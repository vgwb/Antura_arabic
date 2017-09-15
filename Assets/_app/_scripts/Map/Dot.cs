using Antura.Core;
using DG.Tweening;
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
                Highlight(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Highlight(false);
            }
        }

        public void Highlight(bool choice)
        {
            if (choice)
            {
                GetComponent<Renderer>().material = redDot;
            }
            else
            {
                GetComponent<Renderer>().material = blackDot;
            }
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
            scoreFeedbackRenderer.gameObject.SetActive(false);

            // TODO: do something with the score
            /*
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
            }*/
        }

        #region Appear / Disappear

        private bool appeared = false;

        public bool Appeared { get { return appeared;} }

        public void Disappear()
        {
            appeared = false;
            transform.localScale = Vector3.zero;
        }

        public void Appear(float delay, float duration)
        {
            if (appeared) return;
            appeared = true;
            transform.DOScale(Vector3.one * 1.5f, duration)
                .SetEase(Ease.OutElastic)
                .SetDelay(delay);
        }

        public void FlushAppear()
        {
            if (appeared) return;
            appeared = true;
            transform.localScale = Vector3.one * 1.5f;
        }

        #endregion
    }
}