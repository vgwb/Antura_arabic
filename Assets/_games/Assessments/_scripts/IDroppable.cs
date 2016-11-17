using UnityEngine;

namespace EA4S.Assessment
{
    public interface IDroppable
    {
        /// <summary>
        /// Enable input on this GO
        /// </summary>
        void Enable();

        /// <summary>
        /// Disable input on this GO
        /// </summary>
        void Disable();

        /// <summary>
        /// Link to dragManager
        /// </summary>
        void SetDragManager( IDragManager dragManager);

        void StartDrag();

        void StopDrag();

        void LinkToPlaceholder( PlaceholderBehaviour behaviour);

        PlaceholderBehaviour GetLinkedPlaceholder();

        void Detach();

        Transform GetTransform();
    }
}
