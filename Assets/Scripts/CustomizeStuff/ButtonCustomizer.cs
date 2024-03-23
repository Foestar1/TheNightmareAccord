using UnityEngine;
using TMPro;

public class ButtonCustomizer : MonoBehaviour
{
    private SaveAndLoadData saverObject;

    [SerializeField]
    private Renderer[] playerObjects;

    [SerializeField]
    private GameObject[] headsList;

    [SerializeField]
    private Texture[] jammiesList;

    [SerializeField]
    private TextMeshProUGUI playerNameButton;

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

        playerNameButton.text = saverObject.multiplayerNickname;
    }

    #region head stuff
    public void headRightButton()
    {
        //get the current head
        int currentHead = saverObject.chosenHead;

        // Disable the current head.
        headsList[currentHead].SetActive(false);

        // Increment the index. If it goes past the end, cycle back to 0.
        if (currentHead < headsList.Length - 1)
        {
            currentHead++;
        }
        else
        {
            currentHead = 0;
        }

        // Enable the new current head.
        headsList[currentHead].SetActive(true);

        //Set the new head and save it
        saverObject.chosenHead = currentHead;
        saverObject.saveInfo();
    }

    public void headLeftButton()
    {
        //get the current head
        int currentHead = saverObject.chosenHead;

        // Disable the current head.
        headsList[currentHead].SetActive(false);

        // Increment the index. If it goes past the end, cycle back to 0.
        if (currentHead > 0)
        {
            currentHead--;
        }
        else
        {
            currentHead = headsList.Length - 1;
        }

        // Enable the new current head.
        headsList[currentHead].SetActive(true);

        //Set the new head and save it
        saverObject.chosenHead = currentHead;
        saverObject.saveInfo();
    }
    #endregion

    public void jammiesRightButton()
    {
        //get the current head
        int currentJammies = saverObject.chosenJammies;

        // Increment the index. If it goes past the end, cycle back to 0.
        if (currentJammies < jammiesList.Length - 1)
        {
            currentJammies++;
        }
        else
        {
            currentJammies = 0;
        }

        // Enable the new current jammie.
        foreach(Renderer jammieRend in playerObjects)
        {
            jammieRend.GetComponent<Renderer>().material.mainTexture = jammiesList[currentJammies];
        }

        //Set the new head and save it
        saverObject.chosenJammies = currentJammies;
        saverObject.saveInfo();
    }

    public void jammiesLeftButton()
    {
        //get the current head
        int currentJammies = saverObject.chosenJammies;

        // Increment the index. If it goes past the end, cycle back to 0.
        if (currentJammies > 0)
        {
            currentJammies--;
        }
        else
        {
            currentJammies = jammiesList.Length - 1;
        }

        // Enable the new current jammie.
        foreach (Renderer jammieRend in playerObjects)
        {
            jammieRend.GetComponent<Renderer>().material.mainTexture = jammiesList[currentJammies];
        }

        //Set the new head and save it
        saverObject.chosenJammies = currentJammies;
        saverObject.saveInfo();
    }
}
