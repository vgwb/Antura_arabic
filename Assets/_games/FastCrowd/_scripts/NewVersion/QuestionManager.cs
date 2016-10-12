using UnityEngine;

namespace EA4S.FastCrowd
{
    public class QuestionManager : MonoBehaviour
    {
        public DropAreaWidget dropContainer;
        public Crowd crowd;

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

        public void Clean()
        {
            dropContainer.Clean();
            crowd.Clean();
        }
    }
}
