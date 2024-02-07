using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class SoundEffectController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private bool isCrickets;
    [SerializeField]
    private bool isCrow;
    [SerializeField]
    private int minRange;
    [SerializeField]
    private int maxRange;
    [SerializeField]
    private int silencePoint;
    [SerializeField]
    private AudioSource effectAudioSource;

    private void Awake()
    {
        effectAudioSource = this.GetComponent<AudioSource>();
        if (isCrow)
        {
            StartCoroutine(PlayRandomAudio());
        }
    }

    private void Update()
    {
        if (isCrickets)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (var player in players)
            {
                if (PhotonNetwork.IsConnectedAndReady)
                {
                    if (player.GetPhotonView().IsMine)
                    {
                        float distance = Vector3.Distance(this.transform.position, player.transform.position);

                        if (distance <= silencePoint)
                        {
                            effectAudioSource.Stop();
                        }
                        else
                        {
                            if (!effectAudioSource.isPlaying)
                            {
                                effectAudioSource.Play();
                            }
                        }
                    }
                }
                else
                {
                    float distance = Vector3.Distance(this.transform.position, player.transform.position);

                    if (distance <= silencePoint)
                    {
                        effectAudioSource.Stop();
                    }
                    else
                    {
                        if (!effectAudioSource.isPlaying)
                        {
                            effectAudioSource.Play();
                        }
                    }
                }
            }
        }
    }

    private IEnumerator PlayRandomAudio()
    {
        while (true)
        {
            int waitTime = Random.Range(minRange, maxRange + 1);

            yield return new WaitForSeconds(waitTime);

            effectAudioSource.Play();
        }
    }
}
