using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private float easeTimeX = 0.2f;
    [SerializeField]
    private float easeTimeY = 0.1f;

    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        // transform.position = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
        Vector3 targetPosition = target.position + offset;

        Vector3 xVector = new(transform.position.x,0,0);
        Vector3 targetXVector = new(targetPosition.x,0,0);

        Vector3 yVector = new(0,transform.position.y,0);
        Vector3 targetYVector = new(0,targetPosition.y,0);

        yVector = Vector3.SmoothDamp(yVector, targetYVector, ref velocity, easeTimeY);
        xVector = Vector3.SmoothDamp(xVector, targetXVector, ref velocity, easeTimeX);

        transform.position = new(xVector.x,yVector.y,transform.position.z);
        // transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, easeTime);


    }
}
