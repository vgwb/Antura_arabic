using ArabicSupport;
using TMPro;
using UnityEngine;

namespace EA4S.Egg
{
    public class EggLivingLetter : MonoBehaviour
    {
        const string idleAnimation = "idle";
        const string holdAnimation = "hold";
        const string runAnimation = "run";
        const string walkAnimation = "walk";
        const string ninjaAnimation = "ninja";

        public Transform livingLetterTransform;
        public BoxCollider boxCollider;

        public Animator AnturaAnimator;

        public TextMeshPro livingLetterText;

        Vector3 holdPosition;
        Vector3 normalPosition;

        void Awake()
        {
            normalPosition = livingLetterTransform.localPosition;

            holdPosition.x = normalPosition.x + 1f;
            holdPosition.y = normalPosition.y + 5f;

            PlayIdleAnimation();
        }

        public void PlayIdleAnimation()
        {
            PlayAnimation(idleAnimation);

            livingLetterTransform.localPosition = normalPosition;
        }

        public void PlayWalkAnimation()
        {
            PlayAnimation(walkAnimation);

            livingLetterTransform.localPosition = normalPosition;
        }

        public void PlayHoldAnimation()
        {
            PlayAnimation(holdAnimation);

            livingLetterTransform.localPosition = holdPosition;
        }

        void PlayAnimation(string animation)
        {
            AnturaAnimator.Play(animation);
        }

        public void SetQuestionText(ILivingLetterData livingLetterData)
        {
            if (livingLetterData.DataType == LivingLetterDataType.Letter)
            {
                livingLetterText.text = ArabicAlphabetHelper.GetLetterFromUnicode(((LetterData)livingLetterData).Isolated_Unicode);
            }
            else
            {
                livingLetterText.text = ArabicFixer.Fix(((WordData)livingLetterData).Word, false, false);
            }
        }

        public void ClearQuestionText()
        {
            livingLetterText.text = "";
        }
    }
}
