using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.ChangeScene("Main Menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
