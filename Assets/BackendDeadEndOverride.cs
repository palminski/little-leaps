using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendDeadEndOverride : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Portal portal = GetComponent<Portal>();
        if (portal && GameController.Instance.CheckpointBackend != "")
        {
            portal.SetTargetScene(GameController.Instance.CheckpointBackend);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
