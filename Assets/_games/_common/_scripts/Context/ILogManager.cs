namespace EA4S {
    /// <summary>
    /// Interface for Log Manager providers.
    /// </summary>
    public interface ILogManager {
        /// <summary>
        /// Called at any player interaction with actual questionPack.
        /// </summary>
        /// <param name="_data"></param>
        /// <param name="_isPositiveResult"></param>
        void OnAnswer(ILivingLetterData _data, bool _isPositiveResult);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_questionPack"></param>
        /// <param name="_isPositiveResult"></param>
        void OnAnswer(IQuestionPack _questionPack, bool _isPositiveResult);
    }

}