using System;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    public class AnswerSet
    {
        IAnswer[] correctAnswers;
        private static int totalCorrectAnswers = 0;

        public static void ResetTotalCount()
        {
            totalCorrectAnswers = 0;
        }

        public static int GetCorrectCount()
        {
            return totalCorrectAnswers;
        }

        public AnswerSet( IAnswer[] answers)
        {
            //Should filter only correct answers
            int count = 0;
            foreach (var answ in answers)
            {
                if (answ.IsCorrect())
                    count++;

            }

            totalCorrectAnswers += count;

            correctAnswers = new IAnswer[ count];
            int index = 0;
            foreach (var answ in answers)
                if (answ.IsCorrect())
                    correctAnswers[ index++]= answ;
        }

        List< IAnswer> currentAnswers = new List< IAnswer>();
         
        public void OnDroppedAnswer( IAnswer answer)
        {
            currentAnswers.Add( answer);
        }

        public void OnRemovedAnswer( IAnswer answer)
        {
            if (currentAnswers.Remove( answer) == false)
                throw new InvalidOperationException( "Cannot remove something that was not added");
        }

        public bool AllCorrect()
        {
            foreach (var correct in correctAnswers)   
            {
                bool found = false;

                foreach (var ci in currentAnswers)
                {
                    if ( correct.Equals( ci))
                        found = true;
                }

                if (!found)
                    return false;
            }
            return true;
        }

        internal bool IsCorrect( IAnswer answ)
        {
            foreach (var c in correctAnswers)
                if (c == answ)
                    return true;

            return false;
        }
    }
}
