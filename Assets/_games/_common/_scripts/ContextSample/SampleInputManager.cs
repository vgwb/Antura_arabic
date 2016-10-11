using System;
using UnityEngine;

namespace EA4S
{
    public class SampleInputManager : IInputManager
    {
        public bool Enabled { get; set; }

        public Vector2 LastPointerPosition { get { return lastPointerPosition; } }
        public Vector2 LastPointerDelta { get { return deltaPosition; } }

        public event Action onPointerDown;
        public event Action onPointerDrag;
        public event Action onPointerUp;

        bool wasPointerDown = false;
        Vector2 lastPointerPosition = Vector2.zero;
        Vector2 deltaPosition;

        public void Update()
        {
            if (!Enabled)
                return;

            if (Input.GetMouseButton(0))
            {
                if (wasPointerDown)
                {
                    var newPosition = Input.mousePosition;
                    deltaPosition = (Vector2)newPosition - lastPointerPosition;

                    lastPointerPosition = newPosition;

                    if (deltaPosition.x != 0 || deltaPosition.y != 0)
                    {
                        if (onPointerDrag != null)
                            onPointerDrag();
                    }
                }
                else
                {
                    deltaPosition = Vector2.zero;
                    lastPointerPosition = Input.mousePosition;
                    wasPointerDown = true;

                    if (onPointerDown != null)
                        onPointerDown();
                }
            }
            else
            {
                deltaPosition = Vector2.zero;
                if (wasPointerDown)
                {
                    wasPointerDown = false;

                    if (onPointerUp != null)
                        onPointerUp();
                }
            }
        }
    }
}