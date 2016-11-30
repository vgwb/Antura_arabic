namespace EA4S.Assessment
{
    public class PronunceImageDecorator : IQuestionDecorator
    {
        public void DecorateQuestion( QuestionBehaviour question)
        {
            var comp = question.gameObject.AddComponent< PronunceImageDecoration>();
            question.questionAnswered = comp;
        }
    }
}
