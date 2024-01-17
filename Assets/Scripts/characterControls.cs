using UnityEngine;

public class characterControls : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField]
    private float walkSpeed = 3.0f;

    [Header("Look Sensitivity")]
    [SerializeField]
    private float mouseSensitivity = 2.0f;
    [SerializeField]
    private float upDownRange = 80.0f;

    [Header("Inputs Customization")]
    [SerializeField]
    private string horizontalMoveInput = "Horizontal";
    [SerializeField]
    private string verticalMoveInput = "Vertical";
    [SerializeField]
    private string mouseXInput = "Mouse X";
    [SerializeField]
    private string mouseYInput = "Mouse Y";

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
