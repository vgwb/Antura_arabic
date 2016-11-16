namespace EA4S.Assessment
{
    /// <summary>
    /// This class act as adapter between IQuestionProvider and Assessments
    /// </summary>
    public interface IQuestionGenerator
    {
        /// <summary>
        /// Starts caching data for a round (simultaneos questions)
        /// </summary>
        void InitRound();

        /// <summary>
        /// Get next question from IQuestionPack
        /// </summary>
        IQuestion GetNextQuestion();

        /// <summary>
        /// For each question gets also linked answers
        /// </summary>
        IAnswer[] GetNextAnswers();

        /// <summary>
        /// Stops caching data for this round
        /// </summary>
        void CompleteRound();

        /// <summary>
        /// Get all questions to be displayed
        /// </summary>
        IQuestion[] GetAllQuestions();

        /// <summary>
        /// Get all answers to be displayed
        /// </summary>
        IAnswer[] GetAllAnswers();
    }
}