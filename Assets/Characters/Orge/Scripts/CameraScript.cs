using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject myCharacter;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(myCharacter.transform);
    }
}
