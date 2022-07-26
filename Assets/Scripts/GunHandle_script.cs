using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHandle_script : MonoBehaviour
{

    [SerializeField]
    public GameObject currentWeapon;

    [SerializeField]
    private Transform rightHandTransform;

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
        Debug.Log(currentWeapon.transform.position);
        currentWeapon.transform.SetParent(rightHandTransform);
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.transform.localPosition = Vector3.zero;
    }
}
