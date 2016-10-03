using UnityEngine;
using System.Collections.Generic;

namespace EA4S.Tobogan
{
    public class PipesAnswerController : MonoBehaviour
    {
        public PipeAnswer[] pipeAnswers;

        PipeAnswer currentPipeAnswer;

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
            HidePipes();

            currentPipeAnswer = null;

            List<ILivingLetterData> wrongs = new List<ILivingLetterData>();

            foreach (ILivingLetterData answer in wrongAnswers)
            {
                wrongs.Add(answer);
            }

            int answersCount = wrongs.Count + 1;

            if (answersCount > 4)
            {
                answersCount = 4;
            }

            bool setCorrect = false;

            for (int i = 0; i < answersCount; i++)
            {
                if (!setCorrect)
                {
                    if (i < answersCount - 1)
                    {
                        setCorrect = Random.Range(0, 1) == 1;
                    }
                    else
                    {
                        setCorrect = true;
                    }
                }

                if (setCorrect)
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
    }
}
