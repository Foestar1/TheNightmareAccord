using UnityEngine;
using UnityEngine.UI;

public class ButtonColor : MonoBehaviour
{
    private SaveAndLoadData saverObject;
    private Color selectedColor;
    private string hexColor;
    [SerializeField]
    private GameObject[] playerObjects;

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
        if (saverObject == null)
        {
            saverObject = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        }
    }

    public void doTheStuff()
    {
        if (isHeadAccessory)
        {
            selectedColor = this.GetComponent<Image>().color;
            hexColor = ColorUtility.ToHtmlStringRGBA(this.GetComponent<Image>().color);

            //time to save
            saverObject.headColor = hexColor;
        }

        if (isTopAccessory)
        {
            selectedColor = this.GetComponent<Image>().color;
        }

        if (isJammiesAccessory)
        {
            selectedColor = this.GetComponent<Image>().color;
            hexColor = ColorUtility.ToHtmlStringRGBA(this.GetComponent<Image>().color);

            //time to save
            saverObject.JammiesColor = hexColor;
        }

        if (isSlippersAccessory)
        {
            selectedColor = this.GetComponent<Image>().color;
        }

        if (isEyeColor)
        {
            selectedColor = this.GetComponent<Image>().color;
        }

        if (isSkinColor)
        {
            selectedColor = this.GetComponent<Image>().color;
        }

        saverObject.saveInfo();

        //change the models
        foreach (GameObject player in playerObjects)
        {
            player.GetComponent<Renderer>().material.color = selectedColor;
        }
    }
}
