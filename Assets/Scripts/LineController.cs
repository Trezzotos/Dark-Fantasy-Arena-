using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Start()
    {   
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        // Materiale compatibile con Sorting Layer 2D
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Impostazioni di rendering: sopra il tappeto
        lineRenderer.sortingLayerName = "Foreground";
        lineRenderer.sortingOrder = 10;

        // Posizione Z leggermente davanti (opzionale, solo per sicurezza)
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    public void DrawLine(Vector3 endPoint, float duration = 0.1f)
    {   
        DrawLine(transform.position, endPoint, duration);
    }

    public void DrawLine(Vector3 startPoint, Vector3 endPoint, float duration)
    {


        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        lineRenderer.enabled = true;
        StartCoroutine(DeleteLine(duration));
    }

    private IEnumerator DeleteLine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        lineRenderer.enabled = false;
    }
}
