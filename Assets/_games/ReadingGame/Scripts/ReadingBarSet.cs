using System;
using EA4S;
using UnityEngine;

public class ReadingBarSet : MonoBehaviour
{
    public bool active = true;

    public ReadingBar[] bars;

    public ReadingBar activeBar;
    public Camera mainCamera;

    public ReadingBar PickGlass(Camera main, Vector2 lastPointerPosition)
    {
        if (activeBar == null)
            return null;

        var barCollider = activeBar.glass.GetComponentInChildren<Collider>();

        RaycastHit hitInfo;
        if (barCollider.Raycast(mainCamera.ScreenPointToRay(lastPointerPosition), out hitInfo, 1000))
        {
            return activeBar;
        }
        return null;
    }
}
