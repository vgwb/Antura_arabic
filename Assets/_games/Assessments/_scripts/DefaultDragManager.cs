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

        List< PlaceholderBehaviour> placeholders = null;
        List< DroppableBehaviour> answers = null;

        // This should be called onlye once
        public void AddElements( List< PlaceholderBehaviour> placeholders, List< AnswerBehaviour> answers)
        {
            this.placeholders = placeholders;
            this.answers = new List< DroppableBehaviour>();

            foreach (var a in answers)
            {
                var droppable = a.gameObject.AddComponent< DroppableBehaviour>();
                droppable.SetDragManager( this);
                this.answers.Add( droppable);
            }
        }

        public bool AllAnswered()
        {
            bool allAnswered = true;
            foreach (var p in placeholders)
            {
                if (p.LinkedDroppable == null)
                    return false;

                if (p.LinkedDroppable.gameObject.GetComponent< AnswerBehaviour>() == null)
                    return false;

                var droppa = p.LinkedDroppable.gameObject.GetComponent< AnswerBehaviour>();
                var place = p.Placeholder;
                place.LinkAnswer( droppa.GetAnswer().GetAnswerSet());
                if (place.IsAnswered() == false)
                    allAnswered = false;
            }

            if (allAnswered)
                Debug.Log("allAnswered");
            
            if (allAnswered && !checker.IsAnimating())
                checker.Check( placeholders, this);

            return checker.AllCorrect();
        }

        public void Enable()
        {
            ticking = true;
            TimeEngine.AddTickable( this);
            foreach (var a in answers)
                a.Enable();
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

            audioManager.PlaySound( Sfx.ThrowObj);
            this.droppable = droppable;
            droppable.StartDrag();
        }

        public void StopDragging( IDroppable droppable)
        {
            if (this.droppable == droppable)
            {
                audioManager.PlaySound(Sfx.ThrowObj);
                CheckCollidedWithPlaceholder();
                this.droppable.StopDrag();
                this.droppable = null;
            }
        }

        private void CheckCollidedWithPlaceholder()
        { 
            foreach(var p in placeholders)
                if ( NearEnoughToDrop( p.transform))
                {
                    droppable.Detach(false);
                    droppable.LinkToPlaceholder( p);
                    return;
                }

            // In case we just moved out a LL
            droppable.Detach( false);
        }

        bool NearEnoughToDrop( Transform zone)
        {
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
    }
}
