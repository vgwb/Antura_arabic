using UnityEngine;
using System.Collections.Generic;

namespace EA4S.Tobogan
{
    public class PipesAnswerController : MonoBehaviour
    {
        public PipeAnswer[] pipeAnswers;
        public Transform basePosition;

        ToboganGame game;
        float hidingProbability;

        PipeAnswer currentPipeAnswer;
        float hideSignsTimer = 0;

        List<PipeAnswer> toHide = new List<PipeAnswer>();

        public void SetSignHidingProbability(float hidingProbability)
        {
            this.hidingProbability = hidingProbability;
        }

        public void Initialize(ToboganGame game)
        {
            this.game = game;

            for (int i = 0; i < pipeAnswers.Length; i++)
            {
                pipeAnswers[i].onTriggerEnterPipe += OnTriggerEnterPipe;
                pipeAnswers[i].onTriggerExitPipe += OnTriggerExitPipe;
            }

            currentPipeAnswer = null;

            HidePipes();

            game.Context.GetInputManager().onPointerDown += OnPointerDown;
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

            hideSignsTimer = 1.5f + 0.5f * toHide.Count;
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
                pipeAnswers[i].ShowSign = true;
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

        public PipeAnswer GetCorrectPipeAnswer()
        {
            for(int i=0; i<pipeAnswers.Length; i++)
            {
                if(pipeAnswers[i].IsCorrectAnswer)
                {
                    return pipeAnswers[i];
                }
            }

            return null;
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

        void OnPointerDown()
        {
            // If not hiding, play letter sound when touching tube
            for (int i = 0; i < pipeAnswers.Length; ++i)
            {
                var pipe = pipeAnswers[i];
                if (pipe.active && pipe.ShowSign && pipe.Data != null)
                {
                    var pointerPosition = game.Context.GetInputManager().LastPointerPosition;
                    var screenRay = game.tubesCamera.ScreenPointToRay(pointerPosition);

                    RaycastHit hitInfo;
                    if (pipe.signCollider.Raycast(screenRay, out hitInfo, game.tubesCamera.farClipPlane))
                    {
                        game.Context.GetAudioManager().PlayLetterData(pipe.Data, true);
                    }
                }
            }
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
                        toHide[i].ShowSign = false;
                    }
                }
            }
        }
    }
}
