using UnityEngine;
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
    private bool gameFinished;
    private bool gameWon;
    private bool gameLost;

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
    [Tooltip("The player losing animations")]
    [SerializeField]
    private GameObject playerLostAnimations;
    [Tooltip("The player winning animation")]
    [SerializeField]
    private GameObject playerWinAnimations;
    [Tooltip("All the spawn points the player can spawn at")]
    [SerializeField]
    private List<Transform> playerSpawnPoints;
    private int playersReady;
    private bool spawnedPlayers;
    #endregion

    #region unity functions
    private void Awake()
    {
        playersReady = 0;
        goalsNotFound = goalsNotFoundVar;

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
            Crosshair.SetActive(true);
            interactionButton.SetActive(true);
            goalTrackerUI.SetActive(true);
            teddyUI.SetActive(true);
            var point = Random.Range(0, playerSpawnPoints.Count);
            var newPlayerListing = Instantiate(playerPrefab, playerSpawnPoints[point].position, playerSpawnPoints[point].rotation);
        }
    }

    private void Update()
    {
        pickGoals();
        spawnPlayers();
        updateGoals();
        spawnEnemies();
        arePlayersAlive();
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
                if (goalsPicked < 3)
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
        else
        {
            if (goalsPicked < 3)
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

    private void spawnPlayers()
    {
        if (PhotonNetwork.IsMasterClient && playersReady == PhotonNetwork.CurrentRoom.PlayerCount && !spawnedPlayers)
        {
            spawnedPlayers = true;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                playersAlive++;
                var point = Random.Range(0, playerSpawnPoints.Count);
                this.photonView.RPC("activateUIAndPlayer", player, point);
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
            gameWon = true;
            
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
                //which scene are we in
                if (SceneManager.GetActiveScene().name == "Petal's Lament")
                {
                    GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>().level1Complete = 1;
                    GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>().saveInfo();
                }

                //clear out the enemies
                List<GameObject> enemyObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
                foreach (GameObject enemyObject in enemyObjects)
                {
                    Destroy(enemyObject.gameObject);
                }

                //clear out the ghost spirits of lost players
                List<GameObject> playerObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerSpirit"));
                foreach (GameObject playersGhost in playerObjects)
                {
                    Destroy(playersGhost.gameObject);
                }

                int randomAni = Random.Range(0, 2);
                var playersObject = GameObject.FindGameObjectWithTag("Player");
                var newPlayerListing = Instantiate(playerWinAnimations, playersObject.transform.position, playersObject.transform.rotation);
                if (randomAni == 0)
                {
                    newPlayerListing.GetComponent<Animator>().Play("Win1");
                }
                else
                {
                    newPlayerListing.GetComponent<Animator>().Play("Win2");
                }
                Destroy(playersObject.gameObject);
                GameObject.Find("Crosshair").SetActive(false);
                GameObject.Find("InteractionButton").SetActive(false);
                GameObject.Find("GoalBorder").SetActive(false);
                GameObject.Find("TeddyBorder").SetActive(false);
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
                    this.photonView.RPC("gameWasLost", RpcTarget.All, playersAlive);
                }
            }
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
    }

    [PunRPC]
    void activateUIAndPlayer(int point)
    {
        Crosshair.SetActive(true);
        interactionButton.SetActive(true);
        goalTrackerUI.SetActive(true);
        teddyUI.SetActive(true);
        var myPlayerObject = PhotonNetwork.Instantiate(this.playerPrefab.name, playerSpawnPoints[point].position, playerSpawnPoints[point].rotation, 0);
        //myPlayerObject.name = myPlayerObject.GetPhotonView().Owner.UserId;
    }

    [PunRPC]
    void playerReady()
    {
        playersReady++;
    }

    [PunRPC]
    void gameWasLost(int playersAlive)
    {
        gameLost = true;
        List<GameObject> enemyObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        foreach(GameObject enemyObject in enemyObjects)
        {
            Destroy(enemyObject.gameObject);
        }

        List<GameObject> playerObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerSpirit"));
        foreach(GameObject playersGhost in playerObjects)
        {
            if (playersGhost.GetPhotonView().IsMine)
            {
                int randomAni = Random.Range(0, 2);
                var newPlayerListing = Instantiate(playerLostAnimations, playersGhost.transform.position, playersGhost.transform.rotation);
                if (randomAni == 0)
                {
                    newPlayerListing.GetComponent<Animator>().Play("Lose1");
                }
                else
                {
                    newPlayerListing.GetComponent<Animator>().Play("Lose2");
                }
                Destroy(playersGhost.gameObject);
            }
        }

        GameObject.Find("Crosshair").SetActive(false);
        GameObject.Find("GoalBorder").SetActive(false);
        GameObject.Find("TeddyBorder").SetActive(false);
    }

    [PunRPC]
    void gameWasWon()
    {
        //which scene are we in
        if (SceneManager.GetActiveScene().name == "Petal's Lament")
        {
            GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>().level1Complete = 1;
            GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>().saveInfo();
        }

        //clear out the enemies
        List<GameObject> enemyObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        foreach (GameObject enemyObject in enemyObjects)
        {
            Destroy(enemyObject.gameObject);
        }

        //clear out the ghost spirits of lost players
        List<GameObject> playerObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerSpirit"));
        foreach (GameObject playersGhost in playerObjects)
        {
            if (playersGhost.GetPhotonView().IsMine)
            {
                int randomAni = Random.Range(0, 2);
                var newPlayerListing = Instantiate(playerWinAnimations, playersGhost.transform.position, playersGhost.transform.rotation);
                if (randomAni == 0)
                {
                    newPlayerListing.GetComponent<Animator>().Play("Win1");
                }
                else
                {
                    newPlayerListing.GetComponent<Animator>().Play("Win2");
                }
                Destroy(playersGhost.gameObject);
            }
        }

        GameObject.Find("Crosshair").SetActive(false);
        GameObject.Find("InteractionButton").SetActive(false);
        GameObject.Find("GoalBorder").SetActive(false);
        GameObject.Find("TeddyBorder").SetActive(false);
    }
    #endregion
}
