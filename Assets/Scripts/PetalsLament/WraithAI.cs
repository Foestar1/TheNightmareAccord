using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class WraithAI : MonoBehaviourPunCallbacks
{
    #region variables
    [Header("AI Object References")]
    [Tooltip("The AI's navmesh agent")]
    [SerializeField]
    private NavMeshAgent agentAI;
    [Tooltip("The AI's animator")]
    [SerializeField]
    private Animator agentAnimator;
    [Tooltip("The AI's targets list")]
    [SerializeField]
    private List<GameObject> playerTargets;
    [Tooltip("The AI's current target")]
    [SerializeField]
    private Transform currentTarget;
    [Tooltip("The AI's 3D audio source")]
    [SerializeField]
    private AudioSource wraithAudio;

    [Header("AI behavior references")]
    [Tooltip("The AI's dormant status")]
    [SerializeField]
    private bool isDormant;
    [Tooltip("The AI's chase distance")]
    [SerializeField]
    private float chaseDistance;
    public bool isDead { get; set; }
    public string currentZone { get; set; }
    [SerializeField]
    private GameObject floatingFlame;
    #endregion

    #region built in functions
    private void Awake()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //get all the player objects at the start
                playerTargets.AddRange(GameObject.FindGameObjectsWithTag("Player"));
            }
        }
        else
        {
            //get all the player objects at the start
            playerTargets.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        }

        isDormant = true;
    }

    private void Update()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                updateOnPlayers();
                checkDistances4Target();
                actualAI();
            }
        }
        else
        {
            updateOnPlayers();
            checkDistances4Target();
            actualAI();
        }
        audioQueue();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Explosion")
        {
            if (!isDead)
            {
                isDead = true;
                agentAnimator.SetBool("dead", true);
                agentAI.isStopped = true;
                StartCoroutine(destroyWraith());
            }
        }

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
    private void actualAI()
    {
        if (!isDead)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (playerTargets.Count != PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    playerTargets.Clear();
                    playerTargets.AddRange(GameObject.FindGameObjectsWithTag("Player"));
                }
            }

            //chase, wander and dormant part
            if (isDormant)
            {
                if (GameObject.Find("SpawnControls").GetComponent<Spawner>().goalsNotFound < 3)
                {
                    isDormant = false;
                    agentAI.speed = 10;
                }

                if (currentTarget != null)
                {
                    rotateTowardsTarget();
                    StopCoroutine(resetPath());
                    agentAI.SetDestination(currentTarget.position);
                }
                else
                {
                    if (agentAI.hasPath)
                    {
                        StartCoroutine(resetPath());
                    }
                }
            }
            else
            {
                if (currentTarget != null)
                {
                    rotateTowardsTarget();
                    StopCoroutine(resetPath());
                    agentAI.SetDestination(currentTarget.position);
                }
                else
                {
                    if (agentAI.remainingDistance <= agentAI.stoppingDistance) //done with path
                    {
                        Vector3 point;
                        if (RandomPoint(this.transform.position, chaseDistance, out point)) //pass in our centre point and radius of area
                        {
                            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                            agentAI.SetDestination(point);
                        }
                    }
                }
            }

            //animation part
            if (agentAI.remainingDistance <= agentAI.stoppingDistance)
            {
                agentAnimator.SetBool("moving", false);
            }
            else
            {
                agentAnimator.SetBool("moving", true);
            }

            if (!isDormant && agentAnimator.GetBool("dormant") == true)
            {
                agentAnimator.SetBool("dormant", false);
            }
        }
    }

    public void rotateTowardsTarget()
    {
        float distance = Vector3.Distance(currentTarget.position, this.transform.position);
        if (distance <= agentAI.stoppingDistance + 3)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    private void checkDistances4Target()
    {
        float closestDist = Mathf.Infinity;
        foreach (GameObject player in playerTargets)
        {
            float distance = Vector3.Distance(player.transform.position, this.transform.position);

            if (distance <= chaseDistance)
            {
                if (distance < closestDist)
                {
                    closestDist = distance;
                    currentTarget = player.transform;
                }
            }
        }

        if (closestDist == Mathf.Infinity)
        {
            currentTarget = null;
        }
    }

    private void updateOnPlayers()
    {
        // Flag to indicate if the list needs updating
        bool needsUpdate = false;

        // Iterate through the list to check for inactive or null entries
        foreach (GameObject player in playerTargets)
        {
            // Check if the player is null or inactive
            if (player == null || !player.activeSelf)
            {
                needsUpdate = true;
                break; // No need to continue checking once we know an update is needed
            }
        }

        // Update the list if needed
        if (needsUpdate)
        {
            playerTargets.Clear();
            playerTargets.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        }

    }

    public void destroyME()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                var myFlameObject = PhotonNetwork.Instantiate(this.floatingFlame.name, this.transform.position, this.transform.rotation, 0);
                myFlameObject.GetComponent<SpiritFlameStuff>().currentZone = this.currentZone;
                myFlameObject.GetComponent<SpiritFlameStuff>().linkedWraith = this.gameObject;
            }
        }
        else
        {
            var newFlameObject = Instantiate(floatingFlame, this.transform.position, this.transform.rotation);
            newFlameObject.GetComponent<SpiritFlameStuff>().currentZone = this.currentZone;
            newFlameObject.GetComponent<SpiritFlameStuff>().linkedWraith = this.gameObject;
        }

        this.gameObject.SetActive(false);
    }

    public void reviveME()
    {
        isDead = false;
        agentAnimator.SetBool("dead", false);
        agentAI.isStopped = false;
    }

    public void audioQueue()
    {
        //use get current animation and activate the looping grudge audio
        if (agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Chase") || agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Chase 1"))
        {
            wraithAudio.enabled = true;
            if (!wraithAudio.isPlaying)
            {
                wraithAudio.Play();
            }
        }
        else
        {
            wraithAudio.enabled = false;
            if (wraithAudio.isPlaying)
            {
                wraithAudio.Stop();
            }
        }
    }
    #endregion

    #region callbacks
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerTargets.Clear();
        playerTargets.AddRange(GameObject.FindGameObjectsWithTag("Player"));
    }
    #endregion

    #region coroutines
    public IEnumerator resetPath()
    {
        yield return new WaitForSeconds(3);
        agentAI.ResetPath();
    }

    public IEnumerator destroyWraith()
    {
        destroyME();
        yield return new WaitForSeconds(1);
        this.gameObject.SetActive(false);
    }
    #endregion
}