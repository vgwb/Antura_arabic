using Antura.Core;
using DG.DeExtensions;
using DG.Tweening;
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

        private Transform shadowTr;

        [Header("Animation")]
        private Vector3 startPinPosition;
        private Vector3 startRopeScale;

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