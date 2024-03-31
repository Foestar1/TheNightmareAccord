using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PerkControls : MonoBehaviour
{
    [SerializeField]
    private Sprite resetImage;
    [SerializeField]
    private TextMeshProUGUI availablePointsField;
    [SerializeField]
    private Button[] perkButtons;
    private SaveAndLoadData saverObject;

    private void OnEnable()
    {
        //if we don't have a saver object yet, get it
        if (saverObject == null)
        {
            saverObject = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        }
    }

    public void resetPoints()
    {
        //reset all perks
        saverObject.perk1Learned = 0;
        saverObject.perk2Learned = 0;
        saverObject.perk3Learned = 0;
        saverObject.perk4Learned = 0;
        saverObject.saveInfo();

        //adjust their buttons
        foreach (Button button in perkButtons)
        {
            button.GetComponent<Button>().image.sprite = resetImage;
        }

        //make a temp perk checker
        int perksAvailable = 0;

        //check if levels are beaten
        if (saverObject.level1Complete == 1)
        {
            perksAvailable++;
        }
        if (saverObject.level2Complete == 1)
        {
            perksAvailable++;
        }
        if (saverObject.level3Complete == 1)
        {
            perksAvailable++;
        }
        if (saverObject.level4Complete == 1)
        {
            perksAvailable++;
        }

        //and finally set the text field
        availablePointsField.text = perksAvailable.ToString();
    }
}
