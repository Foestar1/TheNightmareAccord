using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class JammieCustomizer : MonoBehaviourPunCallbacks
{
    private SaveAndLoadData saverObject;
    [SerializeField]
    private Texture[] jammieChoices;

    private void OnEnable()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (this.photonView.IsMine)
            {
                doTheStuff();
            }
        }
        else
        {
            doTheStuff();
        }
    }

    private void doTheStuff()
    {
        if (saverObject == null)
        {
            saverObject = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        }

        int jammieChoice = saverObject.chosenJammies;
        if (PhotonNetwork.IsConnectedAndReady)
        {
            this.photonView.RPC("sendData", RpcTarget.All, jammieChoice);
        }
        else
        {
            this.GetComponent<Renderer>().material.mainTexture = jammieChoices[jammieChoice];
        }
    }

    [PunRPC]
    void sendData(int jammieChoice)
    {
        this.GetComponent<Renderer>().material.mainTexture = jammieChoices[jammieChoice];
    }
}
