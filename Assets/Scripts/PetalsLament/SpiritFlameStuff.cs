using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpiritFlameStuff : MonoBehaviourPunCallbacks
{
    public string currentZone { get; set; }
    public GameObject linkedWraith;

    public void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (this.photonView.IsMine)
            {
                if (other.tag == "EnemyRevive")
                {
                    int wraithID = linkedWraith.GetPhotonView().ViewID;
                    int otherID = other.gameObject.GetPhotonView().ViewID;
                    this.photonView.RPC("reviveTime", RpcTarget.All, wraithID, otherID);
                    //Destroy(other.gameObject);
                    //Destroy(this.gameObject);
                }
            }
        }
        else
        {
            if (other.tag == "EnemyRevive")
            {
                linkedWraith.SetActive(true);
                linkedWraith.GetComponent<WraithAI>().reviveME();
                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        }
    }

    #region RPC's
    [PunRPC]
    void reviveTime(int wraithID, int otherID)
    {
        PhotonView.Find(wraithID).gameObject.SetActive(true);
        PhotonView.Find(wraithID).gameObject.GetComponent<WraithAI>().reviveME();
        Destroy(PhotonView.Find(otherID).gameObject);
        Destroy(this.gameObject);
    }
    #endregion
}
