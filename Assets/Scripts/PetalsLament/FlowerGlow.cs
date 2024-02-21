using UnityEngine;
using System.Collections;

public class FlowerGlow : MonoBehaviour
{
    [SerializeField]
    private GameObject fireflyParticles;
    [SerializeField]
    private float emissionStrength = 10.0f;
    [SerializeField]
    private float emissionTime = 1.5f;

    // Assuming the shader property name is "Emission Strength"
    private string emissionStrengthProp = "_EmissionStrength";
    private MaterialPropertyBlock propBlock;
    private Renderer rend;

    private void Start()
    {
        propBlock = new MaterialPropertyBlock();
        rend = GetComponent<Renderer>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TeddyZone")
        {
            Instantiate(fireflyParticles, transform.position, transform.rotation);
            // Start increasing the emission strength
            StopAllCoroutines(); // Ensure to stop any ongoing fade out
            StartCoroutine(FadeEmission(0, emissionStrength, emissionTime)); // Adjust time to desired fade in duration
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "TeddyZone")
        {
            // Start decreasing the emission strength
            StopAllCoroutines(); // Ensure to stop any ongoing fade in
            StartCoroutine(FadeEmission(emissionStrength, 0, emissionTime)); // Adjust time to desired fade out duration
        }
    }

    IEnumerator FadeEmission(float startValue, float endValue, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration; // Normalizes time to a 0-1 scale
            float currentEmission = Mathf.Lerp(startValue, endValue, normalizedTime);

            rend.GetPropertyBlock(propBlock);
            propBlock.SetFloat(emissionStrengthProp, currentEmission);
            rend.SetPropertyBlock(propBlock);

            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set after the loop
        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat(emissionStrengthProp, endValue);
        rend.SetPropertyBlock(propBlock);
    }
}
