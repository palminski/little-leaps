using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SnapToPixelGrid : MonoBehaviour
{
    private PixelPerfectCamera ppc;
    // Start is called before the first frame update
    void Awake()
    {
        ppc = GetComponent<PixelPerfectCamera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = ppc.RoundToPixel(transform.position);
    }
}