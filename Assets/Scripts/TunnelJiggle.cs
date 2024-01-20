using UnityEngine;
using System.Collections;

public class TunnelJiggle : MonoBehaviour
{
    public float rippleScaleMultiplier = 1.2f;
    public float rippleDuration = 0.5f;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(ApplyRipple());
    }

    IEnumerator ApplyRipple()
    {
        float elapsedTime = 0f;

        while (elapsedTime < rippleDuration)
        {
            float scale = Mathf.Lerp(1f, rippleScaleMultiplier, elapsedTime / rippleDuration);
            transform.localScale = originalScale * scale;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale; // Reset to the original scale
    }
}
