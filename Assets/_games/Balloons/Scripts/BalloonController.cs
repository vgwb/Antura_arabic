using UnityEngine;
using System.Collections;
using EA4S;

namespace Balloons
{
    public class BalloonController : MonoBehaviour
    {
        public FloatingLetterController parentFloatingLetter;
        public GameObject rope;
        public Collider balloonTopCollider;
        public Renderer balloonTopRenderer;

        private Animator animator;
        private AudioSource popAudio;
        private int taps = 0;

        void Start() {
            animator = GetComponent<Animator>();
            popAudio = GetComponent<AudioSource>();
        }

        public void OnMouseDown() {
            TapAction();
        }

        void TapAction() {
            taps++;
            if (taps >= parentFloatingLetter.tapsNeeded) {
                Pop();
            } else {
                animator.SetTrigger("Tap");
            }
        }

        public void Pop() {
            balloonTopCollider.enabled = false;
            rope.SetActive(false);
            parentFloatingLetter.Pop();
            AudioManager.I.PlaySound("Sfx/BalloonPop");
            popAudio.Play();
            animator.SetBool("Pop", true);
        }

        public void SetColor(Color color) {
            Debug.Log("COLOR");
            balloonTopRenderer.material.color = color;
        }
    }
}