namespace EA4S
{
    /// <summary>
    /// Word provider sample
    /// </summary>
    public class SampleWordProvider : ILivingLetterDataProvider
    {
        public SampleWordProvider(float difficulty)
        {

        }

        public ILivingLetterData GetNextData()
        {
            return AppManager.Instance.Teacher.GimmeAGoodWordData();
        }
    }
}
