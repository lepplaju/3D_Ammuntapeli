using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
public class SimpleMovementScript : MonoBehaviour
{
    MovementHandler playerInput;
    CharacterController charController;
    Animator animator;
    Rigidbody[] rigidbodies;

    [SerializeField]
    private CinemachineFreeLook freelook;

    [SerializeField]
    private CinemachineFreeLook freelookZoomed;

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

    bool isAimPressed;
    readonly float fieldOfViewVal =45;
    readonly float zoomValue = 30f;

    // Heti alussa tehdään kaikki tämän funktion sisällä oleva
    private void Awake()
    {
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
        playerInput.Controls.AimWeapon.started += OnAim;
        playerInput.Controls.AimWeapon.canceled += OnAim;

        SetupJumpVariables();
        DeactivateRagdoll();
    }

    void OnAim(InputAction.CallbackContext context)
    {
        isAimPressed = context.ReadValueAsButton();
    }
    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
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
    }
    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void HandleAim() // Tähtäämisen Handlaaja - kameran zoomaus ja aseen nostaminen
    {
        Debug.Log(isAimPressed);
        if (!isAimPressed)
        {
            freelook.MoveToTopOfPrioritySubqueue();
            //if(freelook.m_Lens.FieldOfView < fieldOfViewVal) 
            //{ 
            //    freelook.m_Lens.FieldOfView += zoomValue*Time.deltaTime;
            //}
            //freelook.m_LookAt = LookAtWhenNotAiming;
        }
        else if (isAimPressed)
        {
            freelookZoomed.MoveToTopOfPrioritySubqueue();
            //if (freelook.m_Lens.FieldOfView > 35)
            //{
            //    freelook.m_Lens.FieldOfView -= zoomValue * Time.deltaTime;
            //}
            //freelook.m_LookAt = LookAtWhenAiming;
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
        bool isRunning = animator.GetBool(isRunningHash);

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
