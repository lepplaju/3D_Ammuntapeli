using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class myMovementScript : MonoBehaviour
{

    private CharacterController controller;
    private Animator animator;
    [SerializeField] 
    private float moveSpeed = 1;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float gravity = 5f;

    private bool groundedPlayer;

    private Vector3 playerVelocity;
    private Vector2 moveVector;


    private void Start()
    {
        controller = GetComponent<CharacterController>();   // Haetaan ohjaimen komponentti muuttujaan
        animator = GetComponent<Animator>();                // Haetaan animaattorin komponentti muuttujaan
    }

    void Update()
    {
        Debug.Log(playerVelocity);
        Move();
    }

    private void Move()
    {
        if(controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0.1f*gravity*Time.deltaTime;
        }

        Vector3 moveVelocity = (cam.transform.right * moveVector.x + cam.transform.forward * moveVector.y + Vector3.down*0.01f);
        controller.Move(moveVelocity * moveSpeed * Time.deltaTime);
        Rotate(moveVelocity);
    }
    
    // Tänne mennään kun hahmoa liikutetaan
    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
        if (moveVector.magnitude > 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
    // Tänne mennään kun hypätään
    public void OnJump(InputAction.CallbackContext context)
    {
        if(controller.isGrounded && context.performed)
        {
            //print("jump happened");
            //animator.Play("jump");
            Jump();
        }
    }

    private void Jump()
    {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    private void Rotate (Vector3 target)
    {
        transform.LookAt(transform.position + target);
    }

}
