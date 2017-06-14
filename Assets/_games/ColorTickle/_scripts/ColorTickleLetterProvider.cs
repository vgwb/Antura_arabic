using System.Collections.Generic;
using EA4S.Core;
using EA4S.MinigamesAPI;
using EA4S.MinigamesAPI.Sample;
using EA4S.Teacher;

namespace EA4S.Minigames.ColorTickle
{
    /// <summary>
    /// Letter provider sample
    /// </summary>
    public class ColorTickleLetterProvider : IQuestionProvider
    {
        List<IQuestionPack> m_oPacks;
        int m_iCurrentQuestion=0;
        public ColorTickleLetterProvider()
        {
            m_oPacks = new List<IQuestionPack>();
            //10 questions
            for (int i = 0; i < 10; i++)
            {
                List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
                List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

                LL_LetterData _NewLetterData = (AppManager.Instance as AppManager).Teacher.GetRandomTestLetterLL(new LetterFilters(excludeDiacritics:LetterFilters.ExcludeDiacritics.All));

                if (_NewLetterData == null)
                    return;

                correctAnswers.Add(_NewLetterData);

                SampleQuestionPack currentPack = new SampleQuestionPack(_NewLetterData, wrongAnswers, correctAnswers);
                m_oPacks.Add(currentPack);
            }



        }

        IQuestionPack IQuestionProvider.GetNextQuestion()
        {
            if (m_iCurrentQuestion >= m_oPacks.Count)
                m_iCurrentQuestion = 0;

            return m_oPacks[m_iCurrentQuestion++];
        }
    }
}
