using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    [SerializeField]
    private Transform character;
    [SerializeField]
    private Transform respwanPoint;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            character.transform.position = respwanPoint.transform.position;
            Physics.SyncTransforms();
        }
    }
}
