using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class InGameMenu : MonoBehaviourPunCallbacks
{
    [Header("AI Object References")]
    [Tooltip("The in game menu object")]
    [SerializeField]
    private GameObject inGameMenu;
    [Tooltip("The players currently in the scene")]
    [SerializeField]
    private List<GameObject> playerTargets;
    public bool canOpen { get; set; }

    public void Update()
    {
        openMenu();
    }

    public void getPlayers()
    {
        playerTargets.Clear();
        playerTargets.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        playerTargets.AddRange(GameObject.FindGameObjectsWithTag("PlayerSpirit"));
    }

    public void openMenu()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            getPlayers();

            if (PhotonNetwork.IsConnectedAndReady)
            {
                foreach (GameObject player in playerTargets)
                {
                    if (player.GetPhotonView().IsMine)
                    {
                        if (player.tag == "Player")
                        {
                            canOpen = player.GetComponent<characterControls>().canMove;
                        }else if (player.tag == "PlayerSpirit")
                        {
                            canOpen = true;
                        }
                    }
                }
            }
            else
            {
                foreach (GameObject player in playerTargets)
                {
                    canOpen = player.GetComponent<characterControls>().canMove;
                }
            }

            if (canOpen)
            {
                if (PhotonNetwork.IsConnectedAndReady)
                {
                    foreach (GameObject player in playerTargets)
                    {
                        if (player.GetPhotonView().IsMine)
                        {
                            if (player.tag == "Player")
                            {
                                player.GetComponent<characterControls>().canMove = false;
                            }
                        }
                    }
                }
                else
                {
                    foreach (GameObject player in playerTargets)
                    {
                        if (player.tag == "Player")
                        {
                            player.GetComponent<characterControls>().canMove = false;
                        }
                    }
                }

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                inGameMenu.SetActive(true);
                canOpen = false;
            }
        }
    }

    public void resumeGame()
    {
        getPlayers();

        if (PhotonNetwork.IsConnectedAndReady)
        {
            foreach (GameObject player in playerTargets)
            {
                if (player.GetPhotonView().IsMine)
                {
                    if (player.tag == "Player")
                    {
                        player.GetComponent<characterControls>().canMove = true;
                    }
                }
            }
        }
        else
        {
            foreach (GameObject player in playerTargets)
            {
                if (player.tag == "Player")
                {
                    player.GetComponent<characterControls>().canMove = true;
                }
            }
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        inGameMenu.SetActive(false);
        canOpen = true;
    }

    public void exitToMenu()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void exitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
            Application.Quit();
    }
}
