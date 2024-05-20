using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectUIPanels : MonoBehaviour
{
    public RectTransform panel1;
    public RectTransform panel2;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 3;
    }

    void Update()
    {
        if (panel1 != null && panel2 != null)
        {
            if (panel1.gameObject.activeInHierarchy && panel2.gameObject.activeInHierarchy)
            {
                Vector3 panel1Edge = GetClosestEdge(panel1, panel2.position);
                Vector3 panel2Edge = GetClosestEdge(panel2, panel1.position);

                Vector3 controlPoint = (panel1Edge + panel2Edge) / 2f;

                lineRenderer.SetPosition(0, panel1Edge);
                lineRenderer.SetPosition(1, controlPoint);
                lineRenderer.SetPosition(2, panel2Edge);

                lineRenderer.enabled = true;
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private Vector3 GetClosestEdge(RectTransform panel, Vector3 targetPosition)
    {
        Vector3[] corners = new Vector3[4];
        panel.GetWorldCorners(corners);

        Vector3 closestPoint = corners[0];
        float closestDistance = Vector3.Distance(targetPosition, closestPoint);

        for (int i = 1; i < 4; i++)
        {
            float distance = Vector3.Distance(targetPosition, corners[i]);
            if (distance < closestDistance)
            {
                closestPoint = corners[i];
                closestDistance = distance;
            }
        }

        return closestPoint;
    }
}
