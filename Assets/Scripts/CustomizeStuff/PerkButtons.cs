using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PerkButtons : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI availablePerksField;
    [SerializeField]
    private TextMeshProUGUI descriptionField;
    [SerializeField]
    private string[] languageDescriptions;
    private SaveAndLoadData saverObject;
    [SerializeField]
    private Sprite buttonLearnedImage;
    public bool perkController;

    private void OnEnable()
    {
        //if we don't have a saver object yet, get it
        if (saverObject == null)
        {
            saverObject = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        }

        if (perkController)
        {
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

            //now take away available perks based on what's learned
            if (saverObject.perk1Learned == 1)
            {
                perksAvailable--;
            }
            if (saverObject.perk2Learned == 1)
            {
                perksAvailable--;
            }
            if (saverObject.perk3Learned == 1)
            {
                perksAvailable--;
            }
            if (saverObject.perk4Learned == 1)
            {
                perksAvailable--;
            }

            //and finally set the text field
            availablePerksField.text = perksAvailable.ToString();
        }

        //works on all buttons
        if (saverObject.perk1Learned == 1 && this.gameObject.name == "Perk1")
        {
            this.GetComponent<Button>().image.sprite = buttonLearnedImage;
        }

        if (saverObject.perk2Learned == 1 && this.gameObject.name == "Perk2")
        {
            this.GetComponent<Button>().image.sprite = buttonLearnedImage;
        }

        if (saverObject.perk3Learned == 1 && this.gameObject.name == "Perk3")
        {
            this.GetComponent<Button>().image.sprite = buttonLearnedImage;
        }

        if (saverObject.perk4Learned == 1 && this.gameObject.name == "Perk4")
        {
            this.GetComponent<Button>().image.sprite = buttonLearnedImage;
        }
    }

    public void perkButtonSelected()
    {
        descriptionField.text = languageDescriptions[saverObject.selectedLanguage];
    }

    public void unlockPerk1()
    {
        //make a temp perk checker
        int perksAvailable = int.Parse(availablePerksField.text);

        if (saverObject.perk1Learned == 0)
        {
            if (perksAvailable > 0)
            {
                perksAvailable--;
                saverObject.perk1Learned = 1;
                saverObject.saveInfo();
                availablePerksField.text = perksAvailable.ToString();
                this.GetComponent<Button>().image.sprite = buttonLearnedImage;
            }
        }
    }

    public void unlockPerk2()
    {
        //make a temp perk checker
        int perksAvailable = int.Parse(availablePerksField.text);

        if (saverObject.perk2Learned == 0)
        {
            if (perksAvailable > 0)
            {
                perksAvailable--;
                saverObject.perk2Learned = 1;
                saverObject.saveInfo();
                availablePerksField.text = perksAvailable.ToString();
                this.GetComponent<Button>().image.sprite = buttonLearnedImage;
            }
        }
    }
    public void unlockPerk3()
    {
        //make a temp perk checker
        int perksAvailable = int.Parse(availablePerksField.text);

        if (saverObject.perk3Learned == 0)
        {
            if (perksAvailable > 0)
            {
                perksAvailable--;
                saverObject.perk3Learned = 1;
                saverObject.saveInfo();
                availablePerksField.text = perksAvailable.ToString();
                this.GetComponent<Button>().image.sprite = buttonLearnedImage;
            }
        }
    }
    public void unlockPerk4()
    {
        //make a temp perk checker
        int perksAvailable = int.Parse(availablePerksField.text);

        if (saverObject.perk4Learned == 0)
        {
            if (perksAvailable > 0)
            {
                perksAvailable--;
                saverObject.perk4Learned = 1;
                saverObject.saveInfo();
                availablePerksField.text = perksAvailable.ToString();
                this.GetComponent<Button>().image.sprite = buttonLearnedImage;
            }
        }
    }
}
