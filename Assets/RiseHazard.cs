using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiseHazard : MonoBehaviour
{
    [SerializeField] private float raiseSpeed = 0.05f;
    [SerializeField] private float slowSpeed = 0.05f;
    [HideInInspector] public bool shouldStop = false;
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldStop && raiseSpeed > 0)
        {
            raiseSpeed = Mathf.Max(0,(raiseSpeed - slowSpeed * Time.deltaTime));
        }
        float cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;
        if (cameraBottom - 0.75f > transform.position.y)
        {
            transform.position = new(transform.position.x, cameraBottom - 0.75f, transform.position.z);
        }
        transform.Translate(0, raiseSpeed * Time.deltaTime, 0);

    }

}
