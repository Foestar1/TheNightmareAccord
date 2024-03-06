using UnityEngine;
using TMPro;

public class languageTranslatorLevelInfo : MonoBehaviour
{
    private int levelSelected;
    private GameObject hubControl;
    [SerializeField]
    private string[] petalsLament;
    private SaveAndLoadData saveAndLoadStuff;
    private int selectedLanguage;
    private TextMeshProUGUI textItself;

    private void Start()
    {
        saveAndLoadStuff = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        hubControl = GameObject.Find("HubController");
        levelSelected = hubControl.GetComponent<HubController>().levelChoice;
        textItself = this.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (selectedLanguage != saveAndLoadStuff.selectedLanguage)
        {
            selectedLanguage = saveAndLoadStuff.selectedLanguage;
        }

        if (levelSelected == 0)
        {
            if (textItself.text != petalsLament[selectedLanguage])
            {
                textItself.text = petalsLament[selectedLanguage];
            }
        }
    }
}
