using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharCustomizer2 : MonoBehaviourPunCallbacks
{
    private SaveAndLoadData saverObject;
    private int selectedNumber;

    [SerializeField]
    private bool isHeadAccessory;
    [SerializeField]
    private bool isTopAccessory;
    [SerializeField]
    private bool isJammiesAccessory;
    [SerializeField]
    private bool isSlippersAccessory;

    [SerializeField]
    private GameObject[] childrenObjects;

    private void Awake()
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

        if (isHeadAccessory)
        {
            selectedNumber = saverObject.chosenHead;
        }

        if (isTopAccessory)
        {
            selectedNumber = saverObject.chosenTop;
        }

        if (isJammiesAccessory)
        {
            selectedNumber = saverObject.chosenJammies;
        }

        if (isSlippersAccessory)
        {
            selectedNumber = saverObject.chosenFeet;
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            this.photonView.RPC("sendData", RpcTarget.All, selectedNumber);
        }
        else
        {
            childrenObjects[selectedNumber].SetActive(true);
        }
    }

    [PunRPC]
    void sendData(int selectedNumber)
    {
        childrenObjects[selectedNumber].SetActive(true);
    }
}
