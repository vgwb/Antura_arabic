namespace EA4S {
    /// <summary>
    /// Interface for Log Manager providers.
    /// </summary>
    public interface ILogManager {
        /// <summary>
        /// Answer result for question pack.
        /// </summary>
        /// <param name="_data"></param>
        /// <param name="_isPositiveResult"></param>
        void OnAnswer(ILivingLetterData _data, bool _isPositiveResult);

        /// <summary>
        /// Answer result for single player action.
        /// </summary>
        /// <param name="_questionPack"></param>
        /// <param name="_isPositiveResult"></param>
        void OnAnswer(IQuestionPack _questionPack, bool _isPositiveResult);

        /// <summary>
        /// Called when player perform action during gameplay.
        /// </summary>
        /// <param name="_ability">The ability.</param>
        /// <param name="_isPositive">if set to <c>true</c> [is positive].</param>
        void OnGameplayEvent(API.PlayerAbilities _ability, bool _isPositive);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_area"></param>
        /// <param name="_context"></param>
        /// <param name="_action"></param>
        /// <param name="_data"></param>
        void Log(string _area, string _context, string _action, string _data);
    }

}