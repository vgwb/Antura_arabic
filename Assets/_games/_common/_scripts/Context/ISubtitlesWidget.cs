
namespace EA4S
{
    public interface ISubtitlesWidget
    {
        void DisplaySentence(Db.LocalizationDataId text, float enterDuration = 2, bool showSpeaker = false, System.Action onSentenceCompleted = null);
        void Clear();
    }
}
