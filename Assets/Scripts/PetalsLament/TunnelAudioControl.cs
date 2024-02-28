using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections; // Required for Coroutine

public class TunnelAudioControl : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private AudioSource tunnelAudio;
    [SerializeField]
    private float lowCutoffFrequency = 500f; // Example low cutoff frequency
    [SerializeField]
    private float highCutoffFrequency = 5000f; // Example high cutoff frequency
    [SerializeField]
    private float transitionDuration = 1f; // Duration of the transition in seconds

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerRelevant(other))
        {
            StartCoroutine(ChangeCutoffFrequency(lowCutoffFrequency, highCutoffFrequency, transitionDuration));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayerRelevant(other))
        {
            StartCoroutine(ChangeCutoffFrequency(highCutoffFrequency, lowCutoffFrequency, transitionDuration));
        }
    }

    private IEnumerator ChangeCutoffFrequency(float startFreq, float endFreq, float duration)
    {
        float time = 0;
        AudioLowPassFilter filter = tunnelAudio.GetComponent<AudioLowPassFilter>();
        filter.enabled = true; // Make sure the filter is enabled

        while (time < duration)
        {
            filter.cutoffFrequency = Mathf.Lerp(startFreq, endFreq, time / duration);
            time += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        filter.cutoffFrequency = endFreq; // Ensure the final value is set

        // Optionally disable the filter if going back to high frequency (normal audio)
        if (endFreq == highCutoffFrequency)
        {
            filter.enabled = false;
        }
    }

    // Helper method to check if the other collider is the relevant player
    private bool IsPlayerRelevant(Collider other)
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            return other.CompareTag("Player");
        }
        else
        {
            return other.CompareTag("Player") && other.gameObject.GetPhotonView().IsMine;
        }
    }
}
