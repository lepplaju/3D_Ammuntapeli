using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableCollider : MonoBehaviour
{
    [SerializeField] GameObject gun;
    Collider myCollider;
    [SerializeField] private TimerScript timerScript;

    private void Start()
    {
        myCollider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            myCollider.enabled = false;
            timerScript.myStart();
            Destroy(gun);
        }
    }
}
