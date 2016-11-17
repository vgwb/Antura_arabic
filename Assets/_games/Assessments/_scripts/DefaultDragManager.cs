using System;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class DefaultDragManager : IDragManager, ITickable
    {
        private IAudioManager audioManager;
        public DefaultDragManager( IAudioManager audioManager)
        {
            this.audioManager = audioManager;
            ResetRound();
        }

        List< PlaceholderBehaviour> placeholders = null;
        List< DroppableBehaviour> answers = null;

        public void AddElements( List< PlaceholderBehaviour> placeholders, List< AnswerBehaviour> answers)
        {
            this.placeholders = placeholders;
            this.answers = new List< DroppableBehaviour>();

            foreach (var a in answers)
            {
                var droppable = a.gameObject.AddComponent< DroppableBehaviour>();
                droppable.SetDragManager( this);
                this.answers.Add(droppable);
            }
        }

        public bool AllAnswered()
        {
            // TODO: This is the only missing piece for this class

            // TODO: ticking false
            return false;
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
            if (this.droppable != droppable)
                throw new InvalidOperationException("Can drop only previously dragged objet");

            audioManager.PlaySound( Sfx.ThrowObj);
            CheckCollidedWithPlaceholder();
            this.droppable.StopDrag();
            this.droppable = null;
        }

        private void CheckCollidedWithPlaceholder()
        {
            foreach(var p in placeholders)
                if ( NearEnoughToDrop( p.transform))
                {
                    droppable.LinkToPlaceholder( p);
                    return;
                }
        }

        bool NearEnoughToDrop( Transform zone)
        {
            var p1 = zone.transform.position;
            var p2 = droppable.GetTransform().localPosition;
            p1.z = p2.z = 0;
            return p1.DistanceIsLessThan( p2, 3f);
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
    }
}
