using UnityEngine;

public class CubesHub : MonoBehaviour
{
    private SaveAndLoadData saver;
    [SerializeField]
    private GameObject[] cubesToActivate;

    private void Awake()
    {
        if (saver == null)
        {
            saver = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        }

        int tempNum = saver.level2Complete;
        cubesToActivate[tempNum].SetActive(true);
    }
}
