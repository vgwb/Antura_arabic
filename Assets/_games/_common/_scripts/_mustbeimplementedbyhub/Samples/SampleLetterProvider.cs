using EA4S.Core;

namespace EA4S.MinigamesAPI.Sample
{
    /// <summary>
    /// Example implementation of ILivingLetterDataProvider.
    /// Not to be used in actual production code.
    /// </summary>
    public class SampleLetterProvider : ILivingLetterDataProvider
    {
        public SampleLetterProvider()
        {

        }
        
        public ILivingLetterData GetNextData()
        {
            return (AppManager.Instance as AppManager).Teacher.GetRandomTestLetterLL();
        }
    }
}
