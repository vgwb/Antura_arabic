using UnityEngine;
using System.Collections;
using EA4S;

namespace Balloons
{
    public class BalloonController : MonoBehaviour
    {
        public FloatingLetterController parentFloatingLetter;
        public Collider balloonCollider;
        public Renderer balloonRenderer;
        public Animator animator;

        private int taps = 0;


        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void OnMouseDown()
        {
            TapAction();
        }

        void TapAction()
        {
            taps++;
            if (taps >= parentFloatingLetter.tapsNeeded)
            {
                Pop();
            }
            else
            {
                animator.SetTrigger("Tap");
            }
        }

        public void Pop()
        {
            balloonCollider.enabled = false;
            parentFloatingLetter.Pop();
            AudioManager.I.PlaySfx(Sfx.BaloonPop);
            animator.SetBool("Pop", true);
        }

        public void SetColor(Color color)
        {
            balloonRenderer.material.color = color;
        }
    }
}