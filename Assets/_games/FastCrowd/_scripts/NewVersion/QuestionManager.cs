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

        void Start()
        {
            dropContainer.OnComplete += OnContainerComplete;
            dropContainer.OnDropped += OnLetterDropped;
        }

        void OnContainerComplete()
        {
            if (OnCompleted != null)
                OnCompleted();
        }

        void OnLetterDropped(bool result)
        {
            if (OnDropped != null)
                OnDropped(result);
        }

        public void StartQuestion(List<ILivingLetterData> nextChallenge, List<ILivingLetterData> wrongAnswers)
        {
            Clean();

            foreach (var correctAnswer in nextChallenge)
            {
                // Add drop areas
                dropContainer.AddDropArea(correctAnswer);

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
            dropContainer.Clean();
            crowd.Clean();
        }
    }
}
