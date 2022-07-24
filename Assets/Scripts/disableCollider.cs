using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableCollider : MonoBehaviour
{
    new Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            collider.enabled = false;
        }
    }
}
