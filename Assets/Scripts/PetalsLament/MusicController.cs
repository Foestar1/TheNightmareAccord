using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] musicIntensity;
    private AudioSource musicController;
    private SaveAndLoadData saver;
    private Spawner spawner;
    public bool gameStarted { get; set; }

    private void Awake()
    {
        musicController = this.GetComponent<AudioSource>();
        musicController.clip = musicIntensity[0];

        //save and loader
        if (saver == null)
        {
            saver = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        }

        //spawner
        if (spawner == null)
        {
            spawner = GameObject.Find("SpawnControls").GetComponent<Spawner>();
        }
    }

    private void Update()
    {
        if (gameStarted)
        {
            if (!musicController.isPlaying)
            {
                musicController.Play();
            }

            if (spawner.goalsNotFound == 3)
            {
                if (musicController.clip != musicIntensity[0])
                {
                    float currentTime = musicController.time;
                    musicController.clip = musicIntensity[0];
                    musicController.time = currentTime;
                    musicController.Play();
                }
            }
            else if (spawner.goalsNotFound == 2)
            {
                if (musicController.clip != musicIntensity[1])
                {
                    float currentTime = musicController.time;
                    musicController.clip = musicIntensity[1];
                    musicController.time = currentTime;
                    musicController.Play();
                }
            }
            else if (spawner.goalsNotFound == 1)
            {
                if (musicController.clip != musicIntensity[2])
                {
                    float currentTime = musicController.time;
                    musicController.clip = musicIntensity[2];
                    musicController.time = currentTime;
                    musicController.Play();
                }
            }
            else if (spawner.goalsNotFound == 0)
            {
                if (musicController.isPlaying)
                {
                    musicController.Stop();
                }
            }
        }
    }
}
