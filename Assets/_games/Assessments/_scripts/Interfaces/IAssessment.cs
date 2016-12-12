using System;
using System.Collections;

namespace EA4S.Assessment
{
    /// <summary>
    /// This class is implemented by each Assessment to customize logic and appareance
    /// </summary>
    public interface IAssessment
    {
        ILogicInjector LogicInjector { get; }
        IQuestionPlacer QuestionPlacer { get; }
        IAnswerPlacer AnswerPlacer { get; }
        IQuestionGenerator QuestionGenerator { get; }
        IAssessmentConfiguration Configuration { get; }
        IGameContext GameContext { get; }

        IEnumerator PlayCoroutine( Action gameEndedCallback);

        /*
        /// <summary>
        /// Used to read question data
        /// </summary>
        /// <param name="data"> Question to read</param>
        /// <returns> Return the played audio source to control it</returns>
        IAudioSource OnQuestionAppear( IQuestion question);

        /// <summary>
        /// Read the given answer word, letter or image
        /// </summary>
        /// <param name="data"> LivingLetter to read</param>
        /// <returns> Return the played audio source to control it</returns>
        IAudioSource OnReadAnswer( ILivingLetterData data);

        /// <summary>
        /// Read the given question word, letter or image
        /// </summary>
        /// <param name="data"> Question to read</param>
        /// <returns> Return the played audio source to control it</returns>
        IAudioSource OnReadQuestion( IQuestion data);
        */
    }
}
