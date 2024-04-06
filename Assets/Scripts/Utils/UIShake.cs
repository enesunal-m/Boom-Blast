using System.Collections;
using UnityEngine;

public class UIShake : MonoBehaviour
{
    public RectTransform uiElement;

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = uiElement.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f); // damping function

            float x = Random.Range(-1f, 1f) * magnitude * damper;
            float y = Random.Range(-1f, 1f) * magnitude * damper;

            uiElement.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null; // Wait until next frame
        }

        uiElement.localPosition = originalPosition; // Reset to original position
    }
}
