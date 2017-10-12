using Antura.Core;
using DG.Tweening;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    /// A dot on the map. Just visuals. 
    /// </summary>
    public class Dot : MonoBehaviour
    {
        [HideInInspector]
        public bool isLocked;

        [Header("References")]
        public Material blackDot;
        public Material redDot;

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

        /*
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
        */

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