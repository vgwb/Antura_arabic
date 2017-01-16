using Kore.Utils;

namespace EA4S.Assessment
{
    /// <summary>
    /// Text flow options. I assume most languages are LeftToRight
    /// and Right To left, We may have to add further options in future
    /// if we need Chinese/Japanese. 
    /// </summary>
    public enum TextFlow
    {
        LeftToRight,
        RightToLeft
    }

    /// <summary>
    /// These options are setted by AssessmentFactory after MiniGameAPI produced the
    /// AssessmentConfiguration instance.
    /// </summary>
    public class AssessmentOptions : SceneScopedSingleton< AssessmentOptions>
    {
        public TextFlow LocaleTextFlow { get; set; }
        public bool PronunceQuestionWhenClicked { get; set; }
        public bool PronunceAnswerWhenClicked { get; set; }
        public bool ShowQuestionAsImage { get; set; }
    }
}
