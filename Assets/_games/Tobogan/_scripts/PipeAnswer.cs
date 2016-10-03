using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace EA4S.Tobogan
{
    public class PipeAnswer : MonoBehaviour
    {
        public TMP_Text answerText;
        public Image answerImage;

        public GameObject aspirationParticle;
        public GameObject graphics;

        public bool IsCorrectAnswer { get; private set; }

        public event Action<PipeAnswer> onTriggerEnterPipe;
        public event Action<PipeAnswer> onTriggerExitPipe;

        public bool active;

        void Start()
        {
            aspirationParticle.SetActive(false);
            graphics.transform.localPosition = Vector3.up * 6;
        }

        public void Update()
        {
            Vector3 targetPosition = Vector3.zero;

            if (!active)
                targetPosition = Vector3.up*6;

            graphics.transform.localPosition = Vector3.Lerp(graphics.transform.localPosition, targetPosition, 5.0f*Time.deltaTime);
        }

        public void SetAnswer(ILivingLetterData livingLetterData, bool correct)
        {
            if (livingLetterData.DataType == LivingLetterDataType.Letter)
            {
                answerText.gameObject.SetActive(true);
                //answerImage.gameObject.SetActive(false);

                answerText.text = ArabicAlphabetHelper.GetLetterFromUnicode(((LetterData)livingLetterData).Isolated_Unicode);
            }
            else if (livingLetterData.DataType == LivingLetterDataType.Letter)
            {
                answerText.gameObject.SetActive(true);
                //answerImage.gameObject.SetActive(false);

                answerText.text = ((WordData)livingLetterData).Word;
            }
            //else
            //{
            //    answerImage.gameObject.SetActive(true);
            //    answerImage.sprite = livingLetterData.DrawForLivingLetter;

            //    answerText.gameObject.SetActive(false);
            //}
            IsCorrectAnswer = correct;
        }

        void OnTriggerEnter(Collider other)
        {
            if (onTriggerEnterPipe != null)
            {
                onTriggerEnterPipe(this);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (onTriggerExitPipe != null)
            {
                onTriggerExitPipe(this);
            }
        }

        public void EnterAnimation()
        {

        }

        public void ExitAnimation()
        {

        }

        public void PlaySelectedAnimation()
        {
            aspirationParticle.SetActive(true);
        }

        public void StopSelectedAnimation()
        {
            aspirationParticle.SetActive(false);
        }
    }
}