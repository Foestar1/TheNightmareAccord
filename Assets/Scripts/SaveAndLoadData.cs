using UnityEngine;

public class SaveAndLoadData : MonoBehaviour
{
    #region variables
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
    public string headColor { get; set; }
    public string topColor { get; set; }
    public string JammiesColor { get; set; }
    public string FeetColor { get; set; }
    public string EyesColor { get; set; }
    public string SkinColor { get; set; }
    public int chosenHead { get; set; } //will have to list the head choices somewhere
    public int chosenTop { get; set; } //will have to list the top choices somewhere
    public int chosenJammies { get; set; } //will have to list the jammies choices somewhere
    public int chosenFeet { get; set; } //will have to list the feet choices somewhere

    private bool needToSave;
    #endregion

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
        PlayerPrefs.SetString("headColor", headColor);
        PlayerPrefs.SetString("topColor", topColor);
        PlayerPrefs.SetString("JammiesColor", JammiesColor);
        PlayerPrefs.SetString("FeetColor", FeetColor);
        PlayerPrefs.SetString("EyesColor", EyesColor);
        PlayerPrefs.SetString("SkinColor", SkinColor);
        PlayerPrefs.SetInt("chosenHead", chosenHead);
        PlayerPrefs.SetInt("chosenTop", chosenTop);
        PlayerPrefs.SetInt("chosenJammies", chosenJammies);
        PlayerPrefs.SetInt("chosenFeet", chosenFeet);
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
        headColor = PlayerPrefs.GetString("headColor");
        topColor = PlayerPrefs.GetString("topColor");
        JammiesColor = PlayerPrefs.GetString("JammiesColor");
        FeetColor = PlayerPrefs.GetString("FeetColor");
        EyesColor = PlayerPrefs.GetString("EyesColor");
        SkinColor = PlayerPrefs.GetString("SkinColor");
        chosenHead = PlayerPrefs.GetInt("chosenHead");
        chosenTop = PlayerPrefs.GetInt("chosenTop");
        chosenJammies = PlayerPrefs.GetInt("chosenJammies");
        chosenFeet = PlayerPrefs.GetInt("chosenFeet");
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
            headColor = "3A3025";
            topColor = "3A3025";
            JammiesColor = "006C5C";
            FeetColor = "3A3025";
            EyesColor = "845335";
            SkinColor = "896557";
            chosenHead = 0;
            chosenTop = 0;
            chosenJammies = 0;
            chosenFeet = 0;
            saveInfo();
        }
        else
        {
            #region color checks
            #region head color check
            if (headColor == null || headColor == "")
            {
                headColor = "3A3025";
                needToSave = true;
            }
            #endregion

            #region top color check
            if (topColor == null || topColor == "")
            {
                topColor = "3A3025";
                needToSave = true;
            }
            #endregion

            #region jammies color check
            if (JammiesColor == null || JammiesColor == "")
            {
                JammiesColor = "006C5C";
                needToSave = true;
            }
            #endregion

            #region feet color check
            if (FeetColor == null || FeetColor == "")
            {
                FeetColor = "006C5C";
                needToSave = true;
            }
            #endregion

            #region eyes color check
            if (EyesColor == null || EyesColor == "")
            {
                EyesColor = "845335";
                needToSave = true;
            }
            #endregion

            #region skin color check
            if (SkinColor == null || SkinColor == "")
            {
                SkinColor = "896557";
                needToSave = true;
            }
            #endregion
            #endregion
            if (needToSave)
            {
                saveInfo();
                //needToSave = false;
            }
        }
    }
}
