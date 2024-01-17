using UnityEngine;

public class characterControls : MonoBehaviour
{
    [Header("Movement Speed")]
    [Tooltip("The players movementSpeed")]
    [SerializeField]
    private float walkSpeed = 5.0f;

    [Header("Look Sensitivity")]
    [Tooltip("Mouse sensitivity for the camera")]
    [SerializeField]
    private float mouseSensitivity = 2.0f;
    [Tooltip("The range of motion for the camera's up and down")]
    [SerializeField]
    private float upDownRange = 80.0f;

    [Header("Inputs Customization")]
    [Tooltip("The input for horizontal character movement")]
    [SerializeField]
    private string horizontalMoveInput = "Horizontal";
    [Tooltip("The input for vertical character movement")]
    [SerializeField]
    private string verticalMoveInput = "Vertical";
    [Tooltip("The input for horizontal camera control")]
    [SerializeField]
    private string mouseXInput = "Mouse X";
    [Tooltip("The input for vertical camera control")]
    [SerializeField]
    private string mouseYInput = "Mouse Y";

    [Tooltip("The player's camera")]
    [SerializeField]
    private Camera playerCamera;

    private float verticalRotation;
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        float verticalSpeed = Input.GetAxis(verticalMoveInput) * walkSpeed;
        float horizontalSpeed = Input.GetAxis(horizontalMoveInput) * walkSpeed;

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
}
