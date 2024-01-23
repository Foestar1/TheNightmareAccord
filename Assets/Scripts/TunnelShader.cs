using UnityEngine;

public class TunnelShader : MonoBehaviour
{
    public Material shaderMaterial; // Reference to the material with your shader
    public float effectDuration = 5.0f; // Duration of the shader effect in seconds

    private float timer = 0.0f;
    private bool isEffectActive = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has a CharacterController component
        CharacterController characterController = other.GetComponent<CharacterController>();
        if (characterController != null)
        {
            StartShaderEffect();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the colliding object has a CharacterController component
        CharacterController characterController = other.GetComponent<CharacterController>();
        if (characterController != null)
        {
            StopShaderEffect();
        }
    }

    private void Update()
    {
        if (isEffectActive)
        {
            timer += Time.deltaTime;

            // Check if the effect duration has passed
            if (timer >= effectDuration)
            {
                StopShaderEffect();
            }
        }
    }

    private void StartShaderEffect()
    {
        isEffectActive = true;
        timer = 0.0f;
        shaderMaterial.SetFloat("_YourParameterName", 1.0f); // Replace with the actual parameter name you want to control
    }

    private void StopShaderEffect()
    {
        isEffectActive = false;
        shaderMaterial.SetFloat("_YourParameterName", 0.0f); // Replace with the actual parameter name you want to control
    }
}
