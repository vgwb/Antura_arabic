
using System;

namespace EA4S
{
    public class SampleSubtitlesWidget : ISubtitlesWidget
    {
        public void DisplaySentence(TextID text, float enterDuration , bool showSpeaker, System.Action onSentenceCompleted)
        {
            WidgetSubtitles.I.DisplaySentence(text.ToString(), enterDuration, showSpeaker, onSentenceCompleted);
        }

        public void Clear()
        {
            WidgetSubtitles.I.Close();
        }
    }
}
