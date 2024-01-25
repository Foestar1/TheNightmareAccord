using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class WraithCustomAI : MonoBehaviour
{
    [Header("AI Object References")]
    [Tooltip("The AI's rigidbody")]
    [SerializeField]
    private Rigidbody agentAI;
    [Tooltip("The AI's targets list")]
    [SerializeField]
    private List<GameObject> playerTargets;
    [Tooltip("The AI's current target")]
    [SerializeField]
    private Transform currentTarget;

    [Header("AI behavior references")]
    [Tooltip("The AI's dormant status")]
    [SerializeField]
    private bool isDormant;
    [Tooltip("The AI's chase distance")]
    [SerializeField]
    private float chaseDistance;
    [Tooltip("The AI's movement speed")]
    [SerializeField]
    private float aiSpeed;

    [Header("Obstacle Avoidance")]
    [Tooltip("The AI's obstacle detection range")]
    [SerializeField]
    private float obstacleDetectionDistance;
    [Tooltip("The layer mask for the obstacles")]
    [SerializeField]
    private LayerMask obstacleLayer;
    [Tooltip("The AI's movement direction")]
    [SerializeField]
    private Vector3 movementDirection;

    #region built in functions
    private void Awake()
    {
        //get all the player objects at the start
        playerTargets.AddRange(GameObject.FindGameObjectsWithTag("Player"));
    }

    private void Update()
    {
        checkDistances4Target();

        if (!isDormant)
        {
            if (currentTarget != null)
            {
                movementDirection = (currentTarget.transform.position - transform.position).normalized;
                if (IsPathClear())
                {
                    MoveTowardsTarget();
                }
                else
                {
                    AvoidObstacle();
                }
            }
            else
            {
                //this is where we are gonna add random movement code
                Debug.Log("Stopped and dormant");
            }
        }
        else
        {
            //this is the dormant section
        }
    }
    #endregion

    #region custom functions
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

    private bool IsPathClear()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, movementDirection, out hit, obstacleDetectionDistance, obstacleLayer))
        {
            return hit.collider == null; // Path is clear if no collider was hit
        }
        return true;
    }

    private void MoveTowardsTarget()
    {
        Vector3 velocity = movementDirection * aiSpeed;
        agentAI.velocity = velocity;

        RotateTowards(movementDirection);
        Debug.Log("Chasing");
    }

    private void AvoidObstacle()
    {
        Vector3 sideDirection = Vector3.Cross(movementDirection, Vector3.up).normalized;
        Vector3 avoidanceVelocity = sideDirection * aiSpeed;
        agentAI.velocity = avoidanceVelocity;

        RotateTowards(sideDirection);
        Debug.Log("Avoiding Obstacle");
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aiSpeed * Time.deltaTime);
        }
    }
    #endregion
}