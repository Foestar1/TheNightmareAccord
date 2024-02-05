using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LeaveGame : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int timeToWait;

    private void Awake()
    {
        StartCoroutine(leaveTheGame());
    }

    #region coroutines
    private IEnumerator leaveTheGame()
    {
        yield return new WaitForSeconds(timeToWait);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Disconnect();
        }
        SceneManager.LoadScene("DreamHub");
    }
    #endregion
}
