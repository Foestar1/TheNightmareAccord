using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TunnelAudioControl : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private AudioSource tunnelAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (other.tag == "Player")
            {
                if (other.gameObject.GetPhotonView().IsMine)
                {
                    tunnelAudio.GetComponent<AudioLowPassFilter>().enabled = true;
                }
            }
        }
        else
        {
            if (other.tag == "Player")
            {
                tunnelAudio.GetComponent<AudioLowPassFilter>().enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (other.tag == "Player")
            {
                if (other.gameObject.GetPhotonView().IsMine)
                {
                    tunnelAudio.GetComponent<AudioLowPassFilter>().enabled = false;
                }
            }
        }
        else
        {
            if (other.tag == "Player")
            {
                tunnelAudio.GetComponent<AudioLowPassFilter>().enabled = false;
            }
        }
    }
}
