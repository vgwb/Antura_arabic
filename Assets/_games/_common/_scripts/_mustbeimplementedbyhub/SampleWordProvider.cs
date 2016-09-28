// Written by Davide Barbieri <davide.barbieri AT ghostshark.it>

namespace EA4S
{
    /// <summary>
    /// Word provider sample
    /// </summary>
    public class SampleWordProvider : ILivingLetterDataProvider
    {
        public SampleWordProvider(int difficulty)
        {

        }

        public ILivingLetterData GetNextData()
        {
            return AppManager.Instance.Teacher.GimmeAGoodWordData();
        }
    }
}
