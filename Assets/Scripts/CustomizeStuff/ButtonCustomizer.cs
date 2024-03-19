using UnityEngine;

public class ButtonCustomizer : MonoBehaviour
{
    private SaveAndLoadData saverObject;

    [SerializeField]
    private GameObject[] headsList;

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
    }

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
}
