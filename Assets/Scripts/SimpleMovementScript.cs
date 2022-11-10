using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
public class SimpleMovementScript : MonoBehaviour
{
    [SerializeField] private AudioClip pistolAudio;
    [SerializeField] private AudioSource bulletAudioSource;
    Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
    [SerializeField] Vector3 GunAimingOffset;
    [SerializeField] GunHandle_script gunHandle_script;
    [SerializeField] TimerScript _timerScript;
    [SerializeField] Transform GunHolder;
    [SerializeField] GameObject newGun;

    MovementHandler playerInput;
    CharacterController charController;
    Animator animator;
    Rigidbody[] rigidbodies;

    [SerializeField]
    private CinemachineFreeLook freelook;

    [SerializeField]
    private CinemachineFreeLook freelookZoomed;

    [SerializeField]
    private Rig gunHoldingRig;
    [SerializeField]
    private Rig gunAimingPose_Rig;
    [SerializeField]
    private Rig gunNotAimingPose_Rig;
    [SerializeField] 
    private Rig gunAimingRig_Rotation;

    [SerializeField]
    private Transform character;
    [SerializeField]
    private Transform respwanPoint;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private Transform LookAtWhenAiming;
    [SerializeField]
    private Transform LookAtWhenNotAiming;

    [SerializeField]
    private GameObject reticle;

    [SerializeField]
    private Transform gunBarrel;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform bulletParent;

    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;
    bool isJumpAnimating;
    float gravity = -9.81f;
    readonly float groundedGravity = -.05f;
    readonly float runMultiplier = 3f;
    readonly float rotationFactorPerFrame = 10f;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 movementDirection;

    bool isMovementPressed;
    float L2_value;

    bool isJumpPressed = false;
    float initialJumpVelocity;
    readonly float maxJumpHeight =3.0f;
    readonly float maxJumpTime = 1.0f;
    bool isJumping = false;

    bool isR3Pressed;
    bool isAimPressed;
    bool isShootPressed;
    bool canShoot;
    private Vector3 aimPoint;
    bool isGunEquipped = false;
    bool isRunning;
    [SerializeField] private MultiPositionConstraint multiPositionConstraint;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform AimTarget;

    // Heti alussa tehdään kaikki tämän funktion sisällä oleva
    private void Awake()
    {
        reticle = GameObject.FindWithTag("Reticle");
        GameObject freelookObj = GameObject.FindWithTag("VirtualCamera");
        freelook = freelookObj.GetComponent<CinemachineFreeLook>();
        GameObject freelookObjZoomed = GameObject.FindWithTag("VirtualCameraZoomed");
        freelookZoomed = freelookObjZoomed.GetComponent<CinemachineFreeLook>();

        //Mycamera = Camera.main;
        playerInput = new MovementHandler();
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");

        playerInput.Controls.Move.started += OnMovementInput;
        playerInput.Controls.Move.canceled += OnMovementInput;
        playerInput.Controls.Move.performed += OnMovementInput;
        playerInput.Controls.Jump.started += OnJump;
        playerInput.Controls.Jump.canceled += OnJump;
        playerInput.Controls.Run.started += OnRun;
        playerInput.Controls.Run.canceled += OnRun;

        //Aiming and Shooting
        playerInput.Controls.AimWeapon.started += OnAim;
        playerInput.Controls.AimWeapon.canceled += OnAim;
        playerInput.Controls.ShootWeapon.started += OnShoot;
        playerInput.Controls.ShootWeapon.canceled += OnShoot;

        //Tason päättymisen handlaaja
        playerInput.Controls.NextLevel.started += OnR3;
        playerInput.Controls.NextLevel.canceled += OnR3;

        SetupJumpVariables();
        DeactivateRagdoll();

        gunNotAimingPose_Rig.weight = 0;
        gunHoldingRig.weight = 0;
        gunAimingRig_Rotation.weight = 0;
        multiPositionConstraint = gunAimingPose_Rig.GetComponentInChildren<MultiPositionConstraint>();
        GunAimingOffset = new Vector3(-.15f, .55f, .45f);
    }


    void OnR3(InputAction.CallbackContext context)
    {
        isR3Pressed = context.ReadValueAsButton();
    }

    void OnAim(InputAction.CallbackContext context)
    {
        isAimPressed = context.ReadValueAsButton();
    }
    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void OnShoot(InputAction.CallbackContext context)
    {
        isShootPressed = context.ReadValueAsButton();
    }

    void OnRun(InputAction.CallbackContext context) // Juoksu uudellleennimettynä ja eri painikkeen avulla
    {
        L2_value = context.ReadValue<float>();
    }

    void Update()     // Tätä kutsutaan 60 kertaa sekunnissa
    {
        HandleAnimation();
        HandleAnimationRotation();
        HandleMovementRotation();

        if (L2_value >0)
        {
            charController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            charController.Move(currentMovement * Time.deltaTime);
        }
        HandleGravity();
        HandleJump();
        HandleAim();
        HandleShoot();
    }
    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void HandleAim() // Tähtäämisen Handlaaja - kameran zoomaus ja aseen nostaminen
    {
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray,out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            AimTarget.position = raycastHit.point;
        }

        //Debug.Log("aim: "+isAimPressed+ " running: "+isRunning+ "jumping: "+isJumping );

