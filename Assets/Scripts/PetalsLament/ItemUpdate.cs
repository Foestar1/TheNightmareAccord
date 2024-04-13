using UnityEngine;

public class ItemUpdate : MonoBehaviour
{
    [SerializeField]
    private Material notWon;
    [SerializeField]
    private Material haveWon;
    [SerializeField]
    private int levelNumber;

    private void Awake()
    {
        if (levelNumber == 1)
        {
            if (GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>().level1Complete == 0)
            {
                this.GetComponent<Renderer>().material = notWon;
            }
            else
            {
                this.GetComponent<Renderer>().material = haveWon;
            }
        }
    }
}
