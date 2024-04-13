using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviourPunCallbacks
{
    #region variables and stuff
    [Header("World Stuff")]
    [Tooltip("The crosshair icon")]
    [SerializeField]
    private GameObject Crosshair;
    [Tooltip("The interaction button icon")]
    [SerializeField]
    private GameObject interactionButton;
    [Tooltip("The goal UI tracker")]
    [SerializeField]
    private GameObject goalTrackerUI;
    [Tooltip("Teddy's coldown UI")]
    [SerializeField]
    private GameObject teddyUI;
    [Tooltip("The object goals to activate")]
    [SerializeField]
    private List<GameObject> goals;
    private int goalsPicked;
    public int goalsNotFoundVar;
    public int goalsNotFound { get; set; }
    public int playersAlive { get; set; }
    public bool timerRunning { get; set; }
    public float gameTimer { get; set; }
    public bool gameFinished { get; set; }
    public bool gameWon { get; set; }
    public bool gameLost { get; set; }
    private Camera mainStartingCamera;

    [Header("Scoreboard Stuff")]
    [Tooltip("The scoreboard itself")]
    [SerializeField]
    private GameObject endScoreboard;
    [Tooltip("The scoreboard string elements")]
    [SerializeField]
    private string[] scoreboardStat;
    [Tooltip("The scoreboard physical elements")]
    [SerializeField]
    private TextMeshProUGUI[] scoreboardPhysicalStat;
    public int soloObjectives { get; set; }
    public int totalObjectives { get; set; }
    public int soloKills { get; set; }
    public int totalKills { get; set; }
    public int soloRevives { get; set; }
    public int totalRevives { get; set; }
    public int soloDeaths { get; set; }
    public int totalDeaths { get; set; }

    [Header("Randomizer Stuff")]
    [Tooltip("The sections to be randomized via rotation")]
    [SerializeField]
    private List<GameObject> areasToRotate;

    [Header("Enemy Stuff")]
    [Tooltip("The lesser enemies in the map")]
    [SerializeField]
    private GameObject[] smallEnemy;
    [Tooltip("The main enemy in the map")]
    [SerializeField]
    private GameObject bigEnemy;
    private bool enemiesSpawned;

    [Header("Players Stuff")]
    [Tooltip("The player prefab model")]
    [SerializeField]
    private GameObject playerPrefab;
    [Tooltip("All the spawn points the player can spawn at")]
    [SerializeField]
    private List<Transform> playerSpawnPoints;
    private int playersReady;
    private bool spawnedPlayers;
    [Tooltip("The players singleplayer lives UI")]
    [SerializeField]
    private Image livesUI;
    #endregion

    #region unity functions
    private void Awake()
    {
        playersReady = 0;
        goalsNotFound = goalsNotFoundVar;
        mainStartingCamera = GameObject.Find("Main Camera").gameObject.GetComponent<Camera>();

        for (int i = 0; i < scoreboardPhysicalStat.Length; i++)
        {
            scoreboardPhysicalStat[i].text = scoreboardStat[i];
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                playersReady++;
            }
            else
            {
                this.photonView.RPC("playerReady", RpcTarget.MasterClient);
            }
        }
        else
        {
            timerRunning = true;
            GameObject.Find("Music").GetComponent<MusicController>().gameStarted = true;
            Crosshair.SetActive(true);
            interactionButton.SetActive(true);
            goalTrackerUI.SetActive(true);
            teddyUI.SetActive(true);
            var point = Random.Range(0, playerSpawnPoints.Count);
            var newPlayerListing = Instantiate(playerPrefab, playerSpawnPoints[point].position, playerSpawnPoints[point].rotation);
            livesUI.gameObject.SetActive(true);
            livesUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "3";
        }
    }

    private void Update()
    {
        pickGoals();
        spawnPlayers();
        updateGoals();
        spawnEnemies();
        arePlayersAlive();
        timerControl();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playersAlive--;
    }
    #endregion

    #region custom functions
    private void pickGoals()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient && playersReady == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                if (goalsPicked < goalsNotFound)
                {
                    if (goals.Count != 0)
                    {
                        var point = Random.Range(0, goals.Count);
                        if (goals[point].activeSelf == false)
                        {
                            goalsPicked++;
                            this.photonView.RPC("activateGoal", RpcTarget.All, point);
                        }
                        else
                        {
                            pickGoals();
                        }
                    }
                }
            }
        }
        else
        {
            if (goalsPicked < goalsNotFound)
            {
                if (goals.Count != 0)
                {
                    var point = Random.Range(0, goals.Count);
                    if (goals[point].activeSelf == false)
                    {
                        goalsPicked++;
                        goals[point].SetActive(true);
                    }
                    else
                    {
                        pickGoals();
                    }
                }
            }
        }
    }

    private void spawnPlayers()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient && playersReady == PhotonNetwork.CurrentRoom.PlayerCount && !spawnedPlayers)
            {
                //start the timer for everyone!!!
                this.photonView.RPC("activateTimer", RpcTarget.All);
                //spawn the players!!!
                spawnedPlayers = true;
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    playersAlive++;
                    var point = Random.Range(0, playerSpawnPoints.Count);
                    int pointID = playerSpawnPoints[point].gameObject.GetPhotonView().ViewID;
                    playerSpawnPoints.RemoveAt(point);
                    this.photonView.RPC("activateUIAndPlayer", player, pointID);
                }
            }
        }
    }

    private void spawnEnemies()
    {
        if (!enemiesSpawned)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (PhotonNetwork.IsMasterClient && playersReady == PhotonNetwork.CurrentRoom.PlayerCount && spawnedPlayers)
                {
                    this.photonView.RPC("activateEnemies", RpcTarget.All);
                }
            }
            else
            {
                foreach (GameObject enemy in smallEnemy)
                {
                    enemiesSpawned = true;
                    enemy.SetActive(true);
                }
                if (bigEnemy != null)
                {
                    bigEnemy.SetActive(true);
                }
            }
        }
    }

    private void updateGoals()
    {
        if (goalTrackerUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text != goalsNotFound.ToString())
        {
            goalTrackerUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = goalsNotFound.ToString();
        }

        if (goalsNotFound == 0 && !gameFinished)
        {
            gameFinished = true;

            //check if it's multiplayer
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    this.photonView.RPC("gameWasWon", RpcTarget.All);
                }
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                timerRunning = false;
                gameWon = true;
                mainStartingCamera.gameObject.SetActive(true);
                SaveAndLoadData saver = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();

                //which scene are we in
                if (SceneManager.GetActiveScene().name == "Petal's Lament")
                {
                    saver.level1Complete = 1;
                    //save timer if needed
                    if (saver.level1CompleteSpeed == 0 || saver.level1CompleteSpeed > gameTimer)
                    {
                        saver.level1CompleteSpeed = gameTimer;
                    }
                    saver.saveInfo();
                }

                //clear out the enemies
                List<GameObject> enemyObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
                foreach (GameObject enemyObject in enemyObjects)
                {
                    Destroy(enemyObject.gameObject);
                }

                //clear out the ghost spirits of lost players
                List<GameObject> playerSpiritsObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerSpirit"));
                foreach (GameObject playersGhost in playerSpiritsObjects)
                {
                    Destroy(playersGhost.gameObject);
                }

                //clear out the players who are alive
                List<GameObject> playerObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
                foreach (GameObject playersAlive in playerObjects)
                {
                    Destroy(playersAlive.gameObject);
                }

                Crosshair.SetActive(false);
                interactionButton.SetActive(false);
                goalTrackerUI.SetActive(false);
                teddyUI.SetActive(false);
                endScoreboard.SetActive(true);
                endScoreboard.transform.GetChild(5).GetComponent<Button>().Select();
            }
        }
    }

    private void arePlayersAlive()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient && playersReady == PhotonNetwork.CurrentRoom.PlayerCount && spawnedPlayers)
            {
                if (playersAlive <= 0 && !gameLost)
                {
                    this.photonView.RPC("gameWasLost", RpcTarget.All);
                }
            }
        }
    }

    public void openScoreboardLost()
    {
        gameLost = true;
        mainStartingCamera.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //clear out the enemies
        List<GameObject> enemyObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        foreach (GameObject enemyObject in enemyObjects)
        {
            Destroy(enemyObject.gameObject);
        }

        //clear out the ghost spirits of lost players
        List<GameObject> playerSpiritObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerSpirit"));
        foreach (GameObject playersGhost in playerSpiritObjects)
        {
            Destroy(playersGhost.gameObject);
        }

        //clear out the players who are alive
        List<GameObject> playerObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        foreach (GameObject playerAlive in playerObjects)
        {
            Destroy(playerAlive.gameObject);
        }

        Crosshair.SetActive(false);
        interactionButton.SetActive(false);
        goalTrackerUI.SetActive(false);
        teddyUI.SetActive(false);
        endScoreboard.SetActive(true);
        livesUI.gameObject.SetActive(false);
        endScoreboard.transform.GetChild(5).GetComponent<Button>().Select();
    }

    public void timerControl()
    {
        if (timerRunning == true)
        {
            gameTimer += Time.deltaTime;
        }
    }

    #endregion

    #region RPC's
    [PunRPC]
    void activateGoal(int thisGoal)
    {
        goals[thisGoal].SetActive(true);
    }

    [PunRPC]
    void activateEnemies()
    {
        enemiesSpawned = true;
        foreach (GameObject enemy in smallEnemy)
        {
            enemy.SetActive(true);
        }
        if (bigEnemy != null)
        {
            bigEnemy.SetActive(true);
        }
    }

    [PunRPC]
    void activateTimer()
    {
        timerRunning = true;
    }

    [PunRPC]
    void activateUIAndPlayer(int pointID)
    {
        spawnedPlayers = true;
        GameObject.Find("Music").GetComponent<MusicController>().gameStarted = true;
        PhotonView tempView = PhotonView.Find(pointID);
        Crosshair.SetActive(true);
        interactionButton.SetActive(true);
        goalTrackerUI.SetActive(true);
        teddyUI.SetActive(true);
        var myPlayerObject = PhotonNetwork.Instantiate(this.playerPrefab.name, tempView.gameObject.transform.position, tempView.gameObject.transform.rotation, 0);
    }

    [PunRPC]
    void playerReady()
    {
        playersReady++;
    }

    [PunRPC]
    void gameWasLost()
    {
        gameLost = true;
        timerRunning = false;
        mainStartingCamera.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        //clear out the enemies
        List<GameObject> enemyObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        foreach (GameObject enemyObject in enemyObjects)
        {
            Destroy(enemyObject.gameObject);
        }

        //clear out the player cycler objects
        List<GameObject> playerCyclerObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerCycler"));
        foreach (GameObject cycleObject in playerCyclerObjects)
        {
            Destroy(cycleObject.gameObject);
        }

        //clear out the ghost spirits of lost players
        List<GameObject> playerSpiritObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerSpirit"));
        foreach (GameObject playersGhost in playerSpiritObjects)
        {
            Destroy(playersGhost.gameObject);
        }

        //clear out the players who are alive
        List<GameObject> playerObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        foreach (GameObject playerAlive in playerObjects)
        {
            Destroy(playerAlive.gameObject);
        }

        Crosshair.SetActive(false);
        interactionButton.SetActive(false);
        goalTrackerUI.SetActive(false);
        teddyUI.SetActive(false);
        endScoreboard.SetActive(true);
        endScoreboard.transform.GetChild(5).GetComponent<Button>().Select();
    }

    [PunRPC]
    void gameWasWon()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        timerRunning = false;
        gameWon = true;
        mainStartingCamera.gameObject.SetActive(true);
        SaveAndLoadData saver = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();

        //which scene are we in
        if (SceneManager.GetActiveScene().name == "Petal's Lament")
        {
            saver.level1Complete = 1;
            //save timer if needed
            if (saver.level1CompleteSpeed == 0 || saver.level1CompleteSpeed > gameTimer)
            {
                saver.level1CompleteSpeed = gameTimer;
            }
            saver.saveInfo();
        }

        //clear out the enemies
        List<GameObject> enemyObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        foreach (GameObject enemyObject in enemyObjects)
        {
            Destroy(enemyObject.gameObject);
        }

        //clear out the ghost spirits of lost players
        List<GameObject> playerSpiritsObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerSpirit"));
        foreach (GameObject playersGhost in playerSpiritsObjects)
        {
            Destroy(playersGhost.gameObject);
        }

        //clear out the players who are alive
        List<GameObject> playerObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        foreach (GameObject playersAlive in playerObjects)
        {
            Destroy(playersAlive.gameObject);
        }

        Crosshair.SetActive(false);
        interactionButton.SetActive(false);
        goalTrackerUI.SetActive(false);
        teddyUI.SetActive(false);
        endScoreboard.SetActive(true);
        endScoreboard.transform.GetChild(5).GetComponent<Button>().Select();
    }
    #endregion
}
