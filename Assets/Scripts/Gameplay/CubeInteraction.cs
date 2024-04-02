using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CubeInteraction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 originalPosition;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false; // Allow events to pass through the cube, enabling detection of the drop target
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // Adjust for canvas scale
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Determine if the cube has been dragged to a valid new position
        // This example snaps the cube back to its original position
        rectTransform.anchoredPosition = originalPosition;

        // Implement logic to check for matches and handle cube swapping here
        // This might involve calling a method on the GridManager and passing the cube's original and new positions
    }
}
