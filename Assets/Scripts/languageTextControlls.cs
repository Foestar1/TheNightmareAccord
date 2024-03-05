using UnityEngine;
using TMPro;

public class languageTextControlls : MonoBehaviour
{
    [SerializeField]
    private string[] languagedString;
    private SaveAndLoadData saveAndLoadStuff;
    private int selectedLanguage;
    private TextMeshProUGUI textItself;

    private void Start()
    {
        saveAndLoadStuff = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        textItself = this.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (selectedLanguage != saveAndLoadStuff.selectedLanguage)
        {
            selectedLanguage = saveAndLoadStuff.selectedLanguage;
        }

        if (textItself.text != languagedString[selectedLanguage])
        {
            textItself.text = languagedString[selectedLanguage];
        }
    }
}
