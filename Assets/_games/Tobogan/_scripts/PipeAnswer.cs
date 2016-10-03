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

        bool trembling = false;
        Vector3 tremblingOffset;

        void Start()
        {
            StopSelectedAnimation();

            foreach (var particles in aspirationParticle.GetComponentsInChildren<ParticleSystem>(true))
            {
                particles.Clear();
            }

            aspirationParticle.SetActive(true);
            graphics.transform.localPosition = Vector3.up * 6;
        }

        public void Update()
        {
            Vector3 targetPosition = Vector3.zero;

            if (!active)
                targetPosition = Vector3.up * 6;

            graphics.transform.localPosition = tremblingOffset + Vector3.Lerp(graphics.transform.localPosition, targetPosition, 5.0f * Time.deltaTime);

            Vector3 tremblingTarget;

            if (trembling)
            {
                tremblingTarget = 0.03f * new Vector3(
                    Mathf.Cos(Mathf.Repeat(Time.realtimeSinceStartup * 317, 2 * Mathf.PI)),
                    Mathf.Cos(Mathf.Repeat(Time.realtimeSinceStartup * 601, 2 * Mathf.PI)),
                    Mathf.Cos(Mathf.Repeat(Time.realtimeSinceStartup * 363, 2 * Mathf.PI)));

                tremblingOffset = Vector3.Lerp(tremblingOffset, tremblingTarget, 50.0f * Time.deltaTime);
            }
            else
            {
                tremblingTarget = Vector3.zero;
                tremblingOffset = Vector3.Lerp(tremblingOffset, tremblingTarget, 5.0f * Time.deltaTime);
            }

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
            foreach (var particles in aspirationParticle.GetComponentsInChildren<ParticleSystem>(true))
            {
                particles.Play();
            }

            trembling = true;
        }

        public void StopSelectedAnimation()
        {
            foreach (var particles in aspirationParticle.GetComponentsInChildren<ParticleSystem>(true))
            {
                particles.Stop();
            }

            trembling = false;
        }
    }
}