        if (!isAimPressed)
        {
            gunNotAimingPose_Rig.weight = 1;
            gunAimingPose_Rig.weight = 0;
            gunAimingRig_Rotation.weight = 0;
            GunHolder.localRotation = Quaternion.identity; // Palauttaa aseen rotation takaisin normaaliksi tähtäämisen jälkeen
            GunHolder.localPosition = Vector3.zero;
            reticle.SetActive(false);
            freelook.MoveToTopOfPrioritySubqueue();
        }
        else if (isAimPressed && isGunEquipped)
        {
            gunAimingRig_Rotation.weight = 1;
            gunNotAimingPose_Rig.weight = 0;
            gunAimingPose_Rig.weight = 1;

            if (!isJumping && !isRunning)
            {
                multiPositionConstraint.data.offset = GunAimingOffset;            
                reticle.SetActive(true);
                freelookZoomed.MoveToTopOfPrioritySubqueue();
            }

            else if (isRunning || isJumping)
            {
                multiPositionConstraint.data.offset = Vector3.zero;
                reticle.SetActive(true);
                freelook.MoveToTopOfPrioritySubqueue();

            }
            
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GunTag"))
        {
            EquipGun();
        }
    }

    private void EquipGun()
    {
        isGunEquipped = true;
        gunHoldingRig.weight = 1;
        Invoke("getGunBarrel", .02f);
    }

    private void getGunBarrel() // Haetaan aseen piippu, josta luoti tulee lähtemään... Ase pitää hakea erikseen
    {
        GameObject newBarrel = GameObject.FindWithTag("BarrelLocation");
        gunBarrel = newBarrel.transform;
        Invoke("getAudioSource", .02f);
    }
    private void getAudioSource()
    {
        bulletAudioSource = gunBarrel.GetComponent<AudioSource>();
    }

    void HandleShoot() // Täytyy muuttaa sellaiseksi, että rayCast lähtee aseen piipusta
    {
        if (isAimPressed && isShootPressed && canShoot)
        {
            bulletAudioSource.PlayOneShot(pistolAudio,0.1f);
            Vector3 aimDir = (AimTarget.position - gunBarrel.position).normalized;
            Instantiate(bulletPrefab, gunBarrel.position, Quaternion.LookRotation(aimDir,Vector3.up));
            canShoot = false;
            //Debug.Log(aimDir);
            //Debug.Log(Quaternion.LookRotation(aimDir, Vector3.up));
        }
        else if (!isShootPressed){
            canShoot = true;
        }
    }

    void HandleJump()
    {
        if (!isJumping && charController.isGrounded && isJumpPressed){
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            currentMovement.y += initialJumpVelocity * .5f;
            currentRunMovement.y += initialJumpVelocity * .5f;
        }
        else if(!isJumpPressed && isJumping && charController.isGrounded)
        {
            isJumping = false;
        }
    }
    void HandleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;
        if (charController.isGrounded)
        {
            if (isJumpAnimating) { 
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
            }
            
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        } else if (isFalling){
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier* Time.deltaTime);
            float nextYVelocity = Mathf.Max((previousYVelocity + newYVelocity) * .5f, -25.0f);
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        }
    }


    void OnMovementInput(InputAction.CallbackContext context) // Täällä käsitellään liikkumiseen tarvittavaa raakadataa
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        isMovementPressed = (currentMovementInput.x != 0 || currentMovementInput.y != 0);    
    }


    private void OnEnable()
    {
        playerInput.Controls.Enable();
    }
    private void OnDisable()
    {
        playerInput.Controls.Disable();
    }

    void HandleMovementRotation()
    {
        if (isMovementPressed)
        {
            float horiInput = currentMovementInput.x;
            float vertInput = currentMovementInput.y;
            movementDirection = new Vector3(horiInput, 0, vertInput);

            movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
            movementDirection.Normalize();
            currentMovement.x = movementDirection.x;
            currentMovement.z = movementDirection.z;
            currentRunMovement.x = movementDirection.x * runMultiplier;
            currentRunMovement.z = movementDirection.z * runMultiplier;
        }
    }

    /* Animaatioon liittyvä alkaa tästä
        ============================================================================================================================================ 
        ============================================================================================================================================
        ============================================================================================================================================
    */
    void HandleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        isRunning = animator.GetBool(isRunningHash);

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }
        else if (!isMovementPressed && isWalking){
            animator.SetBool("isWalking", false);
        }
        if ((isMovementPressed && L2_value>0) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if((!isMovementPressed || L2_value==0) && isRunning){
            animator.SetBool(isRunningHash, false);
        }
    }
    void HandleAnimationRotation() // ei ota huomioon liikutettavan kameran olemassaoloa :(
    {   // Luodaan täällä mihin suuntaan haluamme hahmon katsovan
        // Tällä hetkellä halutaan rotaatiota vain x- ja z- akseleilla
        Vector3 positionToLookAt;
        positionToLookAt.x = movementDirection.x;
        positionToLookAt.y = 0f;
        positionToLookAt.z = movementDirection.z;
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    // Tason Päättyminen - aktivoidaan R3 että voidaan siirtyä seuraavaan tasoon
    public void handleLevelChange()
    {
        if(_timerScript.gameIsFinished && isR3Pressed)
        {
            Debug.Log("R3 IS pressed");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // Tästä eteenpäin ragdolliin liittyvää asiaa
    /* ============================================================================================================================================ 
       ============================================================================================================================================
       ============================================================================================================================================ */

    private void DeactivateRagdoll()
    {
        foreach (var rigidBody in rigidbodies)
        {
            rigidBody.isKinematic = true;
        }
        animator.enabled = true;
    }
    private void ActivateRagdoll()
    {
        foreach (var rigidBody in rigidbodies)
        {
            rigidBody.isKinematic = false;
        }
        animator.enabled = false;
    }
}
