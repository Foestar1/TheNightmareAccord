using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DestroyMe : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject floatingFlame;
    [SerializeField]
    private Transform parentObject;

    public void destroyME()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                var myPlayerObject = PhotonNetwork.Instantiate(this.floatingFlame.name, parentObject.position, parentObject.rotation, 0);
            }
        }
        else
        {
            var newPlayerListing = Instantiate(floatingFlame, parentObject.position, parentObject.rotation);
        }

        Destroy(parentObject.gameObject);
    }
}
