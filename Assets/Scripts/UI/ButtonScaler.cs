using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonScaler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform leftImage;
    public RectTransform rightImage;
    public float scaleDownFactor = 0.8f; // How much the images will scale down on click
    private Vector3 originalLeftScale;
    private Vector3 originalRightScale;

    void Start()
    {
        originalLeftScale = leftImage.localScale;
        originalRightScale = rightImage.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        leftImage.localScale = new Vector3(originalLeftScale.x, originalLeftScale.y * scaleDownFactor, originalLeftScale.z);
        rightImage.localScale = new Vector3(originalRightScale.x, originalRightScale.y * scaleDownFactor, originalRightScale.z);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        leftImage.localScale = new Vector3(originalLeftScale.x, originalLeftScale.y, originalLeftScale.z);
        rightImage.localScale = new Vector3(originalRightScale.x, originalRightScale.y, originalRightScale.z);
    }
}
