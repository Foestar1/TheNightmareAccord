using UnityEngine;
using TMPro;

public class ControllerKeys : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField targetedField;
    [SerializeField]
    private ControllerKeys[] targetedButtons;

    public bool capsLockedAndLoaded { get; set; }

    [SerializeField]
    private string lowerCaseLetter;
    [SerializeField]
    private string upperCaseLetter;

    public void deleteButton()
    {
        if (targetedField.text.Length > 0)
        {
            targetedField.text = targetedField.text.Substring(0, targetedField.text.Length - 1);
        }
    }

    public void turnOnCaps()
    {
        foreach (ControllerKeys button in targetedButtons)
        {
            if (button.capsLockedAndLoaded)
            {
                button.capsLockedAndLoaded = false;
            }
            else
            {
                button.capsLockedAndLoaded = true;
            }
        }
    }

    public void enterKeyToField()
    {
        if (capsLockedAndLoaded)
        {
            targetedField.text += upperCaseLetter;
        }
        else
        {
            targetedField.text += lowerCaseLetter;
        }
    }
}
