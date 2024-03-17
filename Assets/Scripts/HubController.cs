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
    [SerializeField]
    private TextMeshProUGUI timerStat;
    [Tooltip("The title choices for level title")]
    [SerializeField]
    private string[] levelTitles;
    [Tooltip("The image choices for level image")]
    [SerializeField]
    private Sprite[] levelChoices;
    [Tooltip("The description choices for level description")]
    [SerializeField]
    private string[] levelDescriptions;
    public int levelChoice;//0-Petal's Lament,

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
    [Tooltip("The desired game private room to join")]
    [SerializeField]
    private TMP_InputField gameNumberField;
    [Tooltip("The UI for the character customization screen")]
    [SerializeField]
    private GameObject characterCustomizationUI;
    public bool canOpen { get; set; }

    [Header("Player Stuff")]
    [Tooltip("The player gameobject")]
    [SerializeField]
    private GameObject playerObject;
    [Tooltip("The players name prefab for room listing")]
    [SerializeField]
    private GameObject playerNamePrefab;
    [Tooltip("The hidden player gameobject for customization")]
    [SerializeField]
    private GameObject hiddenPlayerObject;
    public int rotatingPlayer { get; set; }
    #endregion

    #region built in functions
    private void Awake()
    {
        canOpen = true;
    }

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

        if (rotatingPlayer == -1)
        {
            hiddenPlayerObject.transform.Rotate(Vector3.up, 50f * Time.deltaTime);
        }
        else if (rotatingPlayer == 1)
        {
            hiddenPlayerObject.transform.Rotate(Vector3.up, -50f * Time.deltaTime);
        }
    }
    #endregion

    #region custom functions
    #region UI and such custom functions
    public void openLevelInfo()
    {
        canOpen = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        levelInfo.SetActive(true);
        levelInfo.transform.GetChild(4).GetComponent<Button>().Select();
        levelTitle.text = levelTitles[levelChoice];
        levelImage.sprite = levelChoices[levelChoice];
        levelDescription.text = levelDescriptions[levelChoice];
        SaveAndLoadData saver = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        if (levelChoice == 0)
        {
            //timer stat
            if (saver.level1CompleteSpeed == 0)
            {
                timerStat.text = "---";
            }
            else
            {
                int m = (int)saver.level1CompleteSpeed / 60;
                int s = (int)saver.level1CompleteSpeed - m * 60;
                timerStat.text = m.ToString() + ":" + s.ToString();
            }
        }

    }

    public void closeLevelInfo()
    {
        StartCoroutine(canOpenReset());
        playerObject.GetComponent<characterControls>().canMove = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        levelInfo.SetActive(false);
        levelTitle.text = null;
        levelImage.sprite = null;
        levelDescription.text = null;
        timerStat.text = "---";
    }

    public void openCustomizationMenu()
    {
        canOpen = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        characterCustomizationUI.SetActive(true);
        characterCustomizationUI.transform.GetChild(0).GetComponent<Button>().Select();
    }

    public void closeCustomizationMenu()
    {
        characterCustomizationUI.SetActive(false);
        StartCoroutine(canOpenReset());
        playerObject.GetComponent<characterControls>().canMove = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void rotatePlayerLeft()
    {
        if (rotatingPlayer != -1)
        {
            rotatingPlayer = -1;
        }
    }

    public void rotatePlayerRight()
    {
        if (rotatingPlayer != 1)
        {
            rotatingPlayer = 1;
        }
    }

    public void resetRotatePlayer()
    {
        if (rotatingPlayer != 0)
        {
            rotatingPlayer = 0;
        }
    }

    public void loadTheLevel()
    {
        //ensure if connected to online we close the room access
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
            }
        }

        if (levelChoice == 0)
        {
            SceneManager.LoadScene("Petal's Lament");
        }
    }
    #endregion

    #region multiplayer customs
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

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public void createPrivateGame()
    {
        // Initialize the random number generator
        System.Random random = new System.Random();
        // Initialize your string with the starting value
        string randomGameNumber = "";
        // Loop to add 8 random integers to the string
        for (int i = 0; i < 8; i++)
        {
            // Generate a random integer between 0 and 9
            int randomNumber = random.Next(0, 10);

            // Append the random integer to your string
            randomGameNumber += randomNumber.ToString();
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = 4;
        roomOptions.PublishUserId = true;

        PhotonNetwork.CreateRoom(randomGameNumber.ToString(), roomOptions);
    }

    public void joinOpenGame()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void joinPrivateGame()
    {
        if (gameNumberField.text != null || gameNumberField.text != "")
        {
            PhotonNetwork.JoinRoom(gameNumberField.text);
        }
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
    #endregion

    #region callbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.GameVersion = "0.25";
        connectionText.text = "Connected to master, attempting to connecting to master lobby...";

        //we need to check which lobby to join
        TypedLobby typedLobby = new TypedLobby("", LobbyType.Default);
        if (levelChoice == 0)
        {
            typedLobby = new TypedLobby("PetalsLemantOpen", LobbyType.Default);
        }

        PhotonNetwork.JoinLobby(typedLobby);
    }

    public override void OnJoinedLobby()
    {
        //the line below is temporary
        if (PhotonNetwork.NickName == null || PhotonNetwork.NickName == "")
        {
            PhotonNetwork.NickName = "Player" + Random.Range(0, 100);
        }
        connectionText.text = "";
        if (gameNumberField.transform.parent.gameObject.activeSelf == false)
        {
            multiplayerOptions.SetActive(true);
            multiplayerOptions.transform.GetChild(1).GetComponent<Button>().Select();
        }
        connectionUI.SetActive(false);
        connectionUI.transform.GetChild(0).gameObject.SetActive(false);
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
        gameRoomUI.transform.GetChild(3).GetComponent<Button>().Select();
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

        if (!PhotonNetwork.CurrentRoom.IsVisible)
        {
            gameRoomUI.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "GAME ROOM: " + PhotonNetwork.CurrentRoom.Name;
        }
        else
        {
            gameRoomUI.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "GAME ROOM";
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
        if (returnCode == 32766)
        {
            createPrivateGame();
        }
    }
    #endregion

    #region coroutines
    private IEnumerator canOpenReset()
    {
        yield return new WaitForSeconds(.3f);
        canOpen = true;
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
}
