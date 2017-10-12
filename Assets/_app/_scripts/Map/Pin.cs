using Antura.Core;
using DG.DeExtensions;
using DG.Tweening;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    /// A pin on the map. 
    /// Defines a PlaySession. Either assessment or minigame.
    /// </summary>
    public class Pin : MonoBehaviour, IMapLocation
    {
        [HideInInspector]
        public JourneyPosition journeyPosition;

        [Header("References")]
        public Dot dot;     // DEPRECATED (only for visual purposes)

        [HideInInspector]
        public Rope rope;

        public GameObject pinV1;
        public GameObject pinV2;
        public GameObject pinAssessment;

        [HideInInspector]
        public GameObject currentPinMesh;

        [HideInInspector]
        public bool isLocked;

        private Transform shadowTr;

        [Header("Animation")]
        private Vector3 startPinPosition;
        private Vector3 startRopeScale;

        public Vector3 Position
        {
            get { return transform.position; }
        }

        public JourneyPosition JourneyPos
        {
            get { return journeyPosition; }
        }

        public int pinIndex;

        public void Initialise(int _pinIndex, JourneyPosition _journeyPosition) //int _stage, int _learningBlock, int _playSession)
        {
            pinIndex = _pinIndex;
            journeyPosition = _journeyPosition;

            name = "Pin_" + _journeyPosition.ToString();

            // Coloring of the PIN based on the journey position
            pinV1.gameObject.SetActive(false);
            pinV2.gameObject.SetActive(false);
            pinAssessment.gameObject.SetActive(false);
            if (journeyPosition.IsAssessment())
            {
                currentPinMesh = pinAssessment;
            }
            else
            {
                if (journeyPosition.LearningBlock % 2 == 0)
                {
                    currentPinMesh = pinV1;
                }
                else {
                    currentPinMesh = pinV2;
                }
            }
            currentPinMesh.gameObject.SetActive(true);

            // TODO: no more dots?
            // The dot is set at the assessment
            //dot.Initialise(journeyPosition);

            shadowTr = transform.Find("shadow");
        }

        #region Appear / Disappear

        private bool appeared = false;

        public bool Appeared { get { return appeared; } }

        public void Disappear()
        {
            appeared = false;
            startPinPosition = currentPinMesh.transform.position;
            if (rope != null) startRopeScale = rope.meshRenderer.transform.localScale;

            currentPinMesh.transform.position = startPinPosition + Vector3.up * 60;
            dot.transform.SetLocalScale(0);
            if (rope != null)
            {
                rope.meshRenderer.transform.SetLocalScale(0);
                foreach (var ropeDot in rope.dots)
                    ropeDot.Disappear();
            }

            shadowTr.SetLocalScale(0);
        }

        public void Appear(float duration)
        {
            if (appeared) return;
            appeared = true;
            currentPinMesh.transform.DOMove(startPinPosition, duration*0.5f);
            dot.transform.DOScale(Vector3.one * 6, duration * 0.5f).SetEase(Ease.OutElastic).SetDelay(duration * 0.5f);
            shadowTr.DOScale(Vector3.one * 12.5f, duration * 0.5f).SetEase(Ease.OutElastic).SetDelay(duration * 0.5f);

            if (rope != null)
            {
                rope.meshRenderer.transform.DOScale(startRopeScale, duration * 0.5f).SetDelay(duration * 0.5f);
            }
        }

        public void FlushAppear()
        {
            if (appeared) return;
            appeared = true;
            currentPinMesh.transform.position = startPinPosition;
            dot.transform.localScale = Vector3.one*6;
            shadowTr.transform.localScale = Vector3.one * 12.5f;

            if (rope != null)
            {
                rope.meshRenderer.transform.localScale = startRopeScale;
            }
        }

        #endregion

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
            currentPinMesh.SetActive(!choice);
            dot.Highlight(choice);
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