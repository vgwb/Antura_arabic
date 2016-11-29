// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/08/03 16:27
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace EA4S
{
    /// <summary>
    /// Used on the tutorial LivingLetter (instead of the default LivingLetter component)
    /// </summary>
    public class TutorialLetter : MonoBehaviour
    {
        [Header("Starting State")]
        public LivingLetterAnim StartupAnimation;
        [Header("References")]
        public GameObject TextGO; // Not really needed, but useful to jump to the object in the hierarchy

        public LivingLetterAnim CurrState { get; private set; }
        Animator animator;

        void Awake()
        {
            animator = this.GetComponentInChildren<Animator>(true);
        }

        void Start()
        {
            PlayAnimation(StartupAnimation);
        }

        public void PlayAnimation(LivingLetterAnim _anim)
        {
            CurrState = _anim;
            if (_anim == LivingLetterAnim.Nothing) animator.StopPlayback();
            else animator.Play(_anim.ToString());
        }
    }
}