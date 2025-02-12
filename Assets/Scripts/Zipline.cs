using UnityEngine;

public class Zipline : MonoBehaviour
{
    public Transform startPoint; 
    public Transform endPoint;   
    public float zipSpeed = 5f;  

    private void Start()
    {
        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPoint.position);
            lineRenderer.SetPosition(1, endPoint.position);
        }
    }
}
