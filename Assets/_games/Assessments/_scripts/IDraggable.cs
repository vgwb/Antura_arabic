using UnityEngine;

namespace EA4S.Assessment
{
    public interface IDraggable
    {
        void OnBecomeDragged();
        void OnBecomeDropped();
        void SetDragManager( DragManager manager);
        void SetPosition( Vector3 mousePos);
        Vector3 GetPosition();
        void PlacedOnCorrectPlace (Vector3 pos);

        // Check if answer is correct by checking its question's group
        void SetGroupID( int group);
        int GetGroupID();
        void ReturnToOrigin();
    }
}