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

    [Header("Hovering Parameters")]
    [Tooltip("The height at which the AI hovers")]
    [SerializeField]
    private float hoverHeight = 5.0f;
    [Tooltip("The force applied to maintain hover height")]
    [SerializeField]
    private float hoverForce = 9.8f;
    [Tooltip("Raycast layer mask for detecting ground")]
    [SerializeField]
    private LayerMask groundLayer;

    [Header("Searching Behavior")]
    [Tooltip("Time in seconds the AI continues moving after losing the target")]
    [SerializeField]
    private float searchingDuration = 2.0f;
    [Tooltip("The rate at which the AI slows down after losing the target")]
    [SerializeField]
    private float decelerationRate = 0.5f;

    private float timeSinceLostTarget = 0.0f; // Timer to track time since the target was lost
    private bool isSearching = false; // Flag to indicate if the wraith is in searching mode

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
                // Reset searching variables
                isSearching = false;
                timeSinceLostTarget = 0.0f;

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
                if (!isSearching)
                {
                    // Start searching
                    isSearching = true;
                    timeSinceLostTarget = 0.0f;
                }

                if (isSearching)
                {
                    timeSinceLostTarget += Time.deltaTime;
                    if (timeSinceLostTarget < searchingDuration)
                    {
                        // Continue moving in the last direction but start slowing down
                        agentAI.velocity = Vector3.Lerp(agentAI.velocity, Vector3.zero, decelerationRate * Time.deltaTime);
                    }
                    else
                    {
                        // Time is up, stop the wraith
                        agentAI.velocity = Vector3.zero;
                        isSearching = false;
                    }
                }

                MaintainHover();
            }
        }
        else
        {
            //this is the dormant section
            MaintainHover();
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
        Vector3 tempTarget = new Vector3(currentTarget.position.x, currentTarget.position.y + 2, currentTarget.position.z);
        Vector3 tempDir = (tempTarget - transform.position).normalized;
        if (Physics.Raycast(transform.position, tempDir, out hit, obstacleDetectionDistance, obstacleLayer))
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
    }

    private void AvoidObstacle()
    {
        Vector3 sideDirection = Vector3.Cross(movementDirection, Vector3.up).normalized;
        Vector3 avoidanceVelocity = sideDirection * aiSpeed;
        agentAI.velocity = avoidanceVelocity;

        RotateTowards(sideDirection);
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aiSpeed * Time.deltaTime);
        }
    }

    private void MaintainHover()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, hoverHeight * 2, groundLayer))
        {
            float distanceToGround = hit.distance;
            float targetHeight = transform.position.y - distanceToGround + hoverHeight; // Target height the wraith should be at

            // Interpolate current position to the target height at a rate based on aiSpeed
            float newHeight = Mathf.MoveTowards(transform.position.y, targetHeight, aiSpeed * Time.deltaTime);

            // Set the new position maintaining the current x and z positions
            transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);

            // Adjust the velocity to maintain smooth movement, if needed
            agentAI.velocity = new Vector3(agentAI.velocity.x, 0, agentAI.velocity.z);
        }
        else
        {
            float targetHeight = transform.position.y + hoverHeight; // Target height the wraith should be at

            // Interpolate current position to the target height at a rate based on aiSpeed
            float newHeight = Mathf.MoveTowards(transform.position.y, targetHeight, aiSpeed * Time.deltaTime);

            // Set the new position maintaining the current x and z positions
            transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);

            // Adjust the velocity to maintain smooth movement, if needed
            agentAI.velocity = new Vector3(agentAI.velocity.x, 0, agentAI.velocity.z);
        }
    }
    #endregion
}