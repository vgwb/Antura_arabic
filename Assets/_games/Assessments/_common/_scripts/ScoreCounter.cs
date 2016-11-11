using System;

namespace EA4S.Assessment
{
    public class ScoreCounter
    {
        readonly int arraySize = 10;
        int[] groups;
        int[] numerosity;
        int[] correct;

        int maxCorrectAnswers = 0;
        int currentCorrectAnswers = 0;
        int currentErrors = 0;

        public ScoreCounter()
        {
            groups = new int[arraySize];
            numerosity = new int[arraySize];
            correct = new int[arraySize];

            ResetArrays();
        }

        private void ResetArrays()
        {
            for(int i=0; i<arraySize; i++)
            {
                groups[i] = 0;
                numerosity[i] = 0;
                correct[i] = 0;
            }
        }

        public void AddGroup(int groupID, int i)
        {
            groups[groupID - 1] = groupID;
            numerosity[groupID - 1] = i;
            correct[groupID - 1] = 0;
            maxCorrectAnswers += i;
        }

        public void CorrectAnswer( int groupID)
        {
            correct[groupID - 1]++;
            currentCorrectAnswers++;
        }

        public void WrongAnswer(int groupID)
        {
            currentErrors++;
        }

        public int GetCorrectAnswers()
        {
            return currentCorrectAnswers;
        }

        public int GetWrongAnswers()
        {
            return currentErrors;
        }

        public int GetMaxScore()
        {
            return maxCorrectAnswers;
        }

        public bool AnsweredAll()
        {
            bool complete = true;
            for(int i=0; i<arraySize; i++)
            {
                if (groups[i] == i + 1) //group added
                {
                    if (numerosity[i] > correct[i])
                        complete = false;

                    if (numerosity[i] < correct[i])
                        throw new InvalidOperationException( "Cannot have more correct answers than possible answers");
                }
                else
                    break; // no more registered Groups
            }

            return complete;
        }

        public void ResetRound()
        {
            ResetArrays();
        }
    }
}
