using UnityEngine;
using System.Collections;

public class MyLetter : MonoBehaviour
{
    // This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)
    public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;

    // This stores the finger that's currently dragging this GameObject
    private Lean.LeanFinger draggingFinger;

    protected virtual void OnEnable() {
        Lean.LeanTouch.OnFingerDown += OnFingerDown;
        Lean.LeanTouch.OnFingerUp += OnFingerUp;
    }

    protected virtual void OnDisable() {
        Lean.LeanTouch.OnFingerDown -= OnFingerDown;
        Lean.LeanTouch.OnFingerUp -= OnFingerUp;
    }

    protected virtual void LateUpdate() {
        if (draggingFinger != null) {
            Lean.LeanTouch.MoveObject(transform, draggingFinger.DeltaScreenPosition, Camera.main);
        }
    }

    public void OnFingerDown(Lean.LeanFinger finger) {
        // Raycast information
        var ray = finger.GetRay();
        var hit = default(RaycastHit);

        // Was this finger pressed down on a collider?
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true) {
            // Was that collider this one?
            if (hit.collider.gameObject == gameObject) {
                // Set the current finger to this one
                draggingFinger = finger;
            }
        }
    }

    public void OnFingerUp(Lean.LeanFinger finger) {
        if (finger == draggingFinger) {
            draggingFinger = null;
        }
    }
}