using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;

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
                //var point = Random.Range(0, playerSpawnPoints.Count);
                var point = Random.Range(0, playerSpawnPoints.Count);
                this.photonView.RPC("activateUIAndPlayer", player, point);
                //playerSpawnPoints.Remove(playerSpawnPoints[point]);
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
    #endregion
}
