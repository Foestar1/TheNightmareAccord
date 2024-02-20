using UnityEngine;

public class FootstepControl : MonoBehaviour
{
    [SerializeField]
    private AudioSource[] playerFootsteps;

    public void soundOne()
    {
        playerFootsteps[0].Play();
    }

    public void soundTwo()
    {
        playerFootsteps[1].Play();
    }
}
