using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSessionCollectibleOnLoad : MonoBehaviour
{
    public string collectible = "lv_1_started";
    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.TagObjectStringAsCollectedForSession(collectible);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
