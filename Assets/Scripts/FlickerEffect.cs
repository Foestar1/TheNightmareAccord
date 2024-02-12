using UnityEngine;

public class FlickerEffect : MonoBehaviour
{
    public float intensity = 0.1f;
    public float perSecond = 0.3f;
    public float randomness = 1.0f;

    private Light _light;
    private float _startingIntensity;
    private float _time;

    void Start()
    {
        _light = GetComponent<Light>(); // Get the Light component attached to this GameObject
        _startingIntensity = _light.intensity; // Store the starting intensity
    }

    void Update()
    {
        _time += Time.deltaTime * (1 - Random.Range(-randomness, randomness));
        _light.intensity = _startingIntensity + Mathf.Sin(_time * perSecond) * intensity;
    }
}
