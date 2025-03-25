using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOnCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            transform.position = new(cam.transform.position.x, cam.transform.position.y, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
