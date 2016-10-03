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

        public ParticleSystem aspirationParticle;
        ParticleSystem.EmissionModule emissionModule;

        public bool IsCorrectAnswer { get; private set; }

        public event Action<PipeAnswer> onTriggerEnterPipe;
        public event Action<PipeAnswer> onTriggerExitPipe;

        void Start()
        {
            aspirationParticle.Stop();
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
            aspirationParticle.Play();
        }

        public void StopSelectedAnimation()
        {
            aspirationParticle.Stop();
        }
    }
}