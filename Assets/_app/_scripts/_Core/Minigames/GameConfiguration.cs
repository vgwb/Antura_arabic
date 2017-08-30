namespace Antura.Core
{
    /// <summary>
    /// Data passed to a minigame to configure it. 
    /// </summary>
    public class MinigameLaunchConfiguration
    {
        public float Difficulty;
        public int NumberOfRounds;
        public bool TutorialEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinigameLaunchConfiguration"/> class.
        /// </summary>
        public MinigameLaunchConfiguration(float _Difficulty = 0, int _NumberOfRounds = 1, bool tutorialEnabled = true)
        {
            Difficulty = _Difficulty;
            NumberOfRounds = _NumberOfRounds;
            TutorialEnabled = tutorialEnabled;
        }
    }
}