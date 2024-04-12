using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundParallax : MonoBehaviour
{
    private Transform cameraTransform;
    [SerializeField]
    private float parallaxEffectMultiplier = 1f;
    private Vector3 lastCameraPosition;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        
        cameraTransform = Camera.main.transform;
        yield return null;
        lastCameraPosition = cameraTransform.position;
        // transform.position = new(0,0,0);
        print(cameraTransform.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {   
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier, deltaMovement.y * parallaxEffectMultiplier,0);
        lastCameraPosition = cameraTransform.position;
    }
}
