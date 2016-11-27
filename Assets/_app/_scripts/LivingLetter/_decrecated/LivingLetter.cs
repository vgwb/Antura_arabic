using UnityEngine;
using System.Collections;
using TMPro;
using EA4S;

namespace EA4S
{
    public enum LivingLetterAnim
    {
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
        public bool ClickToChangeAnimation = false;
        public bool ClickToChangeLetter = false;
        public bool ClickToSpeakLetter = false;
        public bool DisableAnimator = false;

        [Header("Starting State")]
        public LivingLetterAnim AnimationState;
        public bool ShowLetter = false;

        [Header("References")]
        public Animator AnturaAnimator;
        public GameObject TextGO;
        TextMeshPro myText;

        LL_LetterData letterData;

        void Start()
        {
            if (DisableAnimator) {
                AnturaAnimator.enabled = false;
            }
            myText = TextGO.GetComponent<TextMeshPro>();
            PlayAnimation();
            if (ShowLetter) {
                RandomLetter();
            } else {
                SetText("");
            }
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

        public void Clicked()
        {
            if (ClickToChangeAnimation)
                RandomAnimation();

            if (ClickToChangeAnimation)
                RandomLetter();

            if (ClickToSpeakLetter)
                SpeakLetter();
        }

        void RandomLetter()
        {
            letterData = AppManager.I.Teacher.GetRandomTestLetterLL();
            //Debug.Log(letterData.Key);
            SetText(letterData.TextForLivingLetter);
        }

        void SetText(string title)
        {
            myText.text = title;
        }

        private void SpeakLetter()
        {
            if (letterData != null) {
                AudioManager.I.PlayLetter(letterData.Id);
            }
        }

        void RandomAnimation()
        {
            LivingLetterAnim newAnimationState = LivingLetterAnim.Nothing;

            while ((newAnimationState == LivingLetterAnim.Nothing) || (newAnimationState == AnimationState)) {
                newAnimationState = RandomHelper.GetRandomEnum<LivingLetterAnim>();
            }

            AnimationState = newAnimationState;
            PlayAnimation();
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