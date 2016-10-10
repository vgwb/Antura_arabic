using UnityEngine;
using System.Collections.Generic;

namespace EA4S.Tobogan
{
    public class PipesAnswerController : MonoBehaviour
    {
        public PipeAnswer[] pipeAnswers;

        float hidingProbability;

        PipeAnswer currentPipeAnswer;
        float hideSignsTimer;

        List<PipeAnswer> toHide = new List<PipeAnswer>();

        public void SetSignHidingProbability(float hidingProbability)
        {
            this.hidingProbability = hidingProbability;
        }

        public void Initialize()
        {
            for (int i = 0; i < pipeAnswers.Length; i++)
            {
                pipeAnswers[i].onTriggerEnterPipe += OnTriggerEnterPipe;
                pipeAnswers[i].onTriggerExitPipe += OnTriggerExitPipe;
            }

            currentPipeAnswer = null;
        }

        public void SetPipeAnswers(IEnumerable<ILivingLetterData> wrongAnswers, ILivingLetterData correctAnswers)
        {
            // Selecting auto-hiding signs
            toHide.Clear();
            for (int i = 0; i < pipeAnswers.Length; ++i)
            {
                if (Random.value + 0.0001f < hidingProbability)
                {
                    toHide.Add(pipeAnswers[i]);
                }
            }

            hideSignsTimer = 2.5f + 0.5f * toHide.Count;

            HidePipes();

            currentPipeAnswer = null;

            List<ILivingLetterData> wrongs = new List<ILivingLetterData>();

            foreach (ILivingLetterData answer in wrongAnswers)
                wrongs.Add(answer);

            int answersCount = wrongs.Count + 1;

            if (answersCount > 4)
                answersCount = 4;

            int correctPosition = Random.Range(0, answersCount);

            for (int i = 0; i < answersCount; i++)
            {
                if (i == correctPosition)
                {
                    pipeAnswers[i].SetAnswer(correctAnswers, true);
                }
                else
                {
                    int wrongIndex = Random.Range(0, wrongs.Count);

                    pipeAnswers[i].SetAnswer(wrongs[wrongIndex], false);

                    wrongs.RemoveAt(wrongIndex);
                }

                //pipeAnswers[i].gameObject.SetActive(true);
                pipeAnswers[i].active = true;
                pipeAnswers[i].showSign = true;
            }
        }

        public void HidePipes()
        {
            for (int i = 0; i < pipeAnswers.Length; i++)
            {
                //pipeAnswers[i].gameObject.SetActive(false);
                pipeAnswers[i].active = false;
            }
        }

        public PipeAnswer GetCurrentPipeAnswer()
        {
            return currentPipeAnswer;
        }

        void OnTriggerEnterPipe(PipeAnswer pipe)
        {
            if (currentPipeAnswer != null)
            {
                if (currentPipeAnswer != pipe)
                {
                    currentPipeAnswer.StopSelectedAnimation();
                    pipe.PlaySelectedAnimation();
                }
            }
            else
            {
                pipe.PlaySelectedAnimation();
            }

            currentPipeAnswer = pipe;
        }

        void OnTriggerExitPipe(PipeAnswer pipe)
        {
            pipe.StopSelectedAnimation();

            currentPipeAnswer = null;
        }

        void Update()
        {
            if (hideSignsTimer > 0)
            {
                hideSignsTimer -= Time.deltaTime;

                if (hideSignsTimer <= 0)
                {
                    int len = toHide.Count;

                    for (int i = 0; i < len; i++)
                    {
                        toHide[i].showSign = false;
                    }
                }
            }
        }
    }
}
