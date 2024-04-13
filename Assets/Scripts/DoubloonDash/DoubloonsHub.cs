using UnityEngine;

public class DoubloonsHub : MonoBehaviour
{
    private SaveAndLoadData saver;
    [SerializeField]
    private GameObject doubloonsToActivate;

    private void Awake()
    {
        if (saver == null)
        {
            saver = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        }

        int tempNum = saver.level3Complete;
        if (tempNum == 1)
        {
            doubloonsToActivate.SetActive(true);
        }
    }
}
