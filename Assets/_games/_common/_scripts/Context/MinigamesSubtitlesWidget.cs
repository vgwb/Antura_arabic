namespace EA4S
{
    public class MinigamesSubtitlesWidget : ISubtitlesWidget
    {
        public void DisplaySentence(Db.LocalizationDataId text, float enterDuration , bool showSpeaker, System.Action onSentenceCompleted)
        {
            WidgetSubtitles.I.DisplaySentence(text, enterDuration, showSpeaker, onSentenceCompleted);
        }

        public void Clear()
        {
            WidgetSubtitles.I.Close();
        }
    }
}
