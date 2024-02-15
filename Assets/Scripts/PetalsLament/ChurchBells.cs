using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ChurchBells : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private AudioSource bellAudio;
    [SerializeField]
    private Animator bellAnimator;
    private Spawner spawnObject;
    private int goalsLeft;

    private void Awake()
    {
        spawnObject = GameObject.Find("SpawnControls").GetComponent<Spawner>();
    }

    private void Start()
    {
        goalsLeft = spawnObject.goalsNotFound;
    }

    private void Update()
    {
        if (goalsLeft != spawnObject.goalsNotFound)
        {
            goalsLeft = spawnObject.goalsNotFound;
            playBellsToll();
        }

        if (bellAnimator.GetCurrentAnimatorStateInfo(0).IsName("ChurchBells"))
        {
            bellAnimator.SetBool("booming", false);
        }
    }

    private void playBellsToll()
    {
        bellAudio.Play();
        bellAnimator.SetBool("booming", true);
    }
}
