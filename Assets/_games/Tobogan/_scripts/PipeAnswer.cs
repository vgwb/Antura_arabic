using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using ArabicSupport;

namespace EA4S.Tobogan
{
    public class PipeAnswer : MonoBehaviour
    {
        public TMP_Text answerText;
        public Image answerImage;

        public GameObject aspirationParticle;
        public GameObject graphics;
        public TremblingTube trembling;

        public Transform signTransform;

        public bool IsCorrectAnswer { get; private set; }

        public event Action<PipeAnswer> onTriggerEnterPipe;
        public event Action<PipeAnswer> onTriggerExitPipe;

        public bool active;
        public bool showSign = true;

        const float DISAPPEAR_HEIGHT = 6.5f;
        float disappearSpeed;

        void Start()
        {
            StopSelectedAnimation();

            foreach (var particles in aspirationParticle.GetComponentsInChildren<ParticleSystem>(true))
            {
                particles.Clear();
            }

            aspirationParticle.SetActive(true);
            graphics.transform.localPosition = Vector3.up * DISAPPEAR_HEIGHT;
            disappearSpeed = 4.0f + 2 * UnityEngine.Random.value;
        }

        public void Update()
        {
            Vector3 targetPosition = Vector3.zero;

            if (!active)
                targetPosition = Vector3.up * DISAPPEAR_HEIGHT;

            graphics.transform.localPosition = Vector3.Lerp(graphics.transform.localPosition, targetPosition, disappearSpeed * Time.deltaTime);
            
           
            Quaternion signTargetRotation = Quaternion.identity;
            float targetAlpha = 1;

            if (!showSign)
            { 
                signTargetRotation = Quaternion.Euler(0, 90, 0);
                targetAlpha = 0;
            }

            answerText.alpha = Mathf.Lerp(answerText.alpha, targetAlpha, Time.deltaTime * 5.0f);
            signTransform.localRotation = Quaternion.Slerp(signTransform.localRotation, signTargetRotation, Time.deltaTime * 5.0f);
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

                answerText.text = ArabicFixer.Fix(((WordData)livingLetterData).Word, false, false);
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
            foreach (var particles in aspirationParticle.GetComponentsInChildren<ParticleSystem>(true))
            {
                particles.Play();
            }

            trembling.Trembling = true;
        }

        public void StopSelectedAnimation()
        {
            foreach (var particles in aspirationParticle.GetComponentsInChildren<ParticleSystem>(true))
            {
                particles.Stop();
            }

            trembling.Trembling = false;
        }
    }
}