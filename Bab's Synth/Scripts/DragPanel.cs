using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform panelRectTransform;
    private Canvas canvas;

    void Start()
    {
        panelRectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        panelRectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            panelRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        else
        {
            Vector3 newPos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(panelRectTransform, eventData.position, eventData.pressEventCamera, out newPos))
            {
                panelRectTransform.position = newPos;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // You can add snapping behavior or other logic here if you want:)
    }
}
