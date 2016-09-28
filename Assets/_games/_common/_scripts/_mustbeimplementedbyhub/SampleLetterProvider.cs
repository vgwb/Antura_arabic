// Written by Davide Barbieri <davide.barbieri AT ghostshark.it>

namespace EA4S
{
    /// <summary>
    /// Letter provider sample
    /// </summary>
    public class SampleLetterProvider : ILivingLetterDataProvider
    {
        public SampleLetterProvider(int difficulty)
        {

        }
        
        public ILivingLetterData GetNextData()
        {
            return AppManager.Instance.Teacher.GimmeARandomLetter();
        }
    }
}
