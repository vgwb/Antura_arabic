using System.Collections.Generic;

namespace EA4S.API {

    /// <summary>
    /// Default IQuestionProvider that find the right letter question.
    /// </summary>
    /// <seealso cref="EA4S.IQuestionProvider" />
    public class FindRightLetterQuestionProvider : IQuestionProvider {

        #region properties
        List<IQuestionPack> questions = new List<IQuestionPack>();
        string description;
        int currentQuestion;
        #endregion

        public FindRightLetterQuestionProvider(List<IQuestionPack> _questionsPack, string descriptions) {
            currentQuestion = 0;
            description = "Antura Questions";

            questions.AddRange(_questionsPack);
        }

        public string GetDescription() {
            return description;
        }

        /// <summary>
        /// Provide me another question.
        /// </summary>
        /// <returns></returns>
        IQuestionPack IQuestionProvider.GetNextQuestion() {
            currentQuestion++;

            if (currentQuestion >= questions.Count)
                currentQuestion = 0;

            return questions[currentQuestion];
        }

    }
}