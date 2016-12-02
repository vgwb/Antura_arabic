using System;

namespace EA4S.Assessment
{
    internal class SortingLogicInjector : DefaultLogicInjector
    {
        public SortingLogicInjector( IDragManager dragManager, IQuestionDecorator questionDecorator)
            :base( dragManager, questionDecorator)
        {

        }

        protected override void WireQuestion( IQuestion q, AnswerSet answerSet)
        {
            decorator.DecorateQuestion( q.gameObject.GetComponent< QuestionBehaviour>());
        }

        protected override void WireAnswers( IAnswer[] answers)
        {
            if (answers == null || answers.Length == 0)
                throw new ArgumentException( "What am I supposed to sort without any correct answer?");

            foreach (var a in answers)
            {
                var behaviour = a.gameObject.GetComponent< AnswerBehaviour>();
                answersList.Add( behaviour); // TODO: INVESTIGATE WITHIN DRAG MAANGER
            }
        }

        protected override void WirePlaceHolders( IQuestion question)
        {

        }

        protected override void OnAnswersAdded()
        {
            dragManager.OnAnswerAdded();
        }
    }
}
