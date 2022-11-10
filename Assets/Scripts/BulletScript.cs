using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody bulletRigidBody;
    private Ragdoll_script ragdoll;

    private void Awake()
    {
        bulletRigidBody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
    float speed = 40f;
    bulletRigidBody.velocity = transform.forward * speed;
    }
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("I hit something");
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.CompareTag("TargetEnemy"))
        {
            Debug.Log("I Hit the enemy");
            ragdoll = other.gameObject.GetComponentInParent<Ragdoll_script>();
            ragdoll.ActivateRagdoll();
        }
        Destroy(gameObject);
    }
}
