using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ThresholdMusic : MonoBehaviourPunCallbacks
{
    private AudioSource objectAudioSource;

    private void Awake()
    {
        objectAudioSource = this.GetComponent<AudioSource>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (other.gameObject.GetPhotonView().IsMine)
                {
                    objectAudioSource.Play();
                }
            }
            else
            {
                objectAudioSource.Play();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (other.gameObject.GetPhotonView().IsMine)
                {
                    objectAudioSource.Stop();
                }
            }
            else
            {
                objectAudioSource.Stop();
            }
        }
    }
}
