namespace EA4S
{

    public enum MiniGameCode
    {
        Assessment = 31,
        AlphabetSong = 5,
        Balloons_counting = 6,
        Balloons_letter = 7,
        Balloons_spelling = 3,
        Balloons_words = 8,
        ColorTickle = 9,
        DancingDots = 10,
        DontWakeUp = 2,
        Egg = 11,
        FastCrowd_alphabet = 12,
        FastCrowd_counting = 13,
        FastCrowd_letter = 14,
        FastCrowd_spelling = 1,
        FastCrowd_words = 4,
        HiddenSource = 15,
        HideSeek = 16,
        MakeFriends = 17,
        Maze = 18,
        MissingLetter = 19,
        MissingLetter_phrases = 20,
        MixedLetters_alphabet = 21,
        MixedLetters_spelling = 22,
        MysteriousGuest = 23,
        ReadingGame = 24,
        Scanner = 25,
        Scanner_phrase = 26,
        ThrowBalls_letters = 27,
        ThrowBalls_words = 28,
        Tobogan_letters = 29,
        Tobogan_words = 30
    }

    public enum MinigameState
    {
        Initializing,
        GameIntro,
        RoundIntro,
        Playing,
        Paused,
        RoundEnd,
        GameEnd,
        Result,
        Award
    }

}