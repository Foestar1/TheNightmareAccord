using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlushieControl : MonoBehaviourPunCallbacks
{
    private SaveAndLoadData saver;

    [SerializeField]
    private GameObject[] plushieList;
    [SerializeField]
    private GameObject[] plushieList2;

    private void Awake()
    {
        if (saver == null)
        {
            saver = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        }
    }

    private void OnEnable()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (this.photonView.IsMine)
            {
                int chosenOne = saver.chosenPlushie;
                this.photonView.RPC("changePlushie", RpcTarget.All, chosenOne);
            }
        }
        else
        {
            //get the current head
            int currentPlush = saver.chosenPlushie;
            plushieList[currentPlush].SetActive(true);
        }
    }

    #region head stuff
    public void plushieRightButton()
    {
        //get the current head
        int currentPlush = saver.chosenPlushie;

        // Disable the current head.
        plushieList[currentPlush].SetActive(false);
        plushieList2[currentPlush].SetActive(false);

        // Increment the index. If it goes past the end, cycle back to 0.
        if (currentPlush < plushieList.Length - 1)
        {
            currentPlush++;
        }
        else
        {
            currentPlush = 0;
        }

        // Enable the new current head.
        plushieList[currentPlush].SetActive(true);
        plushieList2[currentPlush].SetActive(true);

        //Set the new head and save it
        saver.chosenPlushie = currentPlush;
        saver.saveInfo();
    }

    public void plushieLeftButton()
    {
        //get the current head
        int currentPlush = saver.chosenPlushie;

        // Disable the current head.
        plushieList[currentPlush].SetActive(false);
        plushieList2[currentPlush].SetActive(false);

        // Increment the index. If it goes past the end, cycle back to 0.
        if (currentPlush > 0)
        {
            currentPlush--;
        }
        else
        {
            currentPlush = plushieList.Length - 1;
        }

        // Enable the new current head.
        plushieList[currentPlush].SetActive(true);
        plushieList2[currentPlush].SetActive(true);

        //Set the new head and save it
        saver.chosenPlushie = currentPlush;
        saver.saveInfo();
    }
    #endregion

    [PunRPC]
    void changePlushie(int chosenOne)
    {
        plushieList[chosenOne].SetActive(true);
    }
}
