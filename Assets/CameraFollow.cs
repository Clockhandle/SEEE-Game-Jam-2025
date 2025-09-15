using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 offset = new Vector3(0, 2, -10f);
    public float smoothSpeed;

    void Update()
    {
        Vector3 targetPosition = target.position + offset;   
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime); 
    }
}
