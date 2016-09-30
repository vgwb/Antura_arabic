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

        public LivingLetter livingLetter;

        TextMeshPro livingLetterText;

        Tweener moveTweener;
        Tweener rotationTweener;

        void Start()
        {
            livingLetterText = livingLetter.TextGO.GetComponent<TextMeshPro>();
        }

        public void PlayIdleAnimation()
        {
            PlayAnimation(idleAnimation);
        }

        public void PlayWalkAnimation()
        {
            PlayAnimation(walkAnimation);
        }

        public void PlayHoldAnimation()
        {
            PlayAnimation(holdAnimation);
        }

        void PlayAnimation(string animation)
        {
            livingLetter.AnturaAnimator.Play(animation);
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
        }

        void OnMouseDrag()
        {
            //mousePosition = Input.mousePosition;
            //mousePosition.z = cameraDistance;

            //parentFloatingLetter.Drag(Camera.main.ScreenToWorldPoint(mousePosition));
        }

        void OnMouseUp()
        {

        }

        public void Drag()
        {
            moveTweener.Kill();
            rotationTweener.Kill();
        }
    }
}
