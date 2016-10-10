

namespace EA4S
{
    /// <summary>
    /// Letter provider sample
    /// </summary>
    public class SampleLetterProvider : ILivingLetterDataProvider
    {
        public SampleLetterProvider(float difficulty)
        {

        }
        
        public ILivingLetterData GetNextData()
        {
            return AppManager.Instance.Teacher.GimmeARandomLetter();
        }
    }
}
