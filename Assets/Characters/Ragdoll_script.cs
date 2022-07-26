using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll_script : MonoBehaviour
{
    Vector3 movement;
    float gravity = -9.81f;
    readonly float groundedGravity = -.05f;
    Rigidbody[] rigidbodies;
    Animator animator;
    CharacterController charController;

    void Start()
    {
        movement = Vector2.zero;
        charController = GetComponent<CharacterController>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();

        DeactivateRagdoll();
    }

    private void Update()
    {
        if (charController.isGrounded)
        {
            movement.y = groundedGravity;
            charController.Move(movement*Time.deltaTime);
        }
        else
        {
            movement.y = gravity;
            charController.Move(movement*Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            ActivateRagdoll();
        }
    }

    private void DeactivateRagdoll()
    {
        foreach(var rigidBody in rigidbodies)
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
