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
        [Header("References")]
        public Material blackDot;
        public Material redDot;

        // Configuration
        public static bool highlightOnPlayerCollision = false;

        private void OnTriggerEnter(Collider other)
        {
            if (highlightOnPlayerCollision && other.gameObject.CompareTag("Player"))
            {
                Highlight(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (highlightOnPlayerCollision && other.gameObject.CompareTag("Player"))
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

        #region Appear / Disappear

        public bool Appeared { get; private set; }

        public void Disappear()
        {
            Appeared = false;
            transform.localScale = Vector3.zero;
        }

        public void Appear(float delay, float duration)
        {
            if (Appeared) return;
            Appeared = true;
            transform.DOScale(Vector3.one * 1.5f, duration)
                .SetEase(Ease.OutElastic)
                .SetDelay(delay);
        }

        public void FlushAppear()
        {
            if (Appeared) return;
            Appeared = true;
            transform.localScale = Vector3.one * 1.5f;
        }

        #endregion
    }
}