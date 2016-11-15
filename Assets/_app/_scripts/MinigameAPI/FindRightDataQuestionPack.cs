using System.Collections.Generic;
using System.Linq;

namespace EA4S.API {
    /// <summary>
    /// Data Pack for "find right question" mechanics.
    /// One data question data, many right answare data, many answare data.
    /// </summary>
    /// <seealso cref="EA4S.IQuestionPack" />
    public class FindRightDataQuestionPack : IQuestionPack {
        IEnumerable<ILivingLetterData> questionsSentences;
        IEnumerable<ILivingLetterData> wrongAnswersSentence;
        IEnumerable<ILivingLetterData> correctAnswersSentence;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindRightDataQuestionPack"/> class.
        /// </summary>
        /// <param name="questionSentence">The question sentence.</param>
        /// <param name="wrongAnswersSentence">The wrong answers sentence.</param>
        /// <param name="correctAnswersSentence">The correct answers sentence.</param>
        public FindRightDataQuestionPack(ILivingLetterData questionSentence, IEnumerable<ILivingLetterData> wrongAnswersSentence, IEnumerable<ILivingLetterData> correctAnswersSentence) {
            this.questionsSentences = new List<ILivingLetterData>() { questionSentence };
            this.wrongAnswersSentence = wrongAnswersSentence;
            this.correctAnswersSentence = correctAnswersSentence;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FindRightDataQuestionPack"/> class.
        /// </summary>
        /// <param name="questionsSentences">The questions sentences.</param>
        /// <param name="wrongAnswersSentence">The wrong answers sentence.</param>
        /// <param name="correctAnswersSentence">The correct answers sentence.</param>
        public FindRightDataQuestionPack(IEnumerable<ILivingLetterData> questionsSentences, IEnumerable<ILivingLetterData> wrongAnswersSentence, IEnumerable<ILivingLetterData> correctAnswersSentence) {
            this.questionsSentences = questionsSentences;
            this.wrongAnswersSentence = wrongAnswersSentence;
            this.correctAnswersSentence = correctAnswersSentence;
        }

        ILivingLetterData IQuestionPack.GetQuestion() {
            return questionsSentences.First();
        }

        public IEnumerable<ILivingLetterData> GetQuestions() {
            return questionsSentences;
        }

        IEnumerable<ILivingLetterData> IQuestionPack.GetWrongAnswers() {
            return wrongAnswersSentence;
        }

        public IEnumerable<ILivingLetterData> GetCorrectAnswers() {
            return correctAnswersSentence;
        }


    }
}