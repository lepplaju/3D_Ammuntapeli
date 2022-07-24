using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHandle_script : MonoBehaviour
{

    [SerializeField]
    GameObject currentWeapon;

    [SerializeField]
    Transform rightHandTransform;



    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentWeapon.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GunTag"))
        {
            currentWeapon = Instantiate(other.gameObject);
            Destroy(other.transform.parent.gameObject);
            EquipGun();
        }
    }
    private void EquipGun()
    {
        currentWeapon.transform.position = rightHandTransform.transform.position;
        currentWeapon.transform.rotation = rightHandTransform.transform.rotation;
        currentWeapon.transform.SetParent(rightHandTransform);
    }
}
