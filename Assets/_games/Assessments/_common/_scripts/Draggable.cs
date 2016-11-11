using DG.Tweening;
using UnityEngine;

namespace EA4S.Assessment
{
    public class Draggable : MonoBehaviour, IDraggable
    {
        Vector3 origin;
        void OnMouseDown()
        {
            origin = transform.localPosition;
            manager.StartDragging( this);
        }

        void OnMouseUp()
        {
            manager.StopDragging( this);
        }

        DragManager manager;

       public  void SetDragManager( DragManager manager)
        {
            this.manager = manager;
        }

        Tween tween = null;

        private void PreTweenCheck()
        {
            if (tween != null)
            {
                tween.Kill( false);
                tween = null;
            }
        }

        public void OnBecomeDragged()
        {
            ShakeScale(1.3f,30);
        }

        void ShakeScale( float scale ,float angle)
        {
            PreTweenCheck();
            Sequence seq = DOTween.Sequence();

            seq.Insert(0, transform.DOLocalRotate( new Vector3( 0, 180, angle), 0.3f)
                .SetEase(Ease.InOutBounce));
            seq.Insert(0, transform.DOScale( new Vector3( scale, scale, scale), 0.45f));
            seq.OnComplete(() => tween = null);

            tween = seq;
        }

        public void OnBecomeDropped()
        {
            ShakeScale(1,0);
        }

        public void SetPosition( Vector3 mousPos)
        {
            var pos = Camera.main.ScreenToWorldPoint( mousPos);
            pos.z = 5;
            transform.localPosition = pos;
        }

        int groupID;

        public void SetGroupID( int group)
        {
            groupID = group;
        }

        public int GetGroupID()
        {
            return groupID;
        }

        public Vector3 GetPosition()
        {
            return transform.localPosition;
        }

        public void PlacedOnCorrectPlace(Vector3 pos)
        {
            //prevent further dragging
            transform.DOLocalMove( pos, 0.4f);
            Destroy(this);
        }

        public void ReturnToOrigin()
        {
            transform.DOLocalMove( origin, 0.4f);
        }
    }
}