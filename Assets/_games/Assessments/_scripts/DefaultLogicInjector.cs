using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class DefaultLogicInjector : ILogicInjector
    {
        protected IDragManager dragManager = null;

        public DefaultLogicInjector( IDragManager dragManager)
        {
            this.dragManager = dragManager;
            ResetRound();
        }

        protected List< PlaceholderBehaviour> placeholdersList;
        protected List< Answer> answersList;
        protected List< IQuestion> questionsList;

        public void ResetRound()
        {
            placeholdersList = new List< PlaceholderBehaviour>();
            answersList = new List< Answer>();
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
        public void Wire( IQuestion question, Answer[] answers)
        {
            AnswerSet answerSet = new AnswerSet( answers);

            WireQuestion( question, answerSet);
            WirePlaceHolders( question);
            WireAnswers( answers);
        }

        protected virtual void WireQuestion( IQuestion q, AnswerSet answerSet)
        {
            if (AssessmentOptions.Instance.QuestionAnsweredFlip)
                q.QuestionBehaviour.FaceDownInstant();

            q.SetAnswerSet( answerSet);
            questionsList.Add( q);
        }

        protected virtual void WireAnswers( Answer[] answers)
        {
            if (answers == null || answers.Length == 0)
                return;

            foreach( var a in answers)
            {
                var behaviour = a.gameObject.GetComponent< Answer>();
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
