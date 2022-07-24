using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAimRotationWithCamera : MonoBehaviour
{

    [SerializeField]
    Transform aimTransform;

    [SerializeField]
    Transform cameraTransform;

    private Vector3 offset;
    private float distanceFromCamera =15f;
    Vector3 resultingPosition;

    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(2, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        resultingPosition = cameraTransform.position + cameraTransform.forward *distanceFromCamera;
        aimTransform.position = resultingPosition+offset;
    }
}
