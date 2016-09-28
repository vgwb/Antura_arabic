// Written by Davide Barbieri <davide.barbieri AT ghostshark.it>

namespace EA4S
{
    /// <summary>
    /// Implement here a word provider based on difficulty, etc.
    /// </summary>
    public class SampleWordProvider : IWordProvider
    {
        public SampleWordProvider(int difficulty)
        {

        }

        public WordData GetNextWord()
        {
            return AppManager.Instance.Teacher.GimmeAGoodWordData();
        }
    }
}
