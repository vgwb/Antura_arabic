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

        public void StartNewQuestion()
        {
            quetionWordData = null;

            ILivingLetterData lLetterData = EggConfiguration.Instance.QuestionProvider.GetNextData();

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
                while (lLetterDataSequence.Count < 6)
                {
                    lLetterDataSequence.Add(lLetterData);

                    do
                    {
                        lLetterData = EggConfiguration.Instance.QuestionProvider.GetNextData();
                    } while (lLetterDataSequence.Contains(lLetterData));

                }
            }
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