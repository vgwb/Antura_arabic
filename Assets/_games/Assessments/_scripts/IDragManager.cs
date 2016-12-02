using System.Collections.Generic;

namespace EA4S.Assessment
{
    public interface IDragManager
    {
        void ResetRound();
        bool AllAnswered();
        void Enable();

        void DisableInput();
        void EnableInput();

        void AddElements(   List< PlaceholderBehaviour> placeholders,
                            List< AnswerBehaviour> answers,
                            List< IQuestion> questions);

        void StartDragging( IDroppable droppable);
        void StopDragging( IDroppable droppable);
        void EnableDragOnly();
        void RemoveDraggables();
        void OnAnswerAdded();
    }
}
