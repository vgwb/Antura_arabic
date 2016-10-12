using UnityEngine;

namespace EA4S
{
    public interface IInputManager
    {
        // does not raise events when disabled
        bool Enabled { set; }

        Vector2 LastPointerPosition { get; }
        Vector2 LastPointerDelta { get; }

        event System.Action onPointerDown;
        event System.Action onPointerUp;
        event System.Action onPointerDrag;

        void Update();
        void Reset();
    }
}
