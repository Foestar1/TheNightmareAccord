using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FaderLoadScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI loadingText;

    [SerializeField]
    private Animator sleepyBoi;

    [SerializeField]
    private GameObject[] thingsToEnable;
    [SerializeField]
    private GameObject[] thingsToDisable;

    private void transitionPhase()
    {
        foreach(GameObject thing in thingsToEnable)
        {
            thing.gameObject.SetActive(true);
        }

        foreach (GameObject thing in thingsToDisable)
        {
            thing.gameObject.SetActive(false);
        }
    }

    private void loadHub()
    {
        loadingText.gameObject.SetActive(true);
        SceneManager.LoadScene("DreamHub");
    }

    private void fadeInActivate()
    {
        sleepyBoi.SetBool("waking", true);
    }
}
