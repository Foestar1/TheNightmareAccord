using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;
using Smooth;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class characterControls : MonoBehaviourPunCallbacks
{
    #region Variables Section
    [Header("Movement Variables")]
    [Tooltip("The players run speed")]
    [SerializeField]
    private float runSpeed = 8.0f;
    public float movementSpeed { get; set; }
    public bool playerSlowed { get; set; }
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
    [Tooltip("Controller sensitivity for the camera")]
    [SerializeField]
    private float controllerSensitivity = 350.0f;
    [Tooltip("The range of motion for the camera's up and down")]
    [SerializeField]
    private float upDownRange = 80.0f;
    [Tooltip("The distance required to see an interactable")]
    [SerializeField]
    private float interactableDistance;
    private GameObject playerCrosshair;
    private GameObject interactionButton;
    [SerializeField]
    private GameObject observerObject;
    private float verticalRotation;

    //[Header("World References")]
    //[Tooltip("Our players view camera")]
    //[SerializeField]
    private HubController hubControlObject;
    private Spawner spawnerObject;

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
            //enable the smoothsync for lag
            this.GetComponent<SmoothSyncPUN2>().enabled = true;
            //if it's ours do stuff
            if (this.photonView.IsMine)
            {
                //spawner object
                if (SceneManager.GetActiveScene().name != "DreamHub")
                {
                    spawnerObject = GameObject.Find("SpawnControls").GetComponent<Spawner>();
                }
                //check the scene for farClipPlane change
                if (SceneManager.GetActiveScene().name == "Doubloon Dash")
                {
                    playerCamera.farClipPlane = 10000;
                }
            }
        }
        else
        {
            //hub control part
            if (SceneManager.GetActiveScene().name == "DreamHub")
            {
                hubControlObject = GameObject.Find("HubController").GetComponent<HubController>();
            }
            //spawner object
            if (SceneManager.GetActiveScene().name != "DreamHub")
            {
                spawnerObject = GameObject.Find("SpawnControls").GetComponent<Spawner>();
            }
            //players lives
            playerLives = 3;
            //scene check for farClipPlane change
            if (SceneManager.GetActiveScene().name == "Doubloon Dash")
            {
                playerCamera.farClipPlane = 10000;
            }
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
                else
                {
                    characterController.SimpleMove(Vector3.zero);
                    HandleAnimation();
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
            else
            {
                targetedObject = null;
                playerCrosshair.GetComponent<Image>().color = CrosshairColors[0];
                interactionButton.SetActive(false);
                characterController.SimpleMove(Vector3.zero);
                HandleAnimation();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (this.photonView.IsMine)
            {
                //ENEMY HIT AREA
                if (other.name == "HitArea" && !isDead)
                {
                    isDead = true;
                    var myPlayerObject = PhotonNetwork.Instantiate(this.playerDeathSpirit.name, this.transform.position, this.transform.rotation, 0);
                    int playerPWID = this.photonView.ViewID;
                    int playerGhostPWID = myPlayerObject.GetPhotonView().ViewID;
                    this.photonView.RPC("playerDied", RpcTarget.All, playerPWID, playerGhostPWID);
                    spawnerObject.soloDeaths++;
                    var observerObjectCamera = Instantiate(observerObject, this.transform.position, this.transform.rotation);
                    observerObjectCamera.transform.GetChild(0).gameObject.SetActive(true);
                    myPlayerObject.GetComponent<LinkedPlayer>().linkedObserver = observerObjectCamera;
                    playerCrosshair.SetActive(false);
                }

                //SLOW ZONE AREA
                if (other.tag == "SlowZone")
                {
                    playerSlowed = true;
                }

                //DEATH ZONE AREA
                if (other.tag == "DeathZone" && !isDead)
                {
                    isDead = true;
                    var myPlayerObject = PhotonNetwork.Instantiate(this.playerDeathSpirit.name, this.transform.position, this.transform.rotation, 0);
                    int playerPWID = this.photonView.ViewID;
                    int playerGhostPWID = myPlayerObject.GetPhotonView().ViewID;
                    this.photonView.RPC("playerDied", RpcTarget.All, playerPWID, playerGhostPWID);
                    spawnerObject.soloDeaths++;
                    var observerObjectCamera = Instantiate(observerObject, this.transform.position, this.transform.rotation);
                    observerObjectCamera.transform.GetChild(0).gameObject.SetActive(true);
                    myPlayerObject.GetComponent<LinkedPlayer>().linkedObserver = observerObjectCamera;
                    playerCrosshair.SetActive(false);
                }
            }
        }
        else
        {
            //ENEMY HIT AREA
            if (other.name == "HitArea" && !isDead)
            {
                if (playerLives > 1)
                {
                    isDead = true;
                    var ghostSpiritSpot = Instantiate(playerDeathSpirit, this.transform.position, this.transform.rotation);
                    ghostSpiritSpot.transform.GetChild(0).gameObject.SetActive(false);
                    Instantiate(lightExplosion, playerCamera.transform.position, playerCamera.transform.rotation);

                    spawnerObject.soloDeaths++;
                    spawnerObject.totalDeaths++;
                    currentCooldownTime = 0;
                    Image fillImage = teddyUI.transform.GetChild(1).transform.GetComponent<Image>();
                    fillImage.fillAmount = 0; // Ensure the fill amount is set to 0 at the end
                    teddyOnCooldown = false;
                    playerLives--;
                    GameObject playersLivesUI = GameObject.Find("PlayerLives");
                    playersLivesUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerLives.ToString();
                    StartCoroutine(UpdateDead());
                }else if (playerLives == 1) {
                    isDead = true;
                    spawnerObject.soloDeaths++;
                    spawnerObject.totalDeaths++;
                    spawnerObject.openScoreboardLost();
                    spawnerObject.timerRunning = false;
                    Destroy(this.gameObject);
                }
            }

            //SLOW ZONE AREA
            if (other.tag == "SlowZone")
            {
                playerSlowed = true;
            }

            //DEATH ZONE AREA
            if (other.tag == "DeathZone" && !isDead)
            {
                if (playerLives > 1)
                {
                    isDead = true;
                    var ghostSpiritSpot = Instantiate(playerDeathSpirit, this.transform.position, this.transform.rotation);
                    ghostSpiritSpot.transform.GetChild(0).gameObject.SetActive(false);
                    spawnerObject.soloDeaths++;
                    spawnerObject.totalDeaths++;
                    playerLives--;
                    GameObject playersLivesUI = GameObject.Find("PlayerLives");
                    playersLivesUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerLives.ToString();
                    StartCoroutine(UpdateDead());
                }
                else if (playerLives == 1)
                {
                    isDead = true;
                    spawnerObject.soloDeaths++;
                    spawnerObject.totalDeaths++;
                    spawnerObject.openScoreboardLost();
                    spawnerObject.timerRunning = false;
                    Destroy(this.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (this.photonView.IsMine)
            {
                //SLOW ZONE AREA
                if (other.tag == "SlowZone")
                {
                    playerSlowed = false;
                }
            }
        }
        else
        {
            //SLOW ZONE AREA
            if (other.tag == "SlowZone")
            {
                playerSlowed = false;
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "MovingObject")
        {
            this.transform.parent = hit.transform;
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
        //unparent moving objects if not grounded
        if (!characterController.isGrounded)
        {
            this.transform.parent = null;
        }

        if (canRun && Input.GetButton("Sprint"))
        {
            if (!playerSlowed)
            {
                movementSpeed = runSpeed;
            }
            else
            {
                movementSpeed = runSpeed / 2;
            }
        }
        else
        {
            if (!playerSlowed)
            {
                movementSpeed = 5;
            }
            else
            {
                movementSpeed = 5 / 2;
            }
        }

        float verticalSpeed = Input.GetAxis("Vertical") * movementSpeed;
        float horizontalSpeed = Input.GetAxis("Horizontal") * movementSpeed;

        Vector3 speed = new Vector3(horizontalSpeed, 0, verticalSpeed);
        speed = transform.rotation * speed;
        //GRAVITY!!!
        speed += Physics.gravity * 2;
        characterController.SimpleMove(speed);
    }

    private void HandleRotation()
    {
        // Determine input source: Mouse or Controller
        float mouseXRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseYRotation = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Controller input (assuming you have set up "RightStickHorizontal" and "RightStickVertical" in the Input Manager)
        float controllerXRotation = Input.GetAxis("RightStickHorizontal") * controllerSensitivity * Time.deltaTime;
        float controllerYRotation = Input.GetAxis("RightStickVertical") * controllerSensitivity * Time.deltaTime;

        // Combine mouse and controller input, allowing for both to be used
        float finalXRotation = mouseXRotation + controllerXRotation;
        float finalYRotation = mouseYRotation + controllerYRotation;

        // Apply horizontal rotation
        transform.Rotate(0, finalXRotation, 0);

        // Apply vertical rotation with clamping
        verticalRotation -= finalYRotation;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);

        // Apply to camera
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
                    if (targetedObject.name == "PetalsLament" && hubControlObject.canOpen)
                    {
                        interactionButton.SetActive(false);
                        canMove = false;
                        hubControlObject.levelChoice = 0;
                        hubControlObject.openLevelInfo();
                    }
                    if (targetedObject.name == "ClothingNightstand" && hubControlObject.canOpen)
                    {
                        interactionButton.SetActive(false);
                        canMove = false;
                        hubControlObject.openCustomizationMenu();
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
                            spawnerObject.soloObjectives++;
                        }
                        else
                        {
                            targetedObject.SetActive(false);
                            spawnerObject.goalsNotFound--;
                            spawnerObject.soloObjectives++;
                            spawnerObject.totalObjectives++;
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

    public void resetDead()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (this.photonView.IsMine)
            {
                playerCrosshair.SetActive(true);
                StartCoroutine(UpdateDead());
                if (teddyOnCooldown)
                {
                    StartCoroutine(UpdateCooldown());
                }
            }
        }
        else
        {
            playerCrosshair.SetActive(true);
            StartCoroutine(UpdateDead());
            StartCoroutine(UpdateCooldown());
        }
    }
    #endregion

    #region coroutines
    private IEnumerator UpdateCooldown()
    {
        Image fillImage = teddyUI.transform.GetChild(1).transform.GetComponent<Image>();
        float endTime = Time.time + teddyCooldownTime; // Time when the cooldown will end

        while (Time.time < endTime && teddyOnCooldown)
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
        GameObject.Find("SpawnControls").GetComponent<Spawner>().totalObjectives++;
    }

    [PunRPC]
    void playerDied(int playerPWID, int playerGhostPWID)
    {
        GameObject.Find("SpawnControls").GetComponent<Spawner>().playersAlive--;
        GameObject.Find("SpawnControls").GetComponent<Spawner>().totalDeaths++;
        PhotonView.Find(playerGhostPWID).GetComponent<LinkedPlayer>().linkedPlayer = PhotonView.Find(playerPWID).gameObject;
        PhotonView.Find(playerPWID).gameObject.SetActive(false);
    }
    #endregion
}
