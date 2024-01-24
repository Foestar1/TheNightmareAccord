using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.SceneManagement;

public class HubController : MonoBehaviourPunCallbacks
{
    #region variables and stuff
    [Header("Level Info")]
    [Tooltip("The UI for the level")]
    [SerializeField]
    private GameObject levelInfo;
    [Tooltip("The title text for the level")]
    [SerializeField]
    private TextMeshProUGUI levelTitle;
    [Tooltip("The screenshot image for the level")]
    [SerializeField]
    private Image levelImage;
    [Tooltip("The description textbox for the level")]
    [SerializeField]
    private TextMeshProUGUI levelDescription;

    [Tooltip("The title choices for level title")]
    [SerializeField]
    private string[] levelTitles;
    [Tooltip("The image choices for level image")]
    [SerializeField]
    private Sprite[] levelChoices;
    [Tooltip("The description choices for level description")]
    [SerializeField]
    private string[] levelDescriptions;
    public int levelChoice;

    [Header("UI Stuff")]
    [Tooltip("The text for the connection UI")]
    [SerializeField]
    private TextMeshProUGUI connectionText;
    [Tooltip("The menu for Multiplayer")]
    [SerializeField]
    private GameObject multiplayerOptions;
    [Tooltip("The connection UI")]
    [SerializeField]
    private GameObject connectionUI;
    [Tooltip("The game room UI")]
    [SerializeField]
    private GameObject gameRoomUI;
    [Tooltip("The players list")]
    [SerializeField]
    private Transform playerListContent;
    private int playersTotalReady;
    private int readyUP;
    [Tooltip("The ready sprite for our players name prefab in lobby")]
    [SerializeField]
    private Sprite readyImage;
    [Tooltip("The not ready sprite for our players name prefab in lobby")]
    [SerializeField]
    private Sprite notReadyImage;

    [Header("Player Stuff")]
    [Tooltip("The player gameobject")]
    [SerializeField]
    private GameObject playerObject;
    [Tooltip("The players name prefab for room listing")]
    [SerializeField]
    private GameObject playerNamePrefab;
    #endregion

    #region built in functions
    private void Update()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            if (playersTotalReady == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                if (gameRoomUI.transform.GetChild(1).gameObject.activeSelf == false)
                {
                    gameRoomUI.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            else
            {
                if (gameRoomUI.transform.GetChild(1).gameObject.activeSelf == true)
                {
                    gameRoomUI.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }
    #endregion

    #region custom functions
    public void openLevelInfo()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        levelInfo.SetActive(true);
        levelTitle.text = levelTitles[levelChoice];
        levelImage.sprite = levelChoices[levelChoice];
        levelDescription.text = levelDescriptions[levelChoice];
    }

    public void closeLevelInfo()
    {
        playerObject.GetComponent<characterControls>().canMove = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        levelInfo.SetActive(false);
        levelTitle.text = null;
        levelImage.sprite = null;
        levelDescription.text = null;
    }

    public void loadTheLevel()
    {
        if (levelChoice == 0)
        {
            SceneManager.LoadScene("Petal's Lament");
        }
    }

    public void timeToConnect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void disconnectFromTheServer()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void disconnectFromRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            readyUP = 0;
            PhotonNetwork.LeaveRoom();
        }
    }

    public void createOpenGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 4;
        roomOptions.PublishUserId = true;

        TypedLobby typedLobby = new TypedLobby("", LobbyType.Default);

        if (levelChoice == 0)
        {
            typedLobby = new TypedLobby("PetalsLemantOpen", LobbyType.Default);
        }

        PhotonNetwork.CreateRoom(null, roomOptions, typedLobby, null);
    }

    public void joinOpenGame()
    {
        TypedLobby typedLobby = new TypedLobby("", LobbyType.Default);

        if (levelChoice == 0)
        {
            typedLobby = new TypedLobby("PetalsLemantOpen", LobbyType.Default);
        }

        PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, typedLobby, null);
    }

    public void sendReady()
    {
        readyUP = 1 - readyUP;

        if (readyUP == 0)
        {
            this.photonView.RPC("playerNotReady", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId.ToString());
        }
        else
        {
            this.photonView.RPC("playerReady", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId.ToString());
        }
    }
    #endregion

    #region callbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.GameVersion = "0.1";
        connectionText.text = "Connected to master, attempting to connecting to master lobby...";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        //the line below is temporary
        PhotonNetwork.NickName = "Player" + Random.Range(0, 100);
        connectionText.text = "Connected...";
        connectionUI.transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(resetText());
    }

    public override void OnJoinedRoom()
    {
        //delete all prior entries
        for (int i = playerListContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(playerListContent.transform.GetChild(i).gameObject);
        }

        //set the UI
        playersTotalReady = 0;
        readyUP = 0;
        connectionUI.SetActive(false);
        connectionText.text = null;
        connectionUI.transform.GetChild(1).gameObject.SetActive(false);
        gameRoomUI.SetActive(true);
        gameRoomUI.transform.GetChild(1).gameObject.SetActive(false);

        //create new list
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            //create the players name object
            var newPlayerListing = Instantiate(playerNamePrefab, playerListContent);
            //and set the players name and gameobject name
            newPlayerListing.name = player.UserId;
            newPlayerListing.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = player.NickName;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (Transform child in playerListContent)
        {
            if (child.transform.GetChild(1).GetComponent<Image>().sprite == readyImage)
            {
                playersTotalReady--;
            }

            if (child.name == otherPlayer.UserId)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //create the players name object
        var newPlayerListing = Instantiate(playerNamePrefab, playerListContent);
        //and set the players name and gameobject name
        newPlayerListing.name = newPlayer.UserId;
        newPlayerListing.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = newPlayer.NickName;

        if (readyUP != 0)
        {
            this.photonView.RPC("playerReady", newPlayer, PhotonNetwork.LocalPlayer.UserId.ToString());
        }
    }
    #endregion

    #region failed callbacks
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        connectionText.text = returnCode + ":" + message;
        StartCoroutine(resetText());
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        connectionText.text = returnCode + ":" + message;
        StartCoroutine(resetText());
    }
    #endregion

    #region RPC's
    [PunRPC]
    void playerReady(string playerNameObject)
    {
        playersTotalReady++;
        GameObject.Find(playerNameObject).transform.GetChild(1).GetComponent<Image>().sprite = readyImage;
    }

    [PunRPC]
    void playerReadyUpdate(string playerNameObject)
    {
        GameObject.Find(playerNameObject).transform.GetChild(1).GetComponent<Image>().sprite = notReadyImage;
    }

    [PunRPC]
    void playerNotReady(string playerNameObject)
    {
        playersTotalReady--;
        GameObject.Find(playerNameObject).transform.GetChild(1).GetComponent<Image>().sprite = notReadyImage;
    }
    #endregion

    #region Coroutines
    public IEnumerator resetText()
    {
        yield return new WaitForSeconds(3);
        connectionText.text = "";
        multiplayerOptions.SetActive(true);
        connectionUI.SetActive(false);
    }
    #endregion
}
