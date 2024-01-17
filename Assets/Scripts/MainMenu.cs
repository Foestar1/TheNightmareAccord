using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private SaveAndLoadData gamesData;

    private void Awake()
    {
        gamesData = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
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

    public void exitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
            Application.Quit();
    }
}
