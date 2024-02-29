using UnityEngine;

public class ThresholdMusic : MonoBehaviour
{
    public AudioSource audioSource;

    private bool isPlayerInside = false;

    void OnTriggerStay(Collider other)
    {
        // Check if the collider inside the trigger zone is the player
        if (other.CompareTag("Player"))
        {
            // If the player is inside the trigger zone and the audio source is not playing, play it
            if (!isPlayerInside && !audioSource.isPlaying)
            {
                audioSource.Play();
                isPlayerInside = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the collider exiting the trigger zone is the player
        if (other.CompareTag("Player"))
        {
            // If the player exits the trigger zone and the audio source is playing, stop it
            if (isPlayerInside && audioSource.isPlaying)
            {
                audioSource.Stop();
                isPlayerInside = false;
            }
        }
    }
}
