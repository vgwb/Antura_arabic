namespace EA4S.Tobogan
{
    class QuestionSentenceSample : IQuestionSentence
    {
        int sentenceID;
        QuestionSentenceType sentenceType;

        public QuestionSentenceSample(int sentenceID, QuestionSentenceType sentenceType)
        {
            this.sentenceID = sentenceID;
            this.sentenceType = sentenceType;
        }

        public int GetSentenceID()
        {
            return sentenceID;
        }

        public QuestionSentenceType GetSentenceType()
        {
            return sentenceType;
        }
    }
}