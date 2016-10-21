using System.Collections.Generic;
using UnityEngine;

namespace EA4S.FastCrowd
{
    public class QuestionManager : MonoBehaviour
    {
        public event System.Action OnCompleted;
        public event System.Action<bool> OnDropped;

        public DropAreaWidget dropContainer;
        public Crowd crowd;
        public WordComposer wordComposer;

        void Start()
        {
            dropContainer.OnComplete += OnContainerComplete;
            crowd.onDropped += OnLetterDropped;
        }

        void OnContainerComplete()
        {
            if (OnCompleted != null)
                OnCompleted();
        }

        void OnLetterDropped(ILivingLetterData data, bool result)
        {
            if (OnDropped != null)
                OnDropped(result);

            if (result)
            {
                dropContainer.AdvanceArea();

                if (data is LetterData)
                    wordComposer.AddLetter(data);
            }
        }

        public void StartQuestion(List<ILivingLetterData> nextChallenge, List<ILivingLetterData> wrongAnswers, int id)
        {
            Clean();
            
            for (int i = 0; i < nextChallenge.Count; ++i)
            {
                var correctAnswer = nextChallenge[i];

                // Add drop areas
                if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Counting)
                    dropContainer.AddDropText(correctAnswer, id.ToString());
                else if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Words)
                    dropContainer.AddDropData(correctAnswer, true);
                else
                    dropContainer.AddDropData(correctAnswer, false);

                // Add living letters
                crowd.AddLivingLetter(correctAnswer);
            }

            foreach (var wrongAnswer in wrongAnswers)
            {
                // Add living letters
                crowd.AddLivingLetter(wrongAnswer);
            }
        }

        /*
        public void StartQuestion(IQuestionPack nextQuestion)
        {
            Clean();

            foreach (var correctAnswer in nextQuestion.GetCorrectAnswers())
            {
                // Add drop areas
                dropContainer.AddDropArea(correctAnswer);

                // Add living letters
                crowd.AddLivingLetter(correctAnswer);
            }

            foreach (var wrongAnswers in nextQuestion.GetWrongAnswers())
            {
                // Add living letters
                crowd.AddLivingLetter(wrongAnswers);
            }
        }
        */

        public void Clean()
        {
            wordComposer.Clean();
            dropContainer.Clean();
            crowd.Clean();
        }
    }
}
