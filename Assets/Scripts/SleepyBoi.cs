using UnityEngine;

public class SleepyBoi : MonoBehaviour
{
    [SerializeField]
    private GameObject fadeAndLoader;

    private void activatePlayer()
    {
        fadeAndLoader.SetActive(true);
    }
}
