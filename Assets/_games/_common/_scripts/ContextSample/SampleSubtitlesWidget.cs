// Written by Davide Barbieri <davide.barbieri AT ghostshark.it>
using System;

namespace EA4S
{
    public class SampleSubtitlesWidget : ISubtitlesWidget
    {
        public void DisplaySentence(GameResult text, float enterDuration , bool showSpeaker, System.Action onSentenceCompleted)
        {
            WidgetSubtitles.I.DisplaySentence(text.ToString(), enterDuration, showSpeaker, onSentenceCompleted);
        }

        public void Clear()
        {
            WidgetSubtitles.I.Close();
        }
    }
}
