// Written by Davide Barbieri <davide.barbieri AT ghostshark.it>

namespace EA4S
{
    /// <summary>
    /// Implement here a word provider based on difficulty, etc.
    /// </summary>
    public class SampleLetterProvider : ILetterProvider
    {
        public SampleLetterProvider(int difficulty)
        {

        }
        
        public LetterData GetNextLetter()
        {
            return AppManager.Instance.Teacher.GimmeARandomLetter();
        }
    }
}
