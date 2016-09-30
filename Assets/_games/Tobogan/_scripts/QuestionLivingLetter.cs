using UnityEngine;
using DG.Tweening;
using TMPro;

namespace EA4S.Tobogan
{
    class QuestionLivingLetter : MonoBehaviour
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

        Tweener moveTweener;
        Tweener rotationTweener;

        Vector3 holdPosition;
        Vector3 normalPosition;

        void Awake()
        {
            normalPosition = livingLetterTransform.localPosition;

            holdPosition.y = normalPosition.y + 5f;

            boxCollider.enabled = false;
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
            if(livingLetterData.DataType == LivingLetterDataType.Letter)
            {
                livingLetterText.text = ArabicAlphabetHelper.GetLetterFromUnicode(((LetterData)livingLetterData).Isolated_Unicode);
            }
            else
            {
                livingLetterText.text = ((WordData)livingLetterData).Word;
            }
        }

        public void ClearQuestionText()
        {
            livingLetterText.text = "";
        }

        public void MoveTo(Vector3 position, float time)
        {
            
        }

        public void RoteteTo(Vector3 rotation, float time)
        {

        }

        public void TransformTo(Transform transformTo, float time)
        {
            MoveTo(transformTo.position, time);
            RoteteTo(transformTo.eulerAngles, time);
        }

        void OnMouseDown()
        {
            //mousePosition = Input.mousePosition;
            //mousePosition.z = cameraDistance;

            //parentFloatingLetter.MouseOffset = parentFloatingLetter.transform.position - Camera.main.ScreenToWorldPoint(mousePosition);

            PlayHoldAnimation();
        }

        void OnMouseDrag()
        {
            //mousePosition = Input.mousePosition;
            //mousePosition.z = cameraDistance;

            //parentFloatingLetter.Drag(Camera.main.ScreenToWorldPoint(mousePosition));
        }

        void OnMouseUp()
        {
            PlayIdleAnimation();
        }

        public void Drag()
        {
            moveTweener.Kill();
            rotationTweener.Kill();
        }

        void EnableCollider(bool enable)
        {
            boxCollider.enabled = enable;
        }
    }
}
