using UnityEngine;

namespace EA4S.FastCrowd
{
    public class QuestionManager : MonoBehaviour
    {
        public event System.Action OnCompleted;

        public DropAreaWidget dropContainer;
        public Crowd crowd;

        IQuestionPack currentQuestion;

        void Start()
        {
            dropContainer.OnComplete += OnContainerComplete;
        }

        void OnContainerComplete()
        {
            if (OnCompleted != null)
                OnCompleted();
        }

        public void StartQuestion(IQuestionPack nextQuestion)
        {
            Clean();
            currentQuestion = nextQuestion;

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

        public void Clean()
        {
            currentQuestion = null;
            dropContainer.Clean();
            crowd.Clean();
        }
    }
}
