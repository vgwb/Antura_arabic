using System.Collections.Generic;

namespace EA4S.Egg
{
    public class QuestionManager
    {
        List<ILivingLetterData> lLetterDataSequence = new List<ILivingLetterData>();

        string questionDescription;

        bool sequence;

        public void StartNewQuestion(float difficulty, bool onlyLetter)
        {
            sequence = false;

            lLetterDataSequence.Clear();

            IQuestionPack questionPack = EggConfiguration.Instance.Questions.GetNextQuestion();

            questionDescription = "";

            List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
            List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

            foreach (ILivingLetterData letterData in questionPack.GetCorrectAnswers())
            {
                correctAnswers.Add(letterData);
            }

            foreach (ILivingLetterData letterData in questionPack.GetWrongAnswers())
            {
                wrongAnswers.Add(letterData);
            }

            if (wrongAnswers.Count == 0)
            {
                sequence = true;
            }

            int numberOfLetters = 2; 

            numberOfLetters += ((int)(difficulty * 5) + 1);

            if (numberOfLetters > 8)
            {
                numberOfLetters = 8;
            }

            if (!sequence)
            {
                lLetterDataSequence.Add(correctAnswers[0]);

                numberOfLetters += -1;

                if (numberOfLetters > wrongAnswers.Count)
                {
                    numberOfLetters = wrongAnswers.Count;
                }

                for (int i = 0; i < numberOfLetters; i++)
                {
                    lLetterDataSequence.Add(wrongAnswers[i]);
                }
            }
            else
            {
                if (numberOfLetters > correctAnswers.Count)
                {
                    numberOfLetters = correctAnswers.Count;
                }

                for (int i = 0; i < numberOfLetters; i++)
                {
                    lLetterDataSequence.Add(correctAnswers[i]);
                }
            }
        }

        public List<ILivingLetterData> GetlLetterDataSequence()
        {
            return lLetterDataSequence;
        }

        public bool IsSequence()
        {
            return sequence;
        }

        public string GetQuestionDescription()
        {
            return questionDescription;
        } 
    }
}