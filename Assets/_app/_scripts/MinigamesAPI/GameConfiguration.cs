namespace EA4S.MinigamesAPI
{

    /// <summary>
    /// Data passed to a minigame to configure it. 
    /// </summary>
    // refactor: rename this, it is similar to IGameConfiguration but has a different purpose. The purpose here is for the MiniGameAPI to be called with a specific difficulty.
    public class GameConfiguration
    {

        public float Difficulty = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameConfiguration"/> class.
        /// </summary>
        /// <param name="_difficulty">The difficulty.</param>
        public GameConfiguration(float _difficulty) {
            Difficulty = _difficulty;
            
        }

    }

}