using UnityEngine;

public class SaveAndLoadData : MonoBehaviour
{
    public int haveSaveData { get; set; } //0 is no saved data, 1 is have data
    public int haveSavedGame { get; set; }//0 is no saved game, 1 is have game
    public int selectedLanguage { get; set; } //0-English,1-Russian,2-Chinese,3-German,4-Japanese,5-Korean,6-Spanish
    public float soundVolume { get; set; }
    public float musicVolume { get; set; }
    public int level1Complete { get; set; }//0 is not complete, 1 is complete (SAME FOR BELOW)
    public int level1CompleteSecondary { get; set; }
    public float level1CompleteSpeed { get; set; }
    public int level2Complete { get; set; }
    public int level3Complete { get; set; }
    public int level4Complete { get; set; }

    public void saveInfo()
    {
        PlayerPrefs.SetInt("HaveSaveData", haveSaveData);
        PlayerPrefs.SetInt("HaveSavedGame", haveSavedGame);
        PlayerPrefs.SetInt("selectedLanguage", selectedLanguage);
        PlayerPrefs.SetFloat("SoundVolume", soundVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("Level1Complete", level1Complete);
        PlayerPrefs.SetInt("level1CompleteSecondary", level1CompleteSecondary);
        PlayerPrefs.SetFloat("level1CompleteSpeed", level1CompleteSpeed);
        PlayerPrefs.SetInt("Level2Complete", level2Complete);
        PlayerPrefs.SetInt("Level3Complete", level3Complete);
        PlayerPrefs.SetInt("Level4Complete", level4Complete);
        PlayerPrefs.Save();
    }

    public void loadInfo()
    {
        haveSaveData = PlayerPrefs.GetInt("HaveSaveData");
        haveSavedGame = PlayerPrefs.GetInt("HaveSavedGame");
        selectedLanguage = PlayerPrefs.GetInt("selectedLanguage");
        soundVolume = PlayerPrefs.GetFloat("SoundVolume");
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        level1Complete = PlayerPrefs.GetInt("Level1Complete");
        level1CompleteSecondary = PlayerPrefs.GetInt("level1CompleteSecondary");
        level1CompleteSpeed = PlayerPrefs.GetInt("level1CompleteSpeed");
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
            selectedLanguage = 0;
            soundVolume = 0.5f;
            musicVolume = 0.5f;
            level1Complete = 0;
            level1CompleteSecondary = 0;
            level1CompleteSpeed = 0;
            level2Complete = 0;
            level3Complete = 0;
            level4Complete = 0;
            saveInfo();
        }
    }
}
