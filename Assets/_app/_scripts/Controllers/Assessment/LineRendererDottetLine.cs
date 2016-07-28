using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LineRendererDottetLine : MonoBehaviour {
    public Vector3 lastPoint, currentPoint;
    RaycastHit hit;

    LineRenderer line;

    // Use this for initialization
    void Start() {
        line = gameObject.AddComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
            if (lastPoint == null) {
                currentPoint = hit.point;
                return;
            } else {
                lastPoint = currentPoint;
                currentPoint = hit.point;
                DrawLine(lastPoint, currentPoint);
            }
        }
    }


    private void DrawLine(Vector3 p0, Vector3 p1) {
        line.SetWidth(1, 1);
        line.SetPosition(0, p0);
        line.SetPosition(1, p1);
        Debug.Log("p0 = " + p0 + " and p1 = " + p1);
    }
}