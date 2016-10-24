using UnityEngine;
using System.Collections;

public class ProjectLineRendererController : MonoBehaviour
{
    public static ProjectLineRendererController instance;

    private LineRenderer lineRenderer;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPoints(Vector3[] points)
    {
        if (SROptions.Current.ShowProjection)
        {
            lineRenderer.SetPositions(points);
        }

        else
        {
            lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
        }
    }
}
