using System.Collections;
using UnityEngine;

public class GrowAndFade : MonoBehaviour
{
    public float growTime = 2.0f; // Time in seconds to grow
    public float fadeTime = 2.0f; // Time in seconds to fade
    public float maxSize = 5.0f;  // Maximum size of the object during the grow phase
    public float fadeScaleMultiplier = 1.2f; // Multiplier for additional scaling during the fade phase
    public Color emissionColor = Color.white; // Emission color

    private Material material;
    private Color originalBaseColor;
    private Color originalEmissionColor;

    void Start()
    {
        material = GetComponent<Renderer>().material;
        originalBaseColor = material.GetColor("_BaseColor");
        originalEmissionColor = material.GetColor("_EmissionColor");

        // Set initial object scale and material properties with slight visibility
        transform.localScale = Vector3.zero;
        SetMaterialProperties(0, 0);

        // Start the grow and fade coroutine
        StartCoroutine(GrowAndFadeRoutine());
    }

    IEnumerator GrowAndFadeRoutine()
    {
        // Grow phase
        float timer = 0;
        while (timer <= growTime)
        {
            float progress = timer / growTime;
            transform.localScale = Vector3.one * maxSize * progress;
            SetMaterialProperties(progress, progress);
            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure object is fully grown and visible at the end of the grow phase
        transform.localScale = Vector3.one * maxSize;
        SetMaterialProperties(1, 1);

        // Fade phase with continuous growth
        timer = 0;
        float currentSize = maxSize;
        while (timer <= fadeTime)
        {
            float progress = 1 - (timer / fadeTime);
            currentSize *= fadeScaleMultiplier; // Continue to increase the size
            transform.localScale = Vector3.one * currentSize;
            SetMaterialProperties(progress, progress);
            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure object is fully faded and then destroy it
        SetMaterialProperties(0, 0);
        Destroy(gameObject);
    }

    void SetMaterialProperties(float emissionIntensity, float alpha)
    {
        // Adjust emission color
        material.SetColor("_EmissionColor", emissionColor * emissionIntensity);

        // Adjust base color and alpha
        Color baseColor = originalBaseColor;
        baseColor.a = alpha;
        material.SetColor("_BaseColor", baseColor);
    }
}
