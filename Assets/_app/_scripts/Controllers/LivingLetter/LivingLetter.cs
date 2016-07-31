using UnityEngine;
using System.Collections;
using EA4S;

namespace EA4S
{

    public enum LivingLetterAnim {
        Nothing = 0,
        idle = 1,
        hold = 2,
        run = 3,
        walk = 4,
        ninja = 5
    }

    public class LivingLetter : MonoBehaviour
    {
        [Header("Behavior")]
        public bool DisableAnimator = false;

        [Header("Starting State")]
        public LivingLetterAnim AnimationState;

        [Header("References")]
        public Animator AnturaAnimator;

        void Start()
        {
            if (DisableAnimator) {
                AnturaAnimator.enabled = false;
            }
            PlayAnimation();
        }

        void PlayAnimation()
        {
            if (!DisableAnimator) {
                if (AnimationState != LivingLetterAnim.Nothing) {
                    AnturaAnimator.Play(GetStateName(AnimationState));
                } else {
                    AnturaAnimator.StopPlayback();
                }
            }
        }

        string GetStateName(LivingLetterAnim state)
        {
            var stateName = "";
            switch (state) {
                case LivingLetterAnim.Nothing:
                    stateName = "";
                    break;
                case LivingLetterAnim.idle:
                    stateName = "idle";
                    break;
                case LivingLetterAnim.hold:
                    stateName = "hold";
                    break;
                case LivingLetterAnim.run:
                    stateName = "run";
                    break;
                case LivingLetterAnim.walk:
                    stateName = "walk";
                    break;
                case LivingLetterAnim.ninja:
                    stateName = "ninja";
                    break;
            }
            return stateName;
        }
    }
}