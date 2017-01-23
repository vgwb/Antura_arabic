namespace EA4S {

    /// <summary>
    /// Defines a type of play-related skill that may be measured.
    /// Used for logging.
    /// </summary>
    public enum PlaySkill {
        None = 0,
        Precision = 1,
        Reaction = 2,
        Memory = 3,
        Logic = 4,
        Rhythm = 5,
        Musicality = 6,
        Sight = 7
    }

    /// <summary>
    /// Defines a type of app-wide event that may happen.
    /// Used for logging.
    /// </summary>
    public enum PlayEvent {
        GameStarted = 0,
        GameFinished = 1,
        Skill = 2
    }
}
