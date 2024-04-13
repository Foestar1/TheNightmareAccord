using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreboardEnabled : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Spawner spawnerObject;

    [SerializeField]
    private Image winLossImage;

    [SerializeField]
    private Sprite[] winLoseSprites;

    [SerializeField]
    private TextMeshProUGUI[] soloStat;

    [SerializeField]
    private TextMeshProUGUI[] totalStat;

    [SerializeField]
    private TextMeshProUGUI timerStat;

    [SerializeField]
    private GameObject[] menusToClose;

    [SerializeField]
    private GameObject inGameMenu;

    private SaveAndLoadData saveAndLoadObject;

    private void OnEnable()
    {
        //disable the ingame menus
        inGameMenu.GetComponent<InGameMenu>().canOpen = false;
        foreach(GameObject menuPart in menusToClose)
        {
            if (menuPart.activeSelf == true)
            {
                menuPart.SetActive(false);
            }
        }
        //win and loss image
        if (spawnerObject.gameWon)
        {
            winLossImage.sprite = winLoseSprites[0];
        }
        else
        {
            winLossImage.sprite = winLoseSprites[1];
        }
        //solo stats
        soloStat[0].text = spawnerObject.soloObjectives.ToString();
        soloStat[1].text = spawnerObject.soloKills.ToString();
        soloStat[2].text = spawnerObject.soloRevives.ToString();
        soloStat[3].text = spawnerObject.soloDeaths.ToString();
        //total stats
        totalStat[0].text = spawnerObject.totalObjectives.ToString();
        totalStat[1].text = spawnerObject.totalKills.ToString();
        totalStat[2].text = spawnerObject.totalRevives.ToString();
        totalStat[3].text = spawnerObject.totalDeaths.ToString();
        //timer stat
        if (spawnerObject.gameLost)
        {
            winLossImage.sprite = winLoseSprites[1];
            timerStat.text = "---";
        }
        else
        {
            int m = (int)spawnerObject.gameTimer / 60;
            int s = (int)spawnerObject.gameTimer - m * 60;
            timerStat.text = m.ToString() + ":" + s.ToString();
        }
        //disconnecting and deleting the saver
        SaveAndLoadData saver = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        Destroy(saver.gameObject);

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.SendAllOutgoingCommands();
                PhotonNetwork.Disconnect();
            }
            else
            {
                PhotonNetwork.Disconnect();
            }
        }
    }

    public void leaveGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
