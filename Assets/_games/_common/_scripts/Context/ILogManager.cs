namespace EA4S
{
    /// <summary>
    /// Interface for Log Manager providers.
    /// </summary>
    public interface ILogManager
    {
        /// <summary>
        /// Called when minigame is finished.
        /// </summary>
        /// <param name="result">The valuation (0 to 3 stars).</param>
        void OnGameEnded(int result);

        /// <summary>
        /// To be called to any action of player linked to learning objective and with positive or negative vote.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isPositiveResult"></param>
        void OnAnswered(ILivingLetterData data, bool isPositiveResult);

        /// <summary>
        /// Called when player perform a [gameplay skill action] action during gameplay.
        /// </summary>
        /// <param name="ability">The ability.</param>
        /// <param name="score">The score (0 to 1).</param>
        void OnGameplaySkillAction(PlaySkill ability, float score);
    }
}