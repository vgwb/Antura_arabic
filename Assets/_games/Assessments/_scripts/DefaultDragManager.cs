using System;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class DefaultDragManager : IDragManager, ITickable
    {
        private IAudioManager audioManager;
        private IAnswerChecker checker;

        public DefaultDragManager( IAudioManager audioManager, IAnswerChecker checker)
        {
            this.audioManager = audioManager;
            this.checker = checker;
            ResetRound();
        }

        private bool dragOnly = false;
        public void EnableDragOnly()
        {
            dragOnly = true;
            ticking = true;
            TimeEngine.AddTickable(this);
            foreach (var a in answers)
                a.Enable();
        }

        List< PlaceholderBehaviour> placeholders = null;
        List< DroppableBehaviour> answers = null;
        List< IQuestion> questions = null;

        // This should be called onlye once
        public void AddElements(
                                    List< PlaceholderBehaviour> placeholders, 
                                    List< AnswerBehaviour> answers,
                                    List< IQuestion> questions)
        {
            this.placeholders = placeholders;
            this.answers = BehaviourFromAnswers( answers);
            this.questions = questions;
        }

        private List< DroppableBehaviour> BehaviourFromAnswers( List< AnswerBehaviour> answers)
        {
            var list = new List< DroppableBehaviour>();

            foreach ( var a in answers)
            {
                var droppable = a.gameObject.AddComponent< DroppableBehaviour>();
                droppable.SetDragManager( this);
                list.Add( droppable);
            }
           
            return list;
        }

        public bool AllAnswered()
        {            
            if (!checker.IsAnimating() && checker.AreAllAnswered(placeholders))
            {
                checker.Check( placeholders, questions, this);
            }

            return checker.AllCorrect();
        }

        public void Enable()
        {
            dragOnly = false; 
        }

        public void ResetRound()
        {
            placeholders = null;
            answers = null;
        }

        IDroppable droppable = null;

        // ALL NEEDED EVENTS ARE HERE
        public void StartDragging( IDroppable droppable)
        {
            if (this.droppable != null)
                return;

            audioManager.PlaySound( Sfx.UIPopup);
            this.droppable = droppable;
            droppable.StartDrag( x=>RemoveFromUpdateAndPlaceholders(x));
        }

        void RemoveFromUpdateAndPlaceholders( IDroppable droppa)
        {
            RemoveFromUpdate();
            if (placeholders.Remove(droppa.GetLinkedPlaceholder()) == false)
                throw new InvalidOperationException("Cannote remove the droppale");
        }

        void RemoveFromUpdate()
        {
            this.droppable.StopDrag();
            this.droppable = null;
        }

        public void StopDragging( IDroppable droppable)
        {
            if (this.droppable == droppable && droppable != null)
            {
                audioManager.PlaySound( Sfx.UIPopup);
                if(dragOnly== false)
                    CheckCollidedWithPlaceholder( droppable);
                RemoveFromUpdate();
            }
        }

        private void CheckCollidedWithPlaceholder( IDroppable droppable)
        { 
            foreach(var p in placeholders)
                if ( NearEnoughToDrop( p.transform))
                {
                    droppable.Detach( false);
                    droppable.LinkToPlaceholder( p);
                    var set = p.Placeholder.GetQuestion().GetAnswerSet();
                    set.OnDroppedAnswer( droppable.GetAnswer());
                    return;
                }

            // In case we just moved out a LL
            droppable.Detach( false);
        }

        bool NearEnoughToDrop( Transform zone)
        {
            if (droppable == null)
                return false;

            var p1 = zone.transform.position;
            var p2 = droppable.GetTransform().localPosition;
            p1.z = p2.z = 0;
            return p1.DistanceIsLessThan( p2, 2f);
        }

        bool ticking = false;
        public bool Update( float deltaTime)
        {
            if (droppable != null)
            {
                var pos = Camera.main.ScreenToWorldPoint( Input.mousePosition);
                pos.z = 5;
                droppable.GetTransform().localPosition = pos;
            }
            return !ticking;
        }

        public void DisableInput()
        {
            foreach (var a in answers)
                a.Disable();
        }

        public void EnableInput()
        {
            foreach (var a in answers)
                a.Enable();
        }

        public void RemoveDraggables()
        {
            dragOnly = true;
            if (this.droppable != null)
            {                
                this.droppable.StopDrag();
                this.droppable = null;
            }
        }

        public void OnAnswerAdded()
        {
            
        }
    }
}
