using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll_script : MonoBehaviour
{
    [SerializeField] Transform lookAtTarget;
    Vector3 movement;
    //float gravity = -9.81f;
    readonly float groundedGravity = -.05f;
    Rigidbody[] rigidbodies;
    Collider[] colliders;
    Animator animator;
    CharacterController characterController;
    [SerializeField] private bool isAlive;
    [SerializeField] private GameObject createPlatform;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ScoreCounterScript scoreCountAudioHandler;

    private void Awake()
    {
        createNewPlatform();
        isAlive = true;
        characterController = GetComponent<CharacterController>();
        movement = Vector2.zero;
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();
        DeactivateRagdoll();
        ActivateColliders();
    }
    void Start()
    {
        
    }

    private void Update()
    {
        movement.y = groundedGravity;
        characterController.Move(movement*Time.deltaTime);
        HandleLookingAtTarget();
        
    }
    private void createNewPlatform()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        Vector3 spotUnder = transform.position + new Vector3(0f, -.3f, 0f);
        Instantiate(createPlatform, spotUnder, Quaternion.identity);
    }
    private void HandleLookingAtTarget()
    {
        if (isAlive)
        {
            Vector3 dir = lookAtTarget.position - transform.position;
            dir.y = 0;
            dir.Normalize();
            Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 20 * Time.deltaTime);
        }
    }

    private void ActivateColliders()
    {
        foreach(var Collider in colliders)
        {
            Collider.isTrigger = false;
        }
    }

    private void DeactivateColliders()
    {
        foreach (var Collider in colliders)
        {
            Collider.isTrigger = false;
        }
    }
    private void DeactivateRagdoll()
    {
        foreach(var rigidBody in rigidbodies)
        {
            rigidBody.isKinematic = true;
            rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        animator.enabled = true;
    }
    public void ActivateRagdoll()
    {
        if (isAlive)
        {
            scoreCountAudioHandler.addANewKill();
            //audioSource.PlayOneShot(hitSound, .05f);
        }

        isAlive = false;
        foreach (var rigidBody in rigidbodies)
        {
            rigidBody.isKinematic = false;
        }
        animator.enabled = false;
        DeactivateColliders();
        
    }
}
