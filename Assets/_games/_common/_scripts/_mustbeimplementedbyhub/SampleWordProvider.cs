

namespace EA4S
{
    /// <summary>
    /// Word provider sample
    /// </summary>
    public class SampleWordProvider : ILivingLetterDataProvider
    {
        public SampleWordProvider()
        {

        }

        public ILivingLetterData GetNextData()
        {
            return AppManager.Instance.Teacher.GetRandomTestWordDataLL();
        }
    }
}
