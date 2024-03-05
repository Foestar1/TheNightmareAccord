using UnityEngine;

public class languageSelector : MonoBehaviour
{
    [SerializeField]
    private int language;
    private SaveAndLoadData saveAndLoadStuff;

    private void Start()
    {
        saveAndLoadStuff = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
    }

    public void changeLanguage()
    {
        saveAndLoadStuff.selectedLanguage = language;
        saveAndLoadStuff.saveInfo();
    }
}
