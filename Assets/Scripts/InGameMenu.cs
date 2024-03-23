using UnityEngine;
using UnityEngine.UI;
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
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.InRoom)
            {
                if (playerTargets.Count < PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    getPlayers();
                }
            }
        }
        else
        {
            if (playerTargets.Count < 1)
            {
                getPlayers();
            }
        }

        openMenu();
    }

    public void getPlayers()
    {
        playerTargets.Clear();
        playerTargets.AddRange(GameObject.FindGameObjectsWithTag("Player"));
    }

    public void openMenu()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                foreach (GameObject player in playerTargets)
                {
                    if (player.GetPhotonView().IsMine)
                    {
                        canOpen = player.GetComponent<characterControls>().canMove;
                    }
                }
            }
            else
            {
                foreach (GameObject player in playerTargets)
                {
                    if (player != null)
                    {
                        canOpen = player.GetComponent<characterControls>().canMove;
                    }
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
                            player.GetComponent<characterControls>().canMove = false;
                        }
                    }
                }
                else
                {
                    foreach (GameObject player in playerTargets)
                    {
                        player.GetComponent<characterControls>().canMove = false;
                    }
                }

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                inGameMenu.SetActive(true);
                inGameMenu.transform.GetChild(0).GetComponent<Button>().Select();
                canOpen = false;
            }
        }
    }

    public void resumeGame()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            foreach (GameObject player in playerTargets)
            {
                if (player.GetPhotonView().IsMine)
                {
                    player.GetComponent<characterControls>().canMove = true;
                }
            }
        }
        else
        {
            foreach (GameObject player in playerTargets)
            {
                player.GetComponent<characterControls>().canMove = true;
            }
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        inGameMenu.SetActive(false);
        canOpen = true;
    }

    public void exitToMenu()
    {
        Destroy(GameObject.Find("PersistantSaveAndLoad").gameObject);
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

    public void setRegionUS()
    {
        SaveAndLoadData saver = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        saver.chosenRegion = "us";
        saver.saveInfo();
    }

    public void setRegionEU()
    {
        SaveAndLoadData saver = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        saver.chosenRegion = "eu";
        saver.saveInfo();
    }

    public void setRegionASIA()
    {
        SaveAndLoadData saver = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        saver.chosenRegion = "asia";
        saver.saveInfo();
    }
}
