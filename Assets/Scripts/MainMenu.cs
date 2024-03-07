using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private SaveAndLoadData gamesData;
    [SerializeField]
    private Button languageButton;
    [SerializeField]
    private Sprite[] flagChoice;
    [SerializeField]
    private TextMeshProUGUI descriptionBox;
    [SerializeField]
    private string[] playButtonLanguages;
    [SerializeField]
    private string[] settingsButtonLanguages;
    [SerializeField]
    private string[] exitButtonLanguages;
    [SerializeField]
    private string[] deleteButtonLanguages;
    [SerializeField]
    private string[] cancelButtonLanguages;
    [SerializeField]
    private string[] acceptButtonLanguages;

    private void Start()
    {
        gamesData = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        chooseFlag();
    }

    private void chooseFlag()
    {
        languageButton.image.sprite = flagChoice[gamesData.selectedLanguage];
    }

    public void playButton()
    {
        if (gamesData.haveSavedGame == 0)
        {
            gamesData.haveSavedGame = 1;
            gamesData.saveInfo();
        }
    }

    public void deleteGameButton()
    {
        //PlayerPrefs.DeleteAll();
        gamesData.haveSavedGame = 0;
        gamesData.level1Complete = 0;
        gamesData.level2Complete = 0;
        gamesData.level3Complete = 0;
        gamesData.level4Complete = 0;
        gamesData.saveInfo();
    }

    #region main menu language changes for description box
    public void playButtonLanguageChange()
    {
        descriptionBox.text = playButtonLanguages[gamesData.selectedLanguage];
    }

    public void settingsButtonLanguageChange()
    {
        descriptionBox.text = settingsButtonLanguages[gamesData.selectedLanguage];
    }

    public void exitButtonLanguageChange()
    {
        descriptionBox.text = exitButtonLanguages[gamesData.selectedLanguage];
    }

    public void deleteButtonLanguageChange()
    {
        descriptionBox.text = deleteButtonLanguages[gamesData.selectedLanguage];
    }

    public void cancelButtonLanguageChange()
    {
        descriptionBox.text = cancelButtonLanguages[gamesData.selectedLanguage];
    }

    public void acceptButtonLanguageChange()
    {
        descriptionBox.text = acceptButtonLanguages[gamesData.selectedLanguage];
    }
    #endregion

    public void exitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
            Application.Quit();
    }

    public void openDiscord()
    {
        Application.OpenURL("https://discord.gg/sWjUvQ5uUr");
    }
}
