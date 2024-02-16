using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HuntingFlame : MonoBehaviourPunCallbacks
{
    public GameObject targetObject;
    public float speed;

    public void Update()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (this.photonView.IsMine)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetObject.transform.position, step);
            }
        }
        else
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetObject.transform.position, step);
        }
    }
}
