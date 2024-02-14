using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LinkedPlayer : MonoBehaviourPunCallbacks
{
    public GameObject linkedPlayer;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Explosion")
        {
            int playerPWID = linkedPlayer.GetPhotonView().ViewID;
            this.photonView.RPC("reactivatePlayer", RpcTarget.All, playerPWID);
            Destroy(this.gameObject);
        }
    }

    #region RPC's
    [PunRPC]
    void reactivatePlayer(int playerPWID)
    {
        GameObject.Find("SpawnControls").GetComponent<Spawner>().playersAlive++;
        PhotonView tempView = PhotonView.Find(playerPWID);
        tempView.gameObject.SetActive(true);
        if (tempView.IsMine)
        {
            tempView.GetComponent<characterControls>().resetDead();
        }
    }
    #endregion
}
