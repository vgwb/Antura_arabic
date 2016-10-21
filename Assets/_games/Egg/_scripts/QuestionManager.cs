using System.Collections.Generic;

namespace EA4S.Egg
{
    public class QuestionManager
    {
        EggGame game;

        List<ILivingLetterData> lLetterDataSequence = new List<ILivingLetterData>();

        WordData quetionWordData;

        public QuestionManager(EggGame game)
        {
            this.game = game;
        }

        public void StartNewQuestion(int difficulty, bool onlyLetter)
        {
            quetionWordData = null;
            lLetterDataSequence.Clear();

            if(difficulty > 5)
            {
                difficulty = 5;
            }

            ILivingLetterData lLetterData = GetNextData(onlyLetter);

            if (lLetterData.DataType == LivingLetterDataType.Word)
            {
                quetionWordData = ((WordData)lLetterData);

                foreach (LetterData letter in ArabicAlphabetHelper.LetterDataListFromWord(quetionWordData.Word, AppManager.Instance.Letters))
                {
                    lLetterDataSequence.Add(letter);
                }
            }
            else if (lLetterData.DataType == LivingLetterDataType.Letter)
            {
                while (lLetterDataSequence.Count < 3 + difficulty)
                {
                    lLetterDataSequence.Add(lLetterData);

                    do
                    {
                        lLetterData = GetNextData(onlyLetter);
                    } while (lLetterDataSequence.Contains(lLetterData));

                }
            }
        }

        ILivingLetterData GetNextData(bool onlyLetter)
        {
            ((SampleEggQuestionProvider)EggConfiguration.Instance.QuestionProvider).SetOnlyLetter(onlyLetter);

            //return EggConfiguration.Instance.QuestionProvider.GetNextData();
            return null;
        }

        public List<ILivingLetterData> GetlLetterDataSequence()
        {
            return lLetterDataSequence;
        }

        public WordData GetQuestionWordData()
        {
            return quetionWordData;
        }
    }
}