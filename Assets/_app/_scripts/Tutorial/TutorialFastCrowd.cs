// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/08/03 18:15
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace EA4S
{
    /// <summary>
    /// AnimationManager for fast crowd tutorial
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class TutorialFastCrowd : MonoBehaviour
    {
        public TutorialLetter RightLetter, WrongLetter;

        // Animator animator;

        void Start()
        {
            // animator = this.GetComponent<Animator>();
        }

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ EVENTS CALLED FROM MECANIM

        public void WrongLetterStartDrag()
        {
            WrongLetter.PlayAnimation(LivingLetterAnim.hold);
        }

        public void WrongLetterStopDrag()
        {
            WrongLetter.PlayAnimation(LivingLetterAnim.run);
        }

        public void RightLetterStartDrag()
        {
            RightLetter.PlayAnimation(LivingLetterAnim.hold);
        }
    }
}