using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody bulletRigidBody;

    private void Awake()
    {
        bulletRigidBody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
    float speed = 20f;
    bulletRigidBody.velocity = transform.forward * speed;
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject,.01f);
    }
}
