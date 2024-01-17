using UnityEngine;

public class SaveAndLoadData : MonoBehaviour
{
    public int haveSaveData { get; set; }
    public int haveSavedGame { get; set; }
    public float soundVolume { get; set; }
    public float musicVolume { get; set; }
    public int level1Complete { get; set; }
    public int level2Complete { get; set; }
    public int level3Complete { get; set; }
    public int level4Complete { get; set; }

    public void saveInfo()
    {
        PlayerPrefs.SetInt("HaveSaveData", haveSaveData);
        PlayerPrefs.SetInt("HaveSavedGame", haveSavedGame);
        PlayerPrefs.SetFloat("SoundVolume", soundVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("Level1Complete", level1Complete);
        PlayerPrefs.SetInt("Level2Complete", level2Complete);
        PlayerPrefs.SetInt("Level3Complete", level3Complete);
        PlayerPrefs.SetInt("Level4Complete", level4Complete);
        PlayerPrefs.Save();
    }

    public void loadInfo()
    {
        haveSaveData = PlayerPrefs.GetInt("HaveSaveData");
        haveSavedGame = PlayerPrefs.GetInt("HaveSavedGame");
        soundVolume = PlayerPrefs.GetFloat("SoundVolume");
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        level1Complete = PlayerPrefs.GetInt("Level1Complete");
        level2Complete = PlayerPrefs.GetInt("Level2Complete");
        level3Complete = PlayerPrefs.GetInt("Level3Complete");
        level4Complete = PlayerPrefs.GetInt("Level4Complete");
    }

    public void Awake()
    {
        loadInfo();
        DontDestroyOnLoad(this);

        if (haveSaveData == 0)
        {
            haveSaveData = 1;
            haveSavedGame = 0;
            soundVolume = 0.5f;
            musicVolume = 0.5f;
            level1Complete = 0;
            level2Complete = 0;
            level3Complete = 0;
            level4Complete = 0;
            saveInfo();
        }
    }
}
