using UnityEngine;
using UnityEngine.UI;

public class ImageCustomizer : MonoBehaviour
{
    private SaveAndLoadData saverObject;
    private Color selectedColor;
    private string hexColor;

    [SerializeField]
    private bool isHeadAccessory;
    [SerializeField]
    private bool isTopAccessory;
    [SerializeField]
    private bool isJammiesAccessory;
    [SerializeField]
    private bool isSlippersAccessory;
    [SerializeField]
    private bool isEyeColor;
    [SerializeField]
    private bool isSkinColor;

    private void OnEnable()
    {
        doTheStuff();
    }

    private void doTheStuff()
    {
        if (saverObject == null)
        {
            saverObject = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        }

        if (isHeadAccessory)
        {
            hexColor = saverObject.headColor;
        }

        if (isTopAccessory)
        {
            hexColor = saverObject.topColor;
        }

        if (isJammiesAccessory)
        {
            hexColor = saverObject.JammiesColor;
        }

        if (isSlippersAccessory)
        {
            hexColor = saverObject.FeetColor;
        }

        if (isEyeColor)
        {
            hexColor = saverObject.EyesColor;
        }

        if (isSkinColor)
        {
            hexColor = saverObject.SkinColor;
        }

        ColorUtility.TryParseHtmlString("#" + hexColor, out selectedColor);
        this.GetComponent<Image>().color = selectedColor;
    }
}
