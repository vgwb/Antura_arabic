using System;

namespace EA4S.Assessment
{
    internal class PronunceQuestionDecorator : IQuestionDecorator
    {
        public void DecorateQuestion( QuestionBehaviour question)
        {
            var comp = question.gameObject.AddComponent< PronounceQuestion>();
            question.questionAnswered = comp;
        }
    }
}
