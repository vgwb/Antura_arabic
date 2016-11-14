namespace EA4S
{
    public interface IGameConfiguration
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        IGameContext Context { get; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        /// <value>
        /// The questions.
        /// </value>
        IQuestionProvider Questions { get; set; }

        /// <summary>
        /// Return the builder that defines the rules to build question packs
        /// </summary>
        /// <returns></returns>
        IQuestionBuilder SetupBuilder();

        /// <summary>
        /// Return the rules for learning related to this minigame
        /// </summary>
        /// <returns></returns>
        MiniGameLearnRules SetupLearnRules();

        /// <summary>
        /// Gets or sets the difficulty.
        /// </summary>
        /// <value>
        /// The difficulty.
        /// </value>
        float Difficulty { get; set; }
    }
}