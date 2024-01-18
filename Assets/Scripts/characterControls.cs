using UnityEngine;

public class characterControls : MonoBehaviour
{
    #region Variables Section
    [Header("Movement Variables")]
    [Tooltip("The players run speed")]
    [SerializeField]
    private float runSpeed = 8.0f;
    private float movementSpeed;
    [Tooltip("Whether the player can run or not(hub related)")]
    [SerializeField]
    private bool canRun;

    [Header("Camera Controls")]
    [Tooltip("Mouse sensitivity for the camera")]
    [SerializeField]
    private float mouseSensitivity = 2.0f;
    [Tooltip("The range of motion for the camera's up and down")]
    [SerializeField]
    private float upDownRange = 80.0f;
    [Tooltip("Radius for final point of raycast(better detection)")]
    [SerializeField]
    public float sphereRadius;
    [SerializeField]
    private LayerMask thingsToAvoid;

    [Header("Inputs Customization")]
    [Tooltip("The input button for horizontal character movement")]
    [SerializeField]
    private string horizontalMoveInput = "Horizontal";
    [Tooltip("The input button for vertical character movement")]
    [SerializeField]
    private string verticalMoveInput = "Vertical";
    [Tooltip("The input button for horizontal camera control")]
    [SerializeField]
    private string mouseXInput = "Mouse X";
    [Tooltip("The input button for vertical camera control")]
    [SerializeField]
    private string mouseYInput = "Mouse Y";
    private float verticalRotation;

    [Header("Player References")]
    [Tooltip("Our players view camera")]
    [SerializeField]
    private Camera playerCamera;
    [Tooltip("Controls the players animations")]
    [SerializeField]
    private Animator playerAnimator;
    [Tooltip("Controls the players actual movement")]
    [SerializeField]
    private CharacterController characterController;
    #endregion

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleAnimation();
        HandleRaySphereCast();
    }

    private void HandleMovement()
    {
        if (canRun && Input.GetButton("Sprint"))
        {
            movementSpeed = runSpeed;
        }
        else
        {
            movementSpeed = 5;
        }
        
        float verticalSpeed = Input.GetAxis(verticalMoveInput) * movementSpeed;
        float horizontalSpeed = Input.GetAxis(horizontalMoveInput) * movementSpeed;

        Vector3 speed = new Vector3(horizontalSpeed, 0, verticalSpeed);
        speed = transform.rotation * speed;

        characterController.SimpleMove(speed);
    }

    private void HandleRotation()
    {
        float mouseXRotation = Input.GetAxis(mouseXInput) * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= Input.GetAxis(mouseYInput) * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleAnimation()
    {
        if (characterController.velocity.magnitude == 0)
        {
            playerAnimator.SetFloat("Movespeed", 0);
        }
        else
        {
            playerAnimator.SetFloat("Movespeed", movementSpeed);
        }
    }

    private void HandleRaySphereCast()
    {
        RaycastHit HitInfo;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out HitInfo, 100.0f, thingsToAvoid))
        {
            if (Physics.CheckSphere(HitInfo.point, sphereRadius))
            {
                
            }
        }

        Vector3 direction = HitInfo.point - playerCamera.transform.position;
        Debug.DrawRay(playerCamera.transform.position, direction, Color.yellow);
    }
}
