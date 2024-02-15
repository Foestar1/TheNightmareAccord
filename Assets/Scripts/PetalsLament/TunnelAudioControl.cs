using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TunnelAudioControl : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private AudioSource[] tunnelAudios;

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (other.gameObject.GetPhotonView().IsMine && other.tag == "Player")
            {
                foreach (AudioSource tunnelAudio in tunnelAudios)
                {
                    tunnelAudio.GetComponent<AudioLowPassFilter>().enabled = true;
                }
            }
        }
        else
        {
            if (other.tag == "Player")
            {
                foreach (AudioSource tunnelAudio in tunnelAudios)
                {
                    tunnelAudio.GetComponent<AudioLowPassFilter>().enabled = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (other.gameObject.GetPhotonView().IsMine && other.tag == "Player")
            {
                foreach (AudioSource tunnelAudio in tunnelAudios)
                {
                    tunnelAudio.GetComponent<AudioLowPassFilter>().enabled = false;
                }
            }
        }
        else
        {
            if (other.tag == "Player")
            {
                foreach (AudioSource tunnelAudio in tunnelAudios)
                {
                    tunnelAudio.GetComponent<AudioLowPassFilter>().enabled = false;
                }
            }
        }
    }
}
