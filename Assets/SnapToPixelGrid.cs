using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnapToPixelGrid : MonoBehaviour
{
    private UnityEngine.Rendering.Universal.PixelPerfectCamera ppc;
    // Start is called before the first frame update
    void Awake()
    {
        ppc = GetComponent<UnityEngine.Rendering.Universal.PixelPerfectCamera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = ppc.RoundToPixel(transform.position);
    }
}