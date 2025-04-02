using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float z = Mathf.Sin(elapsed * Mathf.PI * 4) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}


