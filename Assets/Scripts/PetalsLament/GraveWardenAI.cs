using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

public class GraveWardenAI : MonoBehaviourPunCallbacks
{
    #region variables
    [Header("Warden Stuff")]
    [Tooltip("The pathing spots for the warden")]
    [SerializeField]
    private Transform[] wardenSpots;
    private int currentSpotElement;
    private Transform currentWardenSpot;
    private NavMeshAgent wardenAgent;
    [Tooltip("The wardens animator controlling animations")]
    [SerializeField]
    private Animator wardenAnimator;
    private bool revivingWraiths;
    public string currentZone { get; set; }
    [Tooltip("The wardens wait time")]
    [SerializeField]
    private int wardenWaitTime;
    [Tooltip("The wardens trusty lantern")]
    [SerializeField]
    private Transform lanternSpot;
    [Tooltip("The prefab for the wardens hunting flame")]
    [SerializeField]
    private GameObject wardenFlame;

    [Header("Wraith Stuff")]
    [Tooltip("A list of all the wraiths in the wardens current zone")]
    [SerializeField]
    private List<GameObject> wraithAgents;
    #endregion

    #region built in functions
    private void Awake()
    {
        wardenAgent = this.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                movementTime();
                animationTime();
            }
        }
        else
        {
            movementTime();
            animationTime();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "GraveyardTrigger")
        {
            currentZone = "Graveyard";
        }

        if (other.name == "ForestTrigger")
        {
            currentZone = "Forest";
        }

        if (other.name == "MeadowTrigger")
        {
            currentZone = "Meadow";
        }

        if (other.name == "RockyTrigger")
        {
            currentZone = "Rocky";
        }

        if (other.name == "TunnelTrigger")
        {
            currentZone = "Tunnel";
        }
    }
    #endregion

    #region custom functions
    private void movementTime()
    {
        if (wardenAgent.remainingDistance <= wardenAgent.stoppingDistance)
        {
            if (currentWardenSpot == null)
            {
                int randoSpot = Random.Range(0, wardenSpots.Length);
                currentSpotElement = randoSpot;
                currentWardenSpot = wardenSpots[randoSpot];
                wardenAgent.SetDestination(currentWardenSpot.position);
            }
            else
            {
                if (!revivingWraiths && !wardenAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle2"))
                {
                    revivingWraiths = true;
                    StartCoroutine(spawnFlames());
                    wardenAnimator.SetBool("reviving", true);
                    if (currentSpotElement < wardenSpots.Length - 1)
                    {
                        currentSpotElement++;
                    }
                    else
                    {
                        currentSpotElement = 0;
                    }
                    currentWardenSpot = wardenSpots[currentSpotElement];
                    StartCoroutine(pickNewSpot());
                }
            }
        }
    }

    private void animationTime()
    {
        if (wardenAgent.remainingDistance <= wardenAgent.stoppingDistance)
        {
            wardenAnimator.SetBool("moving", false);
        }
        else
        {
            wardenAnimator.SetBool("moving", true);
        }
    }

    private void getWraiths()
    {
        wraithAgents.Clear();
        GameObject[] allWraiths = GameObject.FindGameObjectsWithTag("EnemySpirit");

        foreach(GameObject wraith in allWraiths)
        {
            if (wraith.GetComponent<SpiritFlameStuff>().currentZone == currentZone)
            {
                wraithAgents.Add(wraith);
            }
        }
    }
    #endregion

    #region coroutines
    private IEnumerator pickNewSpot()
    {
        yield return new WaitForSeconds(wardenWaitTime);
        revivingWraiths = false;
        wardenAnimator.SetBool("reviving", false);
        wardenAgent.SetDestination(currentWardenSpot.position);
    }

    private IEnumerator spawnFlames()
    {
        yield return new WaitForSeconds(2);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                getWraiths();
                foreach (GameObject wraith in wraithAgents)
                {
                    var flameObject = PhotonNetwork.Instantiate(this.wardenFlame.name, lanternSpot.transform.position, Quaternion.identity, 0);
                    flameObject.GetComponent<HuntingFlame>().targetObject = wraith;
                }
            }
        }
        else
        {
            getWraiths();
            foreach (GameObject wraith in wraithAgents)
            {
                var huntingFlame = Instantiate(wardenFlame, lanternSpot.transform.position, Quaternion.identity);
                huntingFlame.GetComponent<HuntingFlame>().targetObject = wraith;
            }
        }
    }
    #endregion
}