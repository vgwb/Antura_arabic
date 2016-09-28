namespace EA4S.Tobogan
{
    enum QuestionSentenceType
    {
        LETTER,
        WORD,
        IMAGE
    }

    interface IQuestionSentence
    {
        QuestionSentenceType GetSentenceType();
        int GetSentenceID();
    }
}
