namespace EA4S.Assessment
{
    internal class PronunceQuestionDecorator : IQuestionDecorator
    {
        public void DecorateQuestion( QuestionBehaviour question)
        {
            var comp = question.gameObject.AddComponent< NoEvent>();
            question.questionAnswered = comp;
        }
    }
}
