using System.Collections.Generic;
using EA4S.Db;

namespace EA4S.Assessment
{
    /// <summary>
    /// Provide test data for LetterShape Assessment.
    /// </summary>
    public class LetterShape_TestProvider : IQuestionProvider
    {
        List<SampleQuestionPack> questions = new List< SampleQuestionPack>();

        int currentQuestion;

        protected LL_LetterData WrapLetter( LetterData data )
        {
            return new LL_LetterData( data.GetId());
        }

        public LetterShape_TestProvider( int rounds, int simultaneosQuestions, int wrongNumber)
        {
            currentQuestion = 0;

            for (int round = 0; round < rounds; round++)
            {
                // Get All base letters
                var AllLetters = AppManager.I.Teacher.wordHelper.GetAllBaseLetters();
                var roundContent = RandomHelper.RouletteSelectNonRepeating( AllLetters, (wrongNumber+1) * simultaneosQuestions);

                for(int q = 0; q<simultaneosQuestions; q++)
                {
                    List< ILivingLetterData> correctAnswers = new List< ILivingLetterData>();
                    List< ILivingLetterData> wrongAnswers = new List< ILivingLetterData>();

                    correctAnswers.Add( WrapLetter( roundContent.Pull()));
                    for (int ans = 0; ans < wrongNumber; ans++)
                        wrongAnswers.Add( WrapLetter( roundContent.Pull()));

                    //TODO: check If I can use same reference value for question and correct answer 
                    // (I Assume LetterOjectView just use ILivingLetterData in a read-only fashion.. which may not be true)
                    var currentPack = new SampleQuestionPack( correctAnswers[0], wrongAnswers, correctAnswers);
                    questions.Add( currentPack);
                }
            }
        }

        IQuestionPack IQuestionProvider.GetNextQuestion()
        {
            currentQuestion++;

            if (currentQuestion >= questions.Count)
                currentQuestion = 0;

            return questions[currentQuestion];
        }
    }
}
