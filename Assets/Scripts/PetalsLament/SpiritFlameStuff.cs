using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpiritFlameStuff : MonoBehaviourPunCallbacks
{
    public string currentZone { get; set; }
    public GameObject linkedWraith;

    public void OnTriggerEnter(Collider other)
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
