using System;
using System.Collections.Generic;

namespace EA4S.Assessment
{
    internal class DefaultLogicInjector : ILogicInjector
    {
        protected IDragManager dragManager = null;
        protected IQuestionDecorator decorator = null;

        public DefaultLogicInjector( IDragManager dragManager, IQuestionDecorator decorator)
        {
            this.dragManager = dragManager;
            this.decorator = decorator;
            ResetRound();
        }

        protected List< PlaceholderBehaviour> placeholdersList;
        protected List< AnswerBehaviour> answersList;
        protected List< IQuestion> questionsList;

        public void ResetRound()
        {
            placeholdersList = new List< PlaceholderBehaviour>();
            answersList = new List< AnswerBehaviour>();
            questionsList = new List< IQuestion>();
            dragManager.ResetRound();
            AnswerSet.ResetTotalCount();
        }

        /// <summary>
        /// Should return true when it's time to reset round.
        /// </summary>
        public bool AllAnswersCorrect()
        {
            return dragManager.AllAnswered();
        }

        public void EnableGamePlay()
        {
            dragManager.Enable();
        }

        // Called many times (for loop in assessment)
        public void Wire( IQuestion question, IAnswer[] answers)
        {
            AnswerSet answerSet = new AnswerSet( answers);

            WireQuestion( question, answerSet);
            WirePlaceHolders( question);
            WireAnswers( answers);
        }

        protected virtual void WireQuestion( IQuestion q, AnswerSet answerSet)
        {
            decorator.DecorateQuestion( q.gameObject.GetComponent< QuestionBehaviour>());
            q.SetAnswerSet( answerSet);
            questionsList.Add( q);
        }

        protected virtual void WireAnswers( IAnswer[] answers)
        {
            if (answers == null || answers.Length == 0)
                return;

            foreach( var a in answers)
            {
                var behaviour = a.gameObject.GetComponent< AnswerBehaviour>();
                answersList.Add( behaviour); // TODO: INVESTIGATE WITHIN DRAG MAANGER
            }
        }

        protected virtual void WirePlaceHolders( IQuestion question)
        {
            foreach ( var p in question.GetPlaceholders())
            {
                var behaviour = p.GetComponent< PlaceholderBehaviour>();
                behaviour.Placeholder = new DragNDropPlaceholder();
                behaviour.Placeholder.SetQuestion( question);
                placeholdersList.Add( behaviour);
            }
        }

        public void CompleteWiring()
        {
            dragManager.AddElements( placeholdersList, answersList, questionsList);
        }

        public void EnableDragOnly()
        {
            dragManager.EnableDragOnly();
        }

        public void RemoveDraggables()
        {
            dragManager.RemoveDraggables();
        }

        public void AnswersAdded()
        {
            OnAnswersAdded();
        }

        protected virtual void OnAnswersAdded()
        {
            // nothing. it's ok
        }
    }
}
