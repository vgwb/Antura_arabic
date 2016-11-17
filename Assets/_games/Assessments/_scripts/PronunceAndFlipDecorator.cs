namespace EA4S.Assessment
{
    internal class PronunceAndFlipDecorator : IQuestionDecorator
    {
        public void DecorateQuestion( QuestionBehaviour question)
        {
            var comp = question.gameObject.AddComponent< PronounceAndFlipQuestion>();
            question.questionAnswered = comp;
        }
    }
}
