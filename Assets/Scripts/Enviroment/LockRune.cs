using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRune : MonoBehaviour
{
    [SerializeField] private string linkedChip;
    private ColorSwapper colorSwapper;

    [SerializeField] private TriggerEvent activateOnEvent;

    
    // Start is called before the first frame update
    void Awake()
    {
        colorSwapper = GetComponent<ColorSwapper>();
        if (colorSwapper) colorSwapper.enabled = false;
    }

    public void LightUp()
    {
        print("DONE!!!");
        if (colorSwapper) colorSwapper.enabled = true;
    }
}
