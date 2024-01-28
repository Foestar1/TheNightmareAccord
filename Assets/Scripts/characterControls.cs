using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;
using Smooth;

public class characterControls : MonoBehaviourPunCallbacks
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
    public bool canMove;

    [Header("Camera Controls")]
    [Tooltip("Mouse sensitivity for the camera")]
    [SerializeField]
    private float mouseSensitivity = 2.0f;
    [Tooltip("The range of motion for the camera's up and down")]
    [SerializeField]
    private float upDownRange = 80.0f;
    [Tooltip("The distance required to see an interactable")]
    [SerializeField]
    private float interactableDistance;
    private GameObject playerCrosshair;
    private GameObject interactionButton;

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
    [Tooltip("What we are targeting interactable wise")]
    [SerializeField]
    private GameObject targetedObject;
    [Tooltip("Controls the players animations")]
    [SerializeField]
    private Animator playerAnimator;
    [Tooltip("Controls the players actual movement")]
    [SerializeField]
    private CharacterController characterController;
    [Tooltip("The characters Teddy bear hanging in their hands")]
    [SerializeField]
    private GameObject TeddyObject;
    [Tooltip("The colors for the crosshair depending on what we hover over")]
    [SerializeField]
    private Color[] CrosshairColors;
    [Tooltip("The players head, enabled on other players but not the one we control")]
    [SerializeField]
    private SkinnedMeshRenderer[] characterHeadStuff;
    [Tooltip("The players head, enabled on other players but not the one we control")]
    [SerializeField]
    private MeshRenderer[] characterHairStuff;
    #endregion

    #region Unity Built in Functions
    private void Awake()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            this.GetComponent<SmoothSyncPUN2>().enabled = true;
        }
    }

    private void Start()
    {
        if (canRun)
        {
            TeddyObject.SetActive(true);
        }
        else
        {
            TeddyObject.SetActive(false);
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (this.photonView.IsMine)
            {
                GameObject.Find("Main Camera").SetActive(false);

                foreach(SkinnedMeshRenderer piece in characterHeadStuff)
                {
                    piece.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }

                foreach (MeshRenderer piece in characterHairStuff)
                {
                    piece.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }

                canMove = true;

                if (canMove)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            else
            {
                foreach (SkinnedMeshRenderer piece in characterHeadStuff)
                {
                    piece.shadowCastingMode = ShadowCastingMode.On;
                }

                foreach (MeshRenderer piece in characterHairStuff)
                {
                    piece.shadowCastingMode = ShadowCastingMode.On;
                }

                playerCamera.gameObject.SetActive(false);
            }
        }
        else
        {
            if (canRun)
            {
                GameObject.Find("Main Camera").SetActive(false);
            }
            canMove = true;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (this.photonView.IsMine)
            {
                if (canMove)
                {
                    HandleUI();
                    HandleMovement();
                    HandleRotation();
                    HandleAnimation();
                    HandleRaySphereCast();
                    HandleInteraction();
                }
            }
        }
        else
        {
            if (canMove)
            {
                HandleUI();
                HandleMovement();
                HandleRotation();
                HandleAnimation();
                HandleRaySphereCast();
                HandleInteraction();
            }
        }
    }
    #endregion

    #region Custom Functions
    private void HandleUI()
    {
        if (playerCrosshair == null)
        {
            playerCrosshair = GameObject.Find("Crosshair");
        }

        if (interactionButton == null)
        {
            interactionButton = GameObject.Find("InteractionButton");
            interactionButton.SetActive(false);
        }
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
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out HitInfo, 100.0f))
        {

            // Check if the collider has the "Interactable" tag
            if (HitInfo.transform.tag == "Interactable")
            {
                float distance = Vector3.Distance(playerCamera.transform.position, HitInfo.transform.position);
                if (distance < interactableDistance)
                {
                    targetedObject = HitInfo.transform.gameObject;
                    interactionButton.SetActive(true);
                    playerCrosshair.GetComponent<Image>().color = CrosshairColors[2];
                }
                else
                {
                    targetedObject = null;
                    playerCrosshair.GetComponent<Image>().color = CrosshairColors[0];
                    interactionButton.SetActive(false);
                }
            }
            else
            {
                interactionButton.SetActive(false);
                targetedObject = null;

                if (HitInfo.transform.tag == "Enemy")
                {
                    float distance = Vector3.Distance(playerCamera.transform.position, HitInfo.transform.position);
                    if (distance < interactableDistance * 4)
                    {
                        playerCrosshair.GetComponent<Image>().color = CrosshairColors[1];
                    }
                    else
                    {
                        playerCrosshair.GetComponent<Image>().color = CrosshairColors[0];
                    }
                }
                else
                {
                    playerCrosshair.GetComponent<Image>().color = CrosshairColors[0];
                }
            }
        }
        else
        {
            targetedObject = null;
            playerCrosshair.GetComponent<Image>().color = CrosshairColors[0];
            interactionButton.SetActive(false);
        }

        Vector3 direction = HitInfo.point - playerCamera.transform.position;
        Debug.DrawRay(playerCamera.transform.position, direction, Color.yellow);
    }

    private void HandleInteraction()
    {
        if (targetedObject != null)
        {
            if (Input.GetButtonDown("Interact"))
            {
                #region DreamHub section
                //the section for the hub
                if (!canRun)
                {
                    if (targetedObject.name == "PetalsLament")
                    {
                        interactionButton.SetActive(false);
                        canMove = false;
                        GameObject.Find("HubController").GetComponent<HubController>().levelChoice = 0;
                        GameObject.Find("HubController").GetComponent<HubController>().openLevelInfo();
                    }
                }
                #endregion

                #region Level section
                //the section for the levels
                if (canRun)
                {
                    if (targetedObject.name == "PetalsLament")
                    {
                        //make a temp var
                        int photonViewNumber = targetedObject.GetPhotonView().ViewID;
                        //send RPC to remove a flower
                        this.photonView.RPC("minusGoal", RpcTarget.All, photonViewNumber);
                    }
                }
                #endregion
            }
        }
    }
    #endregion

    #region RPC's
    [PunRPC]
    void minusGoal(int photonViewNumberTemp)
    {
        PhotonView.Find(photonViewNumberTemp).gameObject.SetActive(false);
        GameObject.Find("SpawnControls").GetComponent<Spawner>().goalsNotFound--;
    }
    #endregion
}
