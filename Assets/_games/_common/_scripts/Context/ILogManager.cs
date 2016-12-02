namespace EA4S
{
    /// <summary>
    /// Interface for Log Manager providers.
    /// </summary>
    public interface ILogManager
    {

        /// <summary>
        /// Initializes the gameplay log session.
        /// </summary>
        /// <param name="_minigameCode">The minigame code.</param>
        void InitGameplayLogSession(MiniGameCode _minigameCode);

        /// <summary>
        /// To be called to any action of player linked to learnig objective and with positive or negative vote.
        /// </summary>
        /// <param name="_data"></param>
        /// <param name="_isPositiveResult"></param>
        void OnAnswer(ILivingLetterData _data, bool _isPositiveResult);

        /// <summary>
        /// Called when minigame is finished.
        /// </summary>
        /// <param name="_valuation">The valuation.</param>
        void OnMiniGameResult(int _valuation);

        /// <summary>
        /// Called when player perform a [gameplay skill action] action during gameplay. .
        /// </summary>
        /// <param name="_ability">The ability.</param>
        /// <param name="_score">The score.</param>
        void OnGameplaySkillAction(PlaySkill _ability, float _score);

        /// <summary>
        /// Log a generic info data.
        /// </summary>
        /// <param name="_event">The event.</param>
        /// <param name="_data">The data.</param>
        void LogInfoData(InfoEvent _event, string _data);
    }

}