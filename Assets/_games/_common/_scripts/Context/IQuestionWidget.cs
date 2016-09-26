namespace EA4S
{
    public interface IQuestionWidget
    {
        void Show(string text, WordData word, System.Action callback);
        void Hide();
    }
}
