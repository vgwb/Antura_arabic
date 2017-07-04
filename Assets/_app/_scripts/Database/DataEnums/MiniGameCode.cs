namespace EA4S
{
    /// <summary>
    /// Enumerator specifying a minigame (or minigame variation) that is supported by the core application. 
    /// </summary>
    // TODO refactor: this enum depends on the specific implemented minigames and should be grouped with them 
    // last is 35 and 114
    public enum MiniGameCode
    {
        Invalid = 0,
        AlphabetSong_alphabet = 5,
        AlphabetSong_letters = 33,
        Balloons_counting = 6,
        Balloons_letter = 7,
        Balloons_spelling = 3,
        Balloons_words = 8,
        ColorTickle = 9,
        DancingDots = 10,
        Egg_letters = 11,
        Egg_sequence = 34,
        FastCrowd_alphabet = 12,
        FastCrowd_counting = 13,
        FastCrowd_letter = 14,
        FastCrowd_spelling = 1,
        FastCrowd_words = 4,
        //HiddenSource = 15,
        HideSeek = 16,
        MakeFriends = 17,
        Maze = 18,
        MissingLetter = 19,
        MissingLetter_phrases = 20,
        MissingLetter_forms = 35,
        MixedLetters_alphabet = 21,
        MixedLetters_spelling = 22,
        SickLetters = 23,
        ReadingGame = 24,
        Scanner = 25,
        Scanner_phrase = 26,
        ThrowBalls_letters = 27,
        ThrowBalls_letterinword = 32,
        ThrowBalls_words = 28,
        Tobogan_letters = 29,
        Tobogan_words = 30,
        TakeMeHome = 31,
        Assessment_LetterForm = 100,
        Assessment_WordsWithLetter = 101,
        Assessment_MatchLettersToWord = 102,
        Assessment_MatchLettersToWord_Form = 114,
        Assessment_CompleteWord = 103,
        Assessment_CompleteWord_Form = 113,
        Assessment_OrderLettersOfWord = 104,
        Assessment_VowelOrConsonant = 105,  // UNUSED
        Assessment_SelectPronouncedWord = 106,
        Assessment_MatchWordToImage = 107,
        Assessment_WordArticle = 108,
        Assessment_SingularDualPlural = 109,
        Assessment_SunMoonWord = 110,
        Assessment_SunMoonLetter = 111,
        Assessment_QuestionAndReply = 112
    }
}
