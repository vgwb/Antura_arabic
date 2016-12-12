using DG.Tweening;
using System;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class SortableBehaviour : MonoBehaviour, IDroppable
    {
        IDragManager dragManager = null;
        Tween tween = null;

        public void SetDragManager( IDragManager dragManager)
        {
            this.dragManager = dragManager;
        }

        public IAnswer GetAnswer()
        {
            return GetComponent< AnswerBehaviour>().GetAnswer();
        }

        Vector3 origin; // Memorize starting position for going back
        void OnMouseDown()
        {
            if (!dragEnabled)
                return;

            dragManager.StartDragging( this);
            SetScale( 1.3f);
        }

        void SetScale(float scale)
        {
            if (tween != null)
                tween.Kill( false);

            tween = transform.DOScale( scale, 0.4f).OnComplete( () => tween = null);
        }

        void OnMouseUp()
        {
            if (!dragEnabled)
                return;

            dragManager.StopDragging( this);
        }

        bool dragEnabled = false;
        public void Disable()
        {
            dragEnabled = false;
        }

        public void Enable()
        {
            dragEnabled = true;
        }

        Action< IDroppable> OnGoDestroyed = null;

        public void StartDrag( Action< IDroppable> onDestroyed)
        {
            OnGoDestroyed = onDestroyed;
            SetScale(1.3f);
        }

        void OnDestroy()
        {
            dragEnabled = false;
            if (OnGoDestroyed != null)
                OnGoDestroyed( this);
        }

        public void StopDrag()
        {
            OnGoDestroyed = null;
            SetScale(1f);
        }

        private int index = -1;
        public bool SetSortIndex( int a)
        {
            if(index!=a)
            {
                index = a;
                return true;
            }
            return false;
        }

        PlaceholderBehaviour linkedBehaviour = null;
        public void LinkToPlaceholder(PlaceholderBehaviour behaviour)
        {

        }

        public void Detach(bool jumpBack = true)
        {

        }

        public Transform GetTransform()
        {
            return transform;
        }

        public PlaceholderBehaviour GetLinkedPlaceholder()
        {
            return null;
        }

        private Tween tweenMove = null;
        internal void Move( Vector3 position, float v)
        {
            if (tweenMove != null && tweenMove.IsComplete() == false)
                tweenMove.Kill(false);

            tweenMove = transform.DOLocalMove( position, v);
        }
    }
}
