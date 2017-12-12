namespace Antura
{
    /// <summary>
    /// Enumerator specifying a minigame (or minigame variation) that is supported by the core application. 
    /// </summary>
    // TODO refactor: this enum depends on the specific implemented minigames and should be grouped with them 
    // last is 39 and 115
    public enum MiniGameCode
    {
        Invalid = 0,
        AlphabetSong_alphabet = 5,
        AlphabetSong_letter = 33, // DISABLED
        Balloons_counting = 6,
        Balloons_letterinword = 7,
        Balloons_spelling = 3, // DISABLED
        Balloons_word = 8,
        ColorTickle_letter = 9,
        DancingDots_letter = 10, // DISABLED
        DancingDots_letterform = 36,  // NEW
        Egg_letter = 11,
        Egg_letterform = 37, // NEW
        Egg_sequence = 34,
        FastCrowd_alphabet = 12, // DISABLED
        FastCrowd_counting = 13,
        FastCrowd_lettername = 14,
        FastCrowd_letterform = 38, // NEW
        FastCrowd_letterinword = 1,
        FastCrowd_word = 4,
        HideSeek_letterform = 16,
        MakeFriends_letterform = 17,
        Maze_letter = 18,
        //MissingLetter_letter = 19, // DISABLED
        MissingLetter_letterform = 39, // NEW
        MissingLetter_phrase = 20, // DISABLED
        MissingLetter_letterinword = 35,
        MixedLetters_alphabet = 21, // DISABLED
        MixedLetters_letterinword = 22,
        ReadingGame_word = 24, // DISABLED
        Scanner_word = 25, // DISABLED
        Scanner_phrase = 26, // DISABLED
        SickLetters_letter = 23,
        TakeMeHome_letter = 31, // DISABLED
        ThrowBalls_letter = 27,
        ThrowBalls_letterform = 15, // NEW
        ThrowBalls_letterinword = 32,
        ThrowBalls_word = 28,
        Tobogan_letter = 29, // DISABLED
        Tobogan_sunmoon = 30, // DISABLED

        Assessment_Letter = 100,
        Assessment_LetterForm = 115, // NEW
        Assessment_WordsWithLetter = 101,
        Assessment_MatchLettersToWord = 102,
        Assessment_MatchLettersToWord_Form = 114,
        Assessment_CompleteWord = 103,
        Assessment_CompleteWord_Form = 113,
        Assessment_OrderLettersOfWord = 104,
        Assessment_VowelOrConsonant = 105, // DISABLED
        Assessment_SelectPronouncedWord = 106,
        Assessment_MatchWordToImage = 107,
        Assessment_WordArticle = 108,
        Assessment_SingularDualPlural = 109,
        Assessment_SunMoonWord = 110,
        Assessment_SunMoonLetter = 111,
        Assessment_QuestionAndReply = 112
    }
}