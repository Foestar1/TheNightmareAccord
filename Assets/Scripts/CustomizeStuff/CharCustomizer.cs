using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharCustomizer : MonoBehaviourPunCallbacks
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

    private void Awake()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (this.photonView.IsMine)
            {
                doTheStuff();
            }
        }
        else
        {
            doTheStuff();
        }
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

        if (PhotonNetwork.IsConnectedAndReady)
        {
            this.photonView.RPC("sendData", RpcTarget.All, hexColor);
        }
        else
        {
            ColorUtility.TryParseHtmlString("#" + hexColor, out selectedColor);
            this.GetComponent<Renderer>().material.color = selectedColor;
        }
        //string materialHexColor = ColorUtility.ToHtmlStringRGBA(this.GetComponent<Renderer>().material.color);
        //ToHtmlStringRGBA is from color to hexa
        //ColorUtility.TryParseHtmlString("#" + materialHexColor, out selectedColor);
        //TryParseHtmlString is from hexa to color
    }

    [PunRPC]
    void sendData(string hexColor)
    {
        ColorUtility.TryParseHtmlString("#" + hexColor, out selectedColor);
        this.GetComponent<Renderer>().material.color = selectedColor;
    }
}