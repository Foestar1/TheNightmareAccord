using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;
using Smooth;
using System.Collections;

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
    private int playerLives;
    public bool isDead { get; set; }

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
    [Tooltip("The colors for the crosshair depending on what we hover over")]
    [SerializeField]
    private Color[] CrosshairColors;
    [Tooltip("The players head, enabled on other players but not the one we control")]
    [SerializeField]
    private SkinnedMeshRenderer[] characterHeadStuff;
    [Tooltip("The players head, enabled on other players but not the one we control")]
    [SerializeField]
    private MeshRenderer[] characterHairStuff;
    [Tooltip("The players head, enabled on other players but not the one we control")]
    [SerializeField]
    private GameObject playerDeathSpirit;

    [Header("Teddy References")]
    [Tooltip("The characters Teddy bear hanging in their hands")]
    [SerializeField]
    private GameObject TeddyObject;
    [Tooltip("The light explosion prefab for the teddy bear")]
    [SerializeField]
    private GameObject lightExplosion;
    [Tooltip("Teddy's cooldown time")]
    [SerializeField]
    private float teddyCooldownTime;
    private float currentCooldownTime;
    private bool teddyOnCooldown;
    private GameObject teddyUI;
    #endregion

    #region Unity Built in Functions
    private void Awake()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            this.GetComponent<SmoothSyncPUN2>().enabled = true;
        }
        else
        {
            playerLives = 3;
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
                    HandleTeddy();
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
                HandleTeddy();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (this.photonView.IsMine)
            {
                if (other.name == "HitArea" && !isDead)
                {
                    isDead = true;
                    var myPlayerObject = PhotonNetwork.Instantiate(this.playerDeathSpirit.name, this.transform.position, this.transform.rotation, 0);
                    int playerPWID = this.photonView.ViewID;
                    this.photonView.RPC("playerDied", RpcTarget.All, playerPWID);
                    myPlayerObject.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (other.name == "HitArea" && !isDead)
            {
                if (playerLives > 1)
                {
                    isDead = true;
                    var ghostSpiritSpot = Instantiate(playerDeathSpirit, this.transform.position, this.transform.rotation);
                    ghostSpiritSpot.transform.GetChild(0).gameObject.SetActive(false);
                    Instantiate(lightExplosion, playerCamera.transform.position, playerCamera.transform.rotation);
                    currentCooldownTime = 0;
                    StopCoroutine(UpdateCooldown());
                    playerLives--;
                    StartCoroutine(UpdateDead());
                }else if (playerLives == 1) {
                    isDead = true;
                    var ghostSpiritSpot = Instantiate(playerDeathSpirit, this.transform.position, this.transform.rotation);
                    ghostSpiritSpot.transform.GetChild(0).gameObject.SetActive(true);
                    this.gameObject.SetActive(false);
                    Debug.Log("GAME OVER!");
                }
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
                    if (distance < interactableDistance * 8)
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
                        if (PhotonNetwork.IsConnectedAndReady)
                        {
                            //make a temp var
                            int photonViewNumber = targetedObject.GetPhotonView().ViewID;
                            //send RPC to remove a flower
                            this.photonView.RPC("minusGoal", RpcTarget.All, photonViewNumber);
                        }
                        else
                        {
                            targetedObject.SetActive(false);
                            GameObject.Find("SpawnControls").GetComponent<Spawner>().goalsNotFound--;
                        }
                    }
                }
                #endregion
            }
        }
    }

    private void HandleTeddy()
    {
        if (canRun)
        {
            if (Input.GetButtonDown("Fire1") && !teddyOnCooldown)
            {
                teddyOnCooldown = true;
                if (teddyUI == null)
                {
                    teddyUI = GameObject.Find("TeddyBorder");
                }

                if (PhotonNetwork.IsConnectedAndReady)
                {
                    PhotonNetwork.Instantiate(this.lightExplosion.name, playerCamera.transform.position, playerCamera.transform.rotation, 0);
                }
                else
                {
                    Instantiate(lightExplosion, playerCamera.transform.position, playerCamera.transform.rotation);
                }

                currentCooldownTime = teddyCooldownTime;
                StartCoroutine(UpdateCooldown());
            }
        }
    }
    #endregion

    #region coroutines
    private IEnumerator UpdateCooldown()
    {
        Image fillImage = teddyUI.transform.GetChild(1).transform.GetComponent<Image>();
        float endTime = Time.time + teddyCooldownTime; // Time when the cooldown will end

        while (Time.time < endTime)
        {
            currentCooldownTime = endTime - Time.time; // Remaining time
            float tempCD = currentCooldownTime / teddyCooldownTime;
            fillImage.fillAmount = tempCD;
            yield return null;
        }

        currentCooldownTime = 0;
        fillImage.fillAmount = 0; // Ensure the fill amount is set to 0 at the end
        teddyOnCooldown = false;
    }

    private IEnumerator UpdateDead()
    {
        yield return new WaitForSeconds(3);
        isDead = false;
    }
    #endregion

    #region RPC's
    [PunRPC]
    void minusGoal(int photonViewNumberTemp)
    {
        PhotonView.Find(photonViewNumberTemp).gameObject.SetActive(false);
        GameObject.Find("SpawnControls").GetComponent<Spawner>().goalsNotFound--;
    }

    [PunRPC]
    void playerDied(int playerPWID)
    {
        GameObject.Find("SpawnControls").GetComponent<Spawner>().playersAlive--;
        PhotonView.Find(playerPWID).gameObject.SetActive(false);
    }
    #endregion
}
