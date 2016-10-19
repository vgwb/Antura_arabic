namespace EA4S
{
    public interface ICountdownClockWidget
    {
        void Show();

        /// <summary>
        /// Set the time of this clock widget; 0 for started countdown, 1 for ended
        /// </summary>
        void SetTime(float time);

        void Hide();
    }
}
