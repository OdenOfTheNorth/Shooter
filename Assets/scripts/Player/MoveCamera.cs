using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    
    void Update()
    {
        transform.position = cameraTransform.position;
    }
}
