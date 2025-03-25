using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackendStartMarker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.SetCheckPointBackend(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
