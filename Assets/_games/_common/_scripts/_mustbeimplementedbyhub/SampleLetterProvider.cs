

namespace EA4S
{
    /// <summary>
    /// Letter provider sample
    /// </summary>
    public class SampleLetterProvider : ILivingLetterDataProvider
    {
        public SampleLetterProvider()
        {

        }
        
        public ILivingLetterData GetNextData()
        {
            return AppManager.I.Teacher.GetRandomTestLetterLL();
        }
    }
}
