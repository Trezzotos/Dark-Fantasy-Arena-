using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class LineController : MonoBehaviour
{
    LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    public void DrawLine(Vector3 startPoint, Vector3 endPoint, float duration = 0.1f)
    {
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        lineRenderer.enabled = true;
        StartCoroutine(nameof(DeleteLine), duration);
    }

    IEnumerator DeleteLine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        lineRenderer.enabled = false;
    }
}