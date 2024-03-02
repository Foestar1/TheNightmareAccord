using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LinkedPlayer : MonoBehaviourPunCallbacks
{
    public GameObject linkedPlayer;
    public GameObject linkedObserver;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Explosion")
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                int playerPWID = linkedPlayer.GetPhotonView().ViewID;
                this.photonView.RPC("reactivatePlayer", RpcTarget.All, playerPWID);
                if (this.photonView.IsMine)
                {
                    GameObject.Find("SpawnControls").GetComponent<Spawner>().soloRevives++;
                    Destroy(linkedObserver);
                }
            }
            Destroy(this.gameObject);
        }
    }

    #region RPC's
    [PunRPC]
    void reactivatePlayer(int playerPWID)
    {
        GameObject.Find("SpawnControls").GetComponent<Spawner>().playersAlive++;
        GameObject.Find("SpawnControls").GetComponent<Spawner>().totalRevives++;
        PhotonView tempView = PhotonView.Find(playerPWID);
        tempView.gameObject.SetActive(true);
        if (tempView.IsMine)
        {
            tempView.GetComponent<characterControls>().resetDead();
        }
    }
    #endregion
}
