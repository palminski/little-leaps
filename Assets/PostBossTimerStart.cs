using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostBossTimerStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.StopTimer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